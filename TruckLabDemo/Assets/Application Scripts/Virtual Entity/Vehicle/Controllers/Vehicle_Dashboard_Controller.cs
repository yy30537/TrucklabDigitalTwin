using System.Collections.Generic;
using Application_Scripts.UI_Controller;
using Application_Scripts.UI_Controller.Application_UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application_Scripts.Virtual_Entity.Vehicle.Controllers
{
    /// <summary>
    /// UI component for displaying and managing VeVehicle information.
    /// </summary>
    public class Vehicle_Dashboard_Controller : Base_UI_Controller
    {

        /// <summary>
        /// Determines whether the dashboard floats above the vehicle in the scene.
        /// </summary>
        [Header("UI Positioning")]
        public bool FloatAboveVehicle = true;

        /// <summary>
        /// Reference to the associated VeVehicle object.
        /// </summary>
        [Header("VE References")]
        public VE_Vehicle VeVehicle;

        /// <summary>
        /// Reference to the associated vehicle data object.
        /// </summary>
        public Vehicle_Data VehicleData;

        /// <summary>
        /// Parent transform under which the UI elements will be instantiated.
        /// </summary>
        [Header("UI References")]
        public Transform VeUiInstanceParentTransform;

        /// <summary>
        /// Top bar of the dashboard, used for displaying titles and dragging the UI.
        /// </summary>
        public GameObject TopBar;

        /// <summary>
        /// Array of page objects in the dashboard UI.
        /// </summary>
        public GameObject[] Pages;

        /// <summary>
        /// Text displayed on the top bar of the dashboard.
        /// </summary>
        public TextMeshProUGUI TopBarText;

        /// <summary>
        /// Text displaying the current kinematics mode.
        /// </summary>
        public TextMeshProUGUI KinematicsMode;

        /// <summary>
        /// Array of content texts for each page in the dashboard.
        /// </summary>
        public TextMeshProUGUI[] PageContents;

        /// <summary>
        /// Array of buttons for navigating between pages.
        /// </summary>
        public Button[] PageButtons;

        /// <summary>
        /// Button to close the dashboard UI.
        /// </summary>
        public Button CloseButton;

        /// <summary>
        /// Dropdown for selecting the actuation source.
        /// </summary>
        public TMP_Dropdown ActuationSourceDropdown;

        /// <summary>
        /// Dropdown for selecting a space to position the vehicle.
        /// </summary>
        public TMP_Dropdown SpaceDropdown;

        /// <summary>
        /// Toggle for enabling or disabling the tractor TractorTrajectory visualization.
        /// </summary>
        public Toggle TractorTrailToggle;

        /// <summary>
        /// Button to set the vehicle's position based on the selected space.
        /// </summary>
        public Button SetPositionButton;

        /// <summary>
        /// Toggle for pinning the dashboard in a fixed position.
        /// </summary>
        public Toggle PinDashboardToggle;

        /// <summary>
        /// Number of pages in the dashboard UI.
        /// </summary>
        private const int PageCount = 4;

        /// <summary>
        /// Vertical offset for the dashboard when floating above the vehicle.
        /// </summary>
        private const int OffsetY = 150;

        /// <summary>
        /// Updates the dashboard content and position.
        /// </summary>
        void Update()
        {
            if (IsActive)
            {
                UpdateDashboard();
            }
        }

        /// <summary>
        /// Sets up dependencies for the dashboard controller.
        /// </summary>
        public void SetDependencies(
            VE_Vehicle vehicle,
            Vehicle_Data data,
            GameObject ui_instance,
            Transform ve_ui_instance_parent_transform,
            UI_Controller_SystemLog systemLog_ui_controller
        )
        {
            VeVehicle = vehicle;
            VehicleData = data;
            UiInstance = ui_instance;
            VeUiInstanceParentTransform = ve_ui_instance_parent_transform;
            SystemLogUiController = systemLog_ui_controller;
        }

        /// <summary>
        /// Initializes the dashboard UI and its components.
        /// </summary>
        public override void Init()
        {
            UiName = VehicleData.Name;
            UiInstance.name = UiName;
            UiNavigationEventChannel = VehicleData.Config.UiNavigationEventChannel;
            base.Init();
            IsActive = true;
            UiInstance.SetActive(true);
            InitializeVehicleDashboardUiObjects();
            SystemLogUiController.LogEvent($"Initialized Vehicle UI for: {UiName}");
        }

        /// <summary>
        /// Initializes all UI elements and configurations for the dashboard.
        /// </summary>
        private void InitializeVehicleDashboardUiObjects()
        {
            PopulateInputDropdown();

            InitializeTopBar();

            InitializePages();

            InitializeButtons();

            InitializeDropdowns();

            InitializeToggle();

            PopulateSpaceDropdown();

            InitializeKinematicsModeText();

            InitializeDashboardPinToggle();

            SetupEventListeners();

            ShowPage(0);
        }

        /// <summary>
        /// Initializes the top bar UI element.
        /// </summary>
        private void InitializeTopBar()
        {
            TopBar = UiInstance.transform.Find("topBar")?.gameObject;
            TopBarText = TopBar?.GetComponentInChildren<TextMeshProUGUI>();
            var draggableUi = TopBar.AddComponent<Draggable_UI>();
            draggableUi.UiGameObject = this.gameObject;
        }

        /// <summary>
        /// Initializes the text displaying the kinematics mode.
        /// </summary>
        private void InitializeKinematicsModeText()
        {
            KinematicsMode = Pages[0].transform.Find("Kinematics Source Text").GetComponent<TextMeshProUGUI>(); ;
        }

        /// <summary>
        /// Configures the pages in the dashboard UI.
        /// </summary>
        private void InitializePages()
        {
            Pages = new GameObject[PageCount];
            PageContents = new TextMeshProUGUI[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                Pages[i] = UiInstance.transform.Find($"Page {i + 1}")?.gameObject;
                Pages[i].SetActive(true);
                PageContents[i] = Pages[i]?.GetComponent<TextMeshProUGUI>();
            }
            ShowPage(0);
        }

        /// <summary>
        /// Sets up the buttons in the dashboard UI.
        /// </summary>
        private void InitializeButtons()
        {
            PageButtons = new Button[PageCount];
            for (int i = 0; i < PageCount; i++)
            {
                PageButtons[i] = UiInstance.transform.Find($"Page {i + 1} Button")?.GetComponent<Button>();
            }
            CloseButton = UiInstance.transform.Find("Close Button")?.GetComponent<Button>();

            SpaceDropdown = UiInstance.transform.Find("Page 3")?.Find("Spaces Dropdown")?.GetComponent<TMP_Dropdown>();
            SetPositionButton = UiInstance.transform.Find("Page 3")?.Find("Set Vehicle Position Button")?.GetComponent<Button>();
        }

        /// <summary>
        /// Configures dropdown UI elements.
        /// </summary>
        private void InitializeDropdowns()
        {
            ActuationSourceDropdown = Pages[0]?.transform.Find("Actuation Source Dropdown")?.GetComponent<TMP_Dropdown>();
        }

        /// <summary>
        /// Configures toggle UI elements.
        /// </summary>
        private void InitializeToggle()
        {
            TractorTrailToggle = Pages[0]?.transform.Find("Tractor Trail Toggle")?.GetComponent<Toggle>();
        }

        /// <summary>
        /// Configures the toggle for pinning the dashboard.
        /// </summary>
        private void InitializeDashboardPinToggle()
        {
            PinDashboardToggle = this.gameObject.transform.Find("Toggle Pin Dashboard")?.GetComponent<Toggle>();
            
        }

        /// <summary>
        /// Attaches event listeners to UI elements.
        /// </summary>
        private void SetupEventListeners()
        {
            for (int i = 0; i < PageCount; i++)
            {
                int pageIndex = i;
                PageButtons[i]?.onClick.AddListener(() => ShowPage(pageIndex));
            }
            CloseButton?.onClick.AddListener(HideUi);
            TractorTrailToggle?.onValueChanged.AddListener(OnToggleTractorTrail);
            ActuationSourceDropdown?.onValueChanged.AddListener(OnActuationSourceChanged);
            SetPositionButton?.onClick.AddListener(SetVehiclePositionToSpace);
            PinDashboardToggle?.onValueChanged.AddListener(OnToggleDashboardPin);
        }

        /// <summary>
        /// Updates the dashboard content and UI elements.
        /// </summary>
        public void UpdateDashboard()
        {
            if (VeVehicle == null || VehicleData == null) return;

            if (FloatAboveVehicle)
            {
                UpdateDashboardPosition();
            }
            
            UpdateTopBar();
            UpdatePage1();
            UpdatePage2();
            UpdatePage3();
            UpdatePage4();
        }

        /// <summary>
        /// Updates the dashboard position to float above the vehicle's current position.
        /// </summary>
        private void UpdateDashboardPosition()
        {
            if (VeVehicle.CameraManager.ActiveCamera != null && VeVehicle.CameraManager.ActiveCamera.isActiveAndEnabled)
            {
                Vector3 screenPos = VeVehicle.CameraManager.ActiveCamera.WorldToScreenPoint(VeVehicle.AnimationController.TractorTransform.position);
                screenPos.y += OffsetY;
                UiInstance.transform.position = screenPos;
            }
        }

        /// <summary>
        /// Updates the text in the top bar with the vehicle's name.
        /// </summary>
        private void UpdateTopBar()
        {
            if (TopBarText != null)
            {
                TopBarText.text = $"{VeVehicle.Name}\n";
            }
        }

        /// <summary>
        /// Updates the first page to display the current kinematics mode of the vehicle.
        /// </summary>
        private void UpdatePage1()
        {
            KinematicsMode.text = "Kinematics Mode: " +  VeVehicle.KinematicsController.KinematicStrategy.KinematicsStrategyName;
        }

        /// <summary>
        /// Updates the second page with vehicle configuration and physical properties.
        /// </summary>
        private void UpdatePage2()
        {
            if (PageContents[1] != null)
            {
                PageContents[1].text = $"Scale: {VeVehicle.Config.Scale}\n" +
                                       $"Front Tractor Length (L1) = {VehicleData.L1} (m)\n" +
                                       $"Rear Tractor Length (l1c) = {VehicleData.L1C} (m)\n" +
                                       $"Trailer Length (L2) = {VehicleData.L2} (m)\n" +
                                       $"Tractor width (w1) = {VehicleData.TractorWidth} (m)\n" +
                                       $"Trailer width (w2) = {VehicleData.TrailerWidth} (m)\n";
            }
        }

        /// <summary>
        /// Updates the third page with the current kinematic state of the vehicle.
        /// </summary>
        private void UpdatePage3()
        {
            if (PageContents[2] != null)
            {
                PageContents[2].text = $"X1,Y1,Psi1 = {VehicleData.X1:F2} (m), {VehicleData.Y1:F2} (m), {VehicleData.Psi1:F2} (rad)\n" +
                                       $"X2,Y2,Psi2 = {VehicleData.X2:F2} (m), {VehicleData.Y2:F2} (m), {VehicleData.Psi2:F2} (rad)\n" +
                                       $"V1 = {VehicleData.V1:F2} (m/s) \n" +
                                       $"Delta = {VehicleData.Delta:F2} (rad) \n" +
                                       $"Gamma = {VehicleData.Gamma:F2} (rad) \n";

            }
        }

        /// <summary>
        /// Updates the fourth page with detected obstacles and their details.
        /// </summary>
        private void UpdatePage4()
        {
            if (PageContents[3] != null)
            {
                PageContents[3].text = "Obstacle(s) Detected:\n";
                if (VeVehicle.CollisionController.DetectedObstacles.Count == 0)
                {
                    PageContents[3].text += "None\n";
                }
                else
                {
                    foreach (var obstacle in VeVehicle.CollisionController.DetectedObstacles)
                    {
                        PageContents[3].text += $"{obstacle.Name}: {obstacle.Distance:F2}m {obstacle.Angle:F2}°\n";
                    }
                }
            }
        }

        /// <summary>
        /// Displays the selected page in the dashboard and hides the others.
        /// </summary>
        /// <param name="pageIndex">Index of the page to show.</param>
        private void ShowPage(int pageIndex)
        {
            for (int i = 0; i < PageCount; i++)
            {
                if (Pages[i] != null)
                {
                    Pages[i].SetActive(i == pageIndex);
                }
            }
        }

        /// <summary>
        /// Populates the actuation source dropdown with available strategies.
        /// </summary>
        public void PopulateInputDropdown()
        {
            if (VeVehicle.Config.IsMoCapAvailable)
            {
                // Hide the actuation dropdown when using motion capture
                if (ActuationSourceDropdown != null)
                {
                    ActuationSourceDropdown.gameObject.SetActive(false);
                    Debug.Log("MoCap mode enabled: Actuation dropdown is hidden.");
                }
            }
            else
            {
                // Show and populate the actuation dropdown if not in motion capture mode
                if (ActuationSourceDropdown != null)
                {
                    ActuationSourceDropdown.gameObject.SetActive(true);
                    ActuationSourceDropdown.ClearOptions();

                    // Check if VehicleInputStrategiesDict has keys
                    if (VeVehicle.Config.VehicleInputStrategiesDict != null && VeVehicle.Config.VehicleInputStrategiesDict.Count > 0)
                    {
                        Debug.Log("Populating VehicleInputStrategiesDict in dropdown:");
                        foreach (var strategy in VeVehicle.Config.VehicleInputStrategiesDict.Keys)
                        {
                            Debug.Log($"- {strategy}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("VehicleInputStrategiesDict is null or empty when attempting to populate dropdown.");
                    }

                    // Populate actuation sources from Config.VehicleInputStrategiesDict keys
                    List<string> actuationSources = new List<string>(VeVehicle.Config.VehicleInputStrategiesDict.Keys);
                    ActuationSourceDropdown.AddOptions(actuationSources);

                    // Set initial value based on the currently active strategy in Vehicle_Kinematics_Controller
                    string currentStrategy = VeVehicle.KinematicsController.GetCurrentInputStrategyName();
                    int initialIndex = actuationSources.IndexOf(currentStrategy);
                    ActuationSourceDropdown.value = initialIndex >= 0 ? initialIndex : 0;
                }
            }
        }

        /// <summary>
        /// Populates the space dropdown with the names of available spaces.
        /// </summary>
        private void PopulateSpaceDropdown()
        {
            if (SpaceDropdown != null)
            {
                SpaceDropdown.ClearOptions();
                List<string> spaceNames = new List<string>();

                foreach (var space in VeVehicle.SpaceCreator.LookupTable.Values)
                {
                    spaceNames.Add(space.Name);
                }
                SpaceDropdown.AddOptions(spaceNames);
            }
        }

        /// <summary>
        /// Sets the vehicle's position to the currently selected space in the dropdown.
        /// </summary>
        private void SetVehiclePositionToSpace()
        {
            if (SpaceDropdown != null && VeVehicle != null)
            {
                int selectedIndex = SpaceDropdown.value;
                string selectedSpaceName = SpaceDropdown.options[selectedIndex].text;

                foreach (var space in VeVehicle.SpaceCreator.LookupTable.Values)
                {
                    if (space.Name == selectedSpaceName)
                    {
                        VeVehicle.KinematicsController.SetVehiclePosition(
                            space.SpaceData.X1, 
                            space.SpaceData.Y1, 
                            space.SpaceData.Psi1Rad * Mathf.Rad2Deg, 
                            space.SpaceData.Psi2Rad * Mathf.Rad2Deg);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Toggles the visibility of the tractor TractorTrajectory visualization.
        /// </summary>
        /// <param name="status">Whether the tractor TractorTrajectory should be visible.</param>
        private void OnToggleTractorTrail(bool status)
        {
            if (VeVehicle?.AnimationController?.TractorTrajectory != null)
            {
                TrailRenderer trailRenderer = VeVehicle.AnimationController.TractorTrajectory.GetComponent<TrailRenderer>();
                if (trailRenderer != null)
                {
                    trailRenderer.Clear();
                }
                VeVehicle.AnimationController.TractorTrajectory.SetActive(status);
            }
        }

        /// <summary>
        /// Toggles the dashboard's pin state, determining if it floats above the vehicle.
        /// </summary>
        /// <param name="status">True if pinned, false if floating.</param>
        private void OnToggleDashboardPin(bool status)
        {
            FloatAboveVehicle = status;
        }

        /// <summary>
        /// Changes the actuation source based on the selected dropdown option.
        /// </summary>
        /// <param name="index">Index of the selected actuation source in the dropdown.</param>
        private void OnActuationSourceChanged(int index)
        {
            if (VeVehicle?.KinematicsController != null)
            {
                string selectedStrategy = ActuationSourceDropdown.options[index].text;
                VeVehicle.KinematicsController.ChangeInputStrategy(selectedStrategy);
            }
        }

        /// <summary>
        /// Cleans up event listeners and UI resources when the dashboard is destroyed.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Remove all event listeners
            for (int i = 0; i < PageCount; i++)
            {
                if (PageButtons[i] != null)
                {
                    PageButtons[i].onClick.RemoveAllListeners();
                }
            }
            if (CloseButton != null) CloseButton.onClick.RemoveAllListeners();
            if (TractorTrailToggle != null) TractorTrailToggle.onValueChanged.RemoveAllListeners();
            if (ActuationSourceDropdown != null) ActuationSourceDropdown.onValueChanged.RemoveAllListeners();
            if (SetPositionButton != null) SetPositionButton.onClick.RemoveAllListeners();
        }

    }
}
