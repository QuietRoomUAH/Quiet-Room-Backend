using QuietRoom.Server.Services.Db;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Startup;

/// <summary>
/// Startup extensions to use a db context with supabase
/// </summary>
public static class SupabaseStartupExtensions
{
    /// <summary>
    /// Adds a supabase db context to the service collection
    /// </summary>
    public static void UseSupabase(this IServiceCollection services)
    {
        services.AddDbContext<SupabaseDbContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.AddTransient<IBuildingRepository, SupabaseDbManager>();
        services.AddTransient<IRoomRepository, SupabaseDbManager>();
    }
}
