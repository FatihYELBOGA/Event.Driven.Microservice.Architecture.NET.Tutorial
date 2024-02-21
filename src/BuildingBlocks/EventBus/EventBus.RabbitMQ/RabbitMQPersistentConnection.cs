using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {

        private readonly IConnectionFactory connectionFactory;
        private IConnection connection;
        private object lockObject = new object();
        private readonly int retryCount;
        private bool disposed;

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        public bool IsConnected => connection != null && connection.IsOpen;

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }

        public void Dispose()
        {
            disposed = true;
            connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lockObject)
            {
                var policy = Policy.Handle<SocketException>().
                    Or<BrokerUnreachableException>().
                    WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => 
                        {

                        } 
                    );

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutDown;
                    connection.CallbackException += Connection_CallbackException;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;


                    return true;
                }

                return false;
            }
        }

        private void Connection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (disposed) return;

            TryConnect();
        }

        private void Connection_CallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (disposed) return;

            TryConnect();
        }

        private void Connection_ConnectionShutDown(object? sender, ShutdownEventArgs e)
        {
            if (disposed) return;

            TryConnect();
        }
    }

}
