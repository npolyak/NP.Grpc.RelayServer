# import python packages
import asyncio
import grpc
import reactivex

# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import RelayService_pb2 as relay_service
import RelayService_pb2_grpc as relay_service_grpc
from google.protobuf import type_pb2 as proto_types
from google.protobuf import timestamp_pb2
from RelayClientBase import RelayClientBase

class ObservingRelayClient(RelayClientBase):
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

