﻿using System;
using System.Linq;
using System.Reflection;

namespace devmon_test.Extensions
{
    internal static class ReflectionHelper
    {
        public static int GetPublicPropertyCount(this object value)
        {
            Type type = value.GetType();
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return props
                .Where(p => p.CanRead && p.CanWrite)
                .Count();
        }
    }
}
