using TerraNova.Application.DTOs;

namespace TerraNova.Application.Services.Interfaces;

public interface ISatvegService
{
    IReadOnlyList<SatvegResponse> GetAll();
    SatvegResponse? GetById(Guid id);
    IReadOnlyList<SatvegResponse> GetByTalhaoId(Guid talhaoId);
    SatvegResponse Create(SatvegRequest request);
    bool Delete(Guid id);
}