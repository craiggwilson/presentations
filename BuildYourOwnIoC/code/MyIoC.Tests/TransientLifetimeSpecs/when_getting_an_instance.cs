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
            var queue = new Queue<DummyService>(new[] { new DummyService(), new DummyService() });
            var context = new Moq.Mock<IResolutionContext>();
            context.Setup(x => x.Activate()).Returns(queue.Dequeue);

            var lifetime = new TransientLifetime();
            _result1 = lifetime.GetInstance(context.Object);
            _result2 = lifetime.GetInstance(context.Object);
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