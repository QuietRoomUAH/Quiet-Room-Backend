using Microsoft.EntityFrameworkCore;
using QuietRoom.Server.Models.Entities;

namespace QuietRoom.Server.Services.Db;

public class SupabaseDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<EventEntity> Events => Set<EventEntity>();
    public DbSet<EventEntity.DayMetEntity> DaysMet => Set<EventEntity.DayMetEntity>();

    public SupabaseDbContext(DbContextOptions<SupabaseDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql($"{_configuration["ConnectionStrings:Supabase"]}Password={_configuration["DB_PASS"]}")
            .EnableSensitiveDataLogging()
            .UseSnakeCaseNamingConvention();
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoomEntity>()
            .HasKey(entity => new { entity.BuildingCode, entity.RoomNumber });
        modelBuilder.Entity<EventEntity>()
            .HasOne(entity => entity.Room)
            .WithMany(room => room.Events)
            .HasForeignKey(entity => new { entity.BuildingCode, entity.RoomNumber });
        modelBuilder.Entity<EventEntity.DayMetEntity>()
            .HasNoKey()
            .HasOne(entity => entity.Event);
    }
}
