using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_container_scoped_type_multiple_times_from_different_containers : ContainerSpecBase
    {
        static IContainer _container;
        static object _result1;
        static object _result2;

        Establish context = () =>
        {
            _builder.Register<DummyService>().ContainerScoped();
            _container = _builder.Build();
        };

        Because of = () =>
        {
            _result1 = _container.CreateChildContainer().Resolve(typeof(DummyService));
            _result2 = _container.CreateChildContainer().Resolve(typeof(DummyService));
        };

        It should_not_return_the_same_instances = () =>
        {
            _result1.ShouldNotBeTheSameAs(_result2);
        };

        private class DummyService { }
    }
}
