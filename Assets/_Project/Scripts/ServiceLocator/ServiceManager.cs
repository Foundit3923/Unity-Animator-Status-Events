using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityServiceLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> services = new();
        public IEnumerable<object> RegisteredServices => services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        public ServiceManager Register<T>(T service, bool overrideExisting = false)
        {
            Type type = typeof(T);

            if (!services.TryAdd(type, service))
            {
                if (overrideExisting)
                {
                    services[type] = service;
                }
                else
                {
                    Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
                }
            }

            return this;
        }

        public ServiceManager Register(Type type, object service, bool overrideExisting = false)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            if (!services.TryAdd(type, service))
            {
                if (overrideExisting)
                {
                    services[type] = service;
                }
                else
                {
                    Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
                }
            }

            return this;
        }
    }
}