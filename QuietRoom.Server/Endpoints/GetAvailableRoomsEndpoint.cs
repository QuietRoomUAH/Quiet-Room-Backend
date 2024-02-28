using System.Diagnostics;
using FastEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using QuietRoom.Server.Services.Interfaces;
using QuietRoom.Server.Utils;

namespace QuietRoom.Server.Endpoints;

public class GetAvailableRoomsEndpoint(IRoomRepository roomRepository)
    : Endpoint<GetAvailableRoomsEndpoint.Request, List<string>>
{
    /// <inheritdoc />
    public override void Configure()
    {
        Get("/availability/{BuildingCode}");
    }

    /// <inheritdoc />
    public override async Task<List<string>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var day = GetDayOfWeek(req.Day);
        Logger.LogInformation("Getting available rooms for request {Request}", req);
        TimeOnly startTime;
        TimeOnly endTime;
        try
        {
            startTime = TimeOnlyUtils.ParseTime(req.StartTime);
            endTime = TimeOnlyUtils.ParseTime(req.EndTime);
        } catch (ArgumentOutOfRangeException)
        {
            ThrowError("One of the times sent as input is not valid");
            return new List<string>();
        }
        var sw = Stopwatch.StartNew();
        var rooms = await roomRepository.GetAvailableRoomsAsync(req.BuildingCode, startTime, endTime, day);
        var roomsList = rooms.OrderBy(s => s.RoomNumber).Select(dto => dto.RoomNumber).ToList();
        Logger.LogInformation("Got {Count} rooms in {Elapsed}ms", roomsList.Count, sw.ElapsedMilliseconds);
        return roomsList;
    }

    private static DayOfWeek GetDayOfWeek(string day)
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
        
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record Request
    {
        public string BuildingCode { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Day { get; set; } = null!;
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
