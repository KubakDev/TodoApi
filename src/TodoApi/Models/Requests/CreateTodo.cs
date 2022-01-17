
namespace TodoApi.Models.Requests;
public class CreateTodo
{

  public string Title { get; set; } = string.Empty;

  public string Note { get; set; } = string.Empty;


  public DateTime Date { get; set; }

  public bool IsTimeAvailable { get; set; } = true;

  public Todo ToTodo(in string userId) => new()
  {
    Note = Note,
    Date = Date,
    Title = Title,
    UserId = userId,
    IsTimeAvailable = IsTimeAvailable,
  };

}
