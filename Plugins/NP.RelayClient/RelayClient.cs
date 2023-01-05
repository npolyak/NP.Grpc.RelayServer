using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Grpc.Core;
using NP.Grpc.ServerClientCommon;
using NP.Protobuf;
using NP.DependencyInjection.Attributes;
using static NP.Grpc.ServerClientCommon.RelayService;
using System.Reactive.Linq;

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

    public async Task<ShortMsg> Broadcast(System.Enum topic, Any message)
    {
        var broadcastMsg = new FullMsg
        {
            Metadata = topic.ToShortMsg(),
            Message = message
        };

        return await _client.BroadcastAsync(broadcastMsg);
    }

    private async IAsyncEnumerable<FullMsg> SubscribeImpl(System.Enum topic)
    {
        ShortMsg subscribeMsg = topic.ToShortMsg();

        using var replies = _client.Subscribe(subscribeMsg);

        while (await replies.ResponseStream.MoveNext())
        {
            var msg = replies.ResponseStream.Current;

            yield return msg;
        }
    }

    public IObservable<FullMsg> Subscribe(System.Enum topic)
    {
        return SubscribeImpl(topic).ToObservable();
    }

    public IObservable<T> Subscribe<T>(System.Enum topic)
        where T : IMessage, new()
    {
        return Subscribe(topic).Select(msg => msg.Message.Unpack<T>());
    }
}