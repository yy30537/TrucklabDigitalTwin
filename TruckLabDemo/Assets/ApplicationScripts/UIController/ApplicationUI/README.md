# Application UI Controllers

This folder contains scripts that manage the application user interface objects in the main scene. These controllers handle the interaction between the user and the application, providing functionalities such as system navigation, path simulation, service management, system events logging, and virtual entity management. They facilitate the user's ability to control and monitor the virtual environment.

## Contents

- [`UI\_Controller\_MainMenu.cs`](#ui\_controller\_mainmenucs)
- [`UI\_Controller\_PathSimulation.cs`](#ui\_controller\_pathsimulationcs)
- [`UI\_Controller\_ServiceMenu.cs`](#ui\_controller\_servicemenucs)
- [`UI\_Controller\_SystemLog.cs`](#ui\_controller\_systemlogcs)
- [`UI\_Controller\_VE\_Creator.cs`](#ui\_controller\_ve\_creatorcs)

---

### `UI\_Controller\_MainMenu.cs`

#### Overview

`UI\_Controller\_MainMenu` manages the main menu UI and its interaction with submenus, toggles, and application dependencies. It handles the toggling of all dashboard panels and updates their states, allowing the user to control the visibility and functionality of different components within the main scene.

#### Key Components

- **Fields:**

  - **Children:**
    - `Camera\_Manager CameraManager`: Reference to the camera manager for controlling camera settings.
    - `Vehicle\_Dragger VehicleDragger`: Reference to the vehicle dragger for enabling drag-and-drop functionality. 

  - **Sub Menu UI Controllers:**
    - `UI\_Controller\_ServiceMenu UiControllerServiceMenu`
    - `UI\_Controller\_SystemLog UiControllerSystemLog`
    - `UI\_Controller\_SpaceDashboards UiControllerSpaceDashboards`
    - `UI\_Controller\_BuildingDashboards UiControllerBuildingDashboards`
    - `UI\_Controller\_VehicleDashboards UiControllerVehicleDashboards`

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
  - `void SetToggleColor(Toggle toggle, Image toggleImage, bool is\_visible)`: Updates the color of a toggle based on its active state.
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

### `UI\_Controller\_PathSimulation.cs`

#### Overview

`UI\_Controller\_PathSimulation` manages the simulation menu for controlling and visualizing vehicle paths. It provides functionality to select, preview, and replay paths for vehicles within the virtual environment, allowing users to control path simulations and monitor vehicle movements along predefined or recorded paths.

#### Key Components

- **Fields:**

  - **Children:**
    - `Vehicle\_Creator VehicleCreator`: Reference to access created vehicles.

  - **UI Objects:**
    - `TMP\_Dropdown CreatedVehicleDropdown`: Dropdown for selecting a created vehicle.
    - `TMP\_Dropdown PathDropdown`: Dropdown for selecting a path.
    - `TextMeshProUGUI StatusText`: Displays the simulation status.
    - `TextMeshProUGUI TimeRemainingText`: Displays the remaining time for the simulation.
    - `TextMeshProUGUI StartPositionText`: Displays the start position of the selected path.
    - `TextMeshProUGUI EndPositionText`: Displays the end position of the selected path.
    - `TextMeshProUGUI InputVelocityText`: Displays the input velocity.
    - `TextMeshProUGUI InputSteeringText`: Displays the input steering angle.

  - **Path Simulation:**
    - `Path\_Manager PathManager`: Manages available paths.
    - `Path\_Previewer PathPreviewer`: Visualizes paths.
    - `Path SelectedPath`: Currently selected path for simulation.
    - `VE\_Vehicle SelectedVeVehicle`: Currently selected vehicle for path simulation.

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

  - Dynamically updates UI objects to reflect the current status of path simulations, such as time remaining and vehicle positions.

- **Coroutines:**

  - Uses `IEnumerator` and `StartCoroutine` to update simulation UI over time during path replaying.

---

### `UI\_Controller\_ServiceMenu.cs`

#### Overview

`UI\_Controller\_ServiceMenu` manages the service menu UI, including visibility toggling and interaction with dependent child controllers for vehicle creation and path simulation, and more services to be integrated will be managed here. It ensures that only one service UI is active at a time.

#### Key Components

- **Fields:**

  - **Service Managers:**
    - `UI\_Controller\_VE\_Creator UiControllerVeCreator`: Controller for vehicle creation UI.
    - `UI\_Controller\_PathSimulation UiControllerPathSimulation`: Controller for path simulation UI.

- **Methods:**

  - `void Start()`: Initializes the service menu controller.
  - `override void Toggle\_UI\_Visibility(string target\_ui\_name)`: Toggles visibility of the service menu based on the target UI name.

#### Implementation Details

- **Visibility Management:**

  - Overrides `Toggle\_UI\_Visibility` to ensure that when one child UI is activated, the others are hidden, preventing multiple service UIs from being active simultaneously.

- **Dependency Handling:**

  - Manages dependencies between child controllers, coordinating their visibility and interaction states.

---

### `UI\_Controller\_SystemLog.cs`

#### Overview

`UI\_Controller\_SystemLog` manages the system log window, including event logging and displaying the system clock. It provides real-time logging of events and displays time.

#### Key Components

- **Fields:**

  - **UI Objects:**
    - `TextMeshProUGUI LogText`: UI text object for displaying the system log.
    - `TextMeshProUGUI ClockText`: UI text object for displaying the system clock.

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

  - Uses a `StringBuilder` to efficiently manage log messages and updates the `LogText` UI object.

- **Time Management:**

  - Calculates elapsed time since system start to provide timestamps for log entries and update the system clock display.

- **UI Updates:**

  - Ensures UI objects are updated appropriately and handles potential null references to avoid runtime errors.

---

### `UI\_Controller\_VE\_Creator.cs`

#### Overview

`UI\_Controller\_VE\_Creator` manages the creator menu for creating, deleting, and displaying information about vehicles, spaces, and buildings. It handles dropdown population, prefab preview rendering, and event-driven creation and deletion operations, providing users with a comprehensive interface to manage virtual entities.

#### Key Components

- **Fields:**

  - **UI Objects:**
    - `TextMeshProUGUI MenuText`: Displays detailed information about the selected entity.
    - `TMP\_Dropdown VehicleDropdown`: Dropdown for selecting vehicles.
    - `TMP\_Dropdown SpaceDropdown`: Dropdown for selecting spaces.
    - `TMP\_Dropdown BuildingDropdown`: Dropdown for selecting buildings.
    - `RawImage PrefabDisplayImage`: Image for displaying a rendered preview of the selected prefab.
    - `Camera PrefabDisplayCamera`: Camera used for rendering the prefab preview.
    - `RenderTexture PrefabRenderTexture`: Render texture used by the prefab display camera.
    - `GameObject CurrentPreviewInstance`: Currently displayed prefab instance in the preview.

  - **Application Dependencies:**
    - `Camera\_Manager CameraManager`: Manages camera operations.

  - **VE Creators:**
    - `Vehicle\_Creator VehicleCreator`
    - `Space\_Creator SpaceCreator`
    - `Building\_Creator BuildingCreator`

  - **VE Configurations:**
    - `List<Vehicle\_Config> VehicleConfigs`
    - `List<Space\_Config> SpaceConfigs`
    - `List<Building\_Config> BuildingConfigs`

  - **Event Channels:**
    - `EventChannel\_Void ConfirmVehicleCreationEvent`
    - `EventChannel\_Void DeleteVehicleEvent`
    - `EventChannel\_Void ConfirmAllVehicleCreationEvent`
    - `EventChannel\_Void DeleteAllVehicleEvent`
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
