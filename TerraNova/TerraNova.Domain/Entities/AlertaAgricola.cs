using TerraNova.Domain.Common;
using TerraNova.Domain.Enums;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Alerta agrícola vinculado diretamente a um <see cref="Talhao"/>.
/// </summary>
public sealed class AlertaAgricola : BaseEntity
{
    public string      Titulo      { get; private set; } = string.Empty;
    public string      Descricao   { get; private set; } = string.Empty;
    public NivelAlerta NivelAlerta { get; private set; }
    public bool        Resolvido   { get; private set; }
    public DateTime    DataAlerta  { get; private set; }
 
    public Guid    TalhaoId { get; private set; }
    public Talhao? Talhao   { get; private set; }
 
    private AlertaAgricola() { }
 
    public AlertaAgricola(string titulo, string descricao, NivelAlerta nivelAlerta, Guid talhaoId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("O título do alerta não pode ser vazio.");
 
        titulo = titulo.Trim();
 
        if (titulo.Length > 100)
            throw new DomainException("O título deve ter no máximo 100 caracteres.");
 
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("A descrição do alerta não pode ser vazia.");
 
        descricao = descricao.Trim();
 
        if (descricao.Length > 300)
            throw new DomainException("A descrição deve ter no máximo 300 caracteres.");
 
        if (talhaoId == Guid.Empty)
            throw new DomainException("O alerta deve estar associado a um talhão válido.");
 
        Titulo      = titulo;
        Descricao   = descricao;
        NivelAlerta = nivelAlerta;
        Resolvido   = false;
        DataAlerta  = DateTime.UtcNow;
        TalhaoId    = talhaoId;
    }
 
    public void Resolver()
    {
        if (Resolvido)
            throw new DomainException("Este alerta já foi resolvido.");
 
        Resolvido = true;
    }

    public void Reabrir()
    {
        if (!Resolvido)
            throw new DomainException("Este alerta já está aberto.");

        Resolvido = false;
    }
 
    public void AtualizarNivel(NivelAlerta novoNivel) => NivelAlerta = novoNivel;
}