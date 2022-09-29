using FastEndpoints;
using JetBrains.Annotations;
using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public class GetRoomEndpoint : Endpoint<GetRoomEndpoint.Request, RoomDto>
{
    private readonly IRoomRepository _roomRepository;

    /// <inheritdoc />
    public GetRoomEndpoint(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc/>
    public override void Configure()
    {
        Get("/building/{BuildingCode}/room/{RoomCode}");
        ResponseCache(500);
    }

    /// <inheritdoc />
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var roomDto = await _roomRepository.GetRoomInfoAsync(req.BuildingCode, req.RoomCode);
        if (roomDto is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendStringAsync(roomDto.ToJson(), contentType: "application/json", cancellation: ct);
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record Request
    {
        public Request()
        {
            BuildingCode = null!;
            RoomCode = null!;
        }
        
        public string BuildingCode { get; set; }
        public string RoomCode { get; set; }
    }
}
