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
        Atualizar(ddd, numero);
        ProdutorId = produtorId;
    }

    public void Atualizar(string ddd, string numero)
    {
        ddd = ddd.Trim();
        numero = numero.Trim();

        if (string.IsNullOrWhiteSpace(ddd) || ddd.Length != 2 || !ddd.All(char.IsDigit))
            throw new DomainException("O DDD deve ter exatamente 2 dígitos.");
 
        if (string.IsNullOrWhiteSpace(numero) || numero.Length is < 8 or > 9 || !numero.All(char.IsDigit))
            throw new DomainException("O número deve ter entre 8 e 9 dígitos.");

        Ddd    = ddd;
        Numero = numero;
    }
 
    /// <summary>Número completo formatado: (DDD) Número.</summary>
    public string Completo => $"({Ddd}) {Numero}";
}