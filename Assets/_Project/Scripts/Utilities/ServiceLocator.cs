using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T serviceInstance) => _services[typeof(T)] = serviceInstance;

    public static void Deregister<T>() => _services[typeof(T)] = null;

    public static T Get<T>() => (T)_services[typeof(T)];

    public static void Reset() => _services.Clear();
}