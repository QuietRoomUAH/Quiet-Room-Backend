using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Endpoints;

public class GetAvailableRoomsEndpoint : Endpoint<GetAvailableRoomsEndpoint.Request, List<string>>
{
    private const string TIME_REGEX = @"^(\d\d)(\d\d)$";
    private readonly IRoomRetriever _roomRetriever;
    
    public GetAvailableRoomsEndpoint(IRoomRetriever roomRetriever)
    {
        _roomRetriever = roomRetriever;
    }
    
    /// <inheritdoc />
    public override void Configure()
    {
        Get("/availability/{BuildingCode}");
    }

    /// <inheritdoc />
    public override async Task<List<string>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var rooms = await _roomRetriever
            .GetAvailableRoomsAsync(req.BuildingCode, ParseTime(req.StartTime), ParseTime(req.EndTime));
        return rooms.ToList();
    }

    private static TimeOnly ParseTime(string time)
    {
        var match = Regex.Match(time, TIME_REGEX);
        var hour = int.Parse(match.Groups[1].Value);
        var minute = int.Parse(match.Groups[2].Value);
        return new TimeOnly(hour, minute);
    }

    [UsedImplicitly]
    public class Request
    {
        public Request()
        {
            BuildingCode = null!;
            StartTime = null!;
            EndTime = null!;
        }
        
        [UsedImplicitly]
        public string BuildingCode { get; set; }
        [UsedImplicitly]
        public string StartTime { get; set; }
        [UsedImplicitly]
        public string EndTime { get; set; }
    }

    public class RequestValidator : Validator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .Matches(TIME_REGEX)
                .WithMessage("Start time must be in the format HHMM");
            RuleFor(request => request.EndTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .Matches(TIME_REGEX)
                .WithMessage("Start time must be in the format HHMM");
        }
    }
}
