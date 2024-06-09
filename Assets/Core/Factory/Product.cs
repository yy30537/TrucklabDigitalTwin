using UnityEngine;

namespace Core
{
    public abstract class Product : MonoBehaviour
    {
        public int productID { get; private set; }
        public string productName { get; private set; }
        public GameObject productInstance { get; private set; }
        public Camera mainCamera { get; private set; }

        public SystemLog systemLog { get; private set; }
        public GetClickedObject getClickedObject { get; private set; }

        public virtual void Init(int pid, string pname, GameObject pinstance, Camera cam, SystemLog log, GetClickedObject objectClicker)
        {
            this.systemLog = log;
            this.getClickedObject = objectClicker;

            productID = pid;
            productName = pname;
            productInstance = pinstance;
            mainCamera = cam;
        }

        public abstract void InitComponents();
    }
}
