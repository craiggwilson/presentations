using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject("In the context of resolving by type")]
    public class when_the_type_has_circular_dependencies : In_the_context_of_resolving_by_type
    {
        static Exception _ex;

        Because of = () =>
        {
            _ex = Catch.Exception(() => _container.Resolve<DummyService>());
        };

        It should_throw_an_exception = () =>
            _ex.ShouldNotBeNull();

        private class DummyService
        {
            public DummyService(DummyServiceDependencyA a, DummyServiceDependencyB b)
            { }
        }

        private class DummyServiceDependencyA
        { }

        private class DummyServiceDependencyB
        {
            public DummyServiceDependencyB(DummyService service)
            { }
        }
    }
}