using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;

namespace Simple.IoC.Tests.ContainerSpecs
{
    public abstract class ContainerSpecBase
    {
        protected static Container _container;

        Establish context = () =>
            _container = new Container();

    }
}
