using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record ProdutorRequest(
    [Required][StringLength(30, MinimumLength = 2)]  string Nome,
    [Required][EmailAddress][StringLength(30)]        string Email,
    [Required][StringLength(30, MinimumLength = 6)]   string Senha,
    [Required][StringLength(11, MinimumLength = 10)]  string TelefoneContato)
{
    public Produtor ToDomain() => new(Nome, Email, Senha, TelefoneContato);
}
