﻿// (c) Nick Polyak 2023 
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using NP.DependencyInjection.Attributes;
using NP.Grpc.CommonRelayApi;

namespace NP.GrpcConfig
{
    [RegisterType]
    public class GrpcConfig : IGrpcConfig
    {
        public string ServerName => "localhost";

        public int Port => 5051;
    }
}