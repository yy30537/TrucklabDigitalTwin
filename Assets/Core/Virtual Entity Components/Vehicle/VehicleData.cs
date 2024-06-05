using UnityEngine;

namespace Core
{
    public class VehicleData
    {
        public int VehicleID { get; private set; }
        public string VehicleName { get; private set; }
        public VehicleConfig VehicleConfig { get; private set; }
        public GameObject VehicleInstance { get; private set; }

        // (x0, y0) - front axle position
        // (x1C, y1C) - 5th wheel tractor (coupling to semi-trailer) position
        // (x2, y2) - semitrailer center axle position
        // gamma - the articulation angle (angle between tractor and semitrailer)
        
        
        public float x0, y0, x1, y1, psi1, x1C, y1C, x2, y2, psi2;
        public float v1, a, delta, gamma, l1, l1C, l2, tractorWidth, trailerWidth;

        public float x1Prev, y1Prev, psi1Prev, x1dot, y1dot, psi1dot, x2dot, y2dot, psi2dot, v2, psi2Prev;
        
        public VehicleData(int vehicleID, string vehicleName, VehicleConfig vehicleConfig, GameObject vehicleInstance)
        {
            VehicleID = vehicleID;
            VehicleName = vehicleName;
            VehicleConfig = vehicleConfig;
            VehicleInstance = vehicleInstance;
            
            l1 = vehicleConfig.l1Scaled * vehicleConfig.scale; 
            l1C = vehicleConfig.l1CScaled * vehicleConfig.scale;
            l2 = vehicleConfig.l2Scaled * vehicleConfig.scale;
            tractorWidth = vehicleConfig.tractorWidthScaled * vehicleConfig.scale;
            trailerWidth = vehicleConfig.trailerWidthScaled * vehicleConfig.scale;
        }
        
        public Vector3[] GetTractorBoundingBox()
        {
            float halfWidth = tractorWidth / 2;

            // Calculate the four corners of the tractor's bounding box
            Vector3 frontLeft = new Vector3(
                x0 - halfWidth * Mathf.Sin(psi1),
                0,
                y0 + halfWidth * Mathf.Cos(psi1)
            );

            Vector3 frontRight = new Vector3(
                x0 + halfWidth * Mathf.Sin(psi1),
                0,
                y0 - halfWidth * Mathf.Cos(psi1)
            );

            Vector3 rearLeft = new Vector3(
                x1 - halfWidth * Mathf.Sin(psi1),
                0,
                y1 + halfWidth * Mathf.Cos(psi1)
            );

            Vector3 rearRight = new Vector3(
                x1 + halfWidth * Mathf.Sin(psi1),
                0,
                y1 - halfWidth * Mathf.Cos(psi1)
            );

            return new Vector3[] { frontLeft, frontRight, rearRight, rearLeft };
        }

        public Vector3[] GetTrailerBoundingBox()
        {
            float halfWidth = trailerWidth / 2;

            // Calculate the four corners of the trailer's bounding box
            Vector3 frontLeft = new Vector3(
                x1C - halfWidth * Mathf.Sin(psi2),
                0,
                y1C + halfWidth * Mathf.Cos(psi2)
            );

            Vector3 frontRight = new Vector3(
                x1C + halfWidth * Mathf.Sin(psi2),
                0,
                y1C - halfWidth * Mathf.Cos(psi2)
            );

            Vector3 rearLeft = new Vector3(
                x2 - halfWidth * Mathf.Sin(psi2),
                0,
                y2 + halfWidth * Mathf.Cos(psi2)
            );

            Vector3 rearRight = new Vector3(
                x2 + halfWidth * Mathf.Sin(psi2),
                0,
                y2 - halfWidth * Mathf.Cos(psi2)
            );

            return new Vector3[] { frontLeft, frontRight, rearRight, rearLeft };
        }


    }
}
