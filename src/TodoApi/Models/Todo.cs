using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApi.Models;

public class Todo
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; } = string.Empty;

  public string UserId { get; set; } = string.Empty;

  public string Title { get; set; } = string.Empty;

  public string Note { get; set; } = string.Empty;

  public DateTime Date { get; set; }

  public bool IsComplete { get; set; } = false;

  public bool IsTimeAvailable { get; set; } = true;
}