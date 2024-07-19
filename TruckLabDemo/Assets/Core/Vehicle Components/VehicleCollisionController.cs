using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class ObstacleInfo
    {
        public GameObject Object;
        public float Distance;
        public float Angle;
        public string Name;
    }


    /// <summary>
    /// Manages collision detection for the vehicle using raycasts.
    /// </summary>
    public class VehicleCollisionController : VehicleComponent
    {
        public bool isActive = false;
        public bool isObstacleDetected = false;

        [Header("Vehicle Collision")]
        public float range = 100f;
        public int numberOfRays = 360;  // Number of rays to cast for 360-degree detection
        public List<ObstacleInfo> detectedObstacles = new List<ObstacleInfo>();

        public float brakingThreshold = 10f; // Threshold distance to trigger braking

        /// <summary>
        /// FixedUpdate is called at fixed intervals, ideal for handling physics and collision detection.
        /// </summary>
        private void FixedUpdate()
        {
            if (isActive)
            {
                float offsetY = 1f; // Adjust this value for the desired lift
                Vector3 rayOrigin = new Vector3(VehicleData.x0, offsetY, VehicleData.y0);

                // Clear the previous detection information
                detectedObstacles.Clear();

                // Cast rays in a circular pattern in world space
                for (int i = 0; i < numberOfRays; i++)
                {
                    float worldAngle = i * 360f / numberOfRays;
                    Vector3 worldDirection = Quaternion.Euler(0, worldAngle, 0) * Vector3.forward;

                    Ray ray = new Ray(rayOrigin, worldDirection);
                    Debug.DrawRay(rayOrigin, ray.direction * range, Color.red);

                    DetectObstacles(ray, worldAngle);
                }

                // Apply brakes if an obstacle is within the braking threshold
                if (isObstacleDetected)
                {
                    VehicleProduct.GetComponent<VehicleKinematics>().ApplyBrakes();
                }
                else
                {
                    VehicleProduct.GetComponent<VehicleKinematics>().ReleaseBrakes();
                }
            }
        }

        /// <summary>
        /// Detects obstacles using raycasting and updates obstacle information.
        /// </summary>
        /// <param name="ray">The ray used for detection.</param>
        /// <param name="worldAngle">The angle of the ray in world coordinates.</param>
        private void DetectObstacles(Ray ray, float worldAngle)
        {
            if (Physics.Raycast(ray, out RaycastHit obstacleHit, range))
            {
                var detectedObject = obstacleHit.collider.gameObject;
                var distance = obstacleHit.distance;
                var obstacleName = detectedObject.name;

                bool isPartOfOwnVehicle = false;

                if (detectedObject.CompareTag("Tractor") || detectedObject.CompareTag("Trailer"))
                {
                    obstacleName = detectedObject.transform.parent.name + " " + detectedObject.name;

                    if (VehicleProduct.ProductName == detectedObject.transform.parent.name)
                    {
                        isPartOfOwnVehicle = true;
                    }
                }

                if (!isPartOfOwnVehicle)
                {
                    // Calculate the angle in the vehicle's local frame
                    float vehicleOrientation = VehicleData.psi1 * Mathf.Rad2Deg;
                    float localAngle = (worldAngle - vehicleOrientation + 360) % 360;

                    // Check if the object is already in the list
                    var existingObstacle = detectedObstacles.Find(o => o.Object == detectedObject);

                    if (existingObstacle != null)
                    {
                        // Update the distance and angle if the new detection is closer
                        if (distance < existingObstacle.Distance)
                        {
                            existingObstacle.Distance = distance;
                            existingObstacle.Angle = localAngle;
                        }
                    }
                    else
                    {
                        // Add the new object to the list
                        detectedObstacles.Add(new ObstacleInfo
                        {
                            Object = detectedObject,
                            Distance = distance,
                            Angle = localAngle,
                            Name = obstacleName
                        });
                    }

                    // Update virtual obstacle detection info in the ROS message to publish
                    VehicleProduct.TwistPublisher.x1 = distance;
                    VehicleProduct.TwistPublisher.y1 = localAngle;

                    isObstacleDetected = true;

                    // Check if the detected obstacle is within the braking threshold
                    if (distance < brakingThreshold)
                    {
                        VehicleProduct.GetComponent<VehicleKinematics>().ApplyBrakes();
                    }
                }
            }
        }
    }
}
