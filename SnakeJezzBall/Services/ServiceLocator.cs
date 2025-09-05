namespace SnakeJezzBall.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<TInterface>(TInterface implementation)
        {
            var interfaceType = typeof(TInterface);

            if (_services.ContainsKey(interfaceType))
            {
                _services[interfaceType] = implementation!;
            }
            else
            {
                _services.Add(interfaceType, implementation!);
            }
        }

        public static TInterface Get<TInterface>()
        {
            var interfaceType = typeof(TInterface);

            if (_services.TryGetValue(interfaceType, out var service))
            {
                return (TInterface)service;
            }

            throw new InvalidOperationException($"Service de type {interfaceType.Name} n'est pas enregistré.");
        }
    }
}