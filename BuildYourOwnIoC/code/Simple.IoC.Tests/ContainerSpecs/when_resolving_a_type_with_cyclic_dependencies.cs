using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_type_with_cyclice_dependencies : ContainerSpecBase
    {
        static Exception _ex;

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
