using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record ProdutorResponse(Guid Id, string Nome, string Email, string TelefoneContato)
{
    public static ProdutorResponse FromDomain(Produtor p) =>
        new(p.Id, p.Nome, p.Email, p.TelefoneContato);
}
