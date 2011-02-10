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
        object Activate(IResolutionContext context);
    }

    public interface IResolutionContext : IResolver
    {
        Type RequestedType { get; }

        Registration Registration { get; }

        object Activate();

        object GetInstance();
    }

    public class SingletonLifetime : ILifetime
    {
        private object locker = new object();
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
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = context.Activate();
                    }
                }
            }
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
        public object Activate(IResolutionContext context)
        {
            var constructors = context.Registration.ImplementationType.GetConstructors();
            var best = ChooseBestConstructor(constructors);

            var args = from p in best.GetParameters()
                       orderby p.Position
                       select context.Resolve(p.ParameterType);

            return best.Invoke(args.ToArray());
        }

        protected virtual ConstructorInfo ChooseBestConstructor(IEnumerable<ConstructorInfo> options)
        {
            return options.OrderByDescending(o => o.GetParameters().Length).Single();
        }
    }

    public class Registration
    {
        public IActivator Activator { get; set; }

        public ILifetime Lifetime { get; set; }

        public Type ImplementationType { get; private set; }

        public Registration(Type implementationType)
        {
            ImplementationType = implementationType;
        }
    }

    public class Container : IRegistrar, IResolver
    {
        private readonly ConcurrentDictionary<Type, Registration> _registrations = new ConcurrentDictionary<Type, Registration>();

        public void Register(Type type, Registration registration)
        {
            registration = AugmentRegistration(registration);
            _registrations.AddOrUpdate(type, registration, (t, r) => registration);
        }

        public object Resolve(Type type)
        {
            Func<Type, Registration> registrationFinder = LookupRegistration;
            var context = new ResolutionContext(type, registrationFinder);
            return context.GetInstance();
        }

        private Registration AugmentRegistration(Registration registration)
        {
            if (registration.Activator == null)
                registration.Activator = new Activator();
            if (registration.Lifetime == null)
                registration.Lifetime = new TransientLifetime();

            return registration;
        }

        private Registration LookupRegistration(Type type)
        {
            return _registrations.GetOrAdd(type,
                t => AugmentRegistration(new Registration(t)));
        }

        private class ResolutionContext : IResolutionContext, IResolver
        {
            private Func<Type, Registration> _registrationFinder;
            private ResolutionContext _parent;

            public Registration Registration { get; private set; }

            public Type RequestedType { get; private set; }

            public ResolutionContext(Type type, Func<Type, Registration> registrationFinder)
            {
                Registration = registrationFinder(type);
                RequestedType = type;
                _registrationFinder = registrationFinder;
            }

            public object Activate()
            {
                return Registration
                    .Activator
                    .Activate(this);
            }

            public object GetInstance()
            {
                return Registration
                    .Lifetime
                    .GetInstance(this);
            }

            public object Resolve(Type type)
            {
                var context = new ResolutionContext(type, _registrationFinder);
                context.SetParent(this);
                return context.GetInstance();
            }

            private void SetParent(ResolutionContext parent)
            {
                _parent = parent;
                CheckForCycles();
            }

            private void CheckForCycles()
            {
                var parent = _parent;
                while (parent != null)
                {
                    if (ReferenceEquals(Registration, parent.Registration))
                        throw new Exception("Cycles detected.");
                    parent = parent._parent;
                }
            }
        }
    }

    public static class IResolverExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            return (T)resolver.Resolve(typeof(T));
        }
    }
}