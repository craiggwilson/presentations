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
        void Register(Type type, object instance);
    }

    public interface ILifetime
    {
        object Resolve(IResolver resolver);
    }

    public class SingletonLifetime : ILifetime
    {
        private readonly object _instance;

        public SingletonLifetime(object instance)
        {
            _instance = instance;
        }

        public object Resolve(IResolver resolver)
        {
            return _instance;
        }
    }

    public class TransientLifetime : ILifetime
    {
        private readonly Type _type;

        public TransientLifetime(Type type)
        {
            _type = type;
        }

        public object Resolve(IResolver resolver)
        {
            var constructors = _type.GetConstructors();
            var best = ChooseBestConstructor(constructors);

            var args = from p in best.GetParameters()
                       orderby p.Position
                       select resolver.Resolve(p.ParameterType);

            return best.Invoke(args.ToArray());
        }

        protected virtual ConstructorInfo ChooseBestConstructor(IEnumerable<ConstructorInfo> options)
        {
            //Pick the one with the most arguments...
            return options.OrderByDescending(o => o.GetParameters().Length).Single();
        }
    }

    public class Container : IRegistrar, IResolver
    {
        private readonly ConcurrentDictionary<Type, ILifetime> _registrations;

        public Container()
        {
            _registrations = new ConcurrentDictionary<Type, ILifetime>();
        }

        public void Register(Type type, object instance)
        {
            var lifetime = new SingletonLifetime(instance);
            _registrations.AddOrUpdate(type, lifetime, (t, l) => lifetime);
        }

        public object Resolve(Type type)
        {
            return _registrations.GetOrAdd(
                type,
                new TransientLifetime(type)).Resolve(this);
        }
    }
}