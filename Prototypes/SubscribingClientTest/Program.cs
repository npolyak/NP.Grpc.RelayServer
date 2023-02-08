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
        private static CancellationTokenSource _cts = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.WriteLine("Subscribing C# Client");

            var containerBuilder = new ContainerBuilder<System.Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

#if DEBUG
            containerBuilder.RegisterSingletonType<IGrpcConfig, GrpcConfig>();
            containerBuilder.RegisterSingletonType<IRelayClient, RelayClient>();
#else
            containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");
#endif
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