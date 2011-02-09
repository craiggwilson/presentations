using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    public abstract class In_the_context_of_resolving_by_type
    {
        protected static Container _container;

        Establish context = () => 
            _container = new Container();
                
    }
}
