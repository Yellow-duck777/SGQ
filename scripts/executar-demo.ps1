<#
.SYNOPSIS
    Prepara e executa o SGQ com dados FICTÍCIOS para testes manuais e revisão das telas.

.DESCRIPTION
    1. cria (ou recria, com -Recriar) o banco de demonstração `sgq_demo_test` no PostgreSQL;
    2. aplica as migrations, cria uma conta por perfil e carrega clientes, produtos, lotes, calendário,
       reclamações (RC), não conformidades (NC) e recalls em vários estados;
    3. inicia o sistema em http://localhost:5024.

    Usa variáveis de ambiente apenas desta sessão: NÃO altera os seus User Secrets nem o seu banco de
    desenvolvimento (`sgq_dev`). O nome do banco termina em `_test`, como exigem as proteções do projeto.
    Nunca use dados reais neste ambiente.

.PARAMETER Servidor   Host do PostgreSQL (padrão: localhost).
.PARAMETER Porta      Porta do PostgreSQL (padrão: 5432).
.PARAMETER Usuario    Usuário do PostgreSQL (padrão: postgres).
.PARAMETER Banco      Nome do banco de demonstração (precisa terminar em _test; padrão: sgq_demo_test).
.PARAMETER Recriar    Apaga o banco de demonstração e recomeça do zero (pede confirmação).
.PARAMETER SomentePreparar  Prepara banco, contas e dados e NÃO inicia o sistema.
.PARAMETER ComEmail   Inicia o Mailpit (se existir no Laragon) e aponta o SMTP do sistema para ele, para testar as
                      notificações por e-mail. As mensagens ficam em http://localhost:8025 e nada sai da sua máquina.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts\executar-demo.ps1
.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts\executar-demo.ps1 -Recriar
#>
[CmdletBinding()]
param(
    [string]$Servidor = "localhost",
    [int]$Porta = 5432,
    [string]$Usuario = "postgres",
    [string]$Banco = "sgq_demo_test",
    [switch]$Recriar,
    [switch]$SomentePreparar,
    [switch]$ComEmail
)

$ErrorActionPreference = "Stop"
$raiz = Split-Path -Parent $PSScriptRoot
$projeto = Join-Path $raiz "src\SGQ.Web\SGQ.Web.csproj"

function Escrever($texto, $cor = "Cyan") { Write-Host "`n==> $texto" -ForegroundColor $cor }

if ($Banco -notmatch "_tests?$") { throw "O banco de demonstração precisa terminar em _test ou _tests (recebido: $Banco)." }
if ($Banco -notmatch "^[A-Za-z0-9_]+$") { throw "Use apenas letras, números e sublinhado no nome do banco." }

# --- 1. Ferramentas -------------------------------------------------------------------------------------------
Escrever "Verificando ferramentas"
# O "dotnet" do PATH pode ser o de 32 bits (sem SDK); procura o que realmente tem o SDK 10.
$candidatos = @()
$candidatos += @(Get-Command dotnet -All -ErrorAction SilentlyContinue | ForEach-Object { $_.Source })
$candidatos += "C:\Program Files\dotnet\dotnet.exe"
$dotnetExe = $candidatos | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique |
    Where-Object { (& $_ --list-sdks 2>$null) -match "^10\." } | Select-Object -First 1
if (-not $dotnetExe) { throw "SDK .NET 10 não encontrado. Instale-o (https://dotnet.microsoft.com/download/dotnet/10.0) e tente de novo." }
$env:Path = (Split-Path $dotnetExe) + ";" + $env:Path
Write-Host "dotnet: $dotnetExe (SDK $(& dotnet --version))"

$psql = (Get-Command psql -ErrorAction SilentlyContinue).Source
if (-not $psql) {
    $psql = Get-ChildItem "C:\Program Files\PostgreSQL\*\bin\psql.exe" -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | Select-Object -First 1 -ExpandProperty FullName
}
if (-not $psql) { throw "psql não encontrado. Instale o PostgreSQL 17 ou inclua a pasta bin dele no PATH." }
Write-Host "psql: $psql"

