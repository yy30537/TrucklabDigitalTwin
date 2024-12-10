using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle
{
    /// <summary>
    /// Holds the state and configuration data for a vehicle entity.
    /// </summary>
    public class Vehicle_Data : MonoBehaviour
    {
        /// <summary>
        /// Unique identifier for the vehicle.
        /// </summary>
        public int Id;

        /// <summary>
        /// Name of the vehicle.
        /// </summary>
        public string Name;

        /// <summary>
        /// Configuration data associated with the vehicle.
        /// </summary>
        public Vehicle_Config Config;

        /// <summary>
        /// GameObject instance of the vehicle in the scene.
        /// </summary>
        public GameObject VehicleInstance;

        // Kinematic points and dimensions
        /// <summary>Front axle position (X0, Y0).</summary>
        public float X0, Y0;

        /// <summary>Rear axle position on the tractor (X1, Y1).</summary>
        public float X1, Y1;

        /// <summary>Fifth wheel tractor (coupling to semi-trailer) position (X1C, Y1C).</summary>
        public float X1C, Y1C;

        /// <summary>Semi-trailer center axle position (X2, Y2).</summary>
        public float X2, Y2;

        /// <summary>Articulation angle (gamma) between tractor and semi-trailer.</summary>
        public float Gamma;

        /// <summary>Orientation angles (psi1 for tractor, psi2 for trailer).</summary>
        public float Psi1, Psi2;

        // Vehicle dimensions
        /// <summary>Distance from front axle to fifth wheel on the tractor.</summary>
        public float L1;

        /// <summary>Distance from fifth wheel to center axle on the trailer.</summary>
        public float L1C;

        /// <summary>Distance from the fifth wheel to the rear axle of the trailer.</summary>
        public float L2;

        /// <summary>Width of the tractor.</summary>
        public float TractorWidth;

        /// <summary>Width of the trailer.</summary>
        public float TrailerWidth;

        // Kinematic state variables
        /// <summary>Velocity of the tractor (V1).</summary>
        public float V1;

        /// <summary>Acceleration of the tractor (a).</summary>
        public float a;

        /// <summary>Steering angle of the tractor (delta).</summary>
        public float Delta;

        // Previous state variables
        /// <summary>Previous positions and orientations of the tractor and trailer.</summary>
        public float X1Prev, Y1Prev, Psi1Prev, Psi2Prev;

        /// <summary>Derivatives of positions and orientations.</summary>
        public float X1dot, Y1dot, Psi1dot, X2dot, Y2dot, Psi2dot;

        /// <summary>Velocity of the trailer (V2).</summary>
        public float V2;

        /// <summary>
        /// Initializes the vehicle data based on configuration.
        /// </summary>
        /// <param name="vehicleId">Unique ID of the vehicle.</param>
        /// <param name="vehicleName">Name of the vehicle.</param>
        /// <param name="vehicleConfig">Configuration data for the vehicle.</param>
        /// <param name="vehicleInstance">GameObject instance of the vehicle in the scene.</param>
        public void Init(int vehicleId, string vehicleName, Vehicle_Config vehicleConfig, GameObject vehicleInstance)
        {
            Id = vehicleId;
            Name = vehicleName;
            Config = vehicleConfig;
            VehicleInstance = vehicleInstance;

            // Scale dimensions using the configuration scale factor
            L1 = vehicleConfig.L1Scaled * vehicleConfig.Scale;
            L1C = vehicleConfig.L1CScaled * vehicleConfig.Scale;
            L2 = vehicleConfig.R2Scaled * vehicleConfig.Scale;
            TractorWidth = vehicleConfig.TractorWidthScaled * vehicleConfig.Scale;
            TrailerWidth = vehicleConfig.TrailerWidthScaled * vehicleConfig.Scale;
        }

        /// <summary>
        /// Calculates the bounding box of the tractor.
        /// </summary>
        /// <returns>An array of Vector3 points representing the corners of the bounding box.</returns>
        public Vector3[] GetTractorBoundingBox()
        {
            float halfWidth = TractorWidth / 2;

            // Calculate the four corners of the tractor's bounding box
            Vector3 frontLeft = new Vector3(
                X0 - halfWidth * Mathf.Sin(Psi1),
                0,
                Y0 + halfWidth * Mathf.Cos(Psi1)
            );

            Vector3 frontRight = new Vector3(
                X0 + halfWidth * Mathf.Sin(Psi1),
                0,
                Y0 - halfWidth * Mathf.Cos(Psi1)
            );

            Vector3 rearLeft = new Vector3(
                X1 - halfWidth * Mathf.Sin(Psi1),
                0,
                Y1 + halfWidth * Mathf.Cos(Psi1)
            );

            Vector3 rearRight = new Vector3(
                X1 + halfWidth * Mathf.Sin(Psi1),
                0,
                Y1 - halfWidth * Mathf.Cos(Psi1)
            );

            return new Vector3[] { frontLeft, frontRight, rearRight, rearLeft };
        }

        /// <summary>
        /// Calculates the bounding box of the trailer.
        /// </summary>
        /// <returns>An array of Vector3 points representing the corners of the bounding box.</returns>
        public Vector3[] GetTrailerBoundingBox()
        {
            float halfWidth = TrailerWidth / 2;

            // Calculate the four corners of the trailer's bounding box
            Vector3 frontLeft = new Vector3(
                X1C - halfWidth * Mathf.Sin(Psi2),
                0,
                Y1C + halfWidth * Mathf.Cos(Psi2)
            );

            Vector3 frontRight = new Vector3(
                X1C + halfWidth * Mathf.Sin(Psi2),
                0,
                Y1C - halfWidth * Mathf.Cos(Psi2)
            );

            Vector3 rearLeft = new Vector3(
                X2 - halfWidth * Mathf.Sin(Psi2),
                0,
                Y2 + halfWidth * Mathf.Cos(Psi2)
            );

            Vector3 rearRight = new Vector3(
                X2 + halfWidth * Mathf.Sin(Psi2),
                0,
                Y2 - halfWidth * Mathf.Cos(Psi2)
            );

            return new Vector3[] { frontLeft, frontRight, rearRight, rearLeft };
        }
    }
}
