using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject("In the context of resolving by type")]
    public class when_an_instance_of_a_type_was_registered : In_the_context_of_resolving_by_type
    {
        static object _result;
        static object _instance;

        Because of = () =>
        {
            _instance = new DummyService();
            _container.Register(typeof(DummyService), _instance);
            _result = _container.Resolve(typeof(DummyService));
        };

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_type_requested = () =>
            _result.ShouldBeOfType<DummyService>();

        It should_return_the_instance_that_was_registered = () =>
            _result.ShouldBeTheSameAs(_instance);


        private class DummyService
        { }
    }
}