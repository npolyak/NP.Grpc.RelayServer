using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Grpc.Core;
using NP.Grpc.CommonRelayInterfaces;
using NP.DependencyInjection.Attributes;
using System.Reactive.Linq;
using static NP.Grpc.RelayServiceProto.RelayService;
using NP.Grpc.RelayServiceProto;
using System.Runtime.CompilerServices;

namespace NP.RelayServer;

[RegisterType(isSingleton: true)]
public class RelayClient : IRelayClient
{
    RelayServiceClient _client;

    [CompositeConstructor]
    public RelayClient([Inject] IGrpcConfig configParams)
    {
        Channel channel =
            new Channel
            (
                configParams.ServerName, 
                configParams.Port, 
                ChannelCredentials.Insecure);

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

    public async Task<ShortMsg> PublishTopic<Message>(System.Enum topic, Message message)
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