using MessageCenter.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter.BLL
{
    public class TopicSvc
    {
        public static void CreateExchange(Exchange exchange)
        {
            if (TopicDA.ExistsExchange(exchange.Name))
            {
                throw new BusinessException("Name已经存在！");
            }
            TopicDA.CreateExchange(exchange);
        }

        public static IEnumerable<Exchange> LoadExchanges()
        {
            return TopicDA.LoadExchanges();
        }

        public static void CreateTopic(Topic topic)
        {
            if (TopicDA.ExistsTopic(topic.Name))
            {
                throw new BusinessException("Name已经存在！");
            }
            TopicDA.CreateTopic(topic);
        }

        public static void EditTopic(Topic topic)
        {
            TopicDA.EditTopic(topic);
        }

        public static Topic LoadTopicBySysNo(int sysno)
        {
            return TopicDA.LoadTopicBySysNo(sysno);
        }

        public static Topic LoadTopicByTopicName(string name)
        {
            return TopicDA.LoadTopicByTopicName(name);
        }

        public static IEnumerable<Topic> LoadTopics()
        {
            return TopicDA.LoadTopics();
        }

        public static IEnumerable<Topic> LoadValidTopics()
        {
            return TopicDA.LoadValidTopics();
        }

        public static void UpdateTopicStatusToValid(int sysno)
        {
            TopicDA.UpdateTopicStatusToValid(sysno);
        }

        public static void UpdateTopicStatusToInValid(int sysno)
        {
            TopicDA.UpdateTopicStatusToInValid(sysno);
        }
    }
}
