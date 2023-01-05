using Grpc.Core;
using NP.DependencyInjection.Attributes;
using NP.Grpc.ServerClientCommon;
using NP.Protobuf;

namespace NP.Grpc.RelayServer
{
    [RegisterType(isSingleton: true)]
    public class RelayServer : IRelayServer
    {
        private Server _server;

        [CompositeConstructor]
        public RelayServer
        (
            [Inject] IGrpcConfig connectionParams,
            [Inject(resolutionKey: IoCKeys.Topics)] IEnumerable<Enum> topics
        )
        {
            BroadcastServiceImplementation serverImpl = new BroadcastServiceImplementation();
            serverImpl.RegisterTopics(topics.ToArray());

            _server = new Server()
            {
                Services = { RelayService.BindService(serverImpl) }
            };

            _server.Ports.Add
                (
                    new ServerPort
                    (
                        connectionParams.ServerName,
                        connectionParams.Port,
                        ServerCredentials.Insecure));

            _server.Start();
        }

        public async Task Shutdown()
        {
            await _server.ShutdownAsync();
        }
    }
}