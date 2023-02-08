from RelayService_pb2 import FullMsg
from BroadcastingRelayClient import BroadcastingRelayClient
from Person_pb2 import Person

from google.protobuf import any_pb2

broadcastingClient = BroadcastingRelayClient("localhost", 5051)

broadcastingClient.connect_if_needed()

#p = Person()
#p.Name = "Joe Doe"
#p.Age = 31

#metadata = broadcastingClient.create_short_msg(topic_name="PersonTopic", topic_number=1)

#a = any_pb2.Any()
#a.Pack(p)

#msg = FullMsg(metadata = metadata, message = a)

#broadcastingClient.broadcast(msg)

p = Person()
p.Name = "Joe Doe"
p.Age = 32

broadcastingClient.broadcast_object(p, topic_name="PersonTopic", topic_number=1)

input("Hello World!")