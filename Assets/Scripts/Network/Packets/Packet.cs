using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Sabotris.Util;
using UnityEngine;

namespace Sabotris.Network.Packets
{
    public abstract class Packet
    {
        public long SenderId = -1;

        public abstract PacketType GetPacketType();

        public NetOutgoingMessage Serialize(NetPeer peer)
        {
            var outgoing = peer.CreateMessage();
            outgoing.Write((byte) GetPacketType().Id);

            foreach (var property in GetType().GetProperties()
                         .Where((property) => property.CanRead && property.CanWrite)
                         .OrderBy((property) => property.Name))
            {
                var value = property.GetValue(this);
                WriteValue(outgoing, value);
            }
            
            return outgoing;
        }

        public void Deserialize(NetIncomingMessage incoming)
        {
            SenderId = incoming.SenderConnection?.RemoteUniqueIdentifier ?? -1;
            
            foreach (var property in GetType().GetProperties()
                         .Where((property) => property.CanRead && property.CanWrite)
                         .OrderBy((property) => property.Name))
            {
                property.SetValue(this, ReadValue(incoming, property.PropertyType));
            }
        }
        
        private object ReadValue(NetBuffer incoming, Type type)
        {
            if (type == typeof(bool)) return incoming.ReadBoolean();
            if (type == typeof(byte)) return incoming.ReadByte();
            if (type == typeof(short)) return incoming.ReadInt16();
            if (type == typeof(ushort)) return incoming.ReadUInt16();
            if (type == typeof(int)) return incoming.ReadInt32();
            if (type == typeof(uint)) return incoming.ReadUInt32();
            if (type == typeof(long)) return incoming.ReadInt64();
            if (type == typeof(ulong)) return incoming.ReadUInt64();
            if (type == typeof(double)) return incoming.ReadDouble();
            if (type == typeof(float)) return incoming.ReadFloat();
            if (type == typeof(string)) return incoming.ReadString();
            if (type == typeof(Guid)) return Guid.Parse(incoming.ReadString());
            if (type == typeof(Vector3)) return new Vector3(incoming.ReadFloat(), incoming.ReadFloat(), incoming.ReadFloat());
            if (type == typeof(Vector3Int)) return new Vector3Int(incoming.ReadInt32(), incoming.ReadInt32(), incoming.ReadInt32());
            if (type == typeof(Quaternion)) return new Quaternion(incoming.ReadFloat(), incoming.ReadFloat(), incoming.ReadFloat(), incoming.ReadFloat());
            if (type == typeof(Pair<Guid, Vector3Int>)) return new Pair<Guid, Vector3Int>((Guid) ReadValue(incoming, typeof(Guid)), (Vector3Int) ReadValue(incoming, typeof(Vector3Int)));
            if (type == typeof(Pair<Guid, int>)) return new Pair<Guid, int>((Guid) ReadValue(incoming, typeof(Guid)), incoming.ReadInt32());
            if (type == typeof(Pair<long, int>)) return new Pair<long, int>(incoming.ReadInt64(), incoming.ReadInt32());
            if (type == typeof(Player)) return new Player(incoming.ReadInt64(), incoming.ReadString());
            if (type == typeof(PlayerScore)) return new PlayerScore(incoming.ReadInt32(), incoming.ReadInt32());
            if (type == typeof(Color)) return new Color(incoming.ReadFloat(), incoming.ReadFloat(), incoming.ReadFloat(), incoming.ReadFloat());
            
            if (type == typeof(bool[])) return ReadArray<bool>(incoming, type.GetElementType());
            if (type == typeof(byte[])) return ReadArray<byte>(incoming, type.GetElementType());
            if (type == typeof(short[])) return ReadArray<short>(incoming, type.GetElementType());
            if (type == typeof(ushort[])) return ReadArray<ushort>(incoming, type.GetElementType());
            if (type == typeof(int[])) return ReadArray<int>(incoming, type.GetElementType());
            if (type == typeof(uint[])) return ReadArray<uint>(incoming, type.GetElementType());
            if (type == typeof(long[])) return ReadArray<long>(incoming, type.GetElementType());
            if (type == typeof(ulong[])) return ReadArray<ulong>(incoming, type.GetElementType());
            if (type == typeof(double[])) return ReadArray<double>(incoming, type.GetElementType());
            if (type == typeof(float[])) return ReadArray<float>(incoming, type.GetElementType());
            if (type == typeof(string[])) return ReadArray<string>(incoming, type.GetElementType());
            if (type == typeof(Guid[])) return ReadArray<Guid>(incoming, type.GetElementType());
            if (type == typeof(Vector3[])) return ReadArray<Vector3>(incoming, type.GetElementType());
            if (type == typeof(Vector3Int[])) return ReadArray<Vector3Int>(incoming, type.GetElementType());
            if (type == typeof(Quaternion[])) return ReadArray<Quaternion>(incoming, type.GetElementType());
            if (type == typeof(Pair<Guid, Vector3Int>[])) return ReadArray<Pair<Guid, Vector3Int>>(incoming, type.GetElementType());
            if (type == typeof(Pair<Guid, int>[])) return ReadArray<Pair<Guid, int>>(incoming, type.GetElementType());
            if (type == typeof(Pair<long, int>[])) return ReadArray<Pair<long, int>>(incoming, type.GetElementType());
            if (type == typeof(Player[])) return ReadArray<Player>(incoming, type.GetElementType());
            if (type == typeof(PlayerScore[])) return ReadArray<PlayerScore>(incoming, type.GetElementType());
            if (type == typeof(Color[])) return ReadArray<Color>(incoming, type.GetElementType());
            
