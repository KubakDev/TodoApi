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

  [HttpGet]
  public async Task<List<Todo>> List()
  {

    return await _todosService.ListAsync();
  }


  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Todo>> List([StringLength(24, MinimumLength = 24)] string id)
  {
    var todo = await _todosService.ListAsync(id);

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
  public async Task<IActionResult> Post(CreateTodo create)
  {
    string token = Request.Headers["Authorization"];
    token = token.Substring(7, token.Length - 7);

    var handler = new JwtSecurityTokenHandler();
    var decodedValue = handler.ReadJwtToken(token);

    if (string.IsNullOrEmpty(decodedValue.Payload.Sub))
      return BadRequest(BasicResponse.CouldNotCreateResource);


    Todo todo = new()
    {
      Title = create.Title,
      Note = create.Note,
      Date = create.Date,
      IsTimeAvailable = create.IsTimeAvailable
    };
    todo.UserId = decodedValue.Payload.Sub;

    await _todosService.CreateAsync(todo);

    if (await _todosService.ListAsync(todo.Id) is Todo createdOrder)
    {
      return Ok(createdOrder);
    }

    return BadRequest(BasicResponse.CouldNotCreateResource);

  }


  //TODO Cannot update todo
  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, CreateTodo update)
  {
    var existingTodo = await _todosService.ListAsync(id);

    Todo todo = new()
    {
      Title = update.Title,
      Note = update.Note,
      Date = update.Date,
      IsTimeAvailable = update.IsTimeAvailable
    };

    if (todo is null)
    {
      return NotFound();
    }


    await _todosService.UpdateAsync(id, todo);

    return NoContent();
  }

  [HttpDelete("{id:length(24)}")]
  public async Task<IActionResult> Delete(string id)
  {
    var todo = await _todosService.ListAsync(id);

    if (todo is null)
    {
      return NotFound();
    }

    await _todosService.RemoveAsync(todo.Id);

    return NoContent();
  }
}