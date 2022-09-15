namespace QuietRoom.Server.Services.Interfaces;

public interface IBuildingRepository
{
    public Task<IEnumerable<string>> GetBuildingNamesAsync();
}
