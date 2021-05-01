﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "MSG", type: ExchangeType.Fanout);
                    // channel.QueueDeclare(queue: "MSG", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue:queueName, exchange:"MSG", routingKey:"");

                    Console.WriteLine("");
                    Console.WriteLine("RabbitMQ Demo: Receiver");
                    Console.WriteLine("press Enter to exit...");
                    Console.WriteLine("");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"Message received: '{message}'");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    Console.ReadLine();
                }
            }
        }
    }
}
