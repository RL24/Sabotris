using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sabotris.Util
{
    public class ClassUtil
    {
        private static readonly Assembly Assembly = typeof(ClassUtil).Assembly;

        public static IEnumerable<Pair<T, MethodInfo>> GetMethodsInTypeWithAttribute<T>(Type type)
        {
            return type.GetMethods().Where(method => !method.IsStatic && (uint) method.GetCustomAttributes(typeof(T), true).Length > 0).Select(methodInfo => new Pair<T, MethodInfo>((T) methodInfo.GetCustomAttributes(typeof(T), true).First(), methodInfo));
        }

        public static IEnumerable<Pair<T, MethodInfo>> GetStaticMethodsInTypeWithAttribute<T>(Type type)
        {
            return type.GetMethods().Where(method => method.IsStatic && (uint) method.GetCustomAttributes(typeof(T), true).Length > 0U).Select(methodInfo => new Pair<T, MethodInfo>((T) methodInfo.GetCustomAttributes(typeof(T), true).First(), methodInfo));
        }

        public static IEnumerable<Pair<T, MethodInfo>> GetMethodsWithAttribute<T>() => GetMethodsWithAttribute<T>(Assembly);

        public static IEnumerable<Pair<T, MethodInfo>> GetMethodsWithAttribute<T>(Assembly assembly)
        {
            Assembly assembly1 = assembly;
            if ((object) assembly1 == null)
                assembly1 = Assembly;
            return assembly1.GetTypes().SelectMany(GetMethodsInTypeWithAttribute<T>);
        }

        public static IEnumerable<Pair<T, MethodInfo>> GetStaticMethodsWithAttribute<T>(Assembly assembly)
        {
            Assembly assembly1 = assembly;
            if ((object) assembly1 == null)
                assembly1 = Assembly;
            return assembly1.GetTypes().SelectMany(GetStaticMethodsInTypeWithAttribute<T>);
        }
    }
}