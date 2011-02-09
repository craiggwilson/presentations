using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ActivatorSpecs
{
    [Subject(typeof(Activator))]
    public class When_activating_a_type_without_dependencies
    {
        static object _result;

        Because of = () =>
        {
            var resolver = new Moq.Mock<IResolver>();
            var activator = new Activator();
            _result = activator.Activate(typeof(DummyService), resolver.Object);
        };

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_type = () =>
            _result.ShouldBeOfType<DummyService>();

        private class DummyService
        { }
    }
}
