using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Concurrent;
using System.Reflection;

namespace MyIoC
{
    /*Goals: 
     * 1) Resolve a type by instantiating it
     * 2) Resolve a type  by looking it up
     * 3) Resolve a type with dependencies
    */
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
        object GetInstance(ResolutionContext context);
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

        public object GetInstance(ResolutionContext context)
        {
            if (_instance == null)
                _instance = context.Activate();
            return _instance;
        }
    }

    public class TransientLifetime : ILifetime
    {
        public object GetInstance(ResolutionContext context)
        {
            return context.Activate();
        }
    }

    public interface IActivator
    {
        object Activate(Type type, IResolver resolver);
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

    public class ResolutionContext
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
        private readonly ConcurrentDictionary<Type, Registration> _registrations;

        public Container()
        {
            _registrations = new ConcurrentDictionary<Type, Registration>();
        }

        public void Register(Type type, Registration registration)
        {
            _registrations.AddOrUpdate(type, registration, (t, r) => registration);
        }

        public object Resolve(Type type)
        {
            var registration = _registrations.GetOrAdd(
                type,
                new Registration(type, new TransientLifetime()));

            var context = new ResolutionContext(registration, this, new Activator());
            return context.GetInstance();
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