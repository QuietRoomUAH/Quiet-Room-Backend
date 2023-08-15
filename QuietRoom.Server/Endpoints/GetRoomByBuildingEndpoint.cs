using System.Collections.Immutable;
using System.Diagnostics;
using FastEndpoints;
using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public class GetRoomByBuildingEndpoint : Endpoint<GetRoomByBuildingEndpoint.Request, ImmutableArray<RoomDto>>
{
    private readonly IBuildingRepository _buildingRepository;
    
    public GetRoomByBuildingEndpoint(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Get("/building/{BuildingCode}/rooms");
        ResponseCache(500);
    }

    /// <inheritdoc />
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        var roomDtos = (await _buildingRepository.GetRoomNumbersAsync(req.BuildingCode)).ToImmutableArray();

        Logger.LogInformation("Got {RoomAmount} rooms for building {BuildingCode} in {ElapsedMilliseconds}ms",
            roomDtos.Length, req.BuildingCode,
            sw.ElapsedMilliseconds);

        await SendOkAsync(roomDtos, cancellation: ct);
    }
    
    public record Request
    {
        public string BuildingCode { get; set; } = null!;
    };
}
