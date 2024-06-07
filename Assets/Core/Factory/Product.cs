using UnityEngine;

namespace Core
{
    public abstract class Product : MonoBehaviour
    {
        public int productID { get; private set; }
        public string productName { get; private set; }
        public GameObject productInstance { get; private set; }
        public Camera mainCamera { get; private set; }

        protected SystemLog systemLog;
        protected GetClickedObject getClickedObject;

        public virtual void Init(int pid, string pname, GameObject instance, Camera cam, GameObject uiObserverParent, Transform dashboardParent)
        {
            productID = pid;
            productName = pname;
            productInstance = instance;
            mainCamera = cam;

            systemLog = FindObjectOfType<SystemLog>();
            getClickedObject = FindObjectOfType<GetClickedObject>();

            InitComponents(uiObserverParent, dashboardParent);
        }

        protected abstract void InitComponents(GameObject uiObserverParent, Transform dashboardParent);
    }
}