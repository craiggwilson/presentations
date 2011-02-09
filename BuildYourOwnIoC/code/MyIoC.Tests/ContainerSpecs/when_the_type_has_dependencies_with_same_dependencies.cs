using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject("In the context of resolving by type")]
    public class when_the_type_has_dependencies_with_same_dependencies : In_the_context_of_resolving_by_type
    {
        static Exception _ex;

        Because of = () =>
        {
            _ex = Catch.Exception(() => _container.Resolve<DummyService>());
        };

        It should_non_throw_an_exception = () =>
            _ex.ShouldBeNull();

        private class DummyService
        {
            public DummyService(DummyServiceDependencyA a, DummyServiceDependencyB b)
            { }
        }

        private class DummyServiceDependencyA
        { }

        private class DummyServiceDependencyB
        {
            public DummyServiceDependencyB(DummyServiceDependencyA a)
            { }
        }
    }
}