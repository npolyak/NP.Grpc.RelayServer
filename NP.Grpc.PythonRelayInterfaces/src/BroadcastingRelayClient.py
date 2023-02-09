# (c) Nick Polyak 2023
# License: MIT License (https://opensource.org/licenses/MIT)


# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import RelayService_pb2 as relay_service
from google.protobuf import message
from google.protobuf import any_pb2
from RelayClientBase import RelayClientBase

class BroadcastingRelayClient(RelayClientBase):
    def __init__(self, hostname:str, port:int):
        super(BroadcastingRelayClient, self).__init__(hostname, port, False)

    def broadcast(self, msg:relay_service.FullMsg) -> relay_service.ShortMsg:
        return self._client_stub.PublishTopic(msg)

    def broadcast_object(self, item:message.Message, topic_name:str, topic_number:int) -> relay_service.ShortMsg:
        a = any_pb2.Any()
        a.Pack(item)
        metadata = self.create_short_msg(topic_name=topic_name, topic_number=topic_number)
        msg = relay_service.FullMsg(metadata = metadata, message = a)
        return self.broadcast(msg)
