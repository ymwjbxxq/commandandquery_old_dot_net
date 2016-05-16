using System;

namespace CommandAndQuery.Queries.Exceptions
{
    public class QueryHandlerNotFoundException : Exception
    {
        public QueryHandlerNotFoundException(Type queryType, Type queryResult)
            : base($"Query handler not found for query type: {queryType}, and query result type: {queryResult}")
        {
        }
    }
}
