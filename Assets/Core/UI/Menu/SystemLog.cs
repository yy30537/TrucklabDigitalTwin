using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

namespace Core
{
    public class SystemLog : MonoBehaviour
    {
        [Header("UI Components")]
        public GameObject systemLogObject;
        
        public TextMeshProUGUI logText;    // Reference for the log
        public TextMeshProUGUI clockText;   // Reference for the system clock
        private StringBuilder logBuilder = new StringBuilder(); 
        private float systemStartTime;
        
        void Start()
        {
            systemLogObject.SetActive(false);
            systemStartTime = Time.timeSinceLevelLoad;
            logBuilder.Append("System Log:\n"); 
        }
        
        void Update() 
        {
            UpdateSystemClock();
        }
        
        public void LogEvent(string eventMessage)
        {
            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            string timestamp = string.Format("[{0:0.00}] ", currentTime); 
            logBuilder.AppendLine(timestamp + eventMessage);
            logText.text = logBuilder.ToString(); // Update the log
            LayoutRebuilder.ForceRebuildLayoutImmediate(logText.GetComponent<RectTransform>());
        }

        private void UpdateSystemClock() 
        {
            float currentTime = Time.timeSinceLevelLoad - systemStartTime;
            int minutes = Mathf.FloorToInt(currentTime / 60.0f);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
            string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
            clockText.text = "DC-Clock: " + timeText;
        }

        public void Toggle()
        {
            systemLogObject.SetActive(!systemLogObject.activeSelf);
        }
        
    }
}