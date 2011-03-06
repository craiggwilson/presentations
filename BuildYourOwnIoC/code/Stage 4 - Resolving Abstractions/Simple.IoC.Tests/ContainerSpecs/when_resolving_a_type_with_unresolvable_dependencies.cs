using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_type_with_unresolvable_dependencies : ContainerSpecBase
    {
        static object _result;

        Establish context = () =>
            _container.Register<DummyService>()
                .ActivateWith((t, r) => new DummyService(9, (DepA)r(typeof(DepA))));

        Because of = () =>
            _result = _container.Resolve(typeof(DummyService));

        It should_not_return_null = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_requested_type = () =>
            _result.ShouldBeOfType<DummyService>();

        private class DummyService
        {
            public DummyService(int i, DepA a)
            { }
        }

        private class DepA { }
    }
}
