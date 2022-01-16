
using System.Collections.Generic;
using System.Linq;

namespace TodoApi.Interfaces
{
  public interface ISearchOptions
  {
    DateTime? Date { get; set; }

    bool HasAny() => !IsEmpty();

    bool IsEmpty() => string.IsNullOrEmpty(Date.ToString());
  }
}