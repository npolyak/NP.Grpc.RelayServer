using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using NP.Grpc.RelayServiceProto;

namespace NP.Grpc.CommonRelayInterfaces;

public interface IRelayClient
{
    IAsyncEnumerable<FullMsg> GetResponseStream(System.Enum topic);

    IObservable<FullMsg> Subscribe(System.Enum topic);

    IObservable<T> Subscribe<T>(System.Enum topic)
        where T : IMessage, new();

    Task<ShortMsg> Broadcast(System.Enum topic, Any message);
}
