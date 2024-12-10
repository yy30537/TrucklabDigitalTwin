using ApplicationScripts.EventChannel;
using ApplicationScripts.UIController.ApplicationUI;
using UnityEngine;

namespace ApplicationScripts.UIController
{
    /// <summary>
    /// Base class for managing UI components, providing functionality for 
    /// visibility toggling, initialization, and handling navigation events.
    /// </summary>
    public abstract class Base_UI_Controller : MonoBehaviour
    {
        /// <summary>
        /// Name of the UI element, used for navigation and visibility control.
        /// </summary>
        [Header("===Parent: Base_UI_Controller===")]
        [Header("UI Name")]
        public string UiName;

        /// <summary>
        /// Indicates whether the UI is currently active and visible.
        /// </summary>
        [Header("Status")]
        public bool IsActive = false;

        /// <summary>
        /// Parent GameObject containing the UI instance.
        /// </summary>
        [Header("UI Object Parent")]
        public GameObject UiInstance;

        /// <summary>
        /// Event channel for managing UI navigation events.
        /// </summary>
        [Header("Navigation Event Channel")]
        public EventChannel_UI_Navigation UiNavigationEventChannel;

        /// <summary>
        /// System log controller for logging UI-related events.
        /// </summary>
        [Header("Application Dependencies")]
        [SerializeField] protected UI_Controller_SystemLog SystemLogUiController;

        /// <summary>
        /// Initializes the UI controller, setting up event listeners and 
        /// initializing the visibility of the UI instance.
        /// </summary>
        public virtual void Init()
        {
            if (UiNavigationEventChannel == null)
            {
                Debug.LogError($"UiNavigationEventChannel is not assigned for {UiName}");
                return;
            }
            UiNavigationEventChannel.OnEventRaised += Toggle_UI_Visibility;
            IsActive = false;
            Set_UI_Visibility(IsActive);
        }

        /// <summary>
        /// Toggles the visibility of the UI based on the given target UI name.
        /// </summary>
        /// <param name="target_ui_name">The name of the UI to toggle visibility for.</param>
        public virtual void Toggle_UI_Visibility(string target_ui_name)
        {
            if (string.IsNullOrEmpty(UiName))
            {
                Debug.LogWarning($"UiName is not set for {gameObject.name}");
                return;
            }

            bool current = (UiName == target_ui_name);

            if (current)
            {
                IsActive = !IsActive;
            }

            Set_UI_Visibility(IsActive);
        }

        /// <summary>
        /// Sets the visibility of the UI instance.
        /// </summary>
        /// <param name="isVisible">Whether the UI should be visible.</param>
        protected virtual void Set_UI_Visibility(bool isVisible)
        {
            if (UiInstance != null)
            {
                IsActive = isVisible;
                UiInstance.SetActive(IsActive);
            }
            else
            {
                SystemLogUiController.LogEvent($"UiInstance is not assigned for {UiName}");
            }
        }

        /// <summary>
        /// Displays the UI by making it visible.
        /// </summary>
        public void ShowUi() => Set_UI_Visibility(true);

        /// <summary>
        /// Hides the UI by making it invisible.
        /// </summary>
        public void HideUi() => Set_UI_Visibility(false);

        /// <summary>
        /// Checks if the dashboard is currently visible and active.
        /// </summary>
        /// <returns>True if the dashboard is visible and active, otherwise false.</returns>
        public bool IsDashboardVisible() => UiInstance != null && UiInstance.activeSelf && IsActive;

        /// <summary>
        /// Cleans up resources by removing event listeners when the UI controller is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (UiNavigationEventChannel != null)
            {
                UiNavigationEventChannel.OnEventRaised -= Toggle_UI_Visibility;
            }
        }
    }
}
