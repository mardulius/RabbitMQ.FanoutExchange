
using RabbitMQ.Client;
using System.Text;

class Program
{

    static void Main(string[] args)
    {
       var random = new Random();

        do
        {
            int timeToSleep = random.Next(1000, 4000);
            Thread.Sleep(timeToSleep);

            var factory = new ConnectionFactory() { HostName = "localhost" };

            var connection = factory.CreateConnection();

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

                var moneyCount = random.Next(1000, 10000);

                string message = $"Поступил перевод на сумму: {moneyCount}";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "notifier",
                    routingKey: "",
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"сообщение{message} - раcпределено c помощью FanoutExchange 'notifier'");
            }
        }
        while (true);
    }

   
}
