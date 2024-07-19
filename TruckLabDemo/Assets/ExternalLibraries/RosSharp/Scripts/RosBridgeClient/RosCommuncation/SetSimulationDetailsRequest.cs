using RosSharp.RosBridgeClient.MessageTypes.Std;
using Newtonsoft.Json;

namespace RosSharp.RosBridgeClient.MessageTypes.Custom
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