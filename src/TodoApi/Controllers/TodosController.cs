using TodoApi.Models;
using TodoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;

namespace TodoApi.Controllers;

[ApiController]
[Route("todos")]
[Produces("application/json")]
// [Authorize]
public class TodosController : ControllerBase
{
  private readonly TodosService _todosService;
  public TodosController(TodosService todosService) =>
      _todosService = todosService;

  [HttpGet]
  public async Task<List<Todo>> Get()
  {

    return await _todosService.GetAsync();
  }


  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Todo>> Get([StringLength(24, MinimumLength = 24)] string id)
  {
    var todo = await _todosService.GetAsync(id);

    if (todo is null)
    {
      return NotFound();
    }

    return todo;
  }

  // [HttpGet]
  // [AllowAnonymous]
  // public ActionResult<BsonDocument> GetNice()
  // {
  //   return Ok(new BsonDocument());
  // }

  [HttpPost]
  public async Task<IActionResult> Post(Todo newTodo)
  {
    string token = Request.Headers["Authorization"];
    token = token.Substring(7, token.Length - 7);

    var handler = new JwtSecurityTokenHandler();
    var decodedValue = handler.ReadJwtToken(token);
    Console.WriteLine(decodedValue.Payload.Sub);

    newTodo.UserId = decodedValue.Payload.Sub;
    await _todosService.CreateAsync(newTodo);


    return CreatedAtAction(nameof(Get), new { id = newTodo.Id }, newTodo);
  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Todo updatedTodo)
  {
    var todo = await _todosService.GetAsync(id);

    if (todo is null)
    {
      return NotFound();
    }

    updatedTodo.Id = todo.Id;

    await _todosService.UpdateAsync(id, updatedTodo);

    return NoContent();
  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    var todo = await _todosService.GetAsync(id);

    if (todo is null)
    {
      return NotFound();
    }

    await _todosService.RemoveAsync(todo.Id);

    return NoContent();
  }
}