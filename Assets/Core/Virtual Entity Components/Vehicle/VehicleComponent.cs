using UnityEngine;

namespace Core
{
    public abstract class VehicleComponent : MonoBehaviour
    {
        protected VehicleProduct vehicleProduct { get; private set; }
        protected VehicleData VehicleData { get; private set; }

        public virtual void Initialize(VehicleProduct vehicleProduct, VehicleData vehicleData)
        {
            this.vehicleProduct = vehicleProduct;
            VehicleData = vehicleData;
        }
    }
}
