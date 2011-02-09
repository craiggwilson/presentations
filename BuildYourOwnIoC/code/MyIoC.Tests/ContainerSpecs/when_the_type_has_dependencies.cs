using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject(typeof(Container))]
    public class when_the_type_has_dependencies : In_the_context_of_resolving_by_type
    {
        static object _result;

        Establish context = () =>
            _container.Register<DummyServiceDependencyA>(new DummyServiceDependencyA());

        Because of = () =>
            _result = _container.Resolve<DummyService>();

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_type_requested = () =>
            _result.ShouldBeOfType<DummyService>();


        private class DummyService
        {
            public DummyService(DummyServiceDependencyA a, DummyServiceDependencyB b)
            { }
        }

        private class DummyServiceDependencyA
        { }

        private class DummyServiceDependencyB
        {
            public DummyServiceDependencyB(DummyServiceDependencyC c)
            { }
        }

        private class DummyServiceDependencyC
        { }
    }
}