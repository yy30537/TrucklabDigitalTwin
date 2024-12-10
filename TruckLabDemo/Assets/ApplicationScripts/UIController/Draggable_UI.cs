using UnityEngine;
using UnityEngine.EventSystems;

namespace ApplicationScripts.UIController
{
    /// <summary>
    /// Handles drag-and-drop functionality for UI panels.
    /// Implements interfaces for detecting drag events and moving UI elements accordingly.
    /// </summary>
    public class Draggable_UI : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        /// <summary>
        /// The UI GameObject that will be moved during dragging.
        /// </summary>
        public GameObject UiGameObject;

        /// <summary>
        /// Stores the offset between the mouse position and the UI panel's position when dragging begins.
        /// </summary>
        private Vector2 dragOffset;

        /// <summary>
        /// Triggered when the user begins dragging the UI element.
        /// Calculates and stores the offset between the mouse position and the UI panel's position.
        /// </summary>
        /// <param name="eventData">Pointer event data containing information about the drag.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Calculate the offset between the panel's position and the mouse position.
            dragOffset = (Vector2)UiGameObject.transform.position - eventData.position;
        }

        /// <summary>
        /// Triggered while the user is dragging the UI element.
        /// Moves the UI panel to follow the mouse pointer, maintaining the initial offset.
        /// </summary>
        /// <param name="eventData">Pointer event data containing information about the drag.</param>
        public void OnDrag(PointerEventData eventData)
        {
            // Update the UI GameObject's position based on the current mouse position and offset.
            UiGameObject.transform.position = eventData.position + dragOffset;
        }
    }
}