using System.Diagnostics;
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
        ResponseCache(500);
    }

    /// <inheritdoc />
    public override async Task<List<string>> ExecuteAsync(CancellationToken ct)
    {
        Logger.LogDebug("Getting buildings");
        var sw = Stopwatch.StartNew();
        var names = await _buildingRepository.GetBuildingNamesAsync();
        Logger.LogInformation("Got buildings in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        return names.ToList();
    }
}
