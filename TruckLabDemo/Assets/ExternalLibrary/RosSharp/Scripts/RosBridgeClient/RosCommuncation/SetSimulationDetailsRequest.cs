using Newtonsoft.Json;
using RosSharp.RosBridgeClient;

namespace RosSharp.Scripts.RosBridgeClient.RosCommuncation
{
    public class SetSimulationDetailsRequest : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "your_package/SetSimulationDetailsRequest";
        public int vehicleID;
        public int pathID;

        public SetSimulationDetailsRequest()
        {
            vehicleID = 0;
            pathID = 0;
        }

        public SetSimulationDetailsRequest(int vehicleID, int pathID)
        {
            this.vehicleID = vehicleID;
            this.pathID = pathID;
        }
    }
}