using OptiTrack.Scripts;
using UnityEngine;

namespace ApplicationScripts.Manager
{
    /// <summary>
    /// Manages the position and rotation synchronization of a drone using data from an Optitrack rigid body.
    /// Handles real-Time updates to the drone's transform based on external tracking data.
    /// </summary>
    public class Drone_Manager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Optitrack rigid body providing the drone's position and orientation data.
        /// </summary>
        public OptitrackRigidBody DroneRigidbody;

        /// <summary>
        /// The transform of the drone object in the scene to be updated.
        /// </summary>
        public Transform Drone;

        /// <summary>
        /// Scaling factor applied to the position data for proper world-space mapping.
        /// </summary>
        private float scale = 13;

        /// <summary>
        /// Gain value controlling the responsiveness of the position and rotation synchronization.
        /// Higher values result in faster updates.
        /// </summary>
        [Header("Time / Sync")]
        public float GainTime = 2.00f;

        /// <summary>
        /// Time count used for internal operations (currently unused but can support future synchronization logic).
        /// </summary>
        public float TimeCount = 0.9f;

        /// <summary>
        /// Updates the drone's position and rotation in the scene every fixed Time step.
        /// </summary>
        void FixedUpdate()
        {
            // Extract and transform position data from the rigid body.
            // Optitrack's Z axis corresponds to Unity's X axis, and X corresponds to Unity's Z axis.
            float x = DroneRigidbody.position.z * scale; // Map Optitrack Z to Unity X
            float y = -DroneRigidbody.position.x * scale; // Map Optitrack X to Unity Z (negated)

            // Extract the yaw angle from the rigid body's rotation data.
            float psi = DroneRigidbody.rotation.eulerAngles.y;

            // Smoothly interpolate the drone's position to match the tracked data.
            Drone.position = Vector3.Lerp(
                Drone.position,                 // Current position
                new Vector3(x, 0, y),          // Target position (mapped and scaled)
                Time.deltaTime * GainTime      // Interpolation factor based on delta Time and gain
            );

            // Smoothly interpolate the drone's rotation to match the tracked data.
            Drone.rotation = Quaternion.Lerp(
                Drone.rotation,                             // Current rotation
                Quaternion.Euler(new Vector3(0, psi, 0)),  // Target rotation (yaw angle only)
                Time.deltaTime * GainTime                  // Interpolation factor based on delta Time and gain
            );
        }
    }
}
