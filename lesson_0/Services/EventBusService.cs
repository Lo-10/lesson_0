
using Autofac;
using Autofac.Core;
using lesson_0.Accession;
using lesson_0.Models;
using lesson_0.Models.Requests.Post;
using MediatR;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace lesson_0.Services
{
    public class EventBusService : IEventBusService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILifetimeScope _scope;

        public EventBusService(ILifetimeScope scope)
        {
            _factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariables()["rabbit_server"].ToString(),
                Port = (int)Environment.GetEnvironmentVariables()["rabbit_port"],
                UserName = Environment.GetEnvironmentVariables()["rabbit_user"].ToString(),
                Password = Environment.GetEnvironmentVariables()["rabbit_password"].ToString()
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _scope = scope;
        }

        public async void Connect(string userId)
        {
            // Создаем для каждого пользователя очередь (TODO ограничить время жизни)
            _channel.QueueDeclare(queue: userId, durable: true, exclusive: false, autoDelete: false);
            // Связываем созданную очередь с точкой обмена по ключу userId
            _channel.QueueBind(queue: userId, "posts", routingKey: userId);

            var consumer = new EventingBasicConsumer(_channel);

            // При получении сообщения из очереди, транслируем его веб-сокет клиенту через фреймворк SignalR
            consumer.Received += async delegate (object model, BasicDeliverEventArgs ea)
            {
                // Send message to all users in SignalR
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                var m = JsonConvert.DeserializeObject<PostModel>(body);

                var ws = _scope.Resolve<WSServer>();
                await ws.PostFeedPosted(userId, m);
            };

            // Consume a RabbitMQ Queue
            _channel.BasicConsume(queue: userId, autoAck: true, consumer: consumer);
        }

        public async Task SendMessageAsync(string userId, object message, CancellationToken cancellationToken)
        {
            var body = JsonConvert.SerializeObject(message);

            _channel.BasicPublish(exchange: "posts",
             routingKey: userId,
             body: Encoding.UTF8.GetBytes(body));
        }
    }
}
