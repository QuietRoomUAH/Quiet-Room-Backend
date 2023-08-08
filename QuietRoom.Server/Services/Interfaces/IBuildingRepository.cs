using QuietRoom.Server.Models;

namespace QuietRoom.Server.Services.Interfaces;

public interface IBuildingRepository
{
    public Task<IEnumerable<string>> GetBuildingNamesAsync();
    
    public Task<IEnumerable<RoomDto>> GetRoomNumbersAsync(string buildingCode);
}
