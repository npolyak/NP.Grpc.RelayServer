// (c) Nick Polyak 2023 
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using Google.Protobuf.WellKnownTypes;
using NP.Grpc.RelayServiceProto;
using NP.Protobuf;

namespace NP.Grpc.ClientRelayApi
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
