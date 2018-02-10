using CommandAndQuery.Commands;

namespace CommandAndQuery.Tests.Fakes
{
    public class MyCommandRequest : ICommand
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
