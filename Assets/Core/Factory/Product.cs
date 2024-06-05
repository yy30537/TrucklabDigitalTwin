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

        public virtual void Init(int pid, string pname, GameObject instance, Camera cam)
        {
            systemLog = FindObjectOfType<SystemLog>();
            getClickedObject = FindObjectOfType<GetClickedObject>();
            
            productID = pid;
            productName = pname;
            productInstance = instance;
            mainCamera = cam;
        }

        public abstract void InitComponents();
    }
}

