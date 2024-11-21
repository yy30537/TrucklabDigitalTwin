using System.Collections.Generic;
using UnityEngine;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers.Collision_Control
{
    /// <summary>
    /// Manages collision detection for the vehicle using a dedicated sensor.
    /// Tracks detected obstacles and triggers actions such as braking based on proximity and angle.
    /// </summary>
    public class Vehicle_Collision_Controller : MonoBehaviour
    {
        /// <summary>
        /// The associated vehicle object.
        /// </summary>
        public VE_Vehicle VeVehicle;

        /// <summary>
        /// The associated vehicle's data.
        /// </summary>
        public Vehicle_Data VehicleData;

        [Header("Collider References")]
        public Collider TractorBoxCollider; // Box collider for the tractor section of the vehicle.
        public Collider TrailerBoxCollider; // Box collider for the trailer section of the vehicle.
        public SphereCollider SensorSphereCollider; // Sphere collider used for obstacle detection around the vehicle.

        /// <summary>
        /// The current center position of the vehicle's obstacle detection sensor.
        /// </summary>
        public Vector3 VehicleObstacleSensorCenter;

        /// <summary>
        /// Indicates whether the collision detection system is active.
        /// </summary>
        public bool IsActive = false;

        /// <summary>
        /// Indicates if an obstacle is currently detected.
        /// </summary>
        public bool IsObstacleDetected = false;

        [Header("Vehicle RayCast Obstacle Detection Settings")]
        public float Range = 100f; // The maximum detection range of the obstacle sensor.
        public int NumRays = 360; // Number of rays cast in a 360-degree pattern for obstacle detection.
        public List<Obstacle_Info> DetectedObstacles = new List<Obstacle_Info>(); // List of currently detected obstacles.

        public float BrakingThreshold = 10f; // Threshold distance to trigger braking.

        /// <summary>
        /// FixedUpdate is called at fixed intervals. Updates the position of the sensor and obstacle data.
        /// </summary>
        private void FixedUpdate()
        {
            if (IsActive)
            {
                // Update the sphere collider center based on vehicle data
                VehicleObstacleSensorCenter.x = VehicleData.X1C;
                VehicleObstacleSensorCenter.z = VehicleData.Y1C;
                SensorSphereCollider.center = VehicleObstacleSensorCenter;

                // Update data for detected obstacles
                UpdateDetectedObstacles();
            }
        }

        /// <summary>
        /// Initializes the collision controller with the given vehicle and data.
        /// </summary>
        /// <param name="vehicle">The associated vehicle.</param>
        /// <param name="data">The vehicle's data object.</param>
        public void Init(VE_Vehicle vehicle, Vehicle_Data data)
        {
            VeVehicle = vehicle;
            VehicleData = data;

            // Assign collider references
            TractorBoxCollider = VeVehicle.Tractor.GetComponent<BoxCollider>();
            TrailerBoxCollider = VeVehicle.Trailer.GetComponent<BoxCollider>();
            SensorSphereCollider = VeVehicle.transform.Find("Sensor").GetComponent<SphereCollider>();

            // Attach collision forwarders to relevant components
            AttachCollisionForwarder(VeVehicle.Tractor);
            AttachCollisionForwarder(VeVehicle.Trailer);
            AttachCollisionForwarder(SensorSphereCollider.gameObject);
        }

        /// <summary>
        /// Attaches a collision forwarder to a specified child object.
        /// </summary>
        /// <param name="child">The child object to attach the forwarder to.</param>
        private void AttachCollisionForwarder(GameObject child)
        {
            if (child != null)
            {
                var forwarder = child.AddComponent<Vehicle_Children_Collision_Forwarder>();
                forwarder.SetParentController(this);
            }
        }

        /// <summary>
        /// Handles trigger enter events for child objects, detecting new obstacles.
        /// </summary>
        /// <param name="child">The child object that detected the collision.</param>
        /// <param name="other">The collider of the object that triggered the event.</param>
        public void OnChildTriggerEnter(GameObject child, Collider other)
        {
            // Only process trigger events for the sensor sphere collider
            if (child != SensorSphereCollider.gameObject || IsSelfCollision(other)) return;

            // Ignore ground and specific sensor tags
            if (other.name == "Ground Plane" || other.CompareTag("Vehicle Obstacle Sensor")) return;

            // Check if the obstacle is already in the list
            if (DetectedObstacles.Exists(o => o.Object == other.gameObject)) return;

            var obstacleName = other.CompareTag("Tractor") || other.CompareTag("Trailer")
                ? other.transform.parent.name + " " + other.name
                : other.name;

            Debug.Log($"Obstacle detected by Sensor: {other.name}");

            // Add the detected obstacle
            DetectedObstacles.Add(new Obstacle_Info
            {
                Object = other.gameObject,
                Distance = Vector3.Distance(SensorSphereCollider.transform.position, other.transform.position),
                Angle = Vector3.SignedAngle(
                    VeVehicle.Tractor.transform.forward,
                    other.transform.position - SensorSphereCollider.transform.position,
                    Vector3.up
                ),
                Name = obstacleName
            });

            IsObstacleDetected = true;

            // Apply brakes if within braking threshold
            if (Vector3.Distance(SensorSphereCollider.transform.position, other.transform.position) < BrakingThreshold)
            {
                VeVehicle.KinematicsController.ApplyBrakes();
            }
        }

        /// <summary>
        /// Handles trigger exit events for child objects, removing obstacles from the detection list.
        /// </summary>
        /// <param name="child">The child object that detected the collision.</param>
        /// <param name="other">The collider of the object that triggered the exit event.</param>
        public void OnChildTriggerExit(GameObject child, Collider other)
        {
            // Only process trigger events for the sensor sphere collider
            if (child != SensorSphereCollider.gameObject || IsSelfCollision(other)) return;

            // Remove the obstacle from the list
            DetectedObstacles.RemoveAll(o => o.Object == other.gameObject);

            // Reset the detection flag if no obstacles remain
            if (DetectedObstacles.Count == 0)
            {
                IsObstacleDetected = false;
                VeVehicle.KinematicsController.ReleaseBrakes();
            }
        }

        /// <summary>
        /// Determines if a collision is with the vehicle itself.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <returns>True if the collider belongs to the same vehicle; otherwise, false.</returns>
        private bool IsSelfCollision(Collider collider)
        {
            var otherVehicle = collider.GetComponentInParent<VE_Vehicle>();
            return otherVehicle != null && otherVehicle == VeVehicle;
        }

        /// <summary>
        /// Updates information for detected obstacles, including distance and angle.
        /// </summary>
        private void UpdateDetectedObstacles()
        {
            for (int i = 0; i < DetectedObstacles.Count; i++)
            {
                var obstacle = DetectedObstacles[i];
                if (obstacle.Object == null)
                {
                    DetectedObstacles.RemoveAt(i--); // Remove and decrement index if the object is destroyed
                    continue;
                }

                var obstacleCollider = obstacle.Object.GetComponent<Collider>();
                if (obstacleCollider != null)
                {
                    // Update distance and angle using the closest point on the collider
                    Vector3 closestPoint = obstacleCollider.ClosestPoint(VehicleObstacleSensorCenter);
                    obstacle.Distance = Vector3.Distance(VehicleObstacleSensorCenter, closestPoint);
                    obstacle.Angle = Vector3.SignedAngle(
                        VeVehicle.Tractor.transform.forward,
                        closestPoint - VehicleObstacleSensorCenter,
                        Vector3.up
                    );
                }
                else
                {
                    // Fallback: use the object's position
                    obstacle.Distance = Vector3.Distance(VehicleObstacleSensorCenter, obstacle.Object.transform.position);
                    obstacle.Angle = Vector3.SignedAngle(
                        VeVehicle.Tractor.transform.forward,
                        obstacle.Object.transform.position - VehicleObstacleSensorCenter,
                        Vector3.up
                    );
                }

                // Trigger braking if the obstacle is within the threshold
                if (obstacle.Distance < BrakingThreshold)
                {
                    VeVehicle.KinematicsController.ApplyBrakes();
                }
            }
        }
    }
}


