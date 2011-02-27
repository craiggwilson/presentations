using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;

namespace Simple.IoC
{
    #region Activation

    public interface IActivator
    {
        object Activate(IResolutionContext context);
    }

    public class ReflectionActivator : IActivator
    {
        public object Activate(IResolutionContext context)
        {
            var ctor = GetBestConstructor(context.Registration.ConcreteType);

            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = context.Resolve(parameters[i].ParameterType);
            }

            return Activator.CreateInstance(context.Registration.ConcreteType, args);
        }

        protected virtual ConstructorInfo GetBestConstructor(Type type)
        {
            var ctors = type.GetConstructors();
            return ctors.OrderByDescending(x => x.GetParameters().Length).First();
        }
    }

    #endregion

    #region Lifetime

    public interface ILifetime
    {
        object GetInstance(IResolutionContext context);
    }

    public abstract class LifetimeBase : ILifetime
    {
        public abstract object GetInstance(IResolutionContext context);

        protected void RegisterInstanceForDisposalIfNecessary(IContainer container, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                container.RegisterContainerDisposalCallback(() => disposable.Dispose());
        }
    }

    public class TransientLifetime : LifetimeBase
    {
        public override object GetInstance(IResolutionContext context)
        {
            var instance = context.Activate();
            RegisterInstanceForDisposalIfNecessary(context.Container, instance);
            return instance;
        }
    }

    public class ContainerScopedLifetime : LifetimeBase
    {
        private ConcurrentDictionary<Guid, object> _instances = new ConcurrentDictionary<Guid, object>();

        public override object GetInstance(IResolutionContext context)
        {
            return _instances.GetOrAdd(context.Container.Id, g =>
            {
                var i = context.Activate();
                context.Container.RegisterContainerDisposalCallback(() => ContainerDisposed(g));
                RegisterInstanceForDisposalIfNecessary(context.Container, i);
                return i;
            });
        }

        private void ContainerDisposed(Guid containerId)
        {
            object value;
            _instances.TryRemove(containerId, out value);
        }
    }

    public class SingletonLifetime : LifetimeBase
    {
        private object locker = new object();
        private object _instance;

        public override object GetInstance(IResolutionContext context)
        {
            if (_instance == null)
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = context.Activate();
                        var rootContainer = context.Container.GetRootContainer();
                        RegisterInstanceForDisposalIfNecessary(rootContainer, _instance);
                    }
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Registration

    public class Registration
    {
        public IActivator Activator { get; set; }

        public ISet<Type> Aliases { get; private set; }

        public Type ConcreteType { get; private set; }

        public ILifetime Lifetime { get; set; }

        public Registration(Type concreteType)
        {
            Activator = new ReflectionActivator();
            Aliases = new HashSet<Type>();
            Aliases.Add(concreteType);
            ConcreteType = concreteType;
            Lifetime = new TransientLifetime();
        }
    }

    public static class RegistrationExtensions
    {
        public static Registration ActivatedBy(this Registration registration, IActivator activator)
        {
            registration.Activator = activator;
            return registration;
        }

        public static Registration ActivatedBy(this Registration registration, Func<IResolutionContext, object> activator)
        {
            registration.Activator = new DelegateActivator(activator);
            return registration;
        }

        public static Registration As<T>(this Registration registration)
        {
            return As(registration, typeof(T));
        }

        public static Registration As(this Registration registration, Type alias)
        {
            registration.Aliases.Add(alias);
            return registration;
        }

        public static Registration ContainerScoped(this Registration registration)
        {
            registration.Lifetime = new ContainerScopedLifetime();
            return registration;
        }

        public static Registration Singleton(this Registration registration)
        {
            registration.Lifetime = new SingletonLifetime();
            return registration;
        }

        public class DelegateActivator : IActivator
        {
            private Func<IResolutionContext, object> _activator;

            public DelegateActivator(Func<IResolutionContext, object> activator)
            {
                _activator = activator;
            }

            public object Activate(IResolutionContext context)
            {
                return _activator(context);
            }
        }
    }


    #endregion

    #region Registration Provider

    public interface IRegistrationProvider
    {
        IEnumerable<Registration> GetRegistrations(Type type);
    }

    public class AnythingRegistrationProvider : IRegistrationProvider
    {
        public IEnumerable<Registration> GetRegistrations(Type type)
        {
            if (!type.IsAbstract && type.IsClass)
            {
                yield return new Registration(type);
            }
        }
    }


    #endregion

    #region Resolution Context

    public interface IResolutionContext
    {
        IContainer Container { get; }

        Registration Registration { get; }

        object Resolve(Type type);
    }

    public static class ResolutionContextExtensions
    {
        public static T Resolve<T>(this IResolutionContext context)
        {
            return (T)context.Resolve(typeof(T));
        }

        public static object Activate(this IResolutionContext context)
        {
            return context.Registration.Activator.Activate(context);
        }

        public static object GetInstance(this IResolutionContext context)
        {
            return context.Registration.Lifetime.GetInstance(context);
        }
    }

    #endregion

    #region Container

    public interface IContainer : IDisposable
    {
        Guid Id { get; }

        IContainer Parent { get; }

        IContainer CreateChildContainer();

        void RegisterContainerDisposalCallback(Action callback);

        object Resolve(Type type);
    }

    public static class ContainerExtensions
    {
        public static IContainer GetRootContainer(this IContainer container)
        {
            container = container.Parent;
            while (container != null)
                container = container.Parent;

            return container;
        }

        public static T Resolve<T>(this IContainer container)
        {
            return (T)container.Resolve(typeof(T));
        }
    }

    #endregion

    #region ContainerBuilder

    public class ContainerBuilder : IRegistrationProvider
    {
        private List<Registration> _registrations = new List<Registration>();
        private List<IRegistrationProvider> _registrationProviders = new List<IRegistrationProvider>();

        public void AddRegistration(Registration registration)
        {
            _registrations.Add(registration);
        }

        public void AddRegistrationProvider(IRegistrationProvider registrationProvider)
        {
            _registrationProviders.Add(registrationProvider);
        }

        public IContainer Build()
        {
            return new Container(this);
        }

        public IEnumerable<Registration> GetRegistrations(Type type)
        {
            return _registrations.Where(x => x.Aliases.Contains(type))
                .Union(_registrationProviders.SelectMany(x => x.GetRegistrations(type)));
        }

        private class Container : IContainer
        {
            private ConcurrentQueue<Action> _containerDisposalCallbacks;
            private ConcurrentDictionary<Type, List<Registration>> _registrations;
            private IRegistrationProvider _registrationProvider;

            public Guid Id { get; private set; }

            public IContainer Parent { get; private set; }

            public Container(IRegistrationProvider registrationProvider)
            {
                Id = Guid.NewGuid();
                _containerDisposalCallbacks = new ConcurrentQueue<Action>();
                _registrations = new ConcurrentDictionary<Type, List<Registration>>();
                _registrationProvider = registrationProvider;
            }

            private Container(Container parent)
                : this(parent._registrationProvider)
            {
                Parent = parent;
            }

            public IContainer CreateChildContainer()
            {
                return new Container(this);
            }

            public void Dispose()
            {
                foreach (var callback in _containerDisposalCallbacks)
                    callback();
            }

            public void RegisterContainerDisposalCallback(Action callback)
            {
                _containerDisposalCallbacks.Enqueue(callback);
            }

            public object Resolve(Type type)
            {
                var context = new ResolutionContext(this, type, GetRegistrations);
                return context.GetInstance();
            }

            private IEnumerable<Registration> GetRegistrations(Type type)
            {
                return _registrations.GetOrAdd(type, t => _registrationProvider.GetRegistrations(type).ToList());
            }

            private class ResolutionContext : IResolutionContext
            {
                private ResolutionContext _parent;
                private Func<Type, IEnumerable<Registration>> _registrationFinder;

                public IContainer Container { get; private set; }

                public Registration Registration { get; private set; }

                public ResolutionContext(IContainer container, Type type, Func<Type, IEnumerable<Registration>> registrationFinder)
                {
                    Container = container;
                    _registrationFinder = registrationFinder;

                    var list = _registrationFinder(type);
                    if (!list.Any())
                        throw new ContainerException(string.Format("No registrations could be found or created for type '{0}'", type));

                    Registration = list.First();
                }

                public object Resolve(Type type)
                {
                    var context = new ResolutionContext(Container, type, _registrationFinder);
                    context.SetParent(this);
                    return context.GetInstance();
                }

                private void SetParent(ResolutionContext parent)
                {
                    _parent = parent;
                    while (parent != null)
                    {
                        if (ReferenceEquals(Registration, parent.Registration))
                            throw new ContainerException(string.Format("Cycles found when resolving type '{0}'.", Registration.ConcreteType));

                        parent = parent._parent;
                    }
                }
            }
        }
    }

    public static class ContainerBuilderExtensions
    {
        public static Registration Register<T>(this ContainerBuilder builder)
        {
            return Register(builder, typeof(T));
        }

        public static Registration Register(this ContainerBuilder builder, Type type)
        {
            var registration = new Registration(type);
            builder.AddRegistration(registration);
            return registration;
        }
    }

    #endregion

    #region Exceptions

    [Serializable]
    public class ContainerException : Exception
    {
        public ContainerException() { }
        public ContainerException(string message) : base(message) { }
        public ContainerException(string message, Exception inner) : base(message, inner) { }
        protected ContainerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion
}
