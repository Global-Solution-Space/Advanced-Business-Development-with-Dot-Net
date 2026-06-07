using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Propriedade rural retornada pela API.</summary>
/// <param name="Id">Identificador da propriedade.</param>
/// <param name="Nome">Nome da propriedade.</param>
/// <param name="TamanhoTotal">Área total da propriedade.</param>
/// <param name="ProdutorId">Identificador do produtor proprietário.</param>
/// <param name="LocalizacaoId">Identificador da localização vinculada.</param>
public record PropriedadeResponse(Guid Id, string Nome, decimal TamanhoTotal, Guid ProdutorId, Guid LocalizacaoId)
{
    public static PropriedadeResponse FromDomain(Propriedade p) =>
        new(p.Id, p.Nome, p.TamanhoTotal, p.ProdutorId, p.LocalizacaoId);
}
