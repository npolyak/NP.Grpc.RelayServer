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
using NP.Grpc.CommonRelayInterfaces;
#if DEBUG
using NP.Grpc.RelayClient;
using NP.GrpcConfig;
#endif
using NP.IoCy;
using NP.PersonTest;
using NP.Protobuf;
using NP.TestTopics;

namespace SimpleBroadcastSubscriptionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Publishing C# Client");

            var containerBuilder = new ContainerBuilder<System.Enum>();

            containerBuilder.RegisterMultiCell(typeof(System.Enum), IoCKeys.Topics);


#if DEBUG
            containerBuilder.RegisterSingletonType<IGrpcConfig, GrpcConfig>();
            containerBuilder.RegisterSingletonType<IRelayClient, RelayClient>();
#else
            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");
#endif
            var container = containerBuilder.Build();

            IRelayClient relayClient = container.Resolve<IRelayClient>();

            relayClient.PublishTopic(TestTopics.PersonTopic, Any.Pack(new Person { Name = "Joe", Age = 25 }));

            Console.ReadLine();
        }
    }
}