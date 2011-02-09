using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyIoC
{
    public interface IResolver
    {
        object Resolve(Type type);
    }

    public interface IRegistrar
    {
        void Register(Type type, Registration registration);
    }

    public interface ILifetime
    {
        object GetInstance(IResolutionContext context);
    }

    public interface IActivator
    {
        object Activate(Type type, IResolver resolver);
    }

    public interface IResolutionContext
    {
        Registration Registration { get; }

        IResolver Resolver { get; }

        object Activate();

        object GetInstance();
    }

    public class SingletonLifetime : ILifetime
    {
        private object _instance;

        public SingletonLifetime()
        { }

        public SingletonLifetime(object instance)
        {
            _instance = instance;
        }

        public object GetInstance(IResolutionContext context)
        {
            if (_instance == null)
                _instance = context.Activate();
            return _instance;
        }
    }

    public class TransientLifetime : ILifetime
    {
        public object GetInstance(IResolutionContext context)
        {
            return context.Activate();
        }
    }

    public class Activator : IActivator
    {
        public object Activate(Type type, IResolver resolver)
        {
            var constructors = type.GetConstructors();
            var best = ChooseBestConstructor(constructors);

            var args = from p in best.GetParameters()
                       orderby p.Position
                       select resolver.Resolve(p.ParameterType);

            return best.Invoke(args.ToArray());
        }

        protected virtual ConstructorInfo ChooseBestConstructor(IEnumerable<ConstructorInfo> options)
        {
            return options.OrderByDescending(o => o.GetParameters().Length).Single();
        }
    }

    public class ResolutionContext : IResolutionContext
    {
        private IActivator _activator;

        public Registration Registration { get; private set; }

        public IResolver Resolver { get; private set; }

        public ResolutionContext(Registration registration, IResolver resolver, IActivator activator)
        {
            Registration = registration;
            Resolver = resolver;
            _activator = activator;
        }

        public object Activate()
        {
            return _activator.Activate(Registration.ImplementationType, Resolver);
        }

        public object GetInstance()
        {
            return Registration.GetInstance(this);
        }
    }

    public class Registration
    {
        private ILifetime _lifetime;

        public Type ImplementationType { get; private set; }

        public Registration(Type implementationType, ILifetime lifetime)
        {
            ImplementationType = implementationType;
            _lifetime = lifetime;
        }

        public object GetInstance(ResolutionContext context)
        {
            return _lifetime.GetInstance(context);
        }
    }

    public class Container : IRegistrar, IResolver
    {
        private readonly ConcurrentDictionary<Type, Registration> _registrations = new ConcurrentDictionary<Type, Registration>();

        public void Register(Type type, Registration registration)
        {
            _registrations.AddOrUpdate(type, registration, (t, r) => registration);
        }

        public object Resolve(Type type)
        {
            var resolver = new ContextualResolver(t => _registrations.GetOrAdd(t, new Registration(t, new TransientLifetime())));
            return resolver.Resolve(type);
        }

        private class ContextualResolver : IResolver
        {
            private Func<Type, Registration> _registrationFinder;
            private Stack<Type> _typeChain = new Stack<Type>();

            public ContextualResolver(Func<Type, Registration> registrationFinder)
            {
                _registrationFinder = registrationFinder;
            }

            public object Resolve(Type type)
            {
                if (_typeChain.Contains(type))
                    throw new Exception(string.Format("Circular dependency found when resolving type {0}", type));

                _typeChain.Push(type);
                var registration = _registrationFinder(type);

                var context = new ResolutionContext(registration, this, new Activator());
                return context.GetInstance();
            }
        }
    }

    public static class IRegistrarExtensions
    {
        public static void Register<T>(this IRegistrar registrar, T instance)
        {
            Register(registrar, typeof(T), instance);
        }

        public static void Register(this IRegistrar registrar, Type type, object instance)
        {
            var lifetime = new SingletonLifetime(instance);
            var registration = new Registration(instance.GetType(), lifetime);
            registrar.Register(type, registration);
        }

        public static void RegisterSingleton<T, TConcrete>(this IRegistrar registrar)
        {
            RegisterSingleton(registrar, typeof(T), typeof(TConcrete));
        }

        public static void RegisterSingleton(this IRegistrar registrar, Type type, Type concreteType)
        {
            var lifetime = new SingletonLifetime();
            var registration = new Registration(concreteType, lifetime);
            registrar.Register(type, registration);
        }

        public static void RegisterTransient<T, TConcrete>(this IRegistrar registrar)
        {
            RegisterTransient(registrar, typeof(T), typeof(TConcrete));
        }

        public static void RegisterTransient(this IRegistrar registrar, Type type, Type concreteType)
        {
            var lifetime = new TransientLifetime();
            var registration = new Registration(concreteType, lifetime);
            registrar.Register(type, registration);
        }
    }
}