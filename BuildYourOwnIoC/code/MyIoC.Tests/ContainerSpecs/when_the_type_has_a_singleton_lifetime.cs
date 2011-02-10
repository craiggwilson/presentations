using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject(typeof(Container))]
    public class when_the_type_has_a_singleton_lifetime : In_the_context_of_resolving_by_type
    {
        static object _result1;
        static object _result2;

        Establish context = () =>
        {
            _container.RegisterSingleton<DummyService, DummyService>();
        };

        Because of = () =>
        {
            _result1 = _container.Resolve<DummyService>();
            _result2 = _container.Resolve<DummyService>();
        };

        It should_return_different_instances = () =>
            _result1.ShouldBeTheSameAs(_result2);

        private class DummyService
        {
            public DummyService()
            { }
        }
    }
}