using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;

namespace MyIoC.TransientLifetimeSpecs
{
    [Subject(typeof(TransientLifetime))]
    public class when_resolving_with_dependencies
    {
        static object _result;

        Because of = () =>
        {
            var resolver = new Moq.Mock<IResolver>();
            resolver.Setup(x => x.Resolve(typeof(DummyServiceDependencyA))).Returns(new DummyServiceDependencyA());
            var lifetime = new TransientLifetime(typeof(DummyService));
            _result = lifetime.Resolve(resolver.Object);
        };

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_lifetimes_type = () =>
            _result.ShouldBeOfType<DummyService>();

        private class DummyService
        {
            public DummyService(DummyServiceDependencyA a)
            { }
        }

        private class DummyServiceDependencyA
        { }
    }
}