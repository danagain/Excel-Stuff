using ExcelDna.Integration;

namespace WebSocketRTD
{
    public static class RtdWebSocketFunctions
    {
        [ExcelFunction(Description = "Connects to a WebSocket stream")]
        public static object dnaRtdWebSocket(string url, string topic)
        {
            return XlCall.RTD("RtdWebSocket.Server", null, url, topic);
        }

        [ExcelFunction(Description = "Test function")]
        public static string helloWorld()
        {
            return "hello from dna";
        }
    }
}