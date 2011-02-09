using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.SingletonLifetimeSpecs
{
    [Subject(typeof(SingletonLifetime))]
    public class when_resolving
    {
        static object _instance;
        static object _result;

        Because of = () =>
        {
            _instance = new DummyService();
            var lifetime = new SingletonLifetime(_instance);
            _result = lifetime.Resolve(null);
        };

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_the_same_instance_that_was_passed_in = () =>
            _result.ShouldBeTheSameAs(_instance);

        private class DummyService
        { }
    }
}