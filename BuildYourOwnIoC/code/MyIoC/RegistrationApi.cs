using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyIoC
{
    public class FluentRegistration
    {
        public Registration Registration { get; private set; }

        public FluentRegistration(Registration registration)
        {
            Registration = registration;
        }
    }

    public static class RegistrationExtensions
    {
        public static FluentRegistration Register<T>(this IRegistrar registrar)
        {
            return Register(registrar, typeof(T));
        }

        public static FluentRegistration Register(this IRegistrar registrar, Type type)
        {
            return Register(registrar, type, type);
        }

        public static FluentRegistration Register<TLookup, TConcrete>(this IRegistrar registrar) where TConcrete : TLookup
        {
            return Register(registrar, typeof(TLookup), typeof(TConcrete));
        }

        public static FluentRegistration Register(this IRegistrar registrar, Type lookupType, Type concreteType)
        {
            var registration = new Registration(concreteType);
            registrar.Register(lookupType, registration);
            return new FluentRegistration(registration);
        }

        public static FluentRegistration ActivateWith(this FluentRegistration registration, IActivator activator)
        {
            registration.Registration.Activator = activator;
            return registration;
        }

        public static FluentRegistration ActivateWith(this FluentRegistration registration, Func<IResolver, object> activator)
        {
            registration.Registration.Activator = new DelegateActivator(activator);
            return registration;
        }

        public static FluentRegistration AsSingleton(this FluentRegistration registration)
        {
            registration.Registration.Lifetime = new SingletonLifetime();
            return registration;
        }

        public static FluentRegistration AsTransient(this FluentRegistration registration)
        {
            registration.Registration.Lifetime = new TransientLifetime();
            return registration;
        }

        public static FluentRegistration WithInstance(this FluentRegistration registration, object instance)
        {
            registration.Registration.Lifetime = new SingletonLifetime(instance);
            return registration;
        }

        private class DelegateActivator : IActivator
        {
            private readonly Func<IResolver, object> _activator;

            public DelegateActivator(Func<IResolver, object> activator)
            {
                _activator = activator;
            }

            public object Activate(IResolutionContext context)
            {
                return _activator(context);
            }
        }
    }
}