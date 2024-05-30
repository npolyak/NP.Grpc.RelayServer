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
using Grpc.Core;
using NP.Grpc.CommonRelayApi;
using NP.Grpc.ClientRelayApi;
using NP.DependencyInjection.Attributes;
using System.Reactive.Linq;
using static NP.Grpc.RelayServiceProto.RelayService;
using NP.Grpc.RelayServiceProto;
using System.Runtime.CompilerServices;
using Grpc.Net.Client;

namespace NP.Grpc.RelayClient;

[RegisterType(isSingleton: true)]
public class RelayClient : IRelayClient
{
    RelayServiceClient _client;

    [CompositeConstructor]
    public RelayClient([Inject] IGrpcConfig configParams)
    {
        GrpcChannel channel =
            GrpcChannel.ForAddress($"https://{configParams.ServerName}:{configParams.Port}");

        _client = new RelayServiceClient(channel);
    }

    public async Task<ShortMsg> PublishTopic(System.Enum topic, Any message)
    {
        var broadcastMsg = new FullMsg
        {
            Metadata = topic.ToShortMsg(),
            Message = message
        };

        return await _client.PublishTopicAsync(broadcastMsg);
    }

    public async Task<ShortMsg> Publish<Message>(System.Enum topic, Message message)
        where Message : IMessage
    {
        return await PublishTopic(topic, Any.Pack(message));
    }

    public async IAsyncEnumerable<FullMsg>
        GetTopicStream
        (
            System.Enum topic, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ShortMsg subscribeMsg = topic.ToShortMsg();

        using var replies = _client.GetTopicStream(subscribeMsg, null, null, cancellationToken);

        while (await replies.ResponseStream.MoveNext(cancellationToken))
        {
            var msg = replies.ResponseStream.Current;

            yield return msg;
        }
    }

    public IObservable<FullMsg> ObserveTopicStream
    (
        System.Enum topic,
        CancellationToken cancellationToken = default)
    {
        return GetTopicStream(topic, cancellationToken).ToObservable();
    }


    public IObservable<T> ObserveTopicStream<T>
    (
        System.Enum topic, 
        CancellationToken cancellationToken = default)
        where T : IMessage, new()
    {
        return ObserveTopicStream(topic, cancellationToken).Select(msg => msg.Message.Unpack<T>());
    }
}