using UnityEngine;

namespace Core
{
    /// <summary>
    /// Represents a generic product in the simulation.
    /// </summary>
    public abstract class Product : MonoBehaviour
    {
        public int productID { get; private set; }
        public string productName { get; private set; }
        public GameObject productInstance { get; private set; }
        public Camera mainCamera { get; private set; }
        public SystemLog systemLog { get; private set; }
        public GetClickedObject getClickedObject { get; private set; }

        /// <summary>
        /// Initializes the product with the provided parameters.
        /// </summary>
        /// <param name="pid">Product ID.</param>
        /// <param name="pname">Product name.</param>
        /// <param name="pinstance">Product instance GameObject.</param>
        /// <param name="cam">Main camera.</param>
        /// <param name="log">System log for logging events.</param>
        /// <param name="objectClicker">Service for detecting clicked objects.</param>
        public virtual void Init(int pid, string pname, GameObject pinstance, Camera cam, SystemLog log, GetClickedObject objectClicker)
        {
            this.systemLog = log;
            this.getClickedObject = objectClicker;

            productID = pid;
            productName = pname;
            productInstance = pinstance;
            mainCamera = cam;
        }

        /// <summary>
        /// Abstract method for initializing the product components.
        /// </summary>
        public abstract void InitComponents();
    }
}
