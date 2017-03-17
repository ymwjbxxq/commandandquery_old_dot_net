using System.Threading.Tasks;

namespace CommandAndQuery.Commands
{
    public interface ICommandProcessor
    {
        Task Process<TCommand>(TCommand command) where TCommand : ICommand;

        Task<TResult> Process<TCommand, TResult>(TCommand command) 
            where TCommand : ICommand
            where TResult : CommandResult, new();
    }
}