# --- 2. PostgreSQL --------------------------------------------------------------------------------------------
Escrever "Conectando ao PostgreSQL em ${Servidor}:${Porta}"
if (-not (Test-NetConnection $Servidor -Port $Porta -WarningAction SilentlyContinue).TcpTestSucceeded) {
    $servico = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($servico -and $servico.Status -ne "Running") {
        Write-Host "Serviço '$($servico.Name)' está parado; tentando iniciar (pode exigir PowerShell como Administrador)..."
        Start-Service $servico.Name
        Start-Sleep 5
    }
    if (-not (Test-NetConnection $Servidor -Port $Porta -WarningAction SilentlyContinue).TcpTestSucceeded) {
        throw "PostgreSQL não responde em ${Servidor}:${Porta}. Inicie o serviço e tente de novo."
    }
}

function Executar-Psql([string]$sql, [string]$bancoAlvo = "postgres") {
    $saida = $sql | & $psql -h $Servidor -p $Porta -U $Usuario -d $bancoAlvo -tA -v ON_ERROR_STOP=1 2>&1
    if ($LASTEXITCODE -ne 0) { throw ($saida -join "`n") }
    return $saida
}

$senhaPg = $null
try { Executar-Psql "select 1" | Out-Null }
catch {
    Write-Host "Autenticação necessária."
    $seguro = Read-Host "Senha do usuário '$Usuario' do PostgreSQL" -AsSecureString
    $senhaPg = [System.Net.NetworkCredential]::new("", $seguro).Password
    $env:PGPASSWORD = $senhaPg
    Executar-Psql "select 1" | Out-Null
}

if ($Recriar) {
    $resposta = Read-Host "Isto APAGA o banco '$Banco' e todos os dados de demonstração. Digite SIM para continuar"
    if ($resposta -ne "SIM") { throw "Cancelado." }
    Executar-Psql "select pg_terminate_backend(pid) from pg_stat_activity where datname = '$Banco' and pid <> pg_backend_pid();" | Out-Null
    Executar-Psql "drop database if exists $Banco;" | Out-Null
}
$existe = Executar-Psql "select 1 from pg_database where datname = '$Banco';"
$bancoNovo = -not $existe
if ($bancoNovo) {
    Executar-Psql "create database $Banco;" | Out-Null
    Write-Host "Banco '$Banco' criado."
} else {
    Write-Host "Banco '$Banco' já existe (use -Recriar para recomeçar do zero)."
}

# --- 3. Configuração desta sessão (nada é gravado em User Secrets) --------------------------------------------
function Nova-Senha {
    # Compatível com Windows PowerShell 5.1 e PowerShell 7 (sem RandomNumberGenerator.GetInt32).
    $gerador = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
    function Sortear([int]$limite) {
        $b = New-Object byte[] 4
        do { $gerador.GetBytes($b); $n = [System.BitConverter]::ToUInt32($b, 0) } while ($n -ge [uint32]([uint32]::MaxValue - ([uint32]::MaxValue % $limite)))
        return [int]($n % $limite)
    }
    $maius = "ABCDEFGHJKLMNPQRSTUVWXYZ".ToCharArray(); $minus = "abcdefghijkmnpqrstuvwxyz".ToCharArray()
    $nums = "23456789".ToCharArray(); $simb = "!@#$%*?".ToCharArray()
    $todos = $maius + $minus + $nums + $simb
    $chars = New-Object System.Collections.ArrayList
    foreach ($conjunto in @($maius, $minus, $nums, $simb)) { [void]$chars.Add($conjunto[(Sortear $conjunto.Length)]) }
    1..12 | ForEach-Object { [void]$chars.Add($todos[(Sortear $todos.Length)]) }
    for ($i = $chars.Count - 1; $i -gt 0; $i--) { $j = Sortear ($i + 1); $t = $chars[$i]; $chars[$i] = $chars[$j]; $chars[$j] = $t }
    return -join $chars
}
$senhaDemo = Nova-Senha

