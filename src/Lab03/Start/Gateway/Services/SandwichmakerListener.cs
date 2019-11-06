using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.Services
{
  public class SandwichmakerListener : IHostedService
  {
    readonly IConfiguration _config;
    readonly IWorkInProgress _wip;
    private readonly Queue _queue;

    public SandwichmakerListener(IConfiguration config, IWorkInProgress wip)
    {
      _config = config;
      _wip = wip;
      _queue = new Queue(_config["rabbitmq:url"], "customer");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _queue.StartListening<Messages.SandwichResponse>((ea, response) =>
      {
        _wip.CompleteWork(ea.BasicProperties.CorrelationId, response);
      });

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _queue.Dispose();

      return Task.CompletedTask;
    }
  }
}
