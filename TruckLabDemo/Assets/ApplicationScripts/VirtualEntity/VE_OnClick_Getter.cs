using ApplicationScripts.Manager;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity
{
    /// <summary>
    /// Handles the detection of objects clicked by the user within the scene.
    /// Utilizes raycasting to determine which object under the mouse pointer has been clicked.
    /// </summary>

    public class VE_OnClick_Getter : MonoBehaviour
    {
        /// <summary>
        /// Reference to the camera manager, used to access the currently active camera for raycasting.
        /// </summary>
        public Camera_Manager CameraManager;

        /// <summary>
        /// Layer mask used to filter the objects that can be clicked.
        /// Only objects on the specified layers will be detected.
        /// </summary>
        public LayerMask LayerMask;


        /// <summary>
        /// Casts A ray from the mouse pointer position to detect clicked objects in the scene.
        /// Only objects on the specified layer mask will be detected.
        /// </summary>
        /// <returns>
        /// The GameObject that was clicked, or null if no object was detected.
        /// </returns>
        public GameObject ReturnClickedObject()
        {
            if (CameraManager == null || CameraManager.ActiveCamera == null)
            {
                Debug.LogWarning("CameraManager or ActiveCamera is not assigned.");
                return null;
            }

            Ray ray = CameraManager.ActiveCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask))
            {
                //Debug.Log($"Clicked {hit.collider.gameObject.name}");
                return hit.collider.gameObject;
            }
            return null;
        }

    }
}