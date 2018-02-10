namespace CommandAndQuery.Commands.Validators
{
    public class Error
    {
        public Error(string message) : this(string.Empty, message)
        {
        }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get;}

        public string Message { get;}
    }
}
