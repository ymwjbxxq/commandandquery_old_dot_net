using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommandAndQuery.Commands.Exceptions;

namespace CommandAndQuery.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IServiceLocator _serviceLocator;

        public CommandProcessor(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public async Task Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

            var handler = _serviceLocator.Resolve<ICommandHandler<TCommand>>();
            if (handler == null)
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand));
            }

            await handler.Handle(command);
        }

        public async Task<TResult> Process<TCommand, TResult>(TCommand command) where TCommand : ICommand
        {
            Validator.ValidateObject(command, new ValidationContext(command, null, null), true);

            var handler = _serviceLocator.Resolve<ICommandHandler<TCommand, TResult>>();
            if (handler == null)
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand), typeof(TResult));
            }

            return await handler.Handle(command);
        }
    }
}
