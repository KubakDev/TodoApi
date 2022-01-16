using Api.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TodoApi.Interfaces;
using TodoApi.Models;
using TodoApi.Models.QueryOptions;

namespace TodoApi.Services
{
  public class TodosService
  {

    public IMongoCollection<Todo> Collection { get; init; }
    public static FilterDefinitionBuilder<Todo> Filter => Builders<Todo>.Filter;
    public static UpdateDefinitionBuilder<Todo> Update => Builders<Todo>.Update;

    public TodosService(IMongoCollection<Todo> collection)
    {
      Collection = collection;
    }


    public Task<List<Todo>> ListAsync(ITodoSearchOptions? searchOptions = null)
    {

      var filter = GetFilterFromSearchOptions(searchOptions);
      var find = Collection.Find(filter);

      return find.ToListAsync();
    }

    public async Task<Todo?> ListAsync(string id) =>
        await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Todo newTodo) =>
        await Collection.InsertOneAsync(newTodo);

    public async Task UpdateAsync(string id, Todo updatedTodo) =>
        await Collection.ReplaceOneAsync(x => x.Id == id, updatedTodo);

    public async Task RemoveAsync(string id) =>
        await Collection.DeleteOneAsync(x => x.Id == id);



    protected static FilterDefinition<Todo> GetFilterFromSearchOptions(in ITodoSearchOptions? searchOptions)
    {
      if (searchOptions?.IsEmpty() ?? true) return Filter.Empty;

      var filters = new List<FilterDefinition<Todo>>();

      if (searchOptions.Date != null)
        filters.Add(Filter.Gte(doc => doc.Date, searchOptions.Date));

      return filters.Count == 0
          ? Filter.Empty
          : filters.Count == 1
              ? filters[0]
              : Filter.And(filters);
    }

  }
}