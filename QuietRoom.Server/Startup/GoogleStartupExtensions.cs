using QuietRoom.Server.Services;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Startup;

public static class GoogleStartupExtensions
{
    public static IServiceCollection UseGoogleServices(this IServiceCollection services)
    {
        services.AddSingleton<IRoomRetriever, FirestoreRoomRetriever>();
        return services;
    }
}
