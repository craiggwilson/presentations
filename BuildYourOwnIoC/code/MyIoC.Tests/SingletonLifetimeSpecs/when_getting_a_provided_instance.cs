using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.SingletonLifetimeSpecs
{
    [Subject(typeof(SingletonLifetime))]
    public class when_getting_a_provided_instance
    {
        static object _instance;
        static object _result1;
        static object _result2;

        Because of = () =>
        {
            _instance = new DummyService();
            var lifetime = new SingletonLifetime(_instance);
            _result1 = lifetime.GetInstance(null);
            _result2 = lifetime.GetInstance(null);
        };

        It should_return_non_null_objects = () =>
        {
            _result1.ShouldNotBeNull();
            _result2.ShouldNotBeNull();
        };
        It should_return_the_same_instance_that_was_passed_in = () =>
        {
            _result1.ShouldBeTheSameAs(_instance);
            _result2.ShouldBeTheSameAs(_instance);
        };

        private class DummyService
        { }
    }
}