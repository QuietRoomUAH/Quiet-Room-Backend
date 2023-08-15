using FastEndpoints;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public class GetIsAvailableEndpoint : Endpoint<GetIsAvailableEndpoint.Request, bool>
{
    private readonly IRoomRepository _roomRepository;
    
    public GetIsAvailableEndpoint(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Get("/building/{BuildingCode}/room/{RoomCode}/available");
        ResponseCache(500);
    }

    /// <inheritdoc />
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var isAvailable = await _roomRepository.IsRoomAvailableAsync(req.BuildingCode, req.RoomCode);
        await SendOkAsync(isAvailable, cancellation: ct);
    }

    public record Request
    {
        public string BuildingCode { get; set; } = string.Empty;
        public string RoomCode { get; set; } = string.Empty;
    }
}
