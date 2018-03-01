using MessageTransit;
using MessageTransit.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter
{
    public class RestApiProcessor : IProcessor
    {
        public async Task<bool> Process(IMessage message)
        {
            await Task.Run(() =>
            {

            });
            return false;
            message = (TextMessage)message;
            string host = TopicConfiguratorGeter.TopicHelper.GetProcessorConfig(message.Headers[BuiltinKeys.Topic]);
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(host);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(message));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(host, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var resStr = await response.Content.ReadAsStringAsync();
                    StandResponse res = JsonConvert.DeserializeObject<StandResponse>(resStr);
                    if (res != null && res.Code == 0)
                    {
                        return true;
                    }
                    throw new MessageTransitException(resStr);
                }
                throw new MessageTransitException($"ResponseStatusCode:{response.StatusCode}\r\n{await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
