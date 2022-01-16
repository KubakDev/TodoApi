namespace TodoApi.Models.Requests;

public class ListTodoItems
{
  public DateTime? From { get; set; }

  public DateTime? To { get; set; }

  public bool IsEmpty()
    => From is null && To is null;
}