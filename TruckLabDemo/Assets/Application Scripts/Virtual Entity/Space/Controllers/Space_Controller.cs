using Application_Scripts.Virtual_Entity.Vehicle;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Space.Controllers
{
    /// <summary>
    /// Manages the behavior and interactions of a space within the simulation, including vehicle detection and management.
    /// </summary>
    public class Space_Controller : MonoBehaviour
    {
        /// <summary>
        /// Data associated with the space, including its configuration and vehicles present within it.
        /// </summary>
        [Header("Space Data")]
        public Space_Data SpaceData;

        /// <summary>
        /// Reference to the vehicle creator for managing vehicles within the space.
        /// </summary>
        [Header("Vehicle Creator")]
        public Vehicle_Creator VehicleCreator;

        /// <summary>
        /// Periodically checks for vehicles present within the space based on their bounding boxes.
        /// </summary>
        private void FixedUpdate()
        {
            // Ensure vehicle detection logic runs only if there are vehicles present.
            if (SpaceData.VehiclesPresentInSpace != null)
            {
                DetectVehiclesInSpace();
            }
        }

        /// <summary>
        /// Initializes the space controller with the specified data and vehicle creator.
        /// </summary>
        /// <param name="data">The space data containing state and configuration.</param>
        /// <param name="creator">The vehicle creator managing vehicle instances.</param>
        public void Init(Space_Data data, Vehicle_Creator creator)
        {
            SpaceData = data;
            VehicleCreator = creator;
        }

        /// <summary>
        /// Detects vehicles present in the space based on their bounding boxes and updates the space data.
        /// </summary>
        private void DetectVehiclesInSpace()
        {
            if (VehicleCreator.LookupTable.Count > 0)
            {
                foreach (var vehicle in VehicleCreator.LookupTable)
                {
                    // Get bounding boxes for the tractor and trailer of the vehicle.
                    Vector3[] tractorBoundingBox = vehicle.Value.Data.GetTractorBoundingBox();
                    Vector3[] trailerBoundingBox = vehicle.Value.Data.GetTrailerBoundingBox();

                    // Check if either bounding box is within the space polygon.
                    bool isVehicleBoundingBoxInSpace =
                        IsBoundingBoxInSpace(tractorBoundingBox, SpaceData.Config.SpaceMarkings) ||
                        IsBoundingBoxInSpace(trailerBoundingBox, SpaceData.Config.SpaceMarkings);

                    // Add or remove the vehicle from the space's vehicle list based on its position.
                    if (isVehicleBoundingBoxInSpace && !IsVehicleInSpace(vehicle.Value.Id))
                    {
                        SpaceData.VehiclesPresentInSpace.Add(vehicle.Value.Id, vehicle.Value);
                    }
                    else if (!isVehicleBoundingBoxInSpace && IsVehicleInSpace(vehicle.Value.Id))
                    {
                        SpaceData.VehiclesPresentInSpace.Remove(vehicle.Value.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether a bounding box is inside the defined space polygon.
        /// </summary>
        /// <param name="boundingBox">The bounding box of the vehicle.</param>
        /// <param name="polygon">The vertices of the space polygon.</param>
        /// <returns>True if any part of the bounding box is inside the polygon; otherwise, false.</returns>
        private bool IsBoundingBoxInSpace(Vector3[] boundingBox, Vector3[] polygon)
        {
            foreach (var point in boundingBox)
            {
                if (IsPointInPolygon(point, polygon))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a given point is inside a polygon defined by vertices.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="polygon">The vertices defining the polygon.</param>
        /// <returns>True if the point is inside the polygon; otherwise, false.</returns>
        private bool IsPointInPolygon(Vector3 point, Vector3[] polygon)
        {
            bool isInside = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; j = i++)
            {
                if (((polygon[i].z > point.z) != (polygon[j].z > point.z)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        /// <summary>
        /// Checks if a vehicle with the specified ID is currently present in the space.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle.</param>
        /// <returns>True if the vehicle is in the space; otherwise, false.</returns>
        public bool IsVehicleInSpace(int id)
        {
            return SpaceData.VehiclesPresentInSpace.ContainsKey(id);
        }
    }
}
