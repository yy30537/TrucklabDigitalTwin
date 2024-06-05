
using System.Collections.Generic;
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
        public GameObject mainUICanvas;


        
        private void Awake()
        {
            foreach (var spaceConfig in spacesConfig)
            {
                spaceFactory.ManufactureProduct(spaceConfig);
            }
            
            foreach (var dockConfig in docksConfig)
            {
                dockFactory.ManufactureProduct(dockConfig);
            }
            mainUICanvas.SetActive(true);
        }
    }
}
