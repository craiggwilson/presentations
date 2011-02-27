using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_type_with_cyclic_dependencies : ContainerSpecBase
    {
        static IContainer _container;
        static Exception _ex;

        Establish context = () =>
            _container = _builder.Build();

        Because of = () =>
            _ex = Catch.Exception(() => _container.Resolve(typeof(DummyService)));

        It should_throw_an_exception = () =>
            _ex.ShouldNotBeNull();

        private class DummyService
        {
            public DummyService(DepA a)
            { }
        }

        private class DepA
        {
            public DepA(DummyService s)
            { }
        }

    }
}
