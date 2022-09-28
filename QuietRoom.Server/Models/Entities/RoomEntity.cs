using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace QuietRoom.Server.Models.Entities;

#pragma warning disable CS8618
[Table("rooms"), UsedImplicitly]
public class RoomEntity
{
    public string BuildingCode { get; set; }
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }
    public string RoomType { get; set; }
    
    public List<EventEntity> Events { get; set; }
}