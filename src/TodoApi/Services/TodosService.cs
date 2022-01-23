using MongoDB.Driver;
using TodoApi.Models;
using TodoApi.Models.Requests;

namespace TodoApi.Services
{
  public class TodosService
  {

    public IMongoCollection<Todo> Collection { get; init; }
    public static FilterDefinitionBuilder<Todo> Filter => Builders<Todo>.Filter;
    public static UpdateDefinitionBuilder<Todo> Update => Builders<Todo>.Update;

    private static FindOneAndReplaceOptions<Todo, Todo> _ReturnAfter = new() { ReturnDocument = ReturnDocument.After };

    public TodosService(IMongoCollection<Todo> collection)
    {
      Collection = collection;
    }


    public Task<List<Todo>> ListAsync(string userId, ListTodoItems? request = null)
    {

      var filter = GetFilterFromSearchOptions(request);
      filter &= Filter.Eq(x => x.UserId, userId);
      var find = Collection.Find(filter);

      return find.ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(string id, string userId)
    {

      var filter = Filter.Eq(x => x.UserId, userId);
      filter &= Filter.Eq(x => x.UserId, userId);

      return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Todo newTodo) =>
        await Collection.InsertOneAsync(newTodo);

    public async Task<Todo?> UpdateAsync(string id, string userId, Todo updatedTodo)
    {
      var filter = Filter.Eq(x => x.UserId, userId);
      filter &= Filter.Eq(x => x.UserId, userId);
      return await Collection.FindOneAndReplaceAsync(Filter.Eq(x => x.Id, id), updatedTodo, _ReturnAfter);
    }


    public async Task RemoveAsync(string id, string userId)
    {

      var filter = Filter.Eq(x => x.UserId, userId);
      filter &= Filter.Eq(x => x.UserId, userId);

      await Collection.DeleteOneAsync(x => x.Id == id);
    }

    protected static FilterDefinition<Todo> GetFilterFromSearchOptions(in ListTodoItems? request)
    {
      if (request?.IsEmpty() ?? true)
        return Filter.Empty;

      var filter = Filter.Empty;

      if (request.From is DateTime from)
        filter &= Filter.Gte(x => x.Date, from);

      if (request.To is DateTime to)
        filter &= Filter.Lte(x => x.Date, to);

      if (request.IsComplete is Boolean IsComplete)
        filter &= Filter.Eq(x => x.IsComplete, IsComplete);

      return filter;
    }

  }
}