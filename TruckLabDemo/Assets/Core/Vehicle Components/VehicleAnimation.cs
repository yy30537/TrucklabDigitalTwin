using UnityEngine;

namespace Core
{
    /// <summary>
    /// Handles the visual representation and animation of the vehicle.
    /// </summary>
    public class VehicleAnimation : VehicleComponent
    {
        public bool isActive = false;

        [Header("Vehicle Transform Components")]
        public Transform vehicleTransform;
        public Transform tractorTransform;
        public Transform trailerTransform;

        public Transform steeringWheel;
        public Transform tractorWheelFrontLeft;
        public Transform tractorWheelFrontRight;
        public Transform tractorWheelRearLeft;
        public Transform tractorWheelRearRight;
        public Transform trailerWheelFrontLeft;
        public Transform trailerWheelFrontRight;
        public Transform trailerWheelCenterLeft;
        public Transform trailerWheelCenterRight;
        public Transform trailerWheelRearLeft;
        public Transform trailerWheelRearRight;

        public GameObject trail;

        [Header("Wheels Rotation Data")]
        public float tractorWheelFrontLeftRot;
        public float tractorWheelFrontRightRot;
        public float tractorWheelRearLeftRot;
        public float tractorWheelRearRightRot;
        public float trailerWheelFrontLeftRot;
        public float trailerWheelFrontRightRot;
        public float trailerWheelCenterLeftRot;
        public float trailerWheelCenterRightRot;
        public float trailerWheelRearLeftRot;
        public float trailerWheelRearRightRot;

        [Header("Time / Sync")]
        public float gainTime = 2.00f;
        public float timeCount = 0.9f;

        /// <summary>
        /// FixedUpdate is called at fixed intervals, ideal for handling physics and animations.
        /// </summary>
        void FixedUpdate()
        {
            if (isActive)
            {
                UpdateVehicleAnimation();
            }
        }

        /// <summary>
        /// Initializes the animation component with the given vehicle and data.
        /// </summary>
        /// <param name="vehicleProduct">The vehicle product instance.</param>
        /// <param name="vehicleData">The vehicle data instance.</param>
        public override void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            base.Initialize(vehicleProduct, vehicleData);

            vehicleTransform = vehicleProduct.ProductInstance.transform;
            tractorTransform = vehicleTransform.Find("Tractor").transform;
            trailerTransform = vehicleTransform.Find("Trailer").transform;

            InitializeWheelTransforms();
            InitializeWheelRotationData();

            trail = tractorTransform.Find("Trail").gameObject;
            trail.SetActive(false);
        }

        /// <summary>
        /// Initializes the wheel transform components.
        /// </summary>
        private void InitializeWheelTransforms()
        {
            steeringWheel = tractorTransform.Find("Body/steering wheel");

            tractorWheelFrontLeft = tractorTransform.Find("Wheels/L1");
            tractorWheelFrontRight = tractorTransform.Find("Wheels/R1");
            tractorWheelRearLeft = tractorTransform.Find("Wheels/L2");
            tractorWheelRearRight = tractorTransform.Find("Wheels/R2");

            trailerWheelFrontLeft = trailerTransform.Find("Wheels/L1");
            trailerWheelFrontRight = trailerTransform.Find("Wheels/R1");
            trailerWheelCenterLeft = trailerTransform.Find("Wheels/L2");
            trailerWheelCenterRight = trailerTransform.Find("Wheels/R2");
            trailerWheelRearLeft = trailerTransform.Find("Wheels/L3");
            trailerWheelRearRight = trailerTransform.Find("Wheels/R3");
        }

        /// <summary>
        /// Initializes the wheel rotation data.
        /// </summary>
        private void InitializeWheelRotationData()
        {
            tractorWheelFrontLeftRot = 0;
            tractorWheelFrontRightRot = 0;
            tractorWheelRearLeftRot = 0;
            tractorWheelRearRightRot = 0;
            trailerWheelFrontLeftRot = 0;
            trailerWheelFrontRightRot = 0;
            trailerWheelCenterLeftRot = 0;
            trailerWheelCenterRightRot = 0;
            trailerWheelRearLeftRot = 0;
            trailerWheelRearRightRot = 0;
        }

