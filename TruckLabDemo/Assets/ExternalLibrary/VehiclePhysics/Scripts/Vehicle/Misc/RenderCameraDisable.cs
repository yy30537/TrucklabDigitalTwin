using UnityEngine;

namespace VehiclePhysics.Scripts.Vehicle.Misc
{
    /// <summary>
    /// Attach this to any cameras rendering to mirrors so that when vehicleTransform is inactive mirror will not update.
    /// </summary>
    public class RenderCameraDisable : MonoBehaviour
    {
        private Camera cameraToDisable;
        private VehicleController vc;

        void Start()
        {
            vc = GetComponentInParent<VehicleController>();
            cameraToDisable = GetComponent<Camera>();
        }

        void Update()
        {
            if(vc != null && cameraToDisable != null)
            {
                if(vc.Active && !cameraToDisable.enabled)
                {
                    cameraToDisable.enabled = true;
                }
                else if(!vc.Active && cameraToDisable.enabled)
                {
                    cameraToDisable.enabled = false;
                }
            }
        }
    }
}

