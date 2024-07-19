using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the visibility of the region status window.
    /// </summary>
    public class RegionStatusWindow : MonoBehaviour
    {
        public GameObject RegionWindowInstance;

        /// <summary>
        /// Toggles the visibility of the region status window.
        /// </summary>
        public void Toggle()
        {
            RegionWindowInstance.SetActive(!RegionWindowInstance.activeSelf);
        }
    }
}