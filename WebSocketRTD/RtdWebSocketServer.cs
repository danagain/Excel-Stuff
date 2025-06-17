using ExcelDna.Integration.Rtd;
using WebSocket4Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WebSocketRTD
{


    [ComVisible(true)]
    [ProgId("RtdWebSocket.Server")]
    public class RtdWebSocketServer : ExcelRtdServer
    {
        class Subscription
        {
            public Topic Topic;
            public string Url;
            public string Channel;
        }

        Dictionary<string, Subscription> _subscriptions = new Dictionary<string, Subscription>();
        WebSocket _ws;

        protected override bool ServerStart() => true;
        protected override void ServerTerminate() { if (_ws != null) _ws.Close(); }

        protected override object ConnectData(Topic topic, IList<string> topicInfo, ref bool newValues)
        {
            if (topicInfo.Count < 2)
                return "Usage: =dnaRtdWebSocket(\"ws://localhost:8765\", \"channel\")";

            string url = topicInfo[0];
            string channel = topicInfo[1];
            string key = url + "|" + channel;

            if (!_subscriptions.ContainsKey(key))
            {
                _subscriptions[key] = new Subscription { Topic = topic, Url = url, Channel = channel };
                ConnectWebSocket(url); // shared connection
            }

            return "Waiting for data...";
        }

        protected override void DisconnectData(Topic topic)
        {
            foreach (var kv in new List<KeyValuePair<string, Subscription>>(_subscriptions))
            {
                if (kv.Value.Topic == topic)
                {
                    _subscriptions.Remove(kv.Key);
                    break;
                }
            }
        }

        private void ConnectWebSocket(string url)
        {
            if (_ws != null && _ws.State == WebSocketState.Open) return;
            _ws = new WebSocket(url);
            _ws.MessageReceived += (s, e) =>
            {
                // Example message: "sensorA:123.45"
                var parts = e.Message.Split(':');
                if (parts.Length == 2)
                {
                    string channel = parts[0];
                    string value = parts[1];
                    foreach (var kv in _subscriptions)
                    {
                        if (kv.Value.Channel == channel)
                            kv.Value.Topic.UpdateValue(value);
                    }
                }
            };
            _ws.Open();
        }
    }
}
