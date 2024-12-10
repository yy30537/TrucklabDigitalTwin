using UnityEngine;

namespace ApplicationScripts.VirtualEntity.Vehicle.Controllers.CollisionControl
{
    /// <summary>
    /// Forwards collision events from child objects to the parent Vehicle_Collision_Controller.
    /// </summary>
    public class Vehicle_Children_Collision_Forwarder : MonoBehaviour
    {
        /// <summary>
        /// Reference to the parent collision controller.
        /// </summary>
        private Vehicle_Collision_Controller parentController;

        /// <summary>
        /// Sets the parent collision controller to forward events to.
        /// </summary>
        /// <param name="controller">The parent collision controller.</param>
        public void SetParentController(Vehicle_Collision_Controller controller)
        {
            parentController = controller;
        }

        /// <summary>
        /// Forwards OnTriggerEnter events to the parent collision controller.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            parentController?.OnChildTriggerEnter(gameObject, other);
        }

        /// <summary>
        /// Forwards OnTriggerExit events to the parent collision controller.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            parentController?.OnChildTriggerExit(gameObject, other);
        }
    }
}