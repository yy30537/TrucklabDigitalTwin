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
    
    public class SpaceDashboard : MonoBehaviour
    {
        public SpaceProduct spaceProduct;
        public GameObject dashboardInstance;
        public TextMeshProUGUI textContent;
        public GetClickedObject getClickedObject;
        public bool isToggleAllowed = false;
        public bool isUpdating = false;
        public VoidEventChannel ecToggleIsActive;
        
        private Transform mainCanvasParent;
        private int offset = 100;
        
        private Button closeButton;

        public void Initialize(SpaceProduct product, Transform canvasParent)
        {
            spaceProduct = product;
            mainCanvasParent = canvasParent;
            getClickedObject = FindObjectOfType<GetClickedObject>();
            spaceProduct.RegisterObserver(this);
            InitSpaceDashboard();
        }

        private void OnDestroy()
        {
            if (spaceProduct != null)
            {
                spaceProduct.RemoveObserver(this);
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
                DetectSpaceClick();
            }
            if (isUpdating)
            {
                UpdateDashboard();
                spaceProduct.meshRenderer.material = spaceProduct.spaceConfig.spaceMaterialActive;
            } 
            else
            {
                spaceProduct.meshRenderer.material = spaceProduct.spaceConfig.spaceMaterial;
            }
        }

        public void InitSpaceDashboard()
        {
            dashboardInstance = Instantiate(spaceProduct.spaceConfig.spaceDashboard, mainCanvasParent);
            textContent = dashboardInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            dashboardInstance.SetActive(false);
            
            closeButton = dashboardInstance.transform.Find("Close Button").GetComponent<Button>();
            closeButton.onClick.AddListener(CloseDashboard);

            ecToggleIsActive = spaceProduct.spaceConfig.ecToggleDashboard;
            ecToggleIsActive.onEventRaised += OnToggleUI;
            isToggleAllowed = true;
        }

        public void UpdateDashboard()
        {
            Vector3 screenPos = spaceProduct.mainCamera.WorldToScreenPoint(spaceProduct.transform.position);
            screenPos.y += offset;
            dashboardInstance.transform.position = screenPos;

            textContent.text = $"{spaceProduct.productName}\n";
            foreach (var vehicle in spaceProduct.vehiclesInside.Values)
            {
                textContent.text += $"[{vehicle.productName}] \n";
            }
        }
        
        private void DetectSpaceClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject hitObject = getClickedObject.ReturnClickedObject();
                if (hitObject != null && hitObject.CompareTag("SpaceProduct"))
                {
                    SpaceProduct clickedSpace = hitObject.GetComponent<SpaceProduct>();
                    if (clickedSpace.productID == spaceProduct.productID)
                    {
                        isUpdating = true;
                        dashboardInstance.SetActive(isUpdating);
                    }
                }
            }
        }

        private void OnToggleUI()
        {
            isToggleAllowed = !isToggleAllowed;
        }
        
        private void CloseDashboard()
        {
            isUpdating = false;
            dashboardInstance.SetActive(isUpdating);
        }
        
    }
}
