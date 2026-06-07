using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Telefone detalhado retornado pela API.</summary>
/// <param name="Id">Identificador do telefone.</param>
/// <param name="Ddd">DDD do telefone.</param>
/// <param name="Numero">Número do telefone.</param>
/// <param name="Completo">Telefone formatado com DDD e número.</param>
/// <param name="ProdutorId">Identificador do produtor dono do telefone.</param>
public record TelefoneResponse(Guid Id, string Ddd, string Numero, string Completo, Guid ProdutorId)
{
    public static TelefoneResponse FromDomain(Telefone t) =>
        new(t.Id, t.Ddd, t.Numero, t.Completo, t.ProdutorId);
}
