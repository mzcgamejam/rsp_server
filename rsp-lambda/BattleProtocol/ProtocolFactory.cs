using BattleProtocol.Entities;
using MessagePack;
using System;

namespace BattleProtocol
{
    public static class ProtocolFactory
    {
        public static byte[] SerializeProtocol(MessageType messageType, BaseProtocol protocol)
        {
            switch (messageType)
            {
                case MessageType.Test:
                    return MessagePackSerializer.Serialize((ProtoTest)protocol);
                default:
                    throw new Exception("[ProtocolFactory] Invalid MessageType : " + messageType);
            }
        }

        public static BaseProtocol DeserializeProtocol(MessageType messageType, byte[] bytes)
        {
            switch (messageType)
            {
                case MessageType.Test:
                    return MessagePackSerializer.Deserialize<ProtoTest>(bytes);
                case MessageType.BattleEnter:
                    return MessagePackSerializer.Deserialize<ProtoBattleEnter>(bytes);
                case MessageType.BattleEnterResult:
                    return MessagePackSerializer.Deserialize<ProtoBattleEnterResult>(bytes);
                case MessageType.BattleResult:
                    return MessagePackSerializer.Deserialize<ProtoBattleResult>(bytes);
                case MessageType.BattleAttack:
                    return MessagePackSerializer.Deserialize<ProtoBattleAttack>(bytes);
                case MessageType.BattleProvoke:
                    return MessagePackSerializer.Deserialize<ProtoBattleProvoke>(bytes);
                default:
                    throw new Exception("[ProtocolFactory] Invalid Message Type : " + messageType);
            }
        }
    }
}
