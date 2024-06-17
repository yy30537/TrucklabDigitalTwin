using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace Core
{
    /// <summary>
    /// Manages the initialization and configuration of factories and dependencies within the application.
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private GameObject mainUICanvas;

        /// <summary>
        /// Unity's Awake method. Initializes factories and activates the main UI.
        /// </summary>
        private void Awake()
        {
            ActivateMainUI();
        }
        
        /// <summary>
        /// Activates the main UI canvas.
        /// </summary>
        private void ActivateMainUI()
        {
            mainUICanvas.SetActive(true);
        }
    }
}
