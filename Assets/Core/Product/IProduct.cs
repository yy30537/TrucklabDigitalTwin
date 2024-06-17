using UnityEngine;

namespace Core
{
    public interface IProduct
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public GameObject productInstance { get; set; }
        public Camera mainCamera { get; set; }
        public SystemLog systemLog { get; set; }
        public GetClickedObject getClickedObject { get; set; }
        
        public void Initialize();
    }
}
