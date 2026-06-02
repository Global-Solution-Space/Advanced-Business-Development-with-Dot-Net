using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>Tipo de plantação (tabela de domínio configurável). Ex.: Soja, Milho, Café.</summary>
public sealed class TipoPlantacao : BaseEntity
{
    public string TipoPlant { get; private set; } = string.Empty;

    public List<Talhao> Talhoes { get; private set; } = [];

    private TipoPlantacao() { }

    public TipoPlantacao(string tipoPlant)
    {
        if (string.IsNullOrWhiteSpace(tipoPlant))
            throw new DomainException("O tipo de plantação não pode ser vazio.");

        tipoPlant = tipoPlant.Trim();

        if (tipoPlant.Length > 30)
            throw new DomainException("O tipo de plantação deve ter no máximo 30 caracteres.");

        TipoPlant = tipoPlant;
    }
}