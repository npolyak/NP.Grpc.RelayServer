# import python packages
import asyncio
import concurrent.futures
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

    async def __observe_topic_async_impl__(self, topic_name:str, topic_number:int) -> None:
        hostport = self._hostname + ":" + str(self._port)

        channel = grpc.aio.insecure_channel(hostport)
        stub = relay_service_grpc.RelayServiceStub(channel)

        topicEnumValue = proto_types.EnumValue(name=topic_name, number=topic_number)
        current_utc_date_time = datetime.datetime.now(datetime.timezone.utc)
        current_utc_timestamp = timestamp_pb2.Timestamp()
        current_utc_timestamp.FromDatetime(current_utc_date_time)

        requestMessage = relay_service.ShortMsg(topic=topicEnumValue, msgSentTime=current_utc_timestamp)

        self._stream = stub.GetTopicStream(requestMessage)


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

