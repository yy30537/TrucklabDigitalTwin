using Newtonsoft.Json;
using RosSharp.RosBridgeClient;

namespace RosSharp.Scripts.RosBridgeClient.RosCommuncation
{
    public class SetSimulationDetailsResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "your_package/SetSimulationDetailsResponse";
        public bool success;
        public string message;

        public SetSimulationDetailsResponse()
        {
            success = false;
            message = "";
        }

        public SetSimulationDetailsResponse(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }
    }
}