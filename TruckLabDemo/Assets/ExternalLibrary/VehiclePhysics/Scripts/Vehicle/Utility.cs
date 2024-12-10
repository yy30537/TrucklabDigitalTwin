using UnityEngine;

namespace VehiclePhysics.Scripts.Vehicle
{
    public static class Utility
    {

        /// <summary>
        /// Avoid using at runtime.
        /// Run it in Start or Awake and cache the result.
        /// </summary>
        public static VehicleController FindRootVehicle(Transform transform)
        {
            if (transform == null) return null;

            if (transform.GetComponent<VehicleController>())
            {
                return transform.GetComponent<VehicleController>();
            }
            else if (transform.parent != null)
            {
                return FindRootVehicle(transform.parent);
            }           
            else
            {
                return null;
            }
        }
    }
}
