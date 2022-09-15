using FastEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using QuietRoom.Server.Services.Interfaces;
using QuietRoom.Server.Utils;

namespace QuietRoom.Server.Endpoints;

public class GetAvailableRoomsEndpoint : Endpoint<GetAvailableRoomsEndpoint.Request, List<string>>
{
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
        var day = GetDayOfWeek(req.Day);
        var rooms = await _roomRetriever
            .GetAvailableRoomsAsync(req.BuildingCode, TimeOnlyUtils.ParseTime(req.StartTime), TimeOnlyUtils.ParseTime(req.EndTime), day);
        return rooms.ToList();
    }

    private DayOfWeek GetDayOfWeek(string day)
    {
        return day switch
        {
            "M" => DayOfWeek.Monday,
            "T" => DayOfWeek.Tuesday,
            "W" => DayOfWeek.Wednesday,
            "R" => DayOfWeek.Thursday,
            "F" => DayOfWeek.Friday,
            "S" => DayOfWeek.Saturday,
            "U" => DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
        };
    }
        
    [UsedImplicitly]
    public class Request
    {
        public Request()
        {
            BuildingCode = null!;
            StartTime = null!;
            EndTime = null!;
            Day = null!;
        }
        
        [UsedImplicitly]
        public string BuildingCode { get; set; }
        [UsedImplicitly]
        public string StartTime { get; set; }
        [UsedImplicitly]
        public string EndTime { get; set; }
        [UsedImplicitly]
        public string Day { get; set; }
    }

    public class RequestValidator : Validator<Request>
    {
        public RequestValidator()
        {
            RuleFor(request => request.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .Matches(TimeOnlyUtils.TIME_REGEX)
                .WithMessage("Start time must be in the format HHMM");
            RuleFor(request => request.EndTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .Matches(TimeOnlyUtils.TIME_REGEX)
                .WithMessage("Start time must be in the format HHMM");
            RuleFor(request => request.Day)
                .NotEmpty()
                .WithMessage("Day is required")
                .Matches("^[MTWRFSU]$")
                .WithMessage("Day must be one of M, T, W, R, F, S, U");
        }
    }
}
