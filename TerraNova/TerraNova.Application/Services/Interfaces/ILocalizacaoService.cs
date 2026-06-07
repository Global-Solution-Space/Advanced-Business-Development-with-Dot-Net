using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ILocalizacaoService
{
    IReadOnlyList<LocalizacaoResponse> GetAll();
    LocalizacaoResponse? GetById(Guid id);
    LocalizacaoResponse Create(LocalizacaoRequest request);
    LocalizacaoResponse Update(Guid id, LocalizacaoRequest request);
    bool Delete(Guid id);
}
