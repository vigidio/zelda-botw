namespace Game.EventHandler
{
    using System;
    using System.Threading.Tasks;
    using DataContracts;
    using KafkaFlow;
    using KafkaFlow.TypedHandler;

    public class NewGameHandler : IMessageHandler<NewGameStarted>
    {
        public Task Handle(IMessageContext context, NewGameStarted message)
        {
            Console.WriteLine($"{nameof(NewGameStarted)} received");
            return Task.CompletedTask;
        }
    }
}