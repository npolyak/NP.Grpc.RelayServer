using NP.DependencyInjection.Attributes;
using NP.Grpc.CommonRelayInterfaces;

namespace NP.GrpcConfig
{
    [RegisterType]
    public class GrpcConfig : IGrpcConfig
    {
        public string ServerName => "localhost";

        public int Port => 5051;
    }
}