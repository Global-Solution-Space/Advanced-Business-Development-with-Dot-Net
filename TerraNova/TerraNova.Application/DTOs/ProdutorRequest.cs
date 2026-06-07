using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

using TerraNova.Application.DTOs.Validators;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para cadastrar um produtor rural e seu telefone principal.</summary>
/// <param name="Nome">Nome do produtor.</param>
/// <param name="Email">E-mail único do produtor.</param>
/// <param name="Senha">Senha de acesso do produtor.</param>
/// <param name="TelefoneContato">Telefone principal com DDD; pode ser enviado com ou sem máscara.</param>
public record ProdutorRequest(
    [Required][StringLength(30, MinimumLength = 2)]  string Nome,
    [Required][EmailAddress][StringLength(30)][UniqueEmail]        string Email,
    [Required][StringLength(30, MinimumLength = 6)]   string Senha,
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^\D*(\d\D*){10,11}$", ErrorMessage = "O telefone deve conter DDD e numero com 10 ou 11 digitos.")]
    [UniqueTelefone]
    string TelefoneContato
)
{
    public Produtor ToDomain() => new(Nome, Email, Senha);
}
