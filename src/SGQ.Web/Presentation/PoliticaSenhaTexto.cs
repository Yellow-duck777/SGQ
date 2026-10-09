using Microsoft.AspNetCore.Identity;

namespace SGQ.Web.Presentation;

/// <summary>Descreve em português a política de senha configurada no Identity, para exibir nos formulários.</summary>
public static class PoliticaSenhaTexto
{
    public static IReadOnlyList<string> Descrever(PasswordOptions opcoes)
    {
        var itens = new List<string> { $"Pelo menos {opcoes.RequiredLength} caracteres" };
        if (opcoes.RequireUppercase) itens.Add("Uma letra maiúscula");
        if (opcoes.RequireLowercase) itens.Add("Uma letra minúscula");
        if (opcoes.RequireDigit) itens.Add("Um número");
        if (opcoes.RequireNonAlphanumeric) itens.Add("Um símbolo (por exemplo: ! @ # $ %)");
        if (opcoes.RequiredUniqueChars > 1) itens.Add($"Pelo menos {opcoes.RequiredUniqueChars} caracteres diferentes");
        return itens;
    }
}
