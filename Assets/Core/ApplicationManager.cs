using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the initialization and configuration of factories and dependencies within the application.
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        [Header("Factories")]
        public List<VehicleConfig> vehiclesConfig;
        public VehicleFactory vehicleFactory;

        public List<SpaceConfig> spacesConfig;
        public SpaceFactory spaceFactory;

        public List<DockConfig> docksConfig;
        public DockFactory dockFactory;

        [Header("Dependencies")]
        public PathManager pathManager;
        [SerializeField] private SetSimulationServiceProvider setSimulationServiceProvider;
        [SerializeField] private SystemLog systemLog;
        [SerializeField] private GetClickedObject getClickedObject;
        [SerializeField] private GameObject mainUICanvas;

        /// <summary>
        /// Unity's Awake method. Initializes factories and activates the main UI.
        /// </summary>
        private void Awake()
        {
            InitializeFactories();
            ActivateMainUI();
        }

        /// <summary>
        /// Initializes the factories with their respective configurations and dependencies.
        /// </summary>
        private void InitializeFactories()
        {
            vehicleFactory.Initialize(systemLog, getClickedObject, setSimulationServiceProvider);
            spaceFactory.Initialize(systemLog, vehicleFactory, getClickedObject);
            dockFactory.Initialize(systemLog, getClickedObject);

            foreach (var spaceConfig in spacesConfig)
            {
                spaceFactory.ManufactureProduct(spaceConfig);
            }

            foreach (var dockConfig in docksConfig)
            {
                dockFactory.ManufactureProduct(dockConfig);
            }

            // Uncomment the following lines to initialize vehicle products if needed
            // foreach (var vehicleConfig in vehiclesConfig)
            // {
            //     vehicleFactory.ManufactureProduct(vehicleConfig);
            // }
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
