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

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NP.Grpc.CommonRelayInterfaces;
using NP.Grpc.RelayServiceProto;
using NP.Utilities;
using System.Collections.Concurrent;

using Enum = System.Enum;

namespace NP.Grpc.RelayServer;

internal class BroadcastServiceImplementation : RelayService.RelayServiceBase
{
    private ConcurrentDictionary<Enum, TopicSubscriptions> _topics =
        new ConcurrentDictionary<Enum, TopicSubscriptions>();

    private TopicSubscriptions FindTopicSubscriptions(EnumValue requestTopic)
    {
        KeyValuePair<Enum, TopicSubscriptions>? nullableKeyValue =
            _topics.FirstOrDefault
            (
                keyValue => keyValue.Key.ToInt() == requestTopic.Number &&
                keyValue.Key.ToStr() == requestTopic.Name);

        if (nullableKeyValue == null)
        {
            throw new Exception($"Topic '{requestTopic.Name}' and value {requestTopic.Number} has not been registered on the server.");
        }

        KeyValuePair<Enum, TopicSubscriptions> keyValue = nullableKeyValue.Value;

        TopicSubscriptions topicSubs = keyValue.Value;

        return topicSubs;
    }

    public override async Task<ShortMsg> PublishTopic(FullMsg request, ServerCallContext context)
    {
        EnumValue requestTopic = request.Metadata.Topic;

        TopicSubscriptions topicSubs = FindTopicSubscriptions(requestTopic);

        foreach (TopicSubscription pluginTopic in topicSubs.Subscriptions)
        {
            pluginTopic.PlaceMessage(request.Message);
        }

        return new ShortMsg
        {
            MsgSentTime = Timestamp.FromDateTime(DateTime.UtcNow),
            Topic = requestTopic,
            ErrorMessage = "Hello World!!!"
        };
    }

    public override async Task GetTopicStream
    (
        ShortMsg request,
        IServerStreamWriter<FullMsg> responseStream,
        ServerCallContext context)
    {
        EnumValue requestTopic = request.Topic;

        TopicSubscriptions topicSubs = FindTopicSubscriptions(requestTopic);

        TopicSubscription pluginTopicSubscription =
            new TopicSubscription(topicSubs.Topic);

        topicSubs.Subscriptions.Add(pluginTopicSubscription);

        while (true)
        {
            try
            {
                Any message = pluginTopicSubscription.WithdrawMessage(context.CancellationToken);

                FullMsg returnMsg = new FullMsg
                {
                    Metadata =
                        new ShortMsg
                        {
                            MsgSentTime = Timestamp.FromDateTime(DateTime.UtcNow),
                            Topic = requestTopic
                        },
                    Message = message
                };

                await responseStream.WriteAsync(returnMsg);
            }
            catch when (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        topicSubs.Subscriptions.Remove(pluginTopicSubscription);
    }

    public void RegisterTopics(params Enum[] topics)
    {
        topics.DoForEach(t =>
        {
            // check that there are no duplicates
            if (_topics.ContainsKey(t))
            {
                throw new Exception($"Topic we attempt to add - '{t}' already exists on the server");
            }

            Enum? foundTopic = _topics.Keys.FirstOrDefault(top => top.ToInt() == t.ToInt());

            if (foundTopic != null)
            {
                throw new Exception($"Existing topic '{foundTopic}' already has the same integer value {t.ToInt()} as the topic we attempt to add - {t}");
            }

            foundTopic = _topics.Keys.FirstOrDefault(top => top.ToStr() == t.ToStr());

            if (foundTopic != null)
            {
                throw new Exception($"Topic with the same name that we attempt to add - '{t}' already exists on the server");
            }

            // add topic
            _topics[t] = new TopicSubscriptions(t);
        });
    }

    private class TopicSubscriptions
    {
        internal Enum Topic { get; }

        internal List<TopicSubscription> Subscriptions { get; } = new List<TopicSubscription>();

        public TopicSubscriptions(Enum topic)
        {
            Topic = topic;
        }
    }

    private class TopicSubscription
    {
        public Enum Topic { get; }

        BlockingCollection<Any> _messages = new BlockingCollection<Any>();

        public TopicSubscription(Enum topic)
        {
            Topic = topic;
        }

        public void PlaceMessage(Any message)
        {
            _messages.Add(message);
        }

        public Any WithdrawMessage(CancellationToken cancellationToken)
        {
            return _messages.Take(cancellationToken);
        }
    }
}