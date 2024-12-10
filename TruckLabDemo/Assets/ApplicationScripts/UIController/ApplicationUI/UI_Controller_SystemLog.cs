using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ApplicationScripts.UIController.ApplicationUI
{
    /// <summary>
    /// Manages the system log window, including event logging and displaying the system clock.
    /// </summary>
    public class UI_Controller_SystemLog : Base_UI_Controller
    {
        /// <summary>
        /// UI text element for displaying the system log.
        /// </summary>
        [Header("===Children: UI_Controller_SystemLog===")]
        [Header("UI Objects")]
        public TextMeshProUGUI LogText;

        /// <summary>
        /// UI text element for displaying the system clock.
        /// </summary>
        public TextMeshProUGUI ClockText;

        /// <summary>
        /// Stores log messages in memory for display in the system log.
        /// </summary>
        [SerializeField] private StringBuilder logBuilder = new StringBuilder();

        /// <summary>
        /// Stores the Time when the system started, used for calculating log timestamps.
        /// </summary>
        [SerializeField] private float systemStartTime;

        /// <summary>
        /// Initializes the system log controller and sets the initial system Time.
        /// </summary>
        void Start()
        {
            Init();

            systemStartTime = Time.timeSinceLevelLoad;
            logBuilder.Append("System Log:\n"); // Initialize the log with a header
        }

        /// <summary>
        /// Updates the system clock at fixed intervals.
        /// </summary>
        private void FixedUpdate()
        {
            UpdateSystemClock();
        }

        /// <summary>
        /// Logs an event to the system log.
        /// </summary>
        /// <param name="eventMessage">The message to log.</param>
        public void LogEvent(string eventMessage)
        {
            if (LogText == null) return; // Avoid errors if LogText is not assigned

            Debug.Log(eventMessage);
            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            string timestamp = $"[{currentTime:0.00}] "; // Format the timestamp
            logBuilder.AppendLine(timestamp + eventMessage); // Append timestamped message to the log

            if (LogText != null) // Ensure LogText is still valid before updating the UI
            {
                LogText.text = logBuilder.ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(LogText.GetComponent<RectTransform>()); // Force layout update
            }
        }

        /// <summary>
        /// Updates the system clock display with the current simulation Time.
        /// </summary>
        private void UpdateSystemClock()
        {
            if (ClockText == null) return; // Avoid errors if ClockText is not assigned

            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            int minutes = Mathf.FloorToInt(currentTime / 60.0f); // Calculate elapsed minutes
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60); // Calculate elapsed seconds

            if (ClockText != null) // Ensure ClockText is still valid before updating the UI
            {
                ClockText.text = $"DC-Clock: {minutes:00}:{seconds:00}"; // Update clock display
            }
        }
    }
}
