namespace SGQ.Web.Security;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string GarantiaQualidade = "GQ";
    public const string ResponsavelTecnico = "RT";
    public const string ControleQualidade = "CQ";
    public const string Auditor = "Auditor";
    public static readonly string[] Todos = [Administrador, GarantiaQualidade, ResponsavelTecnico, ControleQualidade, Auditor];
    public const string GestaoQualidade = Administrador + "," + GarantiaQualidade;
}
