using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Propriedade rural. Pertence a um <see cref="Produtor"/> e possui
/// uma <see cref="Localizacao"/> exclusiva (1:1).
/// Agrupa um conjunto de <see cref="Talhao"/>s.
/// </summary>
public sealed class Propriedade : BaseEntity
{
    public string  Nome         { get; private set; } = string.Empty;
    public decimal TamanhoTotal { get; private set; }
 
    public Guid      ProdutorId { get; private set; }
    public Produtor? Produtor   { get; private set; }
 
    public Guid         LocalizacaoId { get; private set; }
    public Localizacao? Localizacao   { get; private set; }
 
    public List<Talhao> Talhoes { get; private set; } = [];
 
    private Propriedade() { }
 
    public Propriedade(string nome, decimal tamanhoTotal, Guid produtorId, Guid localizacaoId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da propriedade não pode ser vazio.");
 
        nome = nome.Trim();
 
        if (nome.Length > 30)
            throw new DomainException("O nome da propriedade deve ter no máximo 30 caracteres.");
 
        if (tamanhoTotal <= 0)
            throw new DomainException("O tamanho total deve ser maior que zero.");
 
        if (produtorId == Guid.Empty)
            throw new DomainException("A propriedade deve estar associada a um produtor válido.");
 
        if (localizacaoId == Guid.Empty)
            throw new DomainException("A propriedade deve ter uma localização válida.");
 
        Nome          = nome;
        TamanhoTotal  = tamanhoTotal;
        ProdutorId    = produtorId;
        LocalizacaoId = localizacaoId;
    }
}