# import python packages
import asyncio
import concurrent.futures
import grpc
import datetime

# import the client stubs (service_pb2 contains messages, 
# service_pb2_grpc contains RPCs)
import RelayService_pb2 as relay_service
import RelayService_pb2_grpc as relay_service_grpc
from google.protobuf import type_pb2 as proto_types
from google.protobuf import timestamp_pb2

class RelayClientBase:
    def __init__(self, hostname:str, port:int, is_async:bool=True):
        self._is_async = is_async
        self._hostname = hostname
        self._port = port;
        self._executor = concurrent.futures.ThreadPoolExecutor(max_workers=1)
        self._client_stub = None
    
    def __create_channel__(self) -> relay_service_grpc.RelayServiceStub:
        hostport = self._hostname + ":" + str(self._port)
        if self._is_async:
            self._channel = grpc.aio.insecure_channel(hostport)
        else:
            self._channel = grpc.insecure_channel(hostport)
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





