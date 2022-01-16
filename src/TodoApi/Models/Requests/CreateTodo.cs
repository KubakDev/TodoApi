using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TodoApi.DataAnnotations;

namespace TodoApi.Models.Requests
{
  public class CreateTodo
  {

    public string Title { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;


    public DateTime Date { get; set; }

    public Boolean IsTimeAvailable { get; set; } = true;



  }
}