# Virtual Entity UI Controllers

This folder contains scripts that manage the user interface objects specific to virtual entities in the environment, such as buildings, spaces, and vehicles. These controllers handle the visibility, interaction, and behavior of dashboards associated with these entities, facilitating user interaction and providing visual feedback.

## Contents

- [`UI_Controller_BuildingDashboards.cs`](#ui_controller_buildingdashboardscs)
- [`UI_Controller_SpaceDashboards.cs`](#ui_controller_spacedashboardscs)
- [`UI_Controller_VehicleDashboards.cs`](#ui_controller_vehicledashboardscs)

---

### `UI_Controller_BuildingDashboards.cs`

#### Overview

`UI_Controller_BuildingDashboards` controls the visibility and interaction with building dashboards. It extends the functionality of `Base_UI_Controller` and provides mechanisms to detect when a building is clicked in the main scene, enabling the display or update of the corresponding dashboard.

#### Key Components

- **Fields:**

  - `VE_OnClick_Getter VeOnClickGetterGetter`: Utility for detecting clicked objects in the scene.
  - `VE_Building ClickedBuilding`: Stores the currently clicked building in the scene.

- **Methods:**

  - `void Start()`: Initializes the building dashboard UI controller.
  - `void Update()`: Handles updates each frame; placeholder for detecting building clicks.
  - `VE_Building DetectBuildingClick()`: Detects if a building object has been clicked and returns the instance.

#### Implementation Details

- **Initialization:**

  - The `Start()` method calls `Init()` to initialize the base UI controller functionalities.

- **Click Detection:**

  - `DetectBuildingClick()` checks for mouse input and uses `VeOnClickGetterGetter` to determine if a building has been clicked.
  - If a building is clicked, it retrieves the `VE_Building` component from the clicked object.

- **Update Loop:**

  - In the `Update()` method, if the dashboard is active (`IsActive`), it can detect building clicks and handle building-specific functionalities.
  - The actual click detection logic is currently commented out and can be expanded as needed.

---

### `UI_Controller_SpaceDashboards.cs`

#### Overview

`UI_Controller_SpaceDashboards` manages the visibility of space dashboards within the main scene. It extends `Base_UI_Controller` and controls the display of UI objects related to spaces, providing users with information and interaction capabilities for space entities.

#### Key Components

- **Methods:**

  - `void Start()`: Initializes the space dashboard UI controller.

#### Implementation Details

- **Initialization:**

  - The `Start()` method calls `Init()` to set up the base UI controller functionalities.

- **Functionality:**

  - While the script currently contains minimal implementation, it serves as a placeholder for future expansions related to space dashboards.
  - Additional methods and fields can be added to handle specific interactions and updates.

---

### `UI_Controller_VehicleDashboards.cs`

#### Overview

`UI_Controller_VehicleDashboards` controls the visibility and interaction with vehicle dashboards. It extends `Base_UI_Controller` and provides mechanisms to detect when a vehicle is clicked within the main scene, enabling the display or update of the corresponding dashboard.

#### Key Components

- **Fields:**

  - `VE_OnClick_Getter VeOnClickGetterGetter`: Utility for detecting clicked objects in the scene.
  - `VE_Vehicle ClickedVehicle`: Stores the currently clicked vehicle in the scene.

- **Methods:**

  - `void Start()`: Initializes the vehicle dashboard UI controller.
  - `void Update()`: Handles updates each frame; detects clicks on vehicles and shows their dashboards.
  - `VE_Vehicle DetectVehicleClick()`: Detects if a vehicle object has been clicked and returns the instance.

#### Implementation Details

- **Initialization:**

  - The `Start()` method calls `Init()` to initialize the base UI controller functionalities.

- **Click Detection:**

  - `DetectVehicleClick()` checks for mouse input and uses `VeOnClickGetterGetter` to determine if a vehicle has been clicked.
  - It checks if the clicked object is tagged as "Tractor" or "Trailer" and retrieves the `VE_Vehicle` component.

- **Update Loop:**

  - In the `Update()` method, if the dashboard is active (`IsActive`), it detects vehicle clicks.
  - If a vehicle is clicked, it calls `ShowUi()` on the vehicle's dashboard controller to display the dashboard.

---

## Notes

- These controllers rely on the `VE_OnClick_Getter` utility to detect user interactions with virtual entities in the scene.
- They extend `Base_UI_Controller`, inheriting common UI management functionalities such as showing, hiding, and toggling UI objects.
- The scripts can be expanded to include additional functionalities as needed.
