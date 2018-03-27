using System;
using RabbitMQ.Client;
using System.Text;

//Exemplo 01 - fila simples
class Send
{
    public static void Main()
    {
        //cria a conexão com a fila
        var factory = new ConnectionFactory() { HostName = "rabbit,rabbit2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declara a fila
            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            //montar a mensagem
            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            //publicr a mensagem
            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}