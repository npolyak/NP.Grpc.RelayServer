using NP.Grpc.RelayServer;
using NP.GrpcConfig;
using NP.TestTopics;
using NP.IoCy;
using NP.Protobuf;
using NP.Grpc.CommonRelayInterfaces;

namespace RelayServerTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Relay Server");

            var containerBuilder = new ContainerBuilder<Enum>();

            containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

            containerBuilder.RegisterSingletonType<IGrpcConfig, GrpcConfig>();
            containerBuilder.RegisterSingletonType<RelayServiceImplementation, RelayServiceImplementation>();
            containerBuilder.RegisterAttributedStaticFactoryMethodsFromClass(typeof(TopicsUtils));

            var container = containerBuilder.Build();

            RelayServiceImplementation relayService = container.Resolve<RelayServiceImplementation>();

            IEnumerable<System.Enum> topics = container.Resolve<IEnumerable<System.Enum>>(IoCKeys.Topics);

            relayService.RegisterTopics(topics.ToArray());
            // create builder
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            // add grpc service
            builder.Services.AddGrpc();
            builder.Services.AddSingleton(relayService);

            // create the Kestrel application
            var app = builder.Build();

            app.MapGrpcService<RelayServiceImplementation>();

            await app.RunAsync();
        }
    }
}