            if (type == typeof(Dictionary<long, PlayerScore>)) return ReadDictionary<long, PlayerScore>(incoming, type.GetGenericArguments());
            
            return null;
        }

        private void WriteValue<T>(NetBuffer outgoing, T value)
        {
            switch (value)
            {
                case bool parsed: outgoing.Write(parsed); break;
                case byte parsed: outgoing.Write(parsed); break;
                case short parsed: outgoing.Write(parsed);break;
                case ushort parsed: outgoing.Write(parsed);break;
                case int parsed: outgoing.Write(parsed);break;
                case uint parsed: outgoing.Write(parsed);break;
                case long parsed: outgoing.Write(parsed);break;
                case ulong parsed: outgoing.Write(parsed);break;
                case double parsed: outgoing.Write(parsed);break;
                case float parsed: outgoing.Write(parsed);break;
                case string parsed: outgoing.Write(parsed);break;
                case Guid parsed: outgoing.Write(parsed.ToString()); break;
                
                case Vector3 parsed:
                {
                    outgoing.Write(parsed.x);
                    outgoing.Write(parsed.y);
                    outgoing.Write(parsed.z);
                    break;
                }
                
                case Vector3Int parsed:
                {
                    outgoing.Write(parsed.x);
                    outgoing.Write(parsed.y);
                    outgoing.Write(parsed.z);
                    break;
                }

                case Quaternion parsed:
                {
                    outgoing.Write(parsed.x);
                    outgoing.Write(parsed.y);
                    outgoing.Write(parsed.z);
                    outgoing.Write(parsed.w);
                    break;
                }

                case Pair<Guid, Vector3Int> parsed:
                {
                    WriteValue(outgoing, parsed.Key);
                    WriteValue(outgoing, parsed.Value);
                    break;
                }

                case Pair<Guid, int> parsed:
                {
                    WriteValue(outgoing, parsed.Key);
                    WriteValue(outgoing, parsed.Value);
                    break;
                }

                case Pair<long, int> parsed:
                {
                    WriteValue(outgoing, parsed.Key);
                    WriteValue(outgoing, parsed.Value);
                    break;
                }

                case Player parsed:
                {
                    outgoing.Write(parsed.Id);
                    outgoing.Write(parsed.Name);
                    break;
                }

                case PlayerScore parsed:
                {
                    outgoing.Write(parsed.Score);
                    outgoing.Write(parsed.ClearedLayers);
                    break;
                }

                case Color parsed:
                {
                    outgoing.Write(parsed.r);
                    outgoing.Write(parsed.g);
                    outgoing.Write(parsed.b);
                    outgoing.Write(parsed.a);
                    break;
                }

                case IEnumerable<bool> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<short> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<ushort> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<int> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<uint> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<long> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<ulong> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<double> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<float> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<string> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Guid> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Vector3> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Vector3Int> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Quaternion> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Pair<Guid, Vector3Int>> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Pair<Guid, int>> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Pair<long, int>> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Player> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<PlayerScore> parsed: WriteArray(outgoing, parsed); break;
                case IEnumerable<Color> parsed: WriteArray(outgoing, parsed); break;
                
                case Dictionary<long, PlayerScore> parsed: WriteDictionary(outgoing, parsed); break;
            }
        }

        private void WriteArray<T>(NetBuffer outgoing, IEnumerable<T> enumerable)
        {
            var arr = enumerable.ToArray();
            WriteValue(outgoing, arr.Length);
            foreach (var value in arr)
                WriteValue(outgoing, value);
        }

        private void WriteDictionary<TKey, TValue>(NetBuffer outgoing, Dictionary<TKey, TValue> dictionary)
        {
            WriteValue(outgoing, dictionary.Count);
            foreach (var entry in dictionary)
            {
                WriteValue(outgoing, entry.Key);
                WriteValue(outgoing, entry.Value);
            }
        }

        private T[] ReadArray<T>(NetBuffer incoming, Type type)
        {
            var count = incoming.ReadInt32();
            var objects = new T[count];
            for (var i = 0; i < count; i++)
                objects[i] = (T) ReadValue(incoming, type);
            return objects;
        }
        
        private Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(NetBuffer incoming, Type[] types)
        {
            var count = incoming.ReadInt32();
            var dict = new Dictionary<TKey, TValue>();
            for (var i = 0; i < count; i++)
                dict.Add((TKey) ReadValue(incoming, types[0]), (TValue) ReadValue(incoming, types[1]));
            return dict;
        }
    }
}