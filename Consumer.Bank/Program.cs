using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


class Program
{
    private static int allMoneyCount = 0;
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        var connection = factory.CreateConnection();

        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

            var queueName = channel.QueueDeclare("Bank").QueueName;
            channel.QueueBind(queueName, "notifier", "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                var payment = GetPaymenet(message);
                allMoneyCount += payment;
                Console.WriteLine($"Получены средства в размере :{payment}");
                Console.WriteLine($"Общая сумма на счету:{allMoneyCount}");
            };
            channel.BasicConsume(queueName, true, consumer);
            Console.WriteLine($"подписались на очередь {queueName}");
            
            Console.ReadLine();
        }
    }

    private static int GetPaymenet(string message)
    {
        return Convert.ToInt32(message.Split(' ').Last());
    }
}