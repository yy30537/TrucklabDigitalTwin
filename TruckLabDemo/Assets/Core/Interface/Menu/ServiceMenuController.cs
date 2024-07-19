using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    /// <summary>
    /// Manages the service menu, including its visibility and dragging functionality.
    /// </summary>
    public class ServiceMenuController : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public GameObject Panel;
        private bool isActive;
        private Vector2 dragOffset;

        private void Start()
        {
            Panel.SetActive(true);
            isActive = true;
        }

        /// <summary>
        /// Toggles the visibility of the system menu.
        /// </summary>
        public void Toggle()
        {
            isActive = !isActive;
            Panel.SetActive(isActive);
        }

        /// <summary>
        /// Sets the visibility of the system menu.
        /// </summary>
        /// <param name="isVisible">Whether the menu should be visible.</param>
        public void Toggle(bool isVisible)
        {
            isActive = isVisible;
            Panel.SetActive(isActive);
        }

        /// <summary>
        /// Called when dragging begins. Calculates the offset between the mouse position and the panel position.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragOffset = (Vector2)Panel.transform.position - eventData.position;
        }

        /// <summary>
        /// Called while dragging. Moves the panel based on the mouse position and the initial offset.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            Panel.transform.position = eventData.position + dragOffset;
        }
    }
}