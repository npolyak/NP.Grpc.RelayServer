using Google.Protobuf.WellKnownTypes;
using NP.Grpc.RelayServiceProto;
using NP.Protobuf;

namespace NP.Grpc.CommonRelayInterfaces
{
    public static class MessageHelper
    {
        public static ShortMsg ToShortMsg(this System.Enum topic)
        {
            return new ShortMsg
            {
                MsgSentTime = Timestamp.FromDateTime(DateTime.UtcNow),
                Topic = topic.ToEnumVal()
            };
        }
    }
}
