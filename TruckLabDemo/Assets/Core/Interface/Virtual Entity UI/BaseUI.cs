using UnityEngine;

namespace Core
{
    /// <summary>
    /// Base class for Virtual Entity UI components in the application.
    /// </summary>
    public abstract class BaseUI : MonoBehaviour
    {
        /// <summary>
        /// The GameObject instance representing this UI.
        /// </summary>
        protected GameObject UiInstance;


        [SerializeField] protected SystemLogWindow SystemLogWindow;

        /// <summary>
        /// Initializes the UI component.
        /// </summary>
        /// <param name="parent">The parent transform for this UI component.</param>
        public virtual void Initialize(Transform parent)
        {
            SystemLogWindow = FindObjectOfType<SystemLogWindow>();
            UiInstance = this.gameObject;
            if (UiInstance == null)
            {
                SystemLogWindow.LogEvent($"Failed to initialize UI for {GetType().Name}");
            }
        }

        /// <summary>
        /// Updates the UI with current data. Must be implemented by derived classes.
        /// </summary>
        public abstract void UpdateUI();

        public virtual void ShowDashboard() => SetDashboardVisibility(true);
        public virtual void HideDashboard() => SetDashboardVisibility(false);
        public virtual void ToggleDashboard() => SetDashboardVisibility(!IsDashboardVisible());

        protected virtual void SetDashboardVisibility(bool isVisible)
        {
            if (UiInstance != null)
            {
                UiInstance.SetActive(isVisible);
            }
        }

        public bool IsDashboardVisible() => UiInstance != null && UiInstance.activeSelf;

        protected virtual void OnDestroy()
        {
            // Base cleanup logic, if any
        }
    }
}