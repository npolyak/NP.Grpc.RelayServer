using Grpc.Core;
using NP.Grpc.RelayServiceProto;

namespace NP.Grpc.ServerRelayInterfaces
{
    // public proxy to the non-public plugin
    public class RelayServer(IRelayService _relayService) : IRelayService
    {
        public async Task GetTopicStream(ShortMsg request, IServerStreamWriter<FullMsg> responseStream, ServerCallContext context)
        {
            await _relayService.GetTopicStream(request, responseStream, context);
        }

        public async Task<ShortMsg> PublishTopic(FullMsg request, ServerCallContext context)
        {
            return await _relayService.PublishTopic(request, context);
        }

        public void RegisterTopics(params Enum[] topics)
        {
            _relayService.RegisterTopics(topics);
        }
    }
}
