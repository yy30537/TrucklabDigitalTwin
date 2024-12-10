using Newtonsoft.Json.Bson;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle.Controllers
{
    /// <summary>
    /// Handles the visual representation and animation of the VeVehicle.
    /// Manages wheel rotation, vehicle transforms, and other visual components.
    /// </summary>
    public class Vehicle_Animation_Controller : MonoBehaviour
    {
        [Header("Animation Controller Status")]
        public bool IsActive = false;

        [Header("VE Reference")]
        protected VE_Vehicle VeVehicle;
        protected Vehicle_Data VehicleData;

        [Header("Vehicle Transform Components")]
        public Transform VehicleTransform;
        public Transform TractorTransform;
        public Transform TrailerTransform;

        public Transform SteeringWheel;
        public Transform TractorWheelFrontLeft;
        public Transform TractorWheelFrontRight;
        public Transform TractorWheelRearLeft;
        public Transform TractorWheelRearRight;
        public Transform TrailerWheelFrontLeft;
        public Transform TrailerWheelFrontRight;
        public Transform TrailerWheelCenterLeft;
        public Transform TrailerWheelCenterRight;
        public Transform TrailerWheelRearLeft;
        public Transform TrailerWheelRearRight;

        [Header("Tractor Trail Object")]
        public GameObject TractorTrajectory;

        [Header("Wheels Rotation Value")]
        public float TractorWheelFrontLeftRot;
        public float TractorWheelFrontRightRot;
        public float TractorWheelRearLeftRot;
        public float TractorWheelRearRightRot;
        public float TrailerWheelFrontLeftRot;
        public float TrailerWheelFrontRightRot;
        public float TrailerWheelCenterLeftRot;
        public float TrailerWheelCenterRightRot;
        public float TrailerWheelRearLeftRot;
        public float TrailerWheelRearRightRot;

        [Header("Time / Sync")]
        public float GainTime = 10f;
        //public float TimeCount = 0.9f;

        /// <summary>
        /// Called at fixed intervals to update the vehicle's animation if active.
        /// </summary>
        void FixedUpdate()
        {
            if (IsActive)
            {
                UpdateVehicleAnimation();
            }
        }

        /// <summary>
        /// Updates the visual animation of the vehicle, including wheels and transforms.
        /// </summary>
        public void UpdateVehicleAnimation()
        {
            UpdateWheelRotation();
            UpdateVehicleTransforms();
            UpdateSteeringWheel();
        }

        /// <summary>
        /// Initializes the animation controller with references to the associated VeVehicle and its data.
        /// </summary>
        /// <param name="vehicle">The VeVehicle instance to animate.</param>
        /// <param name="data">The kinematic data of the vehicle.</param>
        public void Init(VE_Vehicle vehicle, Vehicle_Data data)
        {
            VeVehicle = vehicle;
            VehicleData = data;

            VehicleTransform = data.VehicleInstance.transform;
            TractorTransform = VehicleTransform.Find("Tractor").transform;
            TrailerTransform = VehicleTransform.Find("Trailer").transform;

            InitializeWheelTransforms();
            InitializeWheelRotationData();

            TractorTrajectory = VehicleTransform.Find("Trail").gameObject;
            TractorTrajectory.SetActive(false);
        }

        /// <summary>
        /// Finds and assigns references to all relevant wheel transform components.
        /// </summary>
        private void InitializeWheelTransforms()
        {
            SteeringWheel = TractorTransform.Find("Body/steering wheel");

            TractorWheelFrontLeft = TractorTransform.Find("Wheels/L1");
            TractorWheelFrontRight = TractorTransform.Find("Wheels/R1");
            TractorWheelRearLeft = TractorTransform.Find("Wheels/L2");
            TractorWheelRearRight = TractorTransform.Find("Wheels/R2");

            TrailerWheelFrontLeft = TrailerTransform.Find("Wheels/L1");
            TrailerWheelFrontRight = TrailerTransform.Find("Wheels/R1");
            TrailerWheelCenterLeft = TrailerTransform.Find("Wheels/L2");
            TrailerWheelCenterRight = TrailerTransform.Find("Wheels/R2");
            TrailerWheelRearLeft = TrailerTransform.Find("Wheels/L3");
            TrailerWheelRearRight = TrailerTransform.Find("Wheels/R3");
        }

        /// <summary>
        /// Resets all wheel rotation values to zero.
        /// </summary>
        private void InitializeWheelRotationData()
        {
            TractorWheelFrontLeftRot = 0;
            TractorWheelFrontRightRot = 0;
            TractorWheelRearLeftRot = 0;
            TractorWheelRearRightRot = 0;
            TrailerWheelFrontLeftRot = 0;
            TrailerWheelFrontRightRot = 0;
            TrailerWheelCenterLeftRot = 0;
            TrailerWheelCenterRightRot = 0;
            TrailerWheelRearLeftRot = 0;
            TrailerWheelRearRightRot = 0;
        }


        /// <summary>
        /// Calculates and updates the rotation values for all wheels based on the vehicle's velocity and angular velocity.
        /// </summary>
        private void UpdateWheelRotation()
        {
            float deltaTime = Time.deltaTime;

            TractorWheelFrontRightRot += (VehicleData.V1 + VehicleData.Psi1dot * VehicleData.TractorWidth / 2) * deltaTime;
            TractorWheelFrontLeftRot += (VehicleData.V1 - VehicleData.Psi1dot * VehicleData.TractorWidth / 2) * deltaTime;
            TractorWheelRearRightRot += (VehicleData.V1 + VehicleData.Psi1dot * VehicleData.TractorWidth / 2) * deltaTime;
            TractorWheelRearLeftRot += (VehicleData.V1 - VehicleData.Psi1dot * VehicleData.TractorWidth / 2) * deltaTime;

            TrailerWheelFrontRightRot += (VehicleData.V2 + VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;
            TrailerWheelFrontLeftRot += (VehicleData.V2 - VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;
            TrailerWheelCenterRightRot += (VehicleData.V2 + VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;
            TrailerWheelCenterLeftRot += (VehicleData.V2 - VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;
            TrailerWheelRearRightRot += (VehicleData.V2 + VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;
            TrailerWheelRearLeftRot += (VehicleData.V2 - VehicleData.Psi2dot * VehicleData.TrailerWidth / 2) * deltaTime;

            UpdateWheelLocalRotation(TractorWheelFrontLeft, TractorWheelFrontLeftRot);
            UpdateWheelLocalRotation(TractorWheelFrontRight, TractorWheelFrontRightRot);
            UpdateWheelLocalRotation(TractorWheelRearLeft, TractorWheelRearLeftRot);
            UpdateWheelLocalRotation(TractorWheelRearRight, TractorWheelRearRightRot);

            UpdateWheelLocalRotation(TrailerWheelFrontLeft, TrailerWheelFrontLeftRot);
            UpdateWheelLocalRotation(TrailerWheelFrontRight, TrailerWheelFrontRightRot);
            UpdateWheelLocalRotation(TrailerWheelCenterLeft, TrailerWheelCenterLeftRot);
            UpdateWheelLocalRotation(TrailerWheelCenterRight, TrailerWheelCenterRightRot);
            UpdateWheelLocalRotation(TrailerWheelRearLeft, TrailerWheelRearLeftRot);
            UpdateWheelLocalRotation(TrailerWheelRearRight, TrailerWheelRearRightRot);
        }

        /// <summary>
        /// Sets the local rotation of a specific wheel transform based on its rotation value.
        /// </summary>
        /// <param name="wheelTransform">The transform of the wheel to update.</param>
        /// <param name="rotation">The rotation value in radians.</param>
        private void UpdateWheelLocalRotation(Transform wheelTransform, float rotation)
        {
            if (wheelTransform != null)
            {
                wheelTransform.localRotation = Quaternion.Euler(new Vector3(rotation * Mathf.Rad2Deg, 0, 0));
            }
        }


        /// <summary>
        /// Updates the position and orientation of the tractor and trailer to match VehicleData exactly.
        /// </summary>
        private void UpdateVehicleTransforms()
        {
            //option1_lerp();
            option2_direct();
        }

        void option2_direct()
        {
            // Define the Y-axis offsets for tractor and trailer
            float tractorYOffset = 0.41f;
            float trailerYOffset = 0.95f;

            // Calculate positions
            Vector3 tractorPosition = new Vector3(VehicleData.X1, tractorYOffset, VehicleData.Y1);
            Vector3 trailerPosition = new Vector3(VehicleData.X1C, trailerYOffset, VehicleData.Y1C);

            // Directly set positions
            TractorTransform.position = tractorPosition;
            TrailerTransform.position = trailerPosition;

            // Calculate rotations
            // Assuming Psi1 and Psi2 are in radians and represent yaw angles around the Y-axis
            // The original code adjusts by subtracting Psi1 from 1.57 radians (≈90 degrees). 
            // This might be necessary based on your model's initial orientation.
            // Verify if this adjustment is required. If not, remove it.

            float tractorRotationY = (1.57f - VehicleData.Psi1) * Mathf.Rad2Deg;
            float trailerRotationY = (1.57f - VehicleData.Psi2) * Mathf.Rad2Deg;

            // Directly set rotations
            TractorTransform.rotation = Quaternion.Euler(0, tractorRotationY, 0);
            TrailerTransform.rotation = Quaternion.Euler(0, trailerRotationY, 0);

            TractorTrajectory.gameObject.transform.localPosition = tractorPosition;
        }
        void option1_lerp()
        {
            // Define the Y-axis offsets for tractor and trailer
            float tractorYOffset = 0.41f;
            float trailerYOffset = 0.95f;

            float deltaTime = Time.fixedDeltaTime;
            Vector3 tractorPosition = new Vector3(VehicleData.X1, tractorYOffset, VehicleData.Y1);
            Vector3 trailerPosition = new Vector3(VehicleData.X1C, trailerYOffset, VehicleData.Y1C);

            TractorTransform.position = Vector3.Lerp(TractorTransform.position, tractorPosition, deltaTime * GainTime);
            TractorTransform.rotation = Quaternion.Lerp(TractorTransform.rotation, Quaternion.Euler(0, (1.57f - VehicleData.Psi1) * Mathf.Rad2Deg, 0), deltaTime * GainTime);

            TrailerTransform.position = Vector3.Lerp(TrailerTransform.position, trailerPosition, deltaTime * GainTime);
            TrailerTransform.rotation = Quaternion.Lerp(TrailerTransform.rotation, Quaternion.Euler(0, (1.57f - VehicleData.Psi2) * Mathf.Rad2Deg, 0), deltaTime * GainTime);

            TractorTrajectory.gameObject.transform.localPosition = tractorPosition;
        }

        /// <summary>
        /// Smoothly rotates the steering wheel based on the vehicle's steering input angle.
        /// </summary>
        private void UpdateSteeringWheel()
        {
            if (SteeringWheel != null)
            {
                SteeringWheel.localRotation =  Quaternion.Lerp(SteeringWheel.localRotation, Quaternion.Euler(0, 0, -VehicleData.Delta * 10 * Mathf.Rad2Deg), Time.deltaTime);
            }
        }
    }
}
