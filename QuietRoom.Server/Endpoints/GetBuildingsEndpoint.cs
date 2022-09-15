using FastEndpoints;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public class GetBuildingsEndpoint : EndpointWithoutRequest<List<string>>
{
    private readonly IBuildingRepository _buildingRepository;
    
    public GetBuildingsEndpoint(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }
    
    /// <inheritdoc />
    public override void Configure()
    {
        Get("/buildings");
    }

    /// <inheritdoc />
    public override async Task<List<string>> ExecuteAsync(CancellationToken ct)
    {
        var names = await _buildingRepository.GetBuildingNamesAsync();
        return names.ToList();
    }
}
