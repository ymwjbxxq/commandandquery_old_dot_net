namespace CommandAndQuery.Commands
{
    public class ServiceLocator
    {
        private static IServiceLocator _serviceLocator;
        private static readonly object _locker = new object();

        public static IServiceLocator Current => _serviceLocator;

        public static void SetServiceLocator(IServiceLocator serviceLocator)
        {
            lock (_locker)
            {
                _serviceLocator = serviceLocator;
            }
        }
    }
}
