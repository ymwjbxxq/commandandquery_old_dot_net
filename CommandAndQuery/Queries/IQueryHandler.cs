using System.Threading.Tasks;

namespace CommandAndQuery.Queries
{
    public interface IQueryHandler<TResult>
    {
        Task<TResult> Execute();
    }
}
