using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Simple.IoC.Tests;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public abstract class ContainerSpecBase
    {
        protected static ContainerBuilder _builder;

        Establish context = () =>
        {
            _builder = new ContainerBuilder();
            _builder.AddRegistrationProvider(new AnythingRegistrationProvider());
        };
    }
}
