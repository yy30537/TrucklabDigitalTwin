using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace Core
{
    public class ApplicationManager : MonoBehaviour
    {
        public List<VehicleConfig> vehiclesConfig;
        public VehicleFactory vehicleFactory;
        
        public List<SpaceConfig> spacesConfig;
        public SpaceFactory spaceFactory;
        
        public List<DockConfig> docksConfig;
        public DockFactory dockFactory;

        public PathManager pathManager;
        public SetSimulationServiceProvider setSimulationServiceProvider;
        public SystemLog systemLog;
        public GetClickedObject getClickedObject;
        public GameObject mainUICanvas;

        private void Awake()
        {
            // systemLog = FindObjectOfType<SystemLog>();
            // getClickedObject = FindObjectOfType<GetClickedObject>();
            // setSimulationServiceProvider = FindObjectOfType<SetSimulationServiceProvider>();
            // pathManager = FindObjectOfType<PathManager>();

            InitializeFactories();
            ActivateMainUI();
        }

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

            // foreach (var vehicleConfig in vehiclesConfig)
            // {
            //     vehicleFactory.ManufactureProduct(vehicleConfig);
            // }
        }

        private void ActivateMainUI()
        {
            mainUICanvas.SetActive(true);
        }
    }
}
