﻿using NP.Grpc.CommonRelayInterfaces;
using NP.IoCy;
using NP.Protobuf;

namespace SimpleBroadcastSubscriptionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder<Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

            var container = containerBuilder.Build();

            IRelayServer relayServer = container.Resolve<IRelayServer>();

            Console.ReadLine();
        }
    }
}