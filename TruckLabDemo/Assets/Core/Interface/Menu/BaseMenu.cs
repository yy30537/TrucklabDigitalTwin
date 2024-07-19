using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base class for all menu components in the application.
    /// Provides common functionality for menu toggling and initialization.
    /// </summary>
    public abstract class BaseMenu : MonoBehaviour
    {
        /// <summary>
        /// The unique name identifier for this menu.
        /// </summary>
        [SerializeField]
        protected string MenuName;

        [Header("UI Event Channels")]
        [SerializeField]
        protected MenuNavigationEventChannel ToggleEc;

        [Header("UI Components")]
        [SerializeField]
        protected GameObject MenuInstance;

        /// <summary>
        /// Initializes the menu by setting up event listeners and initial state.
        /// </summary>
        public virtual void Init()
        {
            if (ToggleEc == null)
            {
                Debug.LogError($"ToggleEc is not assigned for {MenuName}");
                return;
            }

            ToggleEc.onEventRaised += Toggle;
            SetMenuVisibility(false);
        }

        /// <summary>
        /// Toggles the visibility of the menu based on the provided menu name.
        /// </summary>
        /// <param name="menuNameToNavigateTo">The name of the menu to toggle.</param>
        public virtual void Toggle(string menuNameToNavigateTo)
        {
            if (string.IsNullOrEmpty(MenuName))
            {
                Debug.LogWarning($"MenuName is not set for {gameObject.name}");
                return;
            }

            bool shouldActivate = MenuName == menuNameToNavigateTo;
            SetMenuVisibility(shouldActivate ? !MenuInstance.activeSelf : false);
        }

        /// <summary>
        /// Sets the visibility of the menu.
        /// </summary>
        /// <param name="isVisible">Whether the menu should be visible.</param>
        protected virtual void SetMenuVisibility(bool isVisible)
        {
            if (MenuInstance != null)
            {
                MenuInstance.SetActive(isVisible);
            }
            else
            {
                Debug.LogError($"MenuInstance is not assigned for {MenuName}");
            }
        }

        /// <summary>
        /// Unsubscribes from event listeners when the menu is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (ToggleEc != null)
            {
                ToggleEc.onEventRaised -= Toggle;
            }
        }
    }
}
