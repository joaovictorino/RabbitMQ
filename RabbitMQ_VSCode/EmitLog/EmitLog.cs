using System;
using RabbitMQ.Client;
using System.Text;

// Exemplo 03 pub sub
class EmitLog
{
    public static void Main(string[] args)
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "rabbit,rabbit2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração do exchange de broadcast fanout
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            //criar mensagem
            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            //publicar mensagem no exchange logs
            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0)
               ? string.Join(" ", args)
               : "info: Hello World!");
    }
}