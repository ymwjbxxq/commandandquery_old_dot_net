using System.Collections.Generic;
using System.Linq;

namespace CommandAndQuery.Commands.Validators
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            Errors = Enumerable.Empty<Error>();
        }

        public bool IsValid => !Errors.Any();

        public IEnumerable<Error> Errors { get; set; }
    }
}