        /// <summary>
        /// Updates the vehicle animation based on the current vehicle data.
        /// </summary>
        public void UpdateVehicleAnimation()
        {
            UpdateWheelRotation();
            UpdateVehicleTransforms();
            UpdateSteeringWheel();
        }

        /// <summary>
        /// Updates the rotation of the wheels.
        /// </summary>
        private void UpdateWheelRotation()
        {
            float deltaTime = Time.deltaTime;

            tractorWheelFrontRightRot += (VehicleData.v1 + VehicleData.psi1dot * VehicleData.tractorWidth / 2) * deltaTime;
            tractorWheelFrontLeftRot += (VehicleData.v1 - VehicleData.psi1dot * VehicleData.tractorWidth / 2) * deltaTime;
            tractorWheelRearRightRot += (VehicleData.v1 + VehicleData.psi1dot * VehicleData.tractorWidth / 2) * deltaTime;
            tractorWheelRearLeftRot += (VehicleData.v1 - VehicleData.psi1dot * VehicleData.tractorWidth / 2) * deltaTime;

            trailerWheelFrontRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;
            trailerWheelFrontLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;
            trailerWheelCenterRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;
            trailerWheelCenterLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;
            trailerWheelRearRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;
            trailerWheelRearLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * deltaTime;

            UpdateWheelLocalRotation(tractorWheelFrontLeft, tractorWheelFrontLeftRot);
            UpdateWheelLocalRotation(tractorWheelFrontRight, tractorWheelFrontRightRot);
            UpdateWheelLocalRotation(tractorWheelRearLeft, tractorWheelRearLeftRot);
            UpdateWheelLocalRotation(tractorWheelRearRight, tractorWheelRearRightRot);

            UpdateWheelLocalRotation(trailerWheelFrontLeft, trailerWheelFrontLeftRot);
            UpdateWheelLocalRotation(trailerWheelFrontRight, trailerWheelFrontRightRot);
            UpdateWheelLocalRotation(trailerWheelCenterLeft, trailerWheelCenterLeftRot);
            UpdateWheelLocalRotation(trailerWheelCenterRight, trailerWheelCenterRightRot);
            UpdateWheelLocalRotation(trailerWheelRearLeft, trailerWheelRearLeftRot);
            UpdateWheelLocalRotation(trailerWheelRearRight, trailerWheelRearRightRot);
        }

        /// <summary>
        /// Updates the local rotation of the specified wheel.
        /// </summary>
        /// <param name="wheelTransform">The transform of the wheel.</param>
        /// <param name="rotation">The rotation angle in radians.</param>
        private void UpdateWheelLocalRotation(Transform wheelTransform, float rotation)
        {
            if (wheelTransform != null)
            {
                wheelTransform.localRotation = Quaternion.Euler(new Vector3(rotation * Mathf.Rad2Deg, 0, 0));
            }
        }

        /// <summary>
        /// Updates the positions and rotations of the vehicle's tractor and trailer.
        /// </summary>
        private void UpdateVehicleTransforms()
        {
            float deltaTime = Time.deltaTime;
            Vector3 tractorPosition = new Vector3(VehicleData.x1, 0.41f, VehicleData.y1);
            Vector3 trailerPosition = new Vector3(VehicleData.x1C, 0.95f, VehicleData.y1C);

            tractorTransform.position = Vector3.Lerp(tractorTransform.position, tractorPosition, deltaTime * gainTime);
            tractorTransform.rotation = Quaternion.Lerp(tractorTransform.rotation, Quaternion.Euler(0, (1.57f - VehicleData.psi1) * Mathf.Rad2Deg, 0), deltaTime * gainTime);

            trailerTransform.position = Vector3.Lerp(trailerTransform.position, trailerPosition, deltaTime * gainTime);
            trailerTransform.rotation = Quaternion.Lerp(trailerTransform.rotation, Quaternion.Euler(0, (1.57f - VehicleData.psi2) * Mathf.Rad2Deg, 0), deltaTime * gainTime);
        }

        /// <summary>
        /// Updates the rotation of the steering wheel.
        /// </summary>
        private void UpdateSteeringWheel()
        {
            if (steeringWheel != null)
            {
                steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, Quaternion.Euler(0, 0, VehicleData.delta * 10 * Mathf.Rad2Deg), Time.deltaTime);
            }
        }
    }
}
