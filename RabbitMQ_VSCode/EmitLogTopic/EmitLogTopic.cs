using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

//Exemplo 05 - Topic 
class EmitLogTopic
{
    public static void Main(string[] args)
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "rabbirabbit2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração exchange
            channel.ExchangeDeclare(exchange: "topic_logs",
                                    type: "topic");

            //obtendo a chave da rota
            var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";

            //montando a mensagem
            var message = (args.Length > 1)
                          ? string.Join(" ", args.Skip( 1 ).ToArray())
                          : "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            //publicando a mensagem
            channel.BasicPublish(exchange: "topic_logs",
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }
}