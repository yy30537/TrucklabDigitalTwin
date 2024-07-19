using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Manages the system log window, including logging events and updating the system clock.
    /// </summary>
    public class SystemLogWindow : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject systemLogObject;
        public TextMeshProUGUI logText;
        public TextMeshProUGUI clockText;

        private StringBuilder logBuilder = new StringBuilder();
        private float systemStartTime;

        private void Start()
        {
            if (systemLogObject != null)
            {
                systemLogObject.SetActive(true);
            }
            systemStartTime = Time.timeSinceLevelLoad;
            logBuilder.Append("System Log:\n");
        }

        private void Update()
        {
            UpdateSystemClock();
        }

        /// <summary>
        /// Logs an event to the system log.
        /// </summary>
        /// <param name="eventMessage">The message to log.</param>
        public void LogEvent(string eventMessage)
        {
            if (logText == null) return;  // Check if logText is null

            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            string timestamp = $"[{currentTime:0.00}] ";
            logBuilder.AppendLine(timestamp + eventMessage);

            if (logText != null) // Check if logText is still not null before setting the text
            {
                logText.text = logBuilder.ToString();
                LayoutRebuilder.ForceRebuildLayoutImmediate(logText.GetComponent<RectTransform>());
            }
        }

        private void UpdateSystemClock()
        {
            if (clockText == null) return;  // Check if clockText is null

            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            int minutes = Mathf.FloorToInt(currentTime / 60.0f);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

            if (clockText != null) // Check if clockText is still not null before setting the text
            {
                clockText.text = $"DC-Clock: {minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// Toggles the visibility of the system log window.
        /// </summary>
        public void Toggle()
        {
            if (systemLogObject != null)
            {
                systemLogObject.SetActive(!systemLogObject.activeSelf);
            }
        }
    }
}
