from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class Person(_message.Message):
    __slots__ = ["Age", "Name"]
    AGE_FIELD_NUMBER: _ClassVar[int]
    Age: int
    NAME_FIELD_NUMBER: _ClassVar[int]
    Name: str
    def __init__(self, Name: _Optional[str] = ..., Age: _Optional[int] = ...) -> None: ...