$cadeia = "Host=$Servidor;Port=$Porta;Database=$Banco;Username=$Usuario"
if ($senhaPg) { $cadeia += ";Password=$senhaPg" }
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = $cadeia
# Neutraliza o administrador de desenvolvimento dos seus User Secrets (SeedAdmin), para o banco de demonstração conter só as contas listadas abaixo.
$env:SeedAdmin__Email = ""
$env:SeedAdmin__Password = ""
$env:DevelopmentTestUsers__Enabled = "true"
$env:DevelopmentTestUsers__ExpectedDatabase = $Banco
$contas = @(
    @{ Email = "admin@sgq.test"; Perfil = "Administrador" }, @{ Email = "gq@sgq.test"; Perfil = "GQ" },
    @{ Email = "rt@sgq.test"; Perfil = "RT" }, @{ Email = "cq@sgq.test"; Perfil = "CQ" }, @{ Email = "auditor@sgq.test"; Perfil = "Auditor" }
)
for ($i = 0; $i -lt $contas.Count; $i++) {
    Set-Item "Env:DevelopmentTestUsers__Users__${i}__Email" $contas[$i].Email
    Set-Item "Env:DevelopmentTestUsers__Users__${i}__Password" $senhaDemo
    Set-Item "Env:DevelopmentTestUsers__Users__${i}__Roles__0" $contas[$i].Perfil
}

if ($ComEmail) {
    $mailpit = Get-ChildItem "C:\laragon\bin\mailpit\*\mailpit.exe", "C:\laragon\bin\mailpit\mailpit.exe" -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
    if (-not $mailpit) { Write-Warning "Mailpit não encontrado em C:\laragon\bin\mailpit; as notificações por e-mail ficarão desativadas." }
    else {
        if (-not (Get-Process mailpit -ErrorAction SilentlyContinue)) { Start-Process -FilePath $mailpit -WindowStyle Hidden; Start-Sleep 2 }
        $env:Smtp__Host = "localhost"; $env:Smtp__Port = "1025"; $env:Smtp__EnableSsl = "false"; $env:Smtp__From = "sgq@demo.sgq.test"
        Write-Host "E-mails de teste: caixa de entrada em http://localhost:8025 (Mailpit)"
    }
}

# --- 4. Compilar, migrar e carregar dados ---------------------------------------------------------------------
Escrever "Compilando"
& dotnet build $projeto -nologo -v q
if ($LASTEXITCODE -ne 0) { throw "Falha na compilação." }

Escrever "Aplicando migrations e carregando dados fictícios"
& dotnet run --project $projeto --no-build -- --seed-demo-data
if ($LASTEXITCODE -ne 0) { throw "Falha ao carregar os dados de demonstração." }

Escrever "Criando as contas de teste (uma por perfil)"
& dotnet run --project $projeto --no-build -- --bootstrap-test-users
if ($LASTEXITCODE -ne 0) { throw "Falha ao criar as contas de teste." }

if (-not $bancoNovo) {
    # Contas já existentes mantêm a senha antiga no banco; redefine para a senha gerada nesta execução.
    Escrever "Redefinindo as senhas das contas de teste para a senha desta execução"
    foreach ($c in $contas) {
        & dotnet run --project $projeto --no-build -- --reset-test-user-password $c.Email
        if ($LASTEXITCODE -ne 0) { throw "Falha ao redefinir a senha de $($c.Email)." }
    }
}

# --- 5. Resumo ------------------------------------------------------------------------------------------------
Escrever "Tudo pronto" "Green"
Write-Host "Endereço : http://localhost:5024"
Write-Host "Banco    : $Banco ($Servidor`:$Porta)"
Write-Host ""
Write-Host ("{0,-20} {1}" -f "Conta", "Perfil") -ForegroundColor Yellow
foreach ($c in $contas) { Write-Host ("{0,-20} {1}" -f $c.Email, $c.Perfil) }
Write-Host ""
Write-Host "Senha de todas as contas (gerada agora; só vale para este banco): $senhaDemo" -ForegroundColor Yellow
Write-Host "Cada execução gera uma senha nova e a aplica a todas as contas; os dados já criados são mantidos (use -Recriar para recomeçar do zero)." -ForegroundColor DarkYellow
Write-Host "Roteiro de testes: docs\qualidade\roteiros\README.md"

if ($SomentePreparar) { return }

Escrever "Iniciando o sistema (Ctrl+C para encerrar)"
$env:ASPNETCORE_URLS = "http://localhost:5024"
& dotnet run --project $projeto --no-build --no-launch-profile
