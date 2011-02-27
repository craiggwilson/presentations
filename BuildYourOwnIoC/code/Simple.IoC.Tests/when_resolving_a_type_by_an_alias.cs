using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_type_by_its_alias : ContainerSpecBase
    {
        static IContainer _container;
        static object _result;

        Establish context = () =>
        {
            _builder.Register<DummyService>().As<IDummyService>();
            _container = _builder.Build();
        };
        Because of = () =>
            _result = _container.Resolve(typeof(IDummyService));

        It should_not_return_null = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_requested_type = () =>
            _result.ShouldBeOfType<DummyService>();

        private interface IDummyService { }
        private class DummyService : IDummyService { }
    }
}
