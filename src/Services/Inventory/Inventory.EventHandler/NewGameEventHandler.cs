namespace Inventory.EventHandler
{
    using System;
    using System.Threading.Tasks;
    using DataContracts;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.UseCases.NewGame;
    using KafkaFlow;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.Logging;

    public class NewGameEventHandler : IMessageHandler<NewGameStarted>
    {
        private readonly ICommandHandler<NewGameCommand> commandHandler;
        
        private readonly ILogger<NewGameEventHandler> logger;
        
        public NewGameEventHandler(
            ICommandHandler<NewGameCommand> commandHandler,
            ILogger<NewGameEventHandler> logger)
        {
            this.commandHandler = commandHandler ??
                                  throw new ArgumentNullException($"{nameof(commandHandler)}",
                                      "command handler cannot be null");
            this.logger = logger ??
                          throw new ArgumentNullException($"{nameof(logger)}",
                              "command handler cannot be null");;
        }
        
        public async Task Handle(IMessageContext context, NewGameStarted message)
        {
            this.logger.LogInformation($"{nameof(NewGameStarted)} event received");
            
            var newGameCommand = new NewGameCommand(message.UserId);

            await commandHandler.Handle(newGameCommand);
            
            this.logger.LogInformation($"{nameof(NewGameStarted)} event processed");
        }
    }
}