using System;
using System.Collections.Generic;
using System.Linq;
using TodoApi.Interfaces;

namespace Api.Interfaces
{
  public interface ITodoSearchOptions : ISearchOptions
  {
    DateTime? Date { get; set; }

    new bool HasAny() => !IsEmpty();

    new bool IsEmpty() => Date == null;
  }
}