using UnityEngine;
using System;

namespace Core
{
    /// <summary>
    /// Manages the initialization and configuration of core application components.
    /// This class follows the Singleton pattern to ensure a single point of access.
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainUiCanvas;
        [SerializeField] private GameObject plane;
        [SerializeField] private FactoryMenu factoryMenu;
        [SerializeField] private SimulationMenu simulationMenu;
        [SerializeField] private ServiceMenuController serviceMenuController;
        [SerializeField] private SystemLogWindow systemLogWindow;

        /// <summary>
        /// Gets the singleton instance of the ApplicationManager.
        /// </summary>
        public static ApplicationManager Instance { get; private set; }

        /// <summary>
        /// Private constructor to prevent instantiation outside of Unity's instantiation system.
        /// </summary>
        private ApplicationManager() { }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes the singleton instance and sets up initial UI state.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Check if the ApplicationManager is a root GameObject
            if (transform.parent != null)
            {
                // If it's not a root GameObject, make it a root GameObject
                transform.SetParent(null);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
            OnStart();  // Directly start the application
        }

        /// <summary>
        /// Initializes the UI components to their starting state.
        /// </summary>
        private void InitializeUI()
        {
            mainUiCanvas.SetActive(true); // Ensure the main UI is active by default
        }

        /// <summary>
        /// Starts the application, initializing core components and UI elements.
        /// This method is called to begin the application's main functionality.
        /// </summary>
        private void OnStart()
        {
            try
            {
                ActivateMainUI();
                InitializeFactories();
                InitializeSystemMenu();
                LogEvent("Application started successfully.");
            }
            catch (Exception ex)
            {
                LogError($"Error during application start: {ex.Message}");
            }
        }

        /// <summary>
        /// Activates the main UI components.
        /// </summary>
        private void ActivateMainUI()
        {
            mainUiCanvas.SetActive(true);
            plane.SetActive(true);
            LogEvent("Main UI activated.");
        }

        /// <summary>
        /// Initializes the factory components for creating game entities.
        /// </summary>
        private void InitializeFactories()
        {
            if (factoryMenu != null)
            {
                factoryMenu.CreateDockStations();
                factoryMenu.CreateRegions();
                LogEvent("Factories initialized successfully.");
            }
            else
            {
                LogError("FactoryMenu is not assigned in ApplicationManager.");
            }
        }

        /// <summary>
        /// Initializes the system menu, setting its initial state.
        /// </summary>
        private void InitializeSystemMenu()
        {
            if (serviceMenuController != null)
            {
                serviceMenuController.Toggle(false);
                LogEvent("System menu initialized.");
            }
            else
            {
                LogError("ServiceMenuController is not assigned in ApplicationManager.");
            }
        }

        /// <summary>
        /// Logs an error message to both the SystemLogWindow and Unity console.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        private void LogError(string message)
        {
            systemLogWindow?.LogEvent($"ERROR: {message}");
            Debug.LogError(message);
        }

        /// <summary>
        /// Logs an informational event message to the SystemLogWindow.
        /// </summary>
        /// <param name="message">The event message to log.</param>
        private void LogEvent(string message)
        {
            systemLogWindow?.LogEvent(message);
        }
    }
}
