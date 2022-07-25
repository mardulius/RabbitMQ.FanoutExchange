using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


class Program
{
    private static double allCountTax = 0;
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        var connection = factory.CreateConnection();

        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

            var queueName = channel.QueueDeclare("Tax").QueueName;
            channel.QueueBind(queueName, "notifier", "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                var payment = GetPaymenet(message);
                // 5% c суммы
                allCountTax += payment * 0.05;

                Console.WriteLine($"Получены средства:{payment}, будет удержан налог в размере:{payment * 0.05m}");
                Console.WriteLine($"Общяя сумма налога:{allCountTax}");
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