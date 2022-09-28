using FastEndpoints;

namespace QuietRoom.Server.Endpoints;

public class GetEvents
{

    
    
    public record ResponseDto(string Name, string BuildingCode, string RoomNumber, string StartTime, string EndTime, ISet<char> DaysMet,
                           DateOnly StartDate, DateOnly EndDate);
}