//private void DetectObstaclesWithRayCast(Ray ray, float worldAngle)
//{
//    if (Physics.Raycast(ray, out RaycastHit obstacleHit, Range))
//    {
//        var detectedObject = obstacleHit.collider.gameObject;
//        var distance = obstacleHit.distance;
//        var obstacleName = detectedObject.name;

//        bool isPartOfOwnVehicle = false;

//        if (detectedObject.CompareTag("Tractor") || detectedObject.CompareTag("Trailer"))
//        {
//            obstacleName = detectedObject.transform.parent.name + " " + detectedObject.name;

//            if (VeVehicle.Name == detectedObject.transform.parent.name)
//            {
//                isPartOfOwnVehicle = true;
//            }
//        }

//        if (!isPartOfOwnVehicle)
//        {
//            // Calculate the angle in the VeVehicle's local frame
//            float vehicleOrientation = VehicleData.Psi1 * Mathf.Rad2Deg;
//            float localAngle = (worldAngle - vehicleOrientation + 360) % 360;

//            // Check if the object is already in the list
//            var existingObstacle = DetectedObstacles.Find(o => o.Object == detectedObject);

//            if (existingObstacle != null)
//            {
//                // Update the distance and angle if the new detection is closer
//                if (distance < existingObstacle.Distance)
//                {
//                    existingObstacle.Distance = distance;
//                    existingObstacle.Angle = localAngle;
//                }
//            }
//            else
//            {
//                // Add the new object to the list
//                DetectedObstacles.Add(new Obstacle_Info
//                {
//                    Object = detectedObject,
//                    Distance = distance,
//                    Angle = localAngle,
//                    Name = obstacleName
//                });
//            }

