using System;
using System.Collections.Generic;
using System.Text;

namespace BreadService.Messages
{
  internal class BreadBinRequest
  {
    public string Bread { get; set; }
    public bool Returning { get; set; }
  }
}
