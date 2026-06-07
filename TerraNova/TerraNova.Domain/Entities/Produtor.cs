using TerraNova.Domain.Common;
using TerraNova.Domain.Exceptions;

namespace TerraNova.Domain.Entities;

/// <summary>
/// Produtor rural. O telefone fica na tabela <see cref="Telefone"/>,
/// vinculado por <c>produtor_id_produtor</c> em relacionamento 1:1.
/// </summary>
public sealed class Produtor : BaseEntity
{
    public string Nome             { get; private set; } = string.Empty;
    public string Email            { get; private set; } = string.Empty;
    public string Senha            { get; private set; } = string.Empty;
 
    public List<Propriedade> Propriedades     { get; private set; } = [];
    public Telefone?          TelefoneDetalhado { get; private set; }
 
    private Produtor() { }
 
    public Produtor(string nome, string email, string senha)
    {
        AtualizarNome(nome);
        AtualizarEmail(email);
        AtualizarSenha(senha);
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

    public void AtribuirTelefone(Telefone telefone)
    {
        TelefoneDetalhado = telefone ?? throw new ArgumentNullException(nameof(telefone));
    }
}