using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC
{
    #region Activation

    public interface IActivator
    {
        object Activate(Type type, Func<Type, object> resolver);
    }

    public class ReflectionActivator : IActivator
    {
        public object Activate(Type type, Func<Type, object> resolver)
        {
            var ctor = type.GetConstructors()
                            .OrderByDescending(c => c.GetParameters().Length)
                            .First();
            
            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = resolver(parameters[i].ParameterType);

            return ctor.Invoke(args);
        }
    }

    public class DelegateActivator : IActivator
    {
        private readonly Func<Type, Func<Type, object>, object> _activator;

        public DelegateActivator(Func<Type, Func<Type, object>, object> activator)
        {
            _activator = activator;
        }

        public object Activate(Type type, Func<Type, object> resolver)
        {
            return _activator(type, resolver);
        }
    }

    #endregion

    #region Lifetimes

    public interface ILifetime
    {
        object GetInstance(Type type, IActivator activator, Func<Type, object> resolver);
    }

    public class TransientLifetime : ILifetime
    {
        public object GetInstance(Type type, IActivator activator, Func<Type, object> resolver)
        {
            return activator.Activate(type, resolver);
        }
    }

    public class SingletonLifetime : ILifetime
    {
        private object _instance;

        public object GetInstance(Type type, IActivator activator, Func<Type, object> resolver)
        {
            if (_instance == null)
                _instance = activator.Activate(type, resolver);
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

        public Registration ActivateWith(Func<Type, Func<Type, object>, object> activator)
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
            return registration.Lifetime.GetInstance(registration.ConcreteType, registration.Activator, Resolve);
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