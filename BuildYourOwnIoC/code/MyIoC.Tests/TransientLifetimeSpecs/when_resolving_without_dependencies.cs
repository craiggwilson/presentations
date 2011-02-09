using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.TransientLifetimeSpecs
{
    [Subject(typeof(TransientLifetime))]
    public class when_resolving_without_dependencies
    {
        static object _instance;
        static object _result;

        Because of = () =>
        {
            var lifetime = new TransientLifetime(typeof(DummyService));
            _result = lifetime.Resolve(null);
        };

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_lifetimes_type = () =>
            _result.ShouldBeOfType<DummyService>();

        private class DummyService
        { }
    }
}