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
using NP.DependencyInjection.Attributes;
using NP.Grpc.CommonRelayInterfaces;
using NP.Grpc.ServerRelayInterfaces;
using NP.Grpc.RelayServiceProto;
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
            RelayServiceImplementation serverImpl = new RelayServiceImplementation();
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