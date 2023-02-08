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
using Google.Protobuf;
using NP.Grpc.RelayServiceProto;

namespace NP.Grpc.CommonRelayInterfaces;

public interface IRelayClient
{
    IAsyncEnumerable<FullMsg> GetTopicStream(System.Enum topic, CancellationToken cancellationToken = default);

    IObservable<FullMsg> ObserveTopicStream(System.Enum topic, CancellationToken cancellationToken = default);

    IObservable<T> ObserveTopicStream<T>(System.Enum topic, CancellationToken cancellationToken = default)
        where T : IMessage, new();

    Task<ShortMsg> PublishTopic(System.Enum topic, Any message);

    Task<ShortMsg> Publish<Message>(System.Enum topic, Message message) where Message : IMessage;
}
