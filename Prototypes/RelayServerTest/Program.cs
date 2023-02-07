using NP.Grpc.CommonRelayInterfaces;
#if DEBUG
using NP.Grpc.RelayServer;
using NP.GrpcConfig;
using NP.TestTopics;
#endif
using NP.IoCy;
using NP.Protobuf;

namespace SimpleBroadcastSubscriptionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Relay Server");

            var containerBuilder = new ContainerBuilder<Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);


#if DEBUG
            containerBuilder.RegisterSingletonType<IGrpcConfig, GrpcConfig>();
            containerBuilder.RegisterSingletonType<IRelayServer, RelayServer>();
            containerBuilder.RegisterAttributedStaticFactoryMethodsFromClass(typeof(TopicsUtils));
#else
            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");
#endif
            var container = containerBuilder.Build();

            IRelayServer relayServer = container.Resolve<IRelayServer>();

            Console.ReadLine();
        }
    }
}