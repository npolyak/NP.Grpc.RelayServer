from RelayService_pb2 import FullMsg
from ObservingRelayClient import ObservingRelayClient
from Person_pb2 import Person

observingClient = ObservingRelayClient("localhost", 5051)

observingClient.observe_topic(topic_name="PersonTopic", topic_number=1)

def print_msg(msg:FullMsg):
    p = Person()
    if msg.message.Unpack(p) == True:
        print(p.Name)

#observable = observingClient.get_observable();

#observable.subscribe(on_next=print_msg)

person_observable = observingClient.get_concrete_observable(lambda:Person())

person_observable.subscribe(on_next = lambda p:print("\n" + p.Name))

input("Press enter to close program!!!!!!!!!!!\n\n")