using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TelefoneResponse(Guid Id, string Ddd, string Numero, string Completo, Guid ProdutorId)
{
    public static TelefoneResponse FromDomain(Telefone t) =>
        new(t.Id, t.Ddd, t.Numero, t.Completo, t.ProdutorId);
}
