using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics
{
    /// <summary>
    /// Class for storing input states of the vehicleTransform. 
    /// </summary>
    [HideInInspector]
    [System.Serializable]
	public class InputStates
	{
        /// <summary>
        /// True if vehicleTransform is local, false if non-local, networked vehicleTransform
        /// </summary>
        [HideInInspector]
        public bool settable = true;

        /// <summary>
        /// Horizontal axis. Used for steering.
        /// </summary>
        private float horizontal;

        /// <summary>
        /// Vertical axis. Used for accelerating and braking.
        /// </summary>
        private float vertical;

        private float clutch;
        private float handbrake;

        // Shift up and down flags. These will stay true until manual shift function is called.
        private bool shiftUp;
        private bool shiftDown;

        // Lights
        // Turning on one blinker will cancel out the other.
        public bool leftBlinker;
        public bool rightBlinker;
        public bool lowBeamLights;
        public bool fullBeamLights;
        public bool hazardLights;

        // trailerTransform
        /// <summary>
        /// trailerTransform will be attached only if under the threshold set in trailerTransform options.
        /// </summary>
        public bool trailerAttachDetach;

        public bool flipOver;

        // Horn
        public bool horn;

        private VehicleController vc;


        public void Initialize(VehicleController vc)
        {
            this.vc = vc;
        }


		public bool ShiftUp
        {
            get
            {
                if (!vc.Active) return false;
                return shiftUp;
            }
            set
            {
                shiftUp = value;
            }
		}


		public bool ShiftDown
		{
			get
			{
                if (!vc.Active) return false;
                return shiftDown;
			}
            set
            {
                shiftDown = value;
            }
		}


        /// <summary>
        /// Horizontal axis representing steering in range of -1 to 1.
        /// </summary>
        public float Horizontal
        {
            get
            {
                if (!vc) return 0;

#if PHOTON_MULTIPLAYER
                return horizontal;
#else
                if (!vc.Active) return 0;
                return horizontal;
#endif
            }

            set
            {
                if(settable) horizontal = Mathf.Clamp(value, -1f, 1f);
            }
        }


        /// <summary>
        /// Returns vertical input without any processing.
        /// </summary>
        public float RawVertical
        {
            get
            {
                if (!vc.Active) return 0;

                return vertical;
            }
        }


        /// <summary>
        /// Axis representing acceleration and braking. In range of -1 to 1.
        /// </summary>
        public float Vertical
        {
            get
            {
                if (!vc) return 0;

                // IProduct not active, return 0
                if (!vc.Active) return 0;

                // If tracked add vertical input when turning as tank requires power to turn
                if (vc.tracks.trackedVehicle) return Mathf.Clamp(vertical + Mathf.Sign(vertical) * Mathf.Abs(horizontal), -1f, 1f);

                return vertical;
            }

            set
            {
                if (settable) vertical = Mathf.Clamp(value, -1f, 1f);
            }
        }


        public float Clutch
        {
            get
            {
                return clutch;
            }
            set
            {
                clutch = Mathf.Clamp01(value);
            }
        }


        public float Handbrake
        {
            get
            {
                if (!vc.Active) return 0;
                return handbrake;
            }

            set
            {
                handbrake = Mathf.Clamp01(value);
            }
        }
    }
}
