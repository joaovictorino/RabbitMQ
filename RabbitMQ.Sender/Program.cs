using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            //ExemploAprovaRapido(args);

            //ExemploFilaSimples(args);

            //ExemploVariosConsumidores(args);

            ExemploPubSub(args);
        }

        private static void ExemploPubSub(string[] args)
        {
        }

        private static void ExemploVariosConsumidores(string[] args)
        {
            //cria conexão
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //declara afila como durável
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                //criar a mensagem
                var message = GetMessageBroadcast(args);
                var body = Encoding.UTF8.GetBytes(message);

                //declara a mensagem como persistente
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                //publica a mensagem na fila
                channel.BasicPublish(exchange: "",
                                     routingKey: "task_queue",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessageBroadcast(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }

        private static void ExemploFilaSimples(string[] args)
        {
            //conexão
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //declaração fila como não durável
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                //mensagem
                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                //publicação na fila
                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void ExemploAprovaRapido(string[] args)
        {
            //conexão
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //parametros da fila
                Dictionary<string, object> valores = new Dictionary<string, object>();
                //redirecionamento no timeout
                valores.Add("x-dead-letter-exchange", "logs");
                //tempo de timeout
                valores.Add("x-message-ttl", 5000);
                //declaração da fila
                channel.QueueDeclare(queue: "logs_dlx",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: valores);

                //texto da mensagem
                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                //publicação da mensagem na fila
                channel.BasicPublish(exchange: "",
                                     routingKey: "logs_dlx",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            //conexão
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //declaração do exchange
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                //montar a mensagem
                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                //publicação na fila
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
}
