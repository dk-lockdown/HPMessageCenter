using System;

namespace MessageCenter
{
    public class TopicConfiguratorGeter
    {
        private static ITopicConfigurator topicConfigurator;

        public static ITopicConfigurator TopicHelper
        {
            get
            {
                if (topicConfigurator == null)
                {
                    throw new ArgumentException("TopicConfiguratorGeter not init!");
                }
                return topicConfigurator;
            }
        }

        public static void Init(ITopicConfigurator _topicConfigurator)
        {
            topicConfigurator = _topicConfigurator;
        }
    }

    public interface ITopicConfigurator
    {
        /// <summary>
        /// Exchange不为空，为发布订阅模式
        /// Exchange为空，为Topic模式
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        string GetExchange(string topic);

        string GetProcessorConfig(string topic);
    }
}