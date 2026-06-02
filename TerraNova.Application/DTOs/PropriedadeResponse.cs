using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record PropriedadeResponse(Guid Id, string Nome, decimal TamanhoTotal, Guid ProdutorId, Guid LocalizacaoId)
{
    public static PropriedadeResponse FromDomain(Propriedade p) =>
        new(p.Id, p.Nome, p.TamanhoTotal, p.ProdutorId, p.LocalizacaoId);
}
