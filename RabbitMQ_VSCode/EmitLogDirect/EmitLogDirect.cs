using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;

//Exemplo 04 - Routing mensagens pela chave
class EmitLogDirect
{
    public static void Main(string[] args)
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração do exchange como direct (filtro)
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");

            //obter tipo da mensagem
            var severity = (args.Length > 0) ? args[0] : "info";

            //mensagem
            var message = (args.Length > 1)
                          ? string.Join(" ", args.Skip( 1 ).ToArray())
                          : "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            //publicar mensagem informando o routingKey para direcionar para a fila certa
            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: severity,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}