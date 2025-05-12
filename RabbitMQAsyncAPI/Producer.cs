using RabbitMQ.Client;
using System;
using System.Text;

class Producer
{
    public static void SendMessage(string message)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: new BasicProperties
                                 {
                                     DeliveryMode = 2 // Persistente
                                 },
                                 body: body);

            Console.WriteLine($"Mensagem enviada: {message}");
        }
    }
}
