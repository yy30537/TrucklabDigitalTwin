using UnityEngine;

namespace Core
{
    /// <summary>
    /// Holds the state and configuration data for a vehicle.
    /// </summary>
    public class VehicleData
    {
        /// <summary>
        /// Gets the vehicle ID.
        /// </summary>
        public int VehicleID { get; private set; }

        /// <summary>
        /// Gets the vehicle name.
        /// </summary>
        public string VehicleName { get; private set; }

        /// <summary>
        /// Gets the vehicle configuration.
        /// </summary>
        public VehicleConfig VehicleConfig { get; private set; }

        /// <summary>
        /// Gets the vehicle instance in the scene.
        /// </summary>
        public GameObject VehicleInstance { get; private set; }

        // (x0, y0) - front axle position
        // (x1C, y1C) - 5th wheel tractor (coupling to semi-trailer) position
        // (x2, y2) - semitrailer center axle position
        // gamma - the articulation angle (angle between tractor and semitrailer)

        public float x0, y0, x1, y1, psi1, x1C, y1C, x2, y2, psi2;
        public float v1, a, delta, gamma, l1, l1C, l2, tractorWidth, trailerWidth;

        public float x1Prev, y1Prev, psi1Prev, x1dot, y1dot, psi1dot, x2dot, y2dot, psi2dot, v2, psi2Prev;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleData"/> class.
        /// </summary>
        /// <param name="vehicleID">The vehicle ID.</param>
        /// <param name="vehicleName">The vehicle name.</param>
        /// <param name="vehicleConfig">The vehicle configuration.</param>
        /// <param name="vehicleInstance">The vehicle instance in the scene.</param>
        public VehicleData(int vehicleID, string vehicleName, VehicleConfig vehicleConfig, GameObject vehicleInstance)
        {
            VehicleID = vehicleID;
            VehicleName = vehicleName;
            VehicleConfig = vehicleConfig;
            VehicleInstance = vehicleInstance;

            l1 = vehicleConfig.L1Scaled * vehicleConfig.Scale;
            l1C = vehicleConfig.L1CScaled * vehicleConfig.Scale;
            l2 = vehicleConfig.R2Scaled * vehicleConfig.Scale;
            tractorWidth = vehicleConfig.TractorWidthScaled * vehicleConfig.Scale;
            trailerWidth = vehicleConfig.TrailerWidthScaled * vehicleConfig.Scale;
        }

        /// <summary>
        /// Gets the bounding box of the tractor.
        /// </summary>
        /// <returns>An array of <see cref="Vector3"/> representing the corners of the tractor's bounding box.</returns>
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

        /// <summary>
        /// Gets the bounding box of the trailer.
        /// </summary>
        /// <returns>An array of <see cref="Vector3"/> representing the corners of the trailer's bounding box.</returns>
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
