using System;
using System.Linq;
using UnityEngine;

namespace Sabotris.Util
{
    public static class Logging
    {
        public static class Colors
        {
            public const string Argument = "magenta";
            public const string Client = "aqua";
            public const string Server = "lime";
        }

        public static void Log(string message, params object[] arguments)
        {
            Debug.Log(string.Format(message, arguments.Select((argument) => (object) $"<color={Colors.Argument}>{argument}</color>").ToArray()));
            Console.WriteLine(message, arguments.Select((argument) => (object) $"<color={Colors.Argument}>{argument}</color>").ToArray());
        }

        public static void Log(bool server, string message, params object[] arguments)
        {
            Log($"<color={(server ? Colors.Server : Colors.Client)}>[{(server ? "SERVER" : "CLIENT")}] {message}</color>", arguments);
        }

        public static void Warn(bool server, string message, params object[] arguments)
        {
            Log($"<color={(server ? Colors.Server : Colors.Client)}>[{(server ? "SERVER" : "CLIENT")}]</color> <color=yellow>{message}</color>", arguments);
        }

        public static void Error(bool server, string message, params object[] arguments)
        {
            Log($"<color={(server ? Colors.Server : Colors.Client)}>[{(server ? "SERVER" : "CLIENT")}]</color> <color=red>{message}</color>", arguments);
        }
    }
}