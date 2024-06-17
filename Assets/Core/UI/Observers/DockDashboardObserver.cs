using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class DockDashboardObserver : MonoBehaviour
    {
        public DockProduct dockProduct;
        public GameObject dashboardInstance;
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;
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
        }

        private void Update()
        {
            DetectDockClick();

            if (isUpdating)
            {
                UpdateDashboard();
            }
        }

        public void InitDockDashboard()
        {
            dashboardInstance = this.gameObject;
            title = dashboardInstance.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            content = dashboardInstance.transform.Find("Content").GetComponent<TextMeshProUGUI>();
            dashboardInstance.SetActive(true);
            
            SetDashboardVisibility(false);  // Initially set dashboard to invisible
        }

        private void UpdateDashboard()
        {
            // Vector3 screenPos = dockProduct.mainCamera.WorldToScreenPoint(dockProduct.transform.position);
            // screenPos.y += offsetY;
            // dashboardInstance.transform.position = screenPos;

            if (dashboardInstance.activeSelf)
            {
                title.text = $"Distribution Center Schedule\n ";
                content.text = "";
                foreach (var vehicleProduct in vehicleFactory.productLookupTable)
                {
                    content.text += $"[{vehicleProduct.Value.productName}]\n";
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
                        SetDashboardVisibility(isUpdating);
                    }
                }
            }
        }

        public void UpdateUI()
        {
            UpdateDashboard();
        }
        
        private void SetDashboardVisibility(bool isVisible)
        {
            foreach (Transform child in dashboardInstance.transform)
            {
                child.gameObject.SetActive(isVisible);
            }
        }
    }
}
