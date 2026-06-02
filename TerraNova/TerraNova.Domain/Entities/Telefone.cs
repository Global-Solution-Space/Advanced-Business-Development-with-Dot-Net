using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Telefone detalhado (DDD + número) de um <see cref="Produtor"/>.
/// Relacionamento 1:1 — a FK fica nesta tabela (produtor_id_produtor).
/// </summary>
public sealed class Telefone : BaseEntity
{
    public string Ddd    { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
 
    public Guid      ProdutorId { get; private set; }
    public Produtor? Produtor   { get; private set; }
 
    private Telefone() { }
 
    public Telefone(string ddd, string numero, Guid produtorId)
    {
        if (string.IsNullOrWhiteSpace(ddd) || ddd.Trim().Length != 2)
            throw new DomainException("O DDD deve ter exatamente 2 dígitos.");
 
        if (string.IsNullOrWhiteSpace(numero) || numero.Trim().Length > 9)
            throw new DomainException("O número deve ter no máximo 9 dígitos.");
 
        if (produtorId == Guid.Empty)
            throw new DomainException("O telefone deve estar associado a um produtor válido.");
 
        Ddd        = ddd.Trim();
        Numero     = numero.Trim();
        ProdutorId = produtorId;
    }
 
    /// <summary>Número completo formatado: (DDD) Número.</summary>
    public string Completo => $"({Ddd}) {Numero}";
}