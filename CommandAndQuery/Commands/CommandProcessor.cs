using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommandAndQuery.Commands.Exceptions;
using CommandAndQuery.Commands.Validators;

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

        public async Task<TResult> Process<TCommand, TResult>(TCommand command) 
            where TCommand : ICommand 
            where TResult : CommandResult, new()
        {
            var handler = _serviceLocator.Resolve<ICommandHandler<TCommand, TResult>>();
            if(handler == null)
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand), typeof(TResult));
            }

            var validators = _serviceLocator.ResolveAll<ICommandValidatorFor<TCommand>>().ToList();
            if (validators.Any())
            {
                var errors = new List<string>();
                foreach(var validator in validators)
                {
                    errors.AddRange(validator.Validate(command).Errors);
                }

                if(errors.Any())
                {
                    return new TResult
                    {
                        ValidationResult = new ValidationResponse
                        {
                            Errors = errors
                        }
                    };
                }
            }

            return await handler.Handle(command);
        }
    }
}