//            // Update virtual obstacle detection info in the ROS message to publish

//            if (VeVehicle.Config.IsRosAvailable)
//            {
//                VeVehicle.TwistPublisher.x1 = distance;
//                VeVehicle.TwistPublisher.y1 = localAngle;
//            }



//            IsObstacleDetected = true;

//            // Check if the detected obstacle is within the braking threshold
//            if (distance < BrakingThreshold)
//            {
//                VeVehicle.GetComponent<Vehicle_Kinematics_Controller>().ApplyBrakes();
//            }
//        }
//    }
//}

//private void FixedUpdate()
//{
//    if (IsActive)
//    {
//        // Update the sphere collider center position based on vehicle data
//        VehicleObstacleSensorCenter.x = VehicleData.X1C;
//        VehicleObstacleSensorCenter.z = VehicleData.Y1C;
//        SensorSphereCollider.center = VehicleObstacleSensorCenter;

//        // Update the distance and angle for each detected obstacle
//        UpdateDetectedObstacles();

//        float offsetY = 1f; // Adjust this value for the desired lift
//        Vector3 rayOrigin = new Vector3(VehicleData.X0, offsetY, VehicleData.Y0);

//        // Clear the previous detection information
//        DetectedObstacles.Clear();

//        // Cast rays in A circular pattern in world space
//        for (int i = 0; i < NumRays; i++)
//        {
//            float worldAngle = i * 360f / NumRays;
//            Vector3 worldDirection = Quaternion.Euler(0, worldAngle, 0) * Vector3.forward;

//            Ray ray = new Ray(rayOrigin, worldDirection);
//            Debug.DrawRay(rayOrigin, ray.direction * Range, Color.red);

//            DetectObstaclesWithRayCast(ray, worldAngle);
//        }
//    }
//}