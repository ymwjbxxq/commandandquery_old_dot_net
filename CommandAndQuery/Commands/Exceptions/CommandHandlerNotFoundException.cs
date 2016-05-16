using System;

namespace CommandAndQuery.Commands.Exceptions
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(Type type)
            : base($"Command handler not found for command type: {type}")
        {
        }

        public CommandHandlerNotFoundException(Type commandType, Type commandResult)
            : base(
                $"Command handler not found for command type: {commandType}, and command result type: {commandResult}")
        {
        }
    }
}
