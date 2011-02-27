using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public class when_resolving_a_disposable_type : ContainerSpecBase
    {
        static IContainer _container;
        static DummyService _result;

        Establish context = () =>
        {
            _container = _builder.Build();
            _result = _container.Resolve<DummyService>();
        };

        Because of = () =>
            _container.Dispose();

        It should_dispose_the_component = () =>
            _result.IsDisposed.ShouldBeTrue();

        private class DummyService : IDisposable
        {
            public bool IsDisposed = false;

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
