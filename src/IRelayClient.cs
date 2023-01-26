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
