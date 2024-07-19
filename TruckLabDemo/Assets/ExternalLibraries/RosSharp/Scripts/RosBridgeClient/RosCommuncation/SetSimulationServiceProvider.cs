using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Custom;

namespace RosSharp.RosBridgeClient
{
    public class SetSimulationServiceProvider : UnityServiceProvider<SetSimulationDetailsRequest, SetSimulationDetailsResponse>
    {
        public Dictionary<int, int> simulationDetails = new Dictionary<int, int>();

        protected override bool ServiceCallHandler(SetSimulationDetailsRequest request, out SetSimulationDetailsResponse response)
        {
            response = new SetSimulationDetailsResponse();
            if (simulationDetails.TryGetValue(request.vehicleID, out int responseValue) && responseValue == request.pathID)
            {
                response.success = true;
                response.message = "Simulation details set successfully";
                return true;
            }
            response.success = false;
            response.message = "Failed to set simulation details";
            return false;
        }

        public void SetSimulationDetail(int vehicleID, int pathID)
        {
            simulationDetails[vehicleID] = pathID;
        }
    }
}