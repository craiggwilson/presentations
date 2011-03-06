using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_singleton_type_multiple_times : ContainerSpecBase
    {
        static object _result1;
        static object _result2;

        Establish context = () =>
            _container.Register<DummyService>().Singleton();

        Because of = () =>
        {
            _result1 = _container.Resolve(typeof(DummyService));
            _result2 = _container.Resolve(typeof(DummyService));
        };

        It should_not_return_the_same_instances = () =>
        {
            _result1.ShouldBeTheSameAs(_result2);
        };

        private class DummyService { }
    }
}
