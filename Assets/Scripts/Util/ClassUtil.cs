using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sabotris.Util
{
    public class ClassUtil
    {
        private static readonly Assembly Assembly = typeof(ClassUtil).Assembly;

        public static IEnumerable<IEnumerable<(T, MethodInfo)>> GetMethodsInTypeWithAttribute<T>(Type type)
        {
            return type.GetMethods()
                .Where(method => !method.IsStatic && (uint) method.GetCustomAttributes(typeof(T), true).Length > 0)
                .Select((methodInfo) =>
                    methodInfo.GetCustomAttributes(typeof(T), true).Select((attribute) => ((T) attribute, methodInfo)));
        }

        public static IEnumerable<IEnumerable<(T, MethodInfo)>> GetMethodsWithAttribute<T>(Assembly assembly)
        {
            var assembly1 = assembly;
            if ((object) assembly1 == null)
                assembly1 = Assembly;
            return assembly1.GetTypes().SelectMany(GetMethodsInTypeWithAttribute<T>);
        }
    }
}