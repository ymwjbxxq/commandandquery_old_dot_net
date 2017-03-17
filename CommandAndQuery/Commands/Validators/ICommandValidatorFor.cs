namespace CommandAndQuery.Commands.Validators
{
    public interface ICommandValidatorFor<in TCommand> where TCommand : ICommand
    {
        ValidationResponse Validate(TCommand command);
    }
}
