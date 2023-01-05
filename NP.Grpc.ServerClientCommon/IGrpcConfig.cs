namespace NP.Grpc.CommonRelayInterfaces;

public interface IGrpcConfig
{
    string ServerName { get; }

    int Port { get; }
}
