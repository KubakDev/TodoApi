using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoApi.Models;
[BsonIgnoreExtraElements]
public class User
{
  [BsonId]
  [BsonIgnore]
  public string? Id { get; set; }

  public string? Title { get; set; }

  public string Note { get; set; } = string.Empty; public string Date { get; set; } = null!;
}