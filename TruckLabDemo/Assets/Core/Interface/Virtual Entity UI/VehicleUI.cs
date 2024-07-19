using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// UI component for displaying and managing vehicle information.
    /// </summary>
    public class VehicleUI : BaseUI
    {
        [SerializeField] private VehicleProduct vehicleProduct;
        [SerializeField] private VehicleData vehicleData;

        [SerializeField] private GameObject topBar;
        [SerializeField] private GameObject[] pages;
        [SerializeField] private TextMeshProUGUI topBarText;
        [SerializeField] private TextMeshProUGUI[] pageContents;
        [SerializeField] private Button[] pageButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Dropdown kinematicsSourceDropdown, actuationSourceDropdown;
        [SerializeField] private Toggle tractorTrailToggle;

        [SerializeField] private TMP_Dropdown regionDropdown;
        [SerializeField] private Button setPositionButton;

        private const int PageCount = 4;
        private const int OffsetY = 150;

        public override void Initialize(Transform parent)
        {
            base.Initialize(parent);
            InitializeVehicleUIElements();
            HideDashboard();
        }

        public void SetVehicleData(VehicleProduct product, VehicleData data)
        {
            this.vehicleProduct = product;
            this.vehicleData = data;
            PopulateInputDropdown();
            PopulateRegionDropdown();
        }

        private void InitializeVehicleUIElements()
        {
            InitializeTopBar();
            InitializePages();
            InitializeButtons();
            InitializeDropdowns();
            InitializeToggle();

            SetupEventListeners();
            ShowPage(0);
        }

        private void InitializeTopBar()
        {
            topBar = UiInstance.transform.Find("topBar")?.gameObject;
            topBarText = topBar?.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void InitializePages()
        {
            pages = new GameObject[PageCount];
            pageContents = new TextMeshProUGUI[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                pages[i] = UiInstance.transform.Find($"Page {i + 1}")?.gameObject;
                pageContents[i] = pages[i]?.GetComponent<TextMeshProUGUI>();
            }
        }

        private void InitializeButtons()
        {
            pageButtons = new Button[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                pageButtons[i] = UiInstance.transform.Find($"Page {i + 1} Button")?.GetComponent<Button>();
            }
            closeButton = UiInstance.transform.Find("Close Button")?.GetComponent<Button>();

            regionDropdown = UiInstance.transform.Find("Page 3")?.Find("Regions Dropdown")?.GetComponent<TMP_Dropdown>();
            setPositionButton = UiInstance.transform.Find("Page 3")?.Find("Set Vehicle Position Button")?.GetComponent<Button>();
        }

        private void InitializeDropdowns()
        {
            kinematicsSourceDropdown = pages[0]?.transform.Find("Kinematics Source Dropdown")?.GetComponent<TMP_Dropdown>();
            actuationSourceDropdown = pages[0]?.transform.Find("Actuation Source Dropdown")?.GetComponent<TMP_Dropdown>();
        }

        private void InitializeToggle()
        {
            tractorTrailToggle = pages[0]?.transform.Find("Tractor Trail Toggle")?.GetComponent<Toggle>();
        }

        private void SetupEventListeners()
        {
            for (int i = 0; i < PageCount; i++)
            {
                int pageIndex = i;
                pageButtons[i]?.onClick.AddListener(() => ShowPage(pageIndex));
            }
            closeButton?.onClick.AddListener(HideDashboard);
            tractorTrailToggle?.onValueChanged.AddListener(OnToggleTractorTrail);
            kinematicsSourceDropdown?.onValueChanged.AddListener(OnKinematicsSourceChanged);
            actuationSourceDropdown?.onValueChanged.AddListener(OnActuationSourceChanged);
            setPositionButton?.onClick.AddListener(SetVehiclePositionToRegion);
        }

        public override void UpdateUI()
        {
            if (vehicleProduct == null || vehicleData == null) return;

            UpdateDashboardPosition();
            UpdateTopBar();
            UpdatePage2();
            UpdatePage3();
            UpdatePage4();
        }

        private void UpdateDashboardPosition()
        {
            if (vehicleProduct.MainCamera != null && vehicleProduct.MainCamera.isActiveAndEnabled)
            {
                Vector3 screenPos = vehicleProduct.MainCamera.WorldToScreenPoint(vehicleProduct.Animation.tractorTransform.position);
                screenPos.y += OffsetY;
                UiInstance.transform.position = screenPos;
            }
        }

        private void UpdateTopBar()
        {
            if (topBarText != null)
            {
                topBarText.text = $"{vehicleProduct.ProductName}\n";
            }
        }

        private void UpdatePage2()
        {
            if (pageContents[1] != null)
            {
                pageContents[1].text = $"Scale: {vehicleProduct.Config.Scale}\n" +
                                       $"l1 = {vehicleData.l1}m\n" +
                                       $"l1C = {vehicleData.l1C}m\n" +
                                       $"l2 = {vehicleData.l2}m\n" +
                                       $"tractor width = {vehicleData.tractorWidth}m\n" +
                                       $"trailer width = {vehicleData.trailerWidth}m\n";
            }
        }

        private void UpdatePage3()
        {
            if (pageContents[2] != null)
            {
                pageContents[2].text = $"v1 = {vehicleData.v1:F2} m/s\n" +
                                       $"delta = {vehicleData.delta:F2} rad\n" +
                                       $"gamma = {vehicleData.gamma:F2} rad\n" +
                                       $"x1,y1 = ({vehicleData.x1:F2},{vehicleData.y1:F2})m\n" +
                                       $"x2,y2 = ({vehicleData.x2:F2},{vehicleData.y2:F2})m\n" +
                                       $"psi1,psi2 = ({vehicleData.psi1:F2},{vehicleData.psi2:F2})rad\n";
            }
        }

        private void UpdatePage4()
        {
            if (pageContents[3] != null)
            {
                pageContents[3].text = "Obstacle(s) Detected:\n";
                if (vehicleProduct.Collision.detectedObstacles.Count == 0)
                {
                    pageContents[3].text += "None\n";
                }
                else
                {
                    foreach (var obstacle in vehicleProduct.Collision.detectedObstacles)
                    {
                        pageContents[3].text += $"{obstacle.Name} {obstacle.Distance:F2}m {obstacle.Angle:F2}°\n";
                    }
                }
            }
        }

        private void ShowPage(int pageIndex)
        {
            for (int i = 0; i < PageCount; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].SetActive(i == pageIndex);
                }
            }
        }

        private void PopulateInputDropdown()
        {
            if (kinematicsSourceDropdown != null)
            {
                kinematicsSourceDropdown.ClearOptions();
                kinematicsSourceDropdown.AddOptions(new List<string> { "Motion Capture", "Actuation" });
                kinematicsSourceDropdown.value = (int)vehicleProduct.Kinematics.kinematicsSource;
            }

            if (actuationSourceDropdown != null)
            {
                actuationSourceDropdown.ClearOptions();
                actuationSourceDropdown.AddOptions(new List<string> { "Thrust Master", "Controller", "Keyboard" });
                actuationSourceDropdown.value = (int)vehicleProduct.Kinematics.actuationInputSource;
            }
        }

        private void PopulateRegionDropdown()
        {
            if (regionDropdown != null)
            {
                regionDropdown.ClearOptions();
                List<string> regionNames = new List<string>();

                foreach (var region in vehicleProduct.RegionFactory.ProductLookupTable.Values)
                {
                    regionNames.Add(region.ProductName);
                }
                regionDropdown.AddOptions(regionNames);
            }
        }

        private void SetVehiclePositionToRegion()
        {
            if (regionDropdown != null && vehicleProduct != null)
            {
                int selectedIndex = regionDropdown.value;
                string selectedRegionName = regionDropdown.options[selectedIndex].text;

                foreach (var region in vehicleProduct.RegionFactory.ProductLookupTable.Values)
                {
                    if (region.ProductName == selectedRegionName)
                    {
                        var scale = vehicleProduct.Config.Scale;

                        vehicleProduct.Kinematics.SetVehiclePosition(region.x / scale, region.y / scale, region.psi1Rad * Mathf.Rad2Deg, region.psi2Rad * Mathf.Rad2Deg);
                        break;
                    }
                }
            }
        }

        private void OnToggleTractorTrail(bool status)
        {
            if (vehicleProduct?.Animation?.trail != null)
            {
                TrailRenderer trailRenderer = vehicleProduct.Animation.trail.GetComponent<TrailRenderer>();
                if (trailRenderer != null)
                {
                    trailRenderer.Clear();
                }
                vehicleProduct.Animation.trail.SetActive(status);
            }
        }

        private void OnKinematicsSourceChanged(int index)
        {
            if (vehicleProduct?.Kinematics != null)
            {
                vehicleProduct.Kinematics.kinematicsSource = (KinematicsSource)index;
            }
        }

        private void OnActuationSourceChanged(int index)
        {
            if (vehicleProduct?.Kinematics != null)
            {
                vehicleProduct.Kinematics.actuationInputSource = (ActuationInputSource)index;
            }
        }

        public bool DetectVehicleClick(GetClickedObject clickObjectGetter)
        {
            if (Input.GetMouseButtonDown(0) && clickObjectGetter != null)
            {
                GameObject hitObject = clickObjectGetter.ReturnClickedObject();
                if (hitObject != null && (hitObject.CompareTag("Tractor") || hitObject.CompareTag("Trailer")))
                {
                    VehicleProduct clickedVehicle = hitObject.GetComponentInParent<VehicleProduct>();
                    return clickedVehicle != null && clickedVehicle.ProductID == vehicleProduct.ProductID;
                }
            }
            return false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Remove all event listeners
            for (int i = 0; i < PageCount; i++)
            {
                if (pageButtons[i] != null)
                {
                    pageButtons[i].onClick.RemoveAllListeners();
                }
            }
            if (closeButton != null) closeButton.onClick.RemoveAllListeners();
            if (tractorTrailToggle != null) tractorTrailToggle.onValueChanged.RemoveAllListeners();
            if (kinematicsSourceDropdown != null) kinematicsSourceDropdown.onValueChanged.RemoveAllListeners();
            if (actuationSourceDropdown != null) actuationSourceDropdown.onValueChanged.RemoveAllListeners();
            if (setPositionButton != null) setPositionButton.onClick.RemoveAllListeners();
        }
    }
}