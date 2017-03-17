using System.Collections.Generic;
using System.Linq;

namespace CommandAndQuery.Commands.Validators
{
    public class ValidationResponse
    {
        public bool IsValid => !Errors.Any();

        public IEnumerable<string> Errors { get; set; }
    }
}
