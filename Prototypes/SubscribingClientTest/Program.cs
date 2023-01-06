using NP.Grpc.CommonRelayInterfaces;
using NP.IoCy;
using NP.PersonTest;
using NP.Protobuf;
using NP.TestTopics;
using System;

namespace SimpleBroadcastSubscriptionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder<System.Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

            var container = containerBuilder.Build();

            IRelayClient relayClient = container.Resolve<IRelayClient>();

            relayClient.Subscribe<Person>(TestTopics.PersonTopic).Subscribe(OnPersonRecordArrived);

            Console.ReadLine();
        }

        private static void OnPersonRecordArrived(Person p)
        {
            Console.WriteLine(p.ToString());
        }
    }
}