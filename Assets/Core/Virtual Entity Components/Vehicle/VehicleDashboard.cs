using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class VehicleDashboard : VehicleComponent
    {
        public GameObject dashboardInstance;

        private TextMeshProUGUI topbarText;
        private TextMeshProUGUI page1Content;
        private TextMeshProUGUI page2Content;
        private TextMeshProUGUI page3Content;
        private TextMeshProUGUI page4Content;
        
        private GameObject topbar;
        private GameObject page1;
        private GameObject page2;
        private GameObject page3;
        private GameObject page4;

        private Button page1Button;
        private Button page2Button;
        private Button page3Button;
        private Button page4Button;

        private TMP_Dropdown kinematicsSourceDropdown;
        private TMP_Dropdown actuationSourceDropdown;
        
        
        private Button closeButton;

        public GetClickedObject getClickedObject;
        
        public bool isToggleActive = false;
        public bool isUpdating = false;
        
        private VoidEventChannel ecToggleActive;
        private int offset = 200;
        
        
        void Update()
        {
            if (isToggleActive)
            {
                DetectVehicleClick();
            }
            if (isUpdating)
            {
                UpdateDashboard();
            }
        }
        public void InitVehicleDashboard(Transform dashboardParent)
        {
            getClickedObject = FindObjectOfType<GetClickedObject>();
            
            dashboardInstance = Instantiate(vehicleProduct.vehicleConfig.vehicleDashboardPrefab, dashboardParent);
            
            topbar = dashboardInstance.transform.Find("topbar").gameObject;
            page1 = dashboardInstance.transform.Find("Page 1").gameObject;
            page2 = dashboardInstance.transform.Find("Page 2").gameObject;
            page3 = dashboardInstance.transform.Find("Page 3").gameObject;
            page4 = dashboardInstance.transform.Find("Page 4").gameObject;

            topbarText = topbar.GetComponentInChildren<TextMeshProUGUI>();
            page1Content = page1.GetComponent<TextMeshProUGUI>();
            page2Content = page2.GetComponent<TextMeshProUGUI>();
            page3Content = page3.GetComponent<TextMeshProUGUI>();
            page4Content = page4.GetComponent<TextMeshProUGUI>();
            dashboardInstance.SetActive(false);

            kinematicsSourceDropdown = page1.transform.Find("Kinematics Source Dropdown").gameObject.GetComponent<TMP_Dropdown>();
            actuationSourceDropdown = page1.transform.Find("Actuation Source Dropdown").gameObject.GetComponent<TMP_Dropdown>();
            kinematicsSourceDropdown.onValueChanged.AddListener(OnKinematicsSourceChanged);
            actuationSourceDropdown.onValueChanged.AddListener(OnActuationSourceChanged);
            
            
            // Initialize buttons
            page1Button = dashboardInstance.transform.Find("Page 1 Button").GetComponent<Button>();
            page2Button = dashboardInstance.transform.Find("Page 2 Button").GetComponent<Button>();
            page3Button = dashboardInstance.transform.Find("Page 3 Button").GetComponent<Button>();
            page4Button = dashboardInstance.transform.Find("Page 4 Button").GetComponent<Button>();
            closeButton = dashboardInstance.transform.Find("Close Button").GetComponent<Button>();
            
            page1Button.onClick.AddListener(ShowPage1);
            page2Button.onClick.AddListener(ShowPage2);
            page3Button.onClick.AddListener(ShowPage3);
            page4Button.onClick.AddListener(ShowPage4);
            closeButton.onClick.AddListener(CloseDashboard);
            
            PopulateDropdowns();
            ShowPage1();
            
            ecToggleActive = vehicleProduct.vehicleConfig.ecToggleDashboard;
            ecToggleActive.onEventRaised += OnToggleActive;
            isToggleActive = true;
        }
        private void UpdateDashboard()
        {
            Vector3 screenPos = vehicleProduct.mainCamera.WorldToScreenPoint(vehicleProduct.vehicleAnimation.tractorTransform.position);
            screenPos.y += offset;
            dashboardInstance.transform.position = screenPos;
            
            topbarText.text = $"{vehicleProduct.productName} Dashboard\n";

            page1Content.text = 
                $"Name: {vehicleProduct.productName} \n" +
                $"ID:{vehicleProduct.vehicleConfig.vehicleID}\n";
            
            page2Content.text = 
                $"Scale: {vehicleProduct.vehicleConfig.scale}\n" +
                $"l1 = {VehicleData.l1}m\n" +
                $"l1C = {VehicleData.l1C}m\n" +
                $"l2 = {VehicleData.l2}m\n" +
                $"tractor width = {VehicleData.tractorWidth}m\n" +
                $"trailer width = {VehicleData.trailerWidth}m\n";

            page3Content.text =
                $"v1 = {VehicleData.v1:F2} m/s\n" +
                $"delta = {VehicleData.delta:F2} rad\n" +
                $"gamma = {VehicleData.gamma:F2} rad\n" +
                $"x1,y1 = ({VehicleData.x1:F2},{VehicleData.y1:F2})m\n" +
                $"x2,y2 = ({VehicleData.x2:F2},{VehicleData.y2:F2})m\n" +
                $"psi1,psi2 = ({VehicleData.psi1:F2},{VehicleData.psi2:F2})rad\n";
            
            page4Content.text = $"Obstacle(s) Detected: \n";
            
            foreach (var obstacle in vehicleProduct.collisionController.detectedObstacles)
            {
                page4Content.text += $"{obstacle.Name} {obstacle.Distance:F2}m {obstacle.Angle:F2}\u00b0\n";
            }

        }
        public void DetectVehicleClick() 
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitObject = getClickedObject.ReturnClickedObject();
                if (hitObject != null)
                {
                    if (hitObject.CompareTag("Tractor") || hitObject.CompareTag("Trailer"))
                    {
                        VehicleProduct clickedVehicle = hitObject.GetComponentInParent<VehicleProduct>();
                        if (clickedVehicle.productID == vehicleProduct.productID)
                        {
                            Debug.Log($"VehicleDashboard: {clickedVehicle.name} clicked");
                            isUpdating = true;
                            dashboardInstance.SetActive(isUpdating);
                        }
                    } 
                }
            }
        }
        private void OnToggleActive()
        {
            isToggleActive = !isToggleActive;
        }
        private void ShowPage1()
        {
            page1.SetActive(true);
            page2.SetActive(false);
            page3.SetActive(false);
            page4.SetActive(false);
        }
        private void ShowPage2()
        {
            page1.SetActive(false);
            page2.SetActive(true);
            page3.SetActive(false);
            page4.SetActive(false);
        }
        private void ShowPage3()
        {
            page1.SetActive(false);
            page2.SetActive(false);
            page3.SetActive(true);
            page4.SetActive(false);
        }
        private void ShowPage4()
        {
            page1.SetActive(false);
            page2.SetActive(false);
            page3.SetActive(false);
            page4.SetActive(true);
        }
        
        private void PopulateDropdowns()
        {
            kinematicsSourceDropdown.ClearOptions();
            kinematicsSourceDropdown.AddOptions(new List<string> { "Motion Capture", "Actuation" });
            kinematicsSourceDropdown.value = (int)vehicleProduct.vehicleKinematics.kinematicsSource;

            actuationSourceDropdown.ClearOptions();
            actuationSourceDropdown.AddOptions(new List<string> { "Thrust Master", "Controller", "Keyboard" });
            actuationSourceDropdown.value = (int)vehicleProduct.vehicleKinematics.actuationInputSource;
        }

        private void OnKinematicsSourceChanged(int index)
        {
            vehicleProduct.vehicleKinematics.kinematicsSource = (KinematicsSource)index;
        }

        private void OnActuationSourceChanged(int index)
        {
            vehicleProduct.vehicleKinematics.actuationInputSource = (ActuationInputSource)index;
        }
        private void CloseDashboard()
        {
            isUpdating = false;
            dashboardInstance.SetActive(isUpdating);
        }
        void OnDestroy()
        {
            ecToggleActive.onEventRaised -= OnToggleActive;
            Destroy(dashboardInstance);
        }
    }
}
