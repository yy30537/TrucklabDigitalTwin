using UnityEngine;

namespace Core
{
    public class GetClickedObject : MonoBehaviour
    {
        public Camera mainCamera;
        
        public GameObject ReturnClickedObject()
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                return hitObject.gameObject;
            }

            return null;
        }
    }
}