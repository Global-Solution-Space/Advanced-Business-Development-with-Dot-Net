using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Produtor rural. Possui telefone embutido (coluna <c>telefone</c>) e,
/// opcionalmente, um <see cref="Telefone"/> detalhado em tabela separada (1:1).
/// </summary>
public sealed class Produtor : BaseEntity
{
    public string Nome             { get; private set; } = string.Empty;
    public string Email            { get; private set; } = string.Empty;
    public string Senha            { get; private set; } = string.Empty;
 
    /// <summary>Número de telefone concatenado — coluna <c>telefone</c> da tabela produtor.</summary>
    public string TelefoneContato  { get; private set; } = string.Empty;
 
    public List<Propriedade> Propriedades     { get; private set; } = [];
    public Telefone?          TelefoneDetalhado { get; private set; }
 
    private Produtor() { }
 
    public Produtor(string nome, string email, string senha, string telefoneContato)
    {
        AtualizarNome(nome);
        AtualizarEmail(email);
        AtualizarSenha(senha);
        AtualizarTelefoneContato(telefoneContato);
    }
 
    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome não pode ser vazio.");
 
        nome = nome.Trim();
 
        if (nome.Length > 30)
            throw new DomainException("O nome deve ter no máximo 30 caracteres.");
 
        Nome = nome;
    }
 
    public void AtualizarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("O e-mail informado é inválido.");
 
        email = email.Trim();
 
        if (email.Length > 30)
            throw new DomainException("O e-mail deve ter no máximo 30 caracteres.");
 
        Email = email;
    }
 
    public void AtualizarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new DomainException("A senha deve ter pelo menos 6 caracteres.");
 
        if (senha.Length > 30)
            throw new DomainException("A senha deve ter no máximo 30 caracteres.");
 
        Senha = senha;
    }
 
    public void AtualizarTelefoneContato(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new DomainException("O telefone de contato não pode ser vazio.");
 
        telefone = telefone.Trim()
            .Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
 
        if (telefone.Length > 11)
            throw new DomainException("O telefone deve ter no máximo 11 dígitos.");
 
        TelefoneContato = telefone;
    }
}