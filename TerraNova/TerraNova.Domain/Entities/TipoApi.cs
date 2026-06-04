using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>Tabela de lookup: identifica qual API externa foi consultada (ex.: SATVEG, NASAPOWER).</summary>
public sealed class TipoApi : BaseEntity
{
    public string NomeTipoApi { get; private set; } = string.Empty;
 
    public List<ReqApi> ReqApis { get; private set; } = [];
 
    private TipoApi() { }
 
    public TipoApi(string nomeTipoApi)
    {
        if (string.IsNullOrWhiteSpace(nomeTipoApi))
            throw new DomainException("O nome do tipo de API não pode ser vazio.");
 
        nomeTipoApi = nomeTipoApi.Trim().ToUpperInvariant();
 
        if (nomeTipoApi.Length > 10)
            throw new DomainException("O nome do tipo de API deve ter no máximo 10 caracteres.");
 
        NomeTipoApi = nomeTipoApi;
    }
}
