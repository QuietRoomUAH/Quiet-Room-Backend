using System.Collections.Immutable;
using System.Diagnostics;
using FastEndpoints;
using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public record Request
{
    public string BuildingCode { get; set; } = null!;
};

public class GetRoomByBuildingEndpoint : Endpoint<Request, IEnumerable<RoomDto>>
{
    private readonly IBuildingRepository _buildingRepository;
    
    public GetRoomByBuildingEndpoint(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Get("/building/{BuildingCode}/room");
        ResponseCache(500);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<RoomDto>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        var roomDtos = (await _buildingRepository.GetRoomNumbersAsync(req.BuildingCode)).ToImmutableArray();

        Logger.LogInformation("Got {RoomAmount} rooms for building {BuildingCode} in {ElapsedMilliseconds}ms",
            roomDtos.Length, req.BuildingCode,
            sw.ElapsedMilliseconds);

        return roomDtos;
    }
}
