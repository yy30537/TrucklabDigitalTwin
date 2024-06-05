using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class CollisionController : VehicleComponent
    {
        public bool isActive = false;
        public bool isObstacleDetected = false;

        [Header("Vehicle CollisionController")]
        public float range = 100;
        public int numberOfRays = 360;  // Number of rays to cast for 360-degree detection
        public List<ObstacleInfo> detectedObstacles = new List<ObstacleInfo>();

        void FixedUpdate()
        {
            if (isActive)
            {
                float offsetY = 1f; // Adjust this value for the desired lift
                Vector3 rayOrigin = new Vector3(VehicleProduct.vehicleData.x0, offsetY, VehicleProduct.vehicleData.y0);

                // Clear the previous detection information
                detectedObstacles.Clear();

                // Cast rays in a circular pattern
                for (int i = 0; i < numberOfRays; i++)
                {
                    float localAngle = (i * 360f / numberOfRays); // Calculate the angle for each ray in local space
                    Vector3 localDirection = Quaternion.Euler(0, localAngle, 0) * Vector3.forward;

                    // Convert local direction to world direction
                    Vector3 worldDirection = VehicleProduct.vehicleAnimation.tractorTransform.TransformDirection(localDirection);

                    Ray ray = new Ray(rayOrigin, worldDirection);
                    Debug.DrawRay(rayOrigin, ray.direction * range, Color.red);

                    DetectObstacles(ray, localAngle);
                }
            }
        }

        void DetectObstacles(Ray ray, float localAngle)
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

                    if (VehicleProduct.productName == detectedObject.transform.parent.name)
                    {
                        isPartOfOwnVehicle = true;
                    }
                }

                if (!isPartOfOwnVehicle)
                {
                    // Use the local angle directly as the world angle
                    float worldAngle = localAngle;

                    // Check if the object is already in the list
                    var existingObstacle = detectedObstacles.Find(o => o.Object == detectedObject);

                    if (existingObstacle != null)
                    {
                        // Update the distance and angle if the new detection is closer
                        if (distance < existingObstacle.Distance)
                        {
                            existingObstacle.Distance = distance;
                            existingObstacle.Angle = worldAngle;
                        }
                    }
                    else
                    {
                        // Add the new object to the list
                        detectedObstacles.Add(new ObstacleInfo
                        {
                            Object = detectedObject,
                            Distance = distance,
                            Angle = worldAngle,
                            Name = obstacleName
                        });
                    }
                }
            }
            else
            {
                isObstacleDetected = false;
            }
        }
    }

    [System.Serializable]
    public class ObstacleInfo
    {
        public GameObject Object;
        public float Distance;
        public float Angle;
        public string Name;
    }
}
