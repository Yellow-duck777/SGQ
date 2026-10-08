using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using SGQ.Web.Data;
using SGQ.Web.Models;
using SGQ.Web.Security;
using SGQ.Web.Services;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    var keysDirectory = new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, ".data-protection"));
    builder.Services.AddDataProtection().PersistKeysToFileSystem(keysDirectory);
}

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPrazoService, PrazoService>();
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IFluxoNotificacaoService, FluxoNotificacaoService>();
builder.Services.AddHostedService<AlertasPrazoHostedService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' deve ser configurada, por exemplo, com User Secrets.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Toda rota exige usuário autenticado COM perfil reconhecido. Uma conta recém-cadastrada fica sem acesso
// até que um Administrador atribua um perfil. Páginas públicas precisam de [AllowAnonymous] explícito.
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Roles.Todos)
        .Build());

var app = builder.Build();

if (DevelopmentTestUsers.HasRequestedOperation(args))
{
    await DevelopmentTestUsers.ExecuteAsync(args, app.Services, app.Environment, app.Configuration);
    return;
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in Roles.Todos)
        if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));

    var initialAdminEmail = app.Configuration["InitialAdminEmail"];
    if (!string.IsNullOrWhiteSpace(initialAdminEmail))
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        // Só promove quando ainda não existe nenhum Administrador, para que o e-mail configurado não
        // reaplique o perfil a cada reinício nem sirva de atalho depois da implantação inicial.
        if ((await userManager.GetUsersInRoleAsync(Roles.Administrador)).Count == 0)
        {
            var user = await userManager.FindByEmailAsync(initialAdminEmail);
            if (user is not null) await userManager.AddToRoleAsync(user, Roles.Administrador);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    await DevelopmentAdminSeeder.SeedAsync(app.Services, app.Configuration);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
