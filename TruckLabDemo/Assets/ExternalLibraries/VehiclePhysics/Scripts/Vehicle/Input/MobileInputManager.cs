using UnityEngine;
using System.Collections;

namespace NWH.VehiclePhysics
{
    /// <summary>
    /// Class for handling mobile user input via touch screen and sensors.
    /// Avoid having two input managers active at the sime time (mobile and destop) as the last executed script will override the first one.
    /// </summary>
    [DisallowMultipleComponent]
    public class MobileInputManager : MonoBehaviour
    {
        /// <summary>
        /// Steer input device. 
        /// Accelerometer - uses sensors to get horizontal axis.
        /// Screen - uses left side of the screen along with touch input to get steering position.
        /// Steering Wheel - uses steeringWheel script and steering wheel on-screen graphic that can be rotated by dragging.
        /// </summary>
        public enum InputType { Accelerometer, SteeringWheel }

        /// <summary>
        /// Active steer devices.
        /// </summary>
        public InputType inputType = InputType.SteeringWheel;

        /// <summary>
        /// Steering wheel script. Optional and not needed if steeringWheel option is not used.
        /// </summary>
        [Tooltip("Steering wheel script. Optional and not needed if steeringWheel option is not used.")]
        public SteeringWheel steeringWheel;

        /// <summary>
        /// Set to null (none) if you want to use your own vehicleTransform Controller. If this is set to other than null current active vehicleTransform according 
        /// to the assigned vehicleTransform changer will be used instead of the assigned vehicleTransform Controller.
        /// </summary>
        [Tooltip("Set to null (none) if you want to use your own vehicleTransform Controller. If this is set to other than null current active vehicleTransform according " +
            "to the assigned vehicleTransform changer will be used instead of the assigned vehicleTransform Controller.")]
        public VehicleChanger vehicleChanger;

        /// <summary>
        /// If you want to use this script with A single vehicleTransform or want to set your own vehicleTransform Controller from script set vehicleTransform changer field to null / none.
        /// </summary>
        [Tooltip("If you want to use this script with A single vehicleTransform or want to set your own vehicleTransform Controller from script set vehicleTransform changer field to null / none.")]
        public VehicleController vehicleController;

        /// <summary>
        /// Higher value will result in higher steer angle for same tilt.
        /// </summary>
        [Tooltip("Higher value will result in higher steer angle for same tilt.")]
        public float tiltSensitivity = 1.5f;

        private void Start()
        {
            vehicleController = GetComponent<VehicleController>();
        }


        void Update()
        {
            if (vehicleChanger != null)
            {
                vehicleController = vehicleChanger.ActiveVehicleController;
            }

#if PHOTON_MULTIPLAYER
            // Check if selected vehicleTransform has photon view and if it does, if it is mine.
            PhotonView photonView = vehicleController?.GetComponent<PhotonView>();
            if (photonView)
            {
                if (!photonView.isMine) return;
            }
#endif

            UpdateSteering();
        }


        public void UpdateSteering()
        {
            // Steering wheel input
            if (inputType == InputType.SteeringWheel)
            {
                if (steeringWheel != null)
                {
                    vehicleController.input.Horizontal = steeringWheel.GetClampedValue();
                }
                else
                {
                    Debug.LogWarning("Steering Wheel is selected as input but no Steering Wheel has been assigned.");
                }
            }
            // Accelerometer input
            else if (inputType == InputType.Accelerometer)
            {
                vehicleController.input.Horizontal = Input.acceleration.x * tiltSensitivity;
            }

            // Hide steering wheel if not needed, useful if there is an option to switch input in-game
            if (steeringWheel != null && steeringWheel.steeringWheelGraphic != null)
            {
                if (inputType != InputType.SteeringWheel && steeringWheel.steeringWheelGraphic.gameObject.activeInHierarchy)
                {
                    steeringWheel.steeringWheelGraphic.gameObject.SetActive(false);
                }
                else if (inputType == InputType.SteeringWheel && !steeringWheel.steeringWheelGraphic.gameObject.activeInHierarchy)
                {
                    steeringWheel.steeringWheelGraphic.gameObject.SetActive(true);
                }
            }
        }

        public void ThrottleDown()
        {
            vehicleController.input.Vertical = 1f;
        }

        public void ThrottleUp()
        {
            vehicleController.input.Vertical = 0f;
        }

        public void BrakeDown()
        {
            vehicleController.input.Vertical = -1f;
        }

        public void BrakeUp()
        {
            vehicleController.input.Vertical = 0f;
        }

        public void ToggleHighBeams()
        {
            vehicleController.input.fullBeamLights = !vehicleController.input.fullBeamLights;
        }

        public void ToggleLowBeams()
        {
            vehicleController.input.lowBeamLights = !vehicleController.input.lowBeamLights;
        }

        public void EngineStartStop()
        {
            if (vehicleController.engine.IsRunning)
                vehicleController.engine.Stop();
            else
                vehicleController.engine.Start();
        }

        public void ChangeVehicle()
        {
            if(vehicleChanger != null)
            {
                vehicleChanger.NextVehicle();
            }
            else
            {
                Debug.LogWarning("IVirtualEntityProduct Changer object is null.");
            }
        }

        public void ChangeCamera()
        {
            CameraChanger cc = vehicleController.GetComponentInChildren<CameraChanger>();
            if(cc != null)
                cc.NextCamera();
        }

        public void TrailerAttachDetach()
        {
            vehicleController.input.trailerAttachDetach = !vehicleController.input.trailerAttachDetach;
        }
    }
}

