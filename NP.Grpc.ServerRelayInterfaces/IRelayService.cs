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

using Grpc.Core;
using NP.Grpc.RelayServiceProto;

namespace NP.Grpc.ServerRelayInterfaces;

public interface IRelayService
{
    Task<ShortMsg> PublishTopic(FullMsg request, ServerCallContext context);

    Task GetTopicStream(ShortMsg request, IServerStreamWriter<FullMsg> responseStream, ServerCallContext context);

    void RegisterTopics(params Enum[] topics);
}
