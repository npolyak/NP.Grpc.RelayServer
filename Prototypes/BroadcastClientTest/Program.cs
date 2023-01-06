using Google.Protobuf.WellKnownTypes;
using NP.Grpc.CommonRelayInterfaces;
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
            var containerBuilder = new ContainerBuilder<System.Enum>();

            containerBuilder.RegisterMultiCell(typeof(System.Enum), IoCKeys.Topics);

            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

            var container = containerBuilder.Build();

            IRelayClient relayClient = container.Resolve<IRelayClient>();

            relayClient.Broadcast(TestTopics.PersonTopic, Any.Pack(new Person { Name = "Joe", Age = 25 }));

            Console.ReadLine();
        }
    }
}