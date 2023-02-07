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