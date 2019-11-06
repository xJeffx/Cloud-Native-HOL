﻿using System;
using Microsoft.Extensions.Configuration;
using RabbitQueue;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace BreadService
{
  class Program
  {
    private volatile static int _inventory = 10;
    private static Queue _queue;

    static async Task Main(string[] args)
    {
      var config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

      if (_queue == null)
        _queue = new Queue(config["rabbitmq:url"], "breadbin");

      Console.WriteLine("### Bread bin service starting to listen");
      _queue.StartListening<Messages.BreadBinRequest>(HandleMessage);

      // wait forever - we run until the container is stopped
      await new AsyncManualResetEvent().WaitAsync();
    }

    private static void HandleMessage(BasicDeliverEventArgs ea, Messages.BreadBinRequest request)
    {
      var response = new Messages.BreadBinResponse();
      lock (_queue)
      {
        if (request.Returning)
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - returned");
          _inventory++;
        }
        else if (_inventory > 0)
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - filled");
          _inventory--;
          response.Success = true;
          _queue.SendReply(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
        }
        else
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - no inventory");
          response.Success = false;
          _queue.SendReply(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
        }
      }
    }
  }
}
