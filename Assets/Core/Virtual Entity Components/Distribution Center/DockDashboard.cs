using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Core
{
    public class DockDashboard : MonoBehaviour
    {
        public DockProduct dockProduct;
        public GameObject dashboardInstance;
        public TextMeshProUGUI textContent;
        public GetClickedObject getClickedObject;
        public bool isToggleAllowed = false;
        public bool isUpdating = false;
        public VoidEventChannel ecToggleIsActive;
        public VehicleFactory vehicleFactory;
        private Transform mainCanvasParent;
        private int offsetY = 300;
        private int offsetX = 200;
        private Button closeButton;
        
        public void Initialize(DockProduct product, Transform canvasParent)
        {
            dockProduct = product;
            mainCanvasParent = canvasParent;
            getClickedObject = FindObjectOfType<GetClickedObject>();
            vehicleFactory = FindObjectOfType<VehicleFactory>();
            InitDockDashboard();
            dockProduct.RegisterObserver(this);
        }
        private void OnDestroy()
        {
            if (dockProduct != null)
            {
                dockProduct.RemoveObserver(this);
            }
            if (ecToggleIsActive != null)
            {
                ecToggleIsActive.onEventRaised -= OnToggleUI;
            }
        }
        private void Update()
        {
            
            if (isToggleAllowed)
            {
                DetectDockClick();
            }

            if (isUpdating)
            {
                UpdateDashboard();
            }
        }
        public void InitDockDashboard()
        {
            dashboardInstance = Instantiate(dockProduct.dockConfig.dockBuildingDashboard, mainCanvasParent);
            textContent = dashboardInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            dashboardInstance.SetActive(false);
            
            closeButton = dashboardInstance.transform.Find("Close Button").GetComponent<Button>();
            closeButton.onClick.AddListener(CloseDashboard);

            ecToggleIsActive = dockProduct.dockConfig.ecToggleDashboard;
            ecToggleIsActive.onEventRaised += OnToggleUI;
            isToggleAllowed = true;
        }
        private void UpdateDashboard()
        {
            Vector3 screenPos = dockProduct.mainCamera.WorldToScreenPoint(dockProduct.transform.position);
            screenPos.y += offsetY;
            //screenPos.x -= offsetX;
            dashboardInstance.transform.position = screenPos;
            
            if (dashboardInstance.activeSelf)
            {
                textContent.text = $"Distribution Center\n ";
                foreach (var vehicleProduct in vehicleFactory.productLookupTable)
                {
                    textContent.text += $"[{vehicleProduct.Value.productName}]\n";
                }
            }
        }
        private void DetectDockClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitObject = getClickedObject.ReturnClickedObject();
                if (hitObject != null && hitObject.CompareTag("DC"))
                {
                    DockProduct clickedDock = hitObject.GetComponentInParent<DockProduct>();
                    if (clickedDock.productID == dockProduct.productID)
                    {
                        isUpdating = !isUpdating;
                        dashboardInstance.SetActive(isUpdating);
                    }
                }
            }
        }
        private void OnToggleUI()
        {
            isToggleAllowed = !isToggleAllowed;
        }
        public void UpdateUI()
        {
            UpdateDashboard();
        }
        private void CloseDashboard()
        {
            isUpdating = false;
            dashboardInstance.SetActive(isUpdating);
        }
        
    }
}
