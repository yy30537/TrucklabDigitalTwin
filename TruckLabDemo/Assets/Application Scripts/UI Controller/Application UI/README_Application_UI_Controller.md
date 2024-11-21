# Application UI Controllers

This folder contains scripts that manage the main user interface components of the simulation application. These controllers handle the interaction between the user and the application, providing functionalities such as menu navigation, path simulation, service management, system logging, and virtual entity creation. They facilitate the user's ability to control and monitor various aspects of the simulation environment.

## Contents

- [`UI_Controller_MainMenu.cs`](#ui_controller_mainmenucs)
- [`UI_Controller_PathSimulation.cs`](#ui_controller_pathsimulationcs)
- [`UI_Controller_ServiceMenu.cs`](#ui_controller_servicemenucs)
- [`UI_Controller_SystemLog.cs`](#ui_controller_systemlogcs)
- [`UI_Controller_VE_Creator.cs`](#ui_controller_ve_creatorcs)

---

### `UI_Controller_MainMenu.cs`

#### Overview

`UI_Controller_MainMenu` manages the main menu UI and its interaction with submenus, toggles, and application dependencies. It handles the toggling of various dashboard panels and updates their states, allowing the user to control the visibility and functionality of different components within the simulation.

#### Key Components

- **Fields:**

  - **Children:**
    - `Camera_Manager CameraManager`: Reference to the camera manager for controlling camera settings.
    - `Vehicle_Dragger VehicleDragger`: Reference to the vehicle dragger for enabling drag-and-drop functionality.

  - **Sub Menu UI Controllers:**
    - `UI_Controller_ServiceMenu UiControllerServiceMenu`
    - `UI_Controller_SystemLog UiControllerSystemLog`
    - `UI_Controller_SpaceDashboards UiControllerSpaceDashboards`
    - `UI_Controller_BuildingDashboards UiControllerBuildingDashboards`
    - `UI_Controller_VehicleDashboards UiControllerVehicleDashboards`

  - **UI Objects:**
    - `Toggle ServiceMenuToggle`
    - `Toggle SystemLogToggle`
    - `Toggle SpaceDashboardPanelToggle`
    - `Toggle BuildingDashboardPanelToggle`
    - `Toggle VehicleDashboardPanelToggle`
    - `Toggle FreeLookMainMenuToggle`
    - `Toggle DragVehicleToggle`

  - **Toggle Images:**
    - `Image ServiceMenuToggleImage`
    - `Image SystemLogToggleImage`
    - `Image SpaceDashboardPanelToggleImage`
    - `Image BuildingDashboardPanelToggleImage`
    - `Image VehicleDashboardPanelToggleImage`
    - `Image FreeLookMainMenuToggleImage`
    - `Image DragVehicleMainMenuImage`

  - **Colors:**
    - `Color green`: Color used to indicate an active state.
    - `Color red`: Color used to indicate an inactive state.

- **Methods:**

  - `void Start()`: Initializes the main menu controller and sets up colors for toggle states.
  - `void SetToggleColor(Toggle toggle, Image toggleImage, bool is_visible)`: Updates the color of a toggle based on its active state.
  - `void OnServiceMenuToggle()`: Updates the toggle state and color for the service menu.
  - `void OnSystemLogToggle()`: Updates the toggle state and color for the system log.
  - `void OnSpaceDashboardToggle()`: Updates the toggle state and color for the space dashboards.
  - `void OnBuildingDashboardToggle()`: Updates the toggle state and color for the building dashboards.
  - `void OnVehicleDashboardToggle()`: Updates the toggle state and color for the vehicle dashboards.
  - `void OnFreeLookToggle()`: Updates the toggle state and color for the free look camera control.
  - `void OnDragVehicleToggle()`: Updates the toggle state and color for the drag vehicle functionality.

#### Implementation Details

- **Initialization:**

  - In the `Start()` method, the controller initializes itself and sets up the color coding for the toggles, parsing HTML color strings to define `green` and `red` colors.

- **Toggle Management:**

  - Each `On...Toggle()` method is responsible for updating the visual state of a corresponding toggle and reflecting changes in the UI based on user interaction.

- **Color Coding:**

  - Toggles use color coding to indicate their active or inactive state, enhancing user experience by providing immediate visual feedback.

---

### `UI_Controller_PathSimulation.cs`

#### Overview

`UI_Controller_PathSimulation` manages the simulation menu for controlling and visualizing vehicle paths. It provides functionality to select, preview, and replay paths for vehicles within the simulation, allowing users to control path simulations and monitor vehicle movements along predefined or recorded paths.

#### Key Components

- **Fields:**

  - **Children:**
    - `Vehicle_Creator VehicleCreator`: Reference to access created vehicles.

  - **UI Objects:**
    - `TMP_Dropdown CreatedVehicleDropdown`: Dropdown for selecting a created vehicle.
    - `TMP_Dropdown PathDropdown`: Dropdown for selecting a path.
    - `TextMeshProUGUI StatusText`: Displays the simulation status.
    - `TextMeshProUGUI TimeRemainingText`: Displays the remaining time for the simulation.
    - `TextMeshProUGUI StartPositionText`: Displays the start position of the selected path.
    - `TextMeshProUGUI EndPositionText`: Displays the end position of the selected path.
    - `TextMeshProUGUI InputVelocityText`: Displays the input velocity.
    - `TextMeshProUGUI InputSteeringText`: Displays the input steering angle.

  - **Path Simulation:**
    - `Path_Manager PathManager`: Manages available paths.
    - `Path_Previewer PathPreviewer`: Visualizes paths.
    - `Path SelectedPath`: Currently selected path for simulation.
    - `VE_Vehicle SelectedVeVehicle`: Currently selected vehicle for path simulation.

- **Methods:**

  - `void Start()`: Initializes the path simulation controller.
  - `void PopulateCreatedVehicleDropdown()`: Populates the vehicle dropdown with created vehicles.
  - `void PopulatePathDropdown()`: Populates the path dropdown with available paths.
  - `void OnVehicleSelectedFromDropdown()`: Updates the selected vehicle based on dropdown selection.
  - `void OnPathSelectedFromDropdown()`: Updates the selected path based on dropdown selection.
  - `void OnSelectPath()`: Prepares the selected path for simulation.
  - `void OnStartPathReplaying()`: Starts path replaying for the selected vehicle and path.
  - `void OnStopPathReplaying()`: Stops path replaying for the selected vehicle.
  - `void OnStartPathRecording()`: Starts recording a new path for the selected vehicle.
  - `void OnStopPathRecording()`: Stops the current path recording and updates the path list.
  - `void UpdateUiWithPathInfo()`: Updates the UI with information about the selected path.
  - `IEnumerator UpdateSimulationUi()`: Dynamically updates the simulation UI during path replay.

#### Implementation Details

- **Path Management:**

  - Provides methods to select and manage paths, including starting and stopping path replaying and recording.

- **UI Updates:**

  - Dynamically updates UI elements to reflect the current status of path simulations, such as time remaining and vehicle positions.

- **Coroutines:**

  - Uses `IEnumerator` and `StartCoroutine` to update simulation UI over time during path replaying.

---

### `UI_Controller_ServiceMenu.cs`

#### Overview

`UI_Controller_ServiceMenu` manages the service menu UI, including visibility toggling and interaction with dependent child controllers for vehicle creation and path simulation. It ensures that only one service UI is active at a time to avoid conflicts and enhance user experience.

#### Key Components

- **Fields:**

  - **Service Managers:**
    - `UI_Controller_VE_Creator UiControllerVeCreator`: Controller for vehicle creation UI.
    - `UI_Controller_PathSimulation UiControllerPathSimulation`: Controller for path simulation UI.

- **Methods:**

  - `void Start()`: Initializes the service menu controller.
  - `override void Toggle_UI_Visibility(string target_ui_name)`: Toggles visibility of the service menu based on the target UI name.

#### Implementation Details

- **Visibility Management:**

  - Overrides `Toggle_UI_Visibility` to ensure that when one child UI is activated, the others are hidden, preventing multiple service UIs from being active simultaneously.

- **Dependency Handling:**

  - Manages dependencies between child controllers, coordinating their visibility and interaction states.

---

### `UI_Controller_SystemLog.cs`

#### Overview

`UI_Controller_SystemLog` manages the system log window, including event logging and displaying the system clock. It provides real-time logging of events and a visual representation of the simulation time, aiding in monitoring and debugging.

#### Key Components

- **Fields:**

  - **UI Objects:**
    - `TextMeshProUGUI LogText`: UI text element for displaying the system log.
    - `TextMeshProUGUI ClockText`: UI text element for displaying the system clock.

  - **Logging:**
    - `StringBuilder logBuilder`: Stores log messages.
    - `float systemStartTime`: Stores the start time of the system for timestamping logs.

- **Methods:**

  - `void Start()`: Initializes the system log controller and sets the initial system time.
  - `void FixedUpdate()`: Updates the system clock at fixed intervals.
  - `void LogEvent(string eventMessage)`: Logs an event to the system log.
  - `void UpdateSystemClock()`: Updates the system clock display.

#### Implementation Details

- **Event Logging:**

  - Uses a `StringBuilder` to efficiently manage log messages and updates the `LogText` UI element.

- **Time Management:**

  - Calculates elapsed time since system start to provide timestamps for log entries and update the system clock display.

- **UI Updates:**

  - Ensures UI elements are updated appropriately and handles potential null references to avoid runtime errors.

---

### `UI_Controller_VE_Creator.cs`

#### Overview

`UI_Controller_VE_Creator` manages the creator menu for creating, deleting, and displaying information about vehicles, spaces, and buildings. It handles dropdown population, prefab preview rendering, and event-driven creation and deletion operations, providing users with a comprehensive interface to manage virtual entities within the simulation.

#### Key Components

- **Fields:**

  - **UI Objects:**
    - `TextMeshProUGUI MenuText`: Displays detailed information about the selected entity.
    - `TMP_Dropdown VehicleDropdown`: Dropdown for selecting vehicles.
    - `TMP_Dropdown SpaceDropdown`: Dropdown for selecting spaces.
    - `TMP_Dropdown BuildingDropdown`: Dropdown for selecting buildings.
    - `RawImage PrefabDisplayImage`: Image for displaying a rendered preview of the selected prefab.
    - `Camera PrefabDisplayCamera`: Camera used for rendering the prefab preview.
    - `RenderTexture PrefabRenderTexture`: Render texture used by the prefab display camera.
    - `GameObject CurrentPreviewInstance`: Currently displayed prefab instance in the preview.

  - **Application Dependencies:**
    - `Camera_Manager CameraManager`: Manages camera operations.

  - **VE Creators:**
    - `Vehicle_Creator VehicleCreator`
    - `Space_Creator SpaceCreator`
    - `Building_Creator BuildingCreator`

  - **VE Configurations:**
    - `List<Vehicle_Config> VehicleConfigs`
    - `List<Space_Config> SpaceConfigs`
    - `List<Building_Config> BuildingConfigs`

  - **Event Channels:**
    - `EventChannel_Void ConfirmVehicleCreationEvent`
    - `EventChannel_Void DeleteVehicleEvent`
    - `EventChannel_Void ConfirmAllVehicleCreationEvent`
    - `EventChannel_Void DeleteAllVehicleEvent`
    - (Similar events for spaces and buildings)

- **Methods:**

  - `void Start()`: Initializes the menu and subscribes to event channels.
  - `void Update()`: Rotates the currently displayed prefab instance.
  - `void OnUiNavigation(string menu)`: Handles UI navigation events and refreshes dropdown menus.
  - `void PopulateVehicleDropdown()`: Populates the vehicle dropdown with available configurations.
  - `void PopulateSpaceDropdown()`: Populates the space dropdown with available configurations.
  - `void PopulateBuildingDropdown()`: Populates the building dropdown with available configurations.
  - `void OnCreateAllVehicles()`: Creates all vehicles defined in the configuration list.
  - `void OnCreateAllSpaces()`: Creates all spaces defined in the configuration list.
  - `void OnCreateAllBuildings()`: Creates all buildings defined in the configuration list.
  - `void OnDeleteAllVehicles()`: Deletes all vehicles currently present.
  - `void OnDeleteAllSpaces()`: Deletes all spaces currently present.
  - `void OnDeleteAllBuildings()`: Deletes all buildings currently present.
  - `void OnCreateVehicle()`: Creates a vehicle based on the selected configuration.
  - `void OnDeleteVehicle()`: Deletes a vehicle based on the selected item.
  - `void OnCreateSpace()`: Creates a space based on the selected configuration.
  - `void OnDeleteSpace()`: Deletes a space based on the selected item.
  - `void OnCreateBuilding()`: Creates a building based on the selected configuration.
  - `void OnDeleteBuilding()`: Deletes a building based on the selected item.
  - `void OnVehicleSelectedFromDropdown()`: Handles vehicle selection and updates the UI.
  - `void OnSpaceSelectedFromDropdown()`: Handles space selection and updates the UI.
  - `void OnBuildingSelectedFromDropdown()`: Handles building selection and updates the UI.
  - `void SetLayerRecursive(GameObject obj, int newLayer)`: Recursively sets the layer of a GameObject and its children.
  - `void DeletePreviewPrefabInstance()`: Deletes the currently displayed preview prefab instance.
  - `protected override void OnDestroy()`: Cleans up event subscri
