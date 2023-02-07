# import python packages
import asyncio
import concurrent.futures
from winreg import EnumValue
import grpc
import reactivex
import datetime

# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import RelayService_pb2 as relay_service
import RelayService_pb2_grpc as relay_service_grpc
from google.protobuf import type_pb2 as proto_types
from google.protobuf import timestamp_pb2

class ObservingRelayClient:
    def __init__(self, hostname:str, port:int):
        self._hostname = hostname
        self._port = port;
        self._executor = concurrent.futures.ThreadPoolExecutor(max_workers=1)
        self._client_stub = None
    
    def __create_channel__(self) -> relay_service_grpc.RelayServiceStub:
        hostport = self._hostname + ":" + str(self._port)
        self._channel = grpc.aio.insecure_channel(hostport)
        stub = relay_service_grpc.RelayServiceStub(self._channel)
        return stub

    def connect_if_needed(self):
        if (self._client_stub == None):
            self._client_stub = self.__create_channel__()

    def create_short_msg(self, topic_name:str, topic_number:int) -> relay_service.ShortMsg:
        current_utc_date_time = datetime.datetime.now(datetime.timezone.utc)
        current_utc_timestamp = timestamp_pb2.Timestamp()
        current_utc_timestamp.FromDatetime(current_utc_date_time)

        topic_enum_val = proto_types.EnumValue(name=topic_name, number=topic_number)
        msg = relay_service.ShortMsg(topic=topic_enum_val, msgSentTime=current_utc_timestamp)

        return msg

    def broadcast(self, msg:relay_service.FullMsg):
        self._client_stub.PublishTopic(msg)

    async def __observe_topic_async_impl__(self, t_name:str, t_number:int) -> None:
        self.connect_if_needed()

        requestMessage = self.create_short_msg(topic_name=t_name, topic_number=t_number)

        self._stream = self._client_stub.GetTopicStream(requestMessage)

        try:
            async for msg in self._stream:
                self._observable.on_next(msg)
        except grpc.RpcError as rpc_error:
            if (rpc_error.code() == grpc.StatusCode.CANCELLED):
                self._observable.on_completed()
            else:
                self._observable.on_error(rpc_error)

    def __run_observe_topic_impl__(self, topic_name:str, topic_number:int):
        asyncio.run((self.__observe_topic_async_impl__(topic_name, topic_number)))

    def observe_topic(self, topic_name:str, topic_number:int) -> None:
        self._observable = reactivex.Subject[relay_service.FullMsg]()

        self._executor.submit(self.__run_observe_topic_impl__, topic_name, topic_number)

    def cancel(self):
        self._stream.cancel()

    def get_observable(self) -> reactivex.Observable[relay_service.FullMsg]:
        return self._observable;

