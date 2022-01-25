using TodoApi.Models;
using TodoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using TodoApi.Models.Common;
using TodoApi.Models.Requests;
using System.Net.Mime;

namespace TodoApi.Controllers;

[ApiController]
[Route("todos")]
[Produces("application/json")]

[Consumes(MediaTypeNames.Application.Json)]
// [Authorize]
public class TodosController : ControllerBase
{
  private readonly TodosService _todosService;
  public TodosController(TodosService todosService) =>
      _todosService = todosService;


  // GET /todos?From=&To=

  [HttpGet]
  public Task<List<Todo>> List([FromQuery] ListTodoItems model)
    => _todosService.ListAsync(User.GetId(), model);


  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Todo>> GetTodoById([StringLength(24, MinimumLength = 24)] string id)
  {
    var todo = await _todosService.GetByIdAsync(id, User.GetId());

    if (todo is null)
    {
      return NotFound();
    }

    return todo;
  }

  [HttpPost]
  public async Task<ActionResult<Todo>> CreateTodo(CreateTodo create)
  {
    var todo = create.ToTodo(User.GetId());

    await _todosService.CreateAsync(todo);
    return Ok(todo);
  }


  [HttpPut("{id:length(24)}")]
  public async Task<ActionResult<Todo>> Update([IsBsonId] string id, CreateTodo update)
  {
    var todo = update.ToTodo(User.GetId());
    todo.Id = id;
    var updated = await _todosService.UpdateAsync(id, User.GetId(), todo);

    if (updated is null)
      return NotFound();

    return Ok(updated);
  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    var todo = await _todosService.GetByIdAsync(id, User.GetId());

    if (todo is null)
    {
      return NotFound();
    }

    await _todosService.RemoveAsync(todo.Id, User.GetId());

    return NoContent();
  }
}