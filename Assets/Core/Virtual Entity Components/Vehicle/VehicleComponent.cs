using UnityEngine;

namespace Core
{
    public abstract class VehicleComponent : MonoBehaviour
    {
        protected VehicleProduct VehicleProduct { get; private set; }
        protected VehicleData VehicleData { get; private set; }

        public virtual void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            VehicleProduct = vehicleProduct;
            VehicleData = vehicleData;
        }
    }
}
