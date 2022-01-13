using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApi.Models;

[BsonIgnoreExtraElements]
public class Todo
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; } = string.Empty;

  public string Title { get; set; } = string.Empty;

  public string Note { get; set; } = string.Empty;

  public string Date { get; set; }

  public Boolean isComplete { get; set; } = false;
}