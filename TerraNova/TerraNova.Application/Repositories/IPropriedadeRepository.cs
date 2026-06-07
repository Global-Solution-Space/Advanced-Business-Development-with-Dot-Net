using TerraNova.Domain.Entities;

namespace TerraNova.Application.Repositories;

/// <summary>
/// Contrato especializado para acesso a dados de <see cref="Propriedade"/>.
/// Estende o repositório genérico com queries específicas de domínio.
/// </summary>
public interface IPropriedadeRepository : IRepository<Propriedade>
{
    /// <summary>
    /// Retorna as propriedades vinculadas a um produtor. Filtro feito no banco via SQL WHERE.
    /// </summary>
    IReadOnlyList<Propriedade> GetByProdutorId(Guid produtorId);

    /// <summary>
    /// Verifica se uma localização já está em uso por outra propriedade.
    /// Usa COUNT() > 0 diretamente no banco (evita GetAll em memória).
    /// </summary>
    bool ExistsByLocalizacaoId(Guid localizacaoId);
}
