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
            var ctor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = Resolve(parameters[i].ParameterType);

            return ctor.Invoke(args);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}