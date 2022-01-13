using System.ComponentModel.DataAnnotations;

namespace TodoApi.Settings
{
  public class MongoDBOptions
  {
    public const string Config = "MongoDB";

    [Required]
    public string DatabaseName { get; set; } = "todoDatabase";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;
  }
}