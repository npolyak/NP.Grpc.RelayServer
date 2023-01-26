using Grpc.Core;
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
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder<System.Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

            var container = containerBuilder.Build();

            IRelayClient relayClient = container.Resolve<IRelayClient>();

            relayClient.ObserveTopicStream<Person>(TestTopics.PersonTopic, _cts.Token)
                       .Subscribe(OnPersonRecordArrived, OnError, OnComplete);

            Console.ReadLine();
        }

        private static void OnComplete()
        {
            
        }

        private static void OnError(Exception obj)
        {
            if (obj is RpcException rpcException)
            {
                if (rpcException.StatusCode == StatusCode.Cancelled)
                {

                }
            }
        }

        static int numberTimesCalled = 0;
        private static void OnPersonRecordArrived(Person p)
        {
            numberTimesCalled++;

            Console.WriteLine(p.ToString());

            if (numberTimesCalled == 3)
            {
                _cts.Cancel();
            }
        }
    }
}