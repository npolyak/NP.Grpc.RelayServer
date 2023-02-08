# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import RelayService_pb2 as relay_service
from RelayClientBase import RelayClientBase

class BroadcastingRelayClient(RelayClientBase):
    def __init__(self, hostname:str, port:int):
        super(BroadcastingRelayClient, self).__init__(hostname, port, False)

    def broadcast(self, msg:relay_service.FullMsg):
        self._client_stub.PublishTopic(msg)
