using Google.Cloud.Firestore;
using JetBrains.Annotations;
using QuietRoom.Server.Services.Interfaces;
using QuietRoom.Server.Utils;

namespace QuietRoom.Server.Services;

/// <summary>
/// Implementation of the room retriever service using Firestore.
/// </summary>
public class FirestoreDbManager : IRoomRetriever, IBuildingRepository
{
    private const string BUILDINGS_COLLECTION = "buildings";
    private const string ROOMS_COLLECTION = "rooms";
    private const string TERMS_COLLECTION = "terms";
    private const string EVENTS_COLLECTION = "events";
    private const string TERM = "20229";
    private const string PROJECT_ID = "quietroom";
    
    private readonly ILogger<FirestoreDbManager> _logger;
    private readonly FirestoreDb _db;
    
    public FirestoreDbManager(ILogger<FirestoreDbManager> logger)
    {
        _logger = logger;
        _db = FirestoreDb.Create(PROJECT_ID);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime, DayOfWeek day)
    {
        var roomsCollection = _db.Collection(BUILDINGS_COLLECTION)
            .Document(buildingCode)
            .Collection(ROOMS_COLLECTION);
        var rooms = roomsCollection.ListDocumentsAsync();
        var roomsAvailable = new HashSet<string>();
        await ForEachConcurrentAsync(rooms,
            async reference =>
            {
                var events = reference.Collection(TERMS_COLLECTION).Document(TERM).Collection(EVENTS_COLLECTION).ListDocumentsAsync();
                var isAvailable = true;
                // ReSharper disable once LoopCanBeConvertedToQuery
                await foreach (var termEventRef in events)
                {
                    var firestoreEvent = (await termEventRef.GetSnapshotAsync()).ConvertTo<FirestoreEvent>();
                    if (!IsConflict(firestoreEvent, startTime, endTime, day)) continue;
                    isAvailable = false;
                    break;
                }
        
                if (isAvailable)
                {
                    _logger.LogInformation("Room {Room} is available", reference.Id);
                    roomsAvailable.Add(reference.Id);
                }
            });
        _logger.LogInformation("For Building {Building} with StartTime {StartTime} and EndTime {EndTime}, found {Count} available rooms", 
            buildingCode, startTime, endTime, roomsAvailable.Count);
        return roomsAvailable;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetBuildingNamesAsync()
    {
        var buildings = _db.Collection(BUILDINGS_COLLECTION).ListDocumentsAsync();
        return await buildings.Select(reference => reference.Id).ToHashSetAsync();
    }

    private static async Task ForEachConcurrentAsync<T>(
        IAsyncEnumerable<T> source,
        Func<T, Task> action)
    {
        await Task.WhenAll(source.Select(action).ToEnumerable());
    }

    private static bool IsConflict(FirestoreEvent firestoreEvent, TimeOnly startTime, TimeOnly endTime, DayOfWeek dayOfWeek)
    {
        var eventStart = TimeOnlyUtils.ParseTime(firestoreEvent.StartTime);
        var eventEnd = TimeOnlyUtils.ParseTime(firestoreEvent.EndTime);
        return firestoreEvent.DaysMet.Any(s => s == dayOfWeek.ToString())
            && firestoreEvent.StartDate <= DateTimeOffset.Now 
            && firestoreEvent.EndDate >= DateTimeOffset.Now 
            && (eventStart <= startTime && eventEnd >= startTime || eventStart <= endTime && eventEnd >= endTime);
    }

    [UsedImplicitly]
    [FirestoreData]
    private class FirestoreEvent
    {
        public FirestoreEvent()
        {
            DaysMet = new List<string>();
            StartDate = DateTimeOffset.MinValue;
            EndDate = DateTimeOffset.MinValue;
            StartTime = null!;
            EndTime = null!;
            Name = null!;
        }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public List<string> DaysMet { get; set; }
        [FirestoreProperty]
        public DateTimeOffset StartDate { get; set; }
        [FirestoreProperty]
        public DateTimeOffset EndDate { get; set; }
        [FirestoreProperty]
        public string StartTime { get; set; }
        [FirestoreProperty]
        public string EndTime { get; set; }
    }
}
