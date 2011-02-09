﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace MyIoC.ContainerSpecs
{
    [Subject("In the context of resolving by type")]
    public class when_the_type_is_resolvable : In_the_context_of_resolving_by_type
    {
        static object _result;

        Because of = () =>
            _result = _container.Resolve(typeof(DummyService));

        It should_return_a_non_null_object = () =>
            _result.ShouldNotBeNull();

        It should_return_an_instance_of_the_type_requested = () =>
            _result.ShouldBeOfType<DummyService>();


        private class DummyService
        {

        }
    }
}
