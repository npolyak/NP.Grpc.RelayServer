namespace NP.Grpc.ServerClientCommon;

public interface IGrpcConfig
{
    string ServerName { get; }

    int Port { get; }
}
