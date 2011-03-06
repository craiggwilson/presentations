using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_type_with_dependencies : ContainerSpecBase
    {
        static DummyService _result;

        Because of = () =>
            _result = (DummyService)_container.Resolve(typeof(DummyService));

        It should_not_return_null = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_requested_type = () =>
            _result.ShouldBeOfType<DummyService>();

        It should_return_an_instance_with_the_dependency_created = () =>
            _result.A.ShouldNotBeNull();

        private class DummyService
        {
            public DepA A { get; private set; }

            public DummyService(DepA a)
            {
                A = a;
            }
        }

        private class DepA { }

    }
}
