using UnityEngine;

namespace Core
{
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

        void FixedUpdate()
        {
            if (isActive)
            {
                UpdateVehicleAnimation();
            }
        }

        public override void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            base.Initialize(vehicleProduct, vehicleData);
            
            vehicleTransform = vehicleProduct.productInstance.transform;
            tractorTransform = vehicleTransform.Find("Tractor").transform;
            trailerTransform = vehicleTransform.Find("Trailer").transform;

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

            trail = tractorTransform.Find("Trail").gameObject;
            trail.SetActive(false);
        }
        public void UpdateVehicleAnimation()
        {
            // Update wheels rotation angle
            tractorWheelFrontRightRot += (VehicleData.v1 + VehicleData.psi1dot * VehicleData.tractorWidth / 2) * Time.deltaTime;
            tractorWheelFrontLeftRot += (VehicleData.v1 - VehicleData.psi1dot * VehicleData.tractorWidth / 2) * Time.deltaTime;
            tractorWheelRearRightRot += (VehicleData.v1 + VehicleData.psi1dot * VehicleData.tractorWidth / 2) * Time.deltaTime;
            tractorWheelRearLeftRot += (VehicleData.v1 - VehicleData.psi1dot * VehicleData.tractorWidth / 2) * Time.deltaTime;

            trailerWheelFrontRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;
            trailerWheelFrontLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;
            trailerWheelCenterRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;
            trailerWheelCenterLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;
            trailerWheelRearRightRot += (VehicleData.v2 + VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;
            trailerWheelRearLeftRot += (VehicleData.v2 - VehicleData.psi2dot * VehicleData.trailerWidth / 2) * Time.deltaTime;

            // Ensure tractor and trailer positions are updated correctly
            tractorTransform.position = Vector3.Lerp(tractorTransform.position, new Vector3(VehicleData.x1, 0.41f, VehicleData.y1), Time.deltaTime * gainTime);
            tractorTransform.rotation = Quaternion.Lerp(tractorTransform.rotation, Quaternion.Euler(new Vector3(0, (1.57f - VehicleData.psi1) * 57.3f, 0)), Time.deltaTime * gainTime);
            
            trailerTransform.position = Vector3.Lerp(trailerTransform.position, new Vector3(VehicleData.x1C, 0.95f, VehicleData.y1C), Time.deltaTime * gainTime);
            trailerTransform.rotation = Quaternion.Lerp(trailerTransform.rotation, Quaternion.Euler(new Vector3(0, (1.57f - VehicleData.psi2) * 57.3f, 0)), Time.deltaTime * gainTime);

            steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, Quaternion.Euler(new Vector3(0, 0, VehicleData.delta * 10 * Mathf.Rad2Deg)), Time.deltaTime);

            tractorWheelFrontLeft.localRotation = Quaternion.Lerp(tractorWheelFrontLeft.localRotation, Quaternion.Euler(new Vector3(tractorWheelFrontLeftRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            tractorWheelFrontRight.localRotation = Quaternion.Lerp(tractorWheelFrontRight.localRotation, Quaternion.Euler(new Vector3(tractorWheelFrontRightRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            tractorWheelRearLeft.localRotation = Quaternion.Lerp(tractorWheelRearLeft.localRotation, Quaternion.Euler(new Vector3(tractorWheelRearLeftRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            tractorWheelRearRight.localRotation = Quaternion.Lerp(tractorWheelRearRight.localRotation, Quaternion.Euler(new Vector3(tractorWheelRearRightRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);

            trailerWheelFrontLeft.localRotation = Quaternion.Lerp(trailerWheelFrontLeft.localRotation, Quaternion.Euler(new Vector3(trailerWheelFrontLeftRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            trailerWheelFrontRight.localRotation = Quaternion.Lerp(trailerWheelFrontRight.localRotation, Quaternion.Euler(new Vector3(trailerWheelFrontRightRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            trailerWheelCenterLeft.localRotation = Quaternion.Lerp(trailerWheelCenterLeft.localRotation, Quaternion.Euler(new Vector3(trailerWheelCenterLeftRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            trailerWheelCenterRight.localRotation = Quaternion.Lerp(trailerWheelCenterRight.localRotation, Quaternion.Euler(new Vector3(trailerWheelCenterRightRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            trailerWheelRearLeft.localRotation = Quaternion.Lerp(trailerWheelRearLeft.localRotation, Quaternion.Euler(new Vector3(trailerWheelRearLeftRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
            trailerWheelRearRight.localRotation = Quaternion.Lerp(trailerWheelRearRight.localRotation, Quaternion.Euler(new Vector3(trailerWheelRearRightRot * Mathf.Rad2Deg, 0, 0)), Time.deltaTime);
        }
    }
}
