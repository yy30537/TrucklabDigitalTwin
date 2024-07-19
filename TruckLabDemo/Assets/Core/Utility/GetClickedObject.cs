using UnityEngine;

namespace Core
{
    /// <summary>
    /// Handles the detection of clicked objects.
    /// </summary>
    public class GetClickedObject : MonoBehaviour
    {
        public Camera mainCamera;

        /// <summary>
        /// Returns the GameObject that was clicked.
        /// </summary>
        /// <returns>The clicked GameObject, or null if no object was clicked.</returns>
        public GameObject ReturnClickedObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.collider.gameObject;
            }
            return null;
        }
    }
}