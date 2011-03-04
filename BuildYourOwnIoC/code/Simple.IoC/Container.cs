using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC
{
    #region Activation

    public interface IActivator
    {
        object Activate(ResolutionContext context);
    }

    public class ReflectionActivator : IActivator
    {
        public object Activate(ResolutionContext context)
        {
            var ctor = context.Registration.ConcreteType.GetConstructors()
                            .OrderByDescending(c => c.GetParameters().Length)
                            .First();
            
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = context.ResolveDependency(parameters[i].ParameterType);

            return ctor.Invoke(args);
        }
    }

    public class DelegateActivator : IActivator
    {
        private readonly Func<ResolutionContext, object> _activator;

        public DelegateActivator(Func<ResolutionContext, object> activator)
        {
            _activator = activator;
        }

        public object Activate(ResolutionContext context)
        {
            return _activator(context);
        }
    }

    #endregion

    #region Lifetimes

    public interface ILifetime
    {
        object GetInstance(ResolutionContext context);
    }

    public class TransientLifetime : ILifetime
    {
        public object GetInstance(ResolutionContext context)
        {
            return context.Activate();
        }
    }

    public class SingletonLifetime : ILifetime
    {
        private object _instance;

        public object GetInstance(ResolutionContext context)
        {
            if (_instance == null)
                _instance = context.Activate();
            return _instance;
        }
    }

    #endregion

    #region Registration

    public class Registration
    {
        public Type ConcreteType { get; private set; }

        public IActivator Activator { get; private set; }

        public ISet<Type> Aliases { get; private set; }

        public ILifetime Lifetime { get; private set; }

        public Registration(Type concreteType)
        {
            ConcreteType = concreteType;
            Activator = new ReflectionActivator();
            Lifetime = new TransientLifetime();

            Aliases = new HashSet<Type>();
            Aliases.Add(concreteType);
        }

        public Registration ActivateWith(IActivator activator)
        {
            Activator = activator;
            return this;
        }

        public Registration ActivateWith(Func<ResolutionContext, object> activator)
        {
            Activator = new DelegateActivator(activator);
            return this;
        }

        public Registration As<T>()
        {
            Aliases.Add(typeof(T));
            return this;
        }

        public Registration Singleton()
        {
            Lifetime = new SingletonLifetime();
            return this;
        }
    }

    #endregion

    #region ResolutionContext

    public class ResolutionContext
    {
        private readonly Func<Type, Registration> _registrationFinder;

        private ResolutionContext _parent;

        public Registration Registration { get; private set; }

        public ResolutionContext(Registration registration, Func<Type, Registration> registrationFinder)
        {
            Registration = registration;
            _registrationFinder = registrationFinder;
        }

        public object Activate()
        {
            return Registration.Activator.Activate(this);
        }

        public object GetInstance()
        {
            return Registration.Lifetime.GetInstance(this);
        }

        public object ResolveDependency(Type type)
        {
            var registration = _registrationFinder(type);
            var context = new ResolutionContext(registration, _registrationFinder);
            context.SetParent(this);
            return context.GetInstance();
        }

        public T ResolveDependency<T>()
        {
            return (T)ResolveDependency(typeof(T));
        }

        private void SetParent(ResolutionContext parent)
        {
            _parent = parent;
            while (parent != null)
            {
                if (ReferenceEquals(Registration, parent.Registration))
                    throw new Exception("Cycles found");

                parent = parent._parent;
            }
        }
    }

    #endregion

    public class Container
    {
        private List<Registration> _registrations = new List<Registration>();

        public Registration Register(Type type)
        {
            var registration = new Registration(type);
            _registrations.Add(registration);
            return registration;
        }

        public Registration Register<T>()
        {
            return Register(typeof(T));
        }

        public object Resolve(Type type)
        {
            var registration = FindRegistration(type);
            var context = new ResolutionContext(registration, FindRegistration);
            return context.GetInstance();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private Registration FindRegistration(Type type)
        {
            var registration = _registrations.FirstOrDefault(r => r.Aliases.Contains(type));
            if (registration == null)
                registration = Register(type);

            return registration;
        }
    }
}