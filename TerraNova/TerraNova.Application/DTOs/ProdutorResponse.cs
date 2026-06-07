using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Produtor rural retornado pela API. A senha nunca é exposta.</summary>
/// <param name="Id">Identificador do produtor.</param>
/// <param name="Nome">Nome do produtor.</param>
/// <param name="Email">E-mail do produtor.</param>
/// <param name="TelefoneContato">Telefone principal formatado quando houver telefone vinculado.</param>
public record ProdutorResponse(Guid Id, string Nome, string Email, string TelefoneContato)
{
    public static ProdutorResponse FromDomain(Produtor p) =>
        new(p.Id, p.Nome, p.Email, p.TelefoneDetalhado != null ? p.TelefoneDetalhado.Completo : string.Empty);
}
