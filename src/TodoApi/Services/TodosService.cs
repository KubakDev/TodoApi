using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Services
{
  public class TodosService
  {

    public IMongoCollection<Todo> Collection { get; init; }

    public TodosService(IMongoCollection<Todo> collection)
    {
      Collection = collection;
    }


    public async Task<List<Todo>> GetAsync() =>
        await Collection.Find(_ => true).ToListAsync();

    public async Task<Todo?> GetAsync(string id) =>
        await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Todo newTodo) =>
        await Collection.InsertOneAsync(newTodo);

    public async Task UpdateAsync(string id, Todo updatedTodo) =>
        await Collection.ReplaceOneAsync(x => x.Id == id, updatedTodo);

    public async Task RemoveAsync(string id) =>
        await Collection.DeleteOneAsync(x => x.Id == id);

  }
}