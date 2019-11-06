using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Services
{
  public interface ISandwichRequestor
  {
    Task<Messages.SandwichResponse> RequestSandwich(Messages.SandwichRequest request);
  }
}
