using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.IoC
{
    public class Container
    {
        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}