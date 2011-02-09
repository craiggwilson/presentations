using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace MyIoC
{
    /*Goals: 
     * 1) Resolve an instance of an object by type
    */
    public interface IContainer
    {
        object Resolve(Type type);
    }

    public class Container : IContainer
    {
        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
