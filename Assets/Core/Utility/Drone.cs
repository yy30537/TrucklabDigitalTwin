using UnityEngine;

namespace Core
{
    public class Drone : MonoBehaviour
    {
        public OptitrackRigidBody droneRigidbody;
        public Transform drone;
        private float scale = 13;

        [Header("Time / Sync")]
        public float gainTime = 2.00f;
        public float timeCount = 0.9f;
        
        void FixedUpdate()
        {
            float x = droneRigidbody.position.z * scale;
            float y = -droneRigidbody.position.x * scale;
            float psi = droneRigidbody.rotation.eulerAngles.y;
            
            drone.position = Vector3.Lerp(drone.position, new Vector3(x, 0, y), Time.deltaTime * gainTime);
            drone.rotation = Quaternion.Lerp(drone.rotation, Quaternion.Euler(new Vector3(0, psi, 0)), Time.deltaTime * gainTime);
        }
    }
}