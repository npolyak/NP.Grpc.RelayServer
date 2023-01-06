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
        [RegisterMethod(isSingleton:true, resolutionKey:IoCKeys.Topics)]
        public static IEnumerable<Enum> GetTopics()
        {
            return new Enum[] { TestTopics.PersonTopic };
        }
    }
}