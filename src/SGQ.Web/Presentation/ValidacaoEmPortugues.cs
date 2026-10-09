using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace SGQ.Web.Presentation;

/// <summary>
/// Define mensagens de validação em português para todos os atributos de validação que não tenham mensagem própria,
/// inclusive os "obrigatórios implícitos" que o MVC cria para tipos não anuláveis (ex.: um <c>int ProdutoId</c>).
/// Vale para a validação no servidor e para a validação do navegador (os atributos data-val-* usam a mesma mensagem).
/// Mensagens escritas explicitamente nos modelos continuam valendo.
/// </summary>
public sealed class ValidacaoEmPortuguesProvider : IValidationMetadataProvider
{
    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        // O MVC cria um [Required] "implícito" (com mensagem em inglês) para propriedades não anuláveis, como int e bool.
        // Para trocar a mensagem, o atributo é declarado de forma explícita aqui, com texto em português.
        var tipo = context.Key.ModelType;
        var naoAnulavel = tipo.IsValueType && Nullable.GetUnderlyingType(tipo) is null;
        if (naoAnulavel && context.Key.MetadataKind is ModelMetadataKind.Property or ModelMetadataKind.Parameter
            && !context.ValidationMetadata.ValidatorMetadata.OfType<RequiredAttribute>().Any())
            context.ValidationMetadata.ValidatorMetadata.Add(new RequiredAttribute { ErrorMessage = "Este campo é obrigatório." });

        foreach (var metadado in context.ValidationMetadata.ValidatorMetadata)
        {
            if (metadado is not ValidationAttribute atributo) continue;
            if (!string.IsNullOrEmpty(atributo.ErrorMessage) || !string.IsNullOrEmpty(atributo.ErrorMessageResourceName)) continue;

            atributo.ErrorMessage = atributo switch
            {
                RequiredAttribute => "Este campo é obrigatório.",
                StringLengthAttribute { MinimumLength: > 0 } => "Use entre {2} e {1} caracteres.",
                StringLengthAttribute => "Use no máximo {1} caracteres.",
                MaxLengthAttribute => "Use no máximo {1} caracteres.",
                MinLengthAttribute => "Use pelo menos {1} caracteres.",
                RangeAttribute => "Informe um valor entre {1} e {2}.",
                EmailAddressAttribute => "Informe um e-mail válido.",
                CompareAttribute => "Os valores informados não são iguais.",
                PhoneAttribute => "Informe um telefone válido.",
                UrlAttribute => "Informe um endereço válido.",
                RegularExpressionAttribute => "O formato informado não é válido.",
                _ => atributo.ErrorMessage
            };
        }
    }

    /// <summary>Mensagens do próprio model binding (valor ausente, inválido ou com tipo incorreto).</summary>
    public static void ConfigurarMensagensDeBinding(MvcOptions opcoes)
    {
        var mensagens = opcoes.ModelBindingMessageProvider;
        mensagens.SetValueMustNotBeNullAccessor(_ => "Este campo é obrigatório.");
        mensagens.SetMissingBindRequiredValueAccessor(campo => $"Informe o campo {campo}.");
        mensagens.SetMissingKeyOrValueAccessor(() => "Informe um valor.");
        mensagens.SetMissingRequestBodyRequiredValueAccessor(() => "O conteúdo da requisição é obrigatório.");
        mensagens.SetAttemptedValueIsInvalidAccessor((valor, _) => $"O valor '{valor}' não é válido.");
        mensagens.SetUnknownValueIsInvalidAccessor(_ => "O valor informado não é válido.");
        mensagens.SetValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
        mensagens.SetValueMustBeANumberAccessor(_ => "Informe um número válido.");
        mensagens.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"O valor '{valor}' não é válido.");
        mensagens.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor informado não é válido.");
        mensagens.SetNonPropertyValueMustBeANumberAccessor(() => "Informe um número válido.");
    }
}
