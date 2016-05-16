using System.Collections.Generic;

namespace CommandAndQuery.Commands
{
    public interface IServiceLocator
    {
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>();
    }
}
