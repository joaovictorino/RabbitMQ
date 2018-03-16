using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Exemplo 03 pub sub
class ReceiveLogs
{
    public static void Main()
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração do exchange de broadcast fanout
            channel.ExchangeDeclare(exchange: "logs", type: "fanout");

            //buscar fila criada dinamicamente pelo exchange
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: "");

            Console.WriteLine(" [*] Waiting for logs.");

            //evento disparado para cada mensagem
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0}", message);
            };

            //assinar a fila
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}