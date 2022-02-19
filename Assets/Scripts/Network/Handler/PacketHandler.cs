using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sabotris.Util;

namespace Sabotris.Network.Packets
{
    public enum PacketDirection
    {
        Client,
        Server
    }

    public class PacketHandler
    {
        public readonly PacketDirection PacketDirection;
        private readonly Dictionary<PacketTypeId, ConcurrentDictionary<object, MethodInfo>> _cache = new Dictionary<PacketTypeId, ConcurrentDictionary<object, MethodInfo>>();

        public PacketHandler(PacketDirection packetDirection)
        {
            PacketDirection = packetDirection;
        }

        public void Register<T>(T instance)
        {
            foreach (var pairs in ClassUtil.GetMethodsInTypeWithAttribute<PacketListener>(typeof(T)).Where((pairs) => pairs.All((pair) => pair.Item1.PacketDirection == PacketDirection)))
            {
                foreach (var (packetListener, methodInfo) in pairs)
                {
                    var packetTypeId = packetListener.PacketType;
                    if (!_cache.ContainsKey(packetTypeId))
                        _cache.Add(packetTypeId, new ConcurrentDictionary<object, MethodInfo>());
                    _cache[packetTypeId].TryAdd(instance, methodInfo);
                }
            }
        }

        public void Deregister<T>(T instance)
        {
            foreach (var pairs in ClassUtil.GetMethodsInTypeWithAttribute<PacketListener>(typeof(T)).Where((pairs) => pairs.All((pair) => pair.Item1.PacketDirection == PacketDirection)))
            {
                foreach (var (packetListener, _) in pairs)
                {
                    var packetTypeId = packetListener.PacketType;
                    if (_cache.ContainsKey(packetTypeId))
                        _cache[packetTypeId].TryRemove(instance, out _);
                }
            }
        }

        public void Process(Packet packet)
        {
            if (!_cache.TryGetValue(packet.GetPacketType().Id, out var listeners))
            {
                Logging.Log(PacketDirection == PacketDirection.Server, "No listeners for packet type {0}", packet.GetPacketType().Id);
                return;
            }

            foreach (var listener in listeners)
                listener.Value?.Invoke(listener.Key.GetType() == typeof(Type) ? null : listener.Key, new object[] {packet});
        }
    }
}