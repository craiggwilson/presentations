using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.TransientLifetimeSpecs
{
    [Subject(typeof(TransientLifetime))]
    public class when_getting_an_instance
    {
        static object _result1;
        static object _result2;

        Because of = () =>
        {
            var lifetime = new TransientLifetime();
            var registration = new Registration(typeof(DummyService), lifetime);
            var resolver = new Moq.Mock<IResolver>();
            var activator = new Moq.Mock<IActivator>();
            var dummyServiceQueue = new Queue<DummyService>(new[] { new DummyService(), new DummyService() });
            activator.Setup(x => x.Activate(typeof(DummyService), resolver.Object)).Returns(dummyServiceQueue.Dequeue);
            var context = new ResolutionContext(registration, resolver.Object, activator.Object);

            _result1 = lifetime.GetInstance(context);
            _result2 = lifetime.GetInstance(context);
        };

        It should_return_non_null_objects = () =>
        {
            _result1.ShouldNotBeNull();
            _result2.ShouldNotBeNull();
        };

        It should_return_different_objects = () =>
            _result1.ShouldNotBeTheSameAs(_result2);

        private class DummyService
        { }
    }
}