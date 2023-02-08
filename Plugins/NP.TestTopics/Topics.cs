// (c) Nick Polyak 2023 
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
using NP.Protobuf;

namespace NP.TestTopics
{
    public enum TestTopics
    {
        None, 
        PersonTopic
    }

    [HasRegisterMethods]
    public static class TopicsUtils
    {
        [RegisterMultiCellMethod(cellType:typeof(Enum), resolutionKey:IoCKeys.Topics)]
        public static IEnumerable<Enum> GetTopics()
        {
            return new Enum[] { TestTopics.PersonTopic };
        }
    }
}