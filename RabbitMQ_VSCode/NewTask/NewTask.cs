using System;
using RabbitMQ.Client;
using System.Text;

//Exemplo 02 Working Queues (dois consumidores)
class NewTask
{
    public static void Main(string[] args)
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "rabbit,rabbit2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração da fila durável
            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            //criação da mensagem
            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            //mensagem persistente
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            //publicação da mensagem
            channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        //Console.WriteLine(" Press [enter] to exit.");
        //Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}