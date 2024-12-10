using UnityEngine;

namespace VehiclePhysics.Models.Vehicles.Euro_Truck_by_GR3D.GR3D_010416TRUK.Demo_Scene.Script
{
    public class GRCamRotate : MonoBehaviour {

        void Update () {
            transform.Rotate(Vector3.up * 0.3f);
        }
    }
}
