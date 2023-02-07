from google.protobuf import timestamp_pb2 as _timestamp_pb2
from google.protobuf import any_pb2 as _any_pb2
from google.protobuf import type_pb2 as _type_pb2
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class FullMsg(_message.Message):
    __slots__ = ["message", "metadata"]
    MESSAGE_FIELD_NUMBER: _ClassVar[int]
    METADATA_FIELD_NUMBER: _ClassVar[int]
    message: _any_pb2.Any
    metadata: ShortMsg
    def __init__(self, metadata: _Optional[_Union[ShortMsg, _Mapping]] = ..., message: _Optional[_Union[_any_pb2.Any, _Mapping]] = ...) -> None: ...

class ShortMsg(_message.Message):
    __slots__ = ["errorMessage", "msgSentTime", "topic"]
    ERRORMESSAGE_FIELD_NUMBER: _ClassVar[int]
    MSGSENTTIME_FIELD_NUMBER: _ClassVar[int]
    TOPIC_FIELD_NUMBER: _ClassVar[int]
    errorMessage: str
    msgSentTime: _timestamp_pb2.Timestamp
    topic: _type_pb2.EnumValue
    def __init__(self, msgSentTime: _Optional[_Union[_timestamp_pb2.Timestamp, _Mapping]] = ..., topic: _Optional[_Union[_type_pb2.EnumValue, _Mapping]] = ..., errorMessage: _Optional[str] = ...) -> None: ...
