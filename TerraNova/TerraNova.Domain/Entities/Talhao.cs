using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Talhão: subdivisão de uma <see cref="Propriedade"/> com tipo de plantação e
/// localização exclusiva (1:1). Origem dos dados de <see cref="Satveg"/> e <see cref="NasaPower"/>.
/// </summary>
public sealed class Talhao : BaseEntity
{
    public string  NomeTalhao { get; private set; } = string.Empty;
    public decimal VolumArea  { get; private set; }
 
    public Guid           TipoPlantacaoId { get; private set; }
    public TipoPlantacao? TipoPlantacao   { get; private set; }
 
    public Guid         PropriedadeId { get; private set; }
    public Propriedade? Propriedade   { get; private set; }
 
    public Guid         LocalizacaoId { get; private set; }
    public Localizacao? Localizacao   { get; private set; }
 
    public List<DadoTemporal>   DadosTemporais { get; private set; } = [];
    public List<AlertaAgricola> Alertas        { get; private set; } = [];
 
    private Talhao() { }
 
    public Talhao(string nomeTalhao, decimal volumArea, Guid tipoPlantacaoId, Guid propriedadeId, Guid localizacaoId)
    {
        Atualizar(nomeTalhao, volumArea, tipoPlantacaoId, propriedadeId, localizacaoId);
    }

    public void Atualizar(string nomeTalhao, decimal volumArea, Guid tipoPlantacaoId, Guid propriedadeId, Guid localizacaoId)
    {
        if (string.IsNullOrWhiteSpace(nomeTalhao))
            throw new DomainException("O nome do talhão não pode ser vazio.");
 
        nomeTalhao = nomeTalhao.Trim();
 
        if (nomeTalhao.Length > 30)
            throw new DomainException("O nome do talhão deve ter no máximo 30 caracteres.");
 
        if (volumArea <= 0)
            throw new DomainException("O volume/área deve ser maior que zero.");
 
        if (tipoPlantacaoId == Guid.Empty)
            throw new DomainException("O talhão deve ter um tipo de plantação válido.");
 
        if (propriedadeId == Guid.Empty)
            throw new DomainException("O talhão deve estar associado a uma propriedade válida.");
 
        if (localizacaoId == Guid.Empty)
            throw new DomainException("O talhão deve ter uma localização válida.");
 
        NomeTalhao      = nomeTalhao;
        VolumArea       = volumArea;
        TipoPlantacaoId = tipoPlantacaoId;
        PropriedadeId   = propriedadeId;
        LocalizacaoId   = localizacaoId;
    }
}