namespace QuietRoom.Server.Models;

public record RoomDto(string BuildingCode, string RoomNumber, int Capacity, string? RoomType);
