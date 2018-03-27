using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Exemplo 04 - Routing mensagens pela chave
class ReceiveLogsDirect
{
    public static void Main(string[] args)
    {
        //conexão
        var factory = new ConnectionFactory() { HostName = "rabbit,rabbit2" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            //declaração do exchange como direct (filtro)
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");

            //obtendo a fila gerada dinamicamente
            var queueName = channel.QueueDeclare().QueueName;

            //gera erro se não vier nos parametro a routingKey da fila
            if(args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                                        Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return;
            }

            //fazendo bind com a fila para cada routingKey informada
            foreach(var severity in args)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: "direct_logs",
                                  routingKey: severity);
            }

            Console.WriteLine(" [*] Waiting for messages.");

            //evento para receber a mensagem
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey, message);
            };

            //assinando a fila
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}