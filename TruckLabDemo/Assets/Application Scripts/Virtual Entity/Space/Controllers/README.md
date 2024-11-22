# Space Controllers

This folder contains scripts that manage the behavior and user interface of space entities within the simulation. Spaces are defined areas within the simulation environment that can detect and interact with vehicles. The controllers in this folder handle vehicle detection within spaces and provide a user interface for displaying space information.

## Contents

- [`Space_Controller.cs`](#space_controllercs)
- [`Space_Dashboard_Controller.cs`](#space_dashboard_controllercs)

---

### `Space_Controller.cs`

#### Overview

`Space_Controller` manages the behavior and interactions of a space within the simulation, including the detection of vehicles present within the space. It periodically checks for vehicles whose bounding boxes intersect with the space's defined area and updates the space data accordingly.

#### Key Components

- **Fields:**

  - `Space_Data SpaceData`: Holds data associated with the space, including configuration and the list of vehicles currently present within it.

  - `Vehicle_Creator VehicleCreator`: Reference to the `Vehicle_Creator`, used to access and manage vehicles within the simulation.

- **Methods:**

  - `void Init(Space_Data data, Vehicle_Creator creator)`: Initializes the space controller with the provided space data and vehicle creator.

  - `void FixedUpdate()`: Unity's physics update method, called at fixed intervals. It invokes `DetectVehiclesInSpace()` to check for vehicle presence.

  - `void DetectVehiclesInSpace()`: Detects vehicles present within the space based on their bounding boxes and updates the `VehiclesPresentInSpace` dictionary in `SpaceData`.

  - `bool IsBoundingBoxInSpace(Vector3[] boundingBox, Vector3[] polygon)`: Determines whether any point of a bounding box is inside the defined space polygon.

  - `bool IsPointInPolygon(Vector3 point, Vector3[] polygon)`: Checks if a given point is inside a polygon using the ray casting algorithm.

  - `bool IsVehicleInSpace(int id)`: Checks if a vehicle with the specified ID is currently present in the space.

#### Implementation Details

- **Vehicle Detection:**

  - The controller retrieves bounding boxes for both the tractor and trailer of each vehicle using `GetTractorBoundingBox()` and `GetTrailerBoundingBox()` methods from `Vehicle_Data`.

  - It checks whether any corner of these bounding boxes is inside the space's polygonal area defined in `Space_Config.SpaceMarkings`.

  - Vehicles detected within the space are added to `SpaceData.VehiclesPresentInSpace`, and removed when they exit.

- **Spatial Calculations:**

  - Uses a point-in-polygon algorithm (`IsPointInPolygon`) to determine if a point lies within the space's area.

  - The algorithm accounts for complex polygon shapes and is executed for each corner point of a vehicle's bounding box.

---

### `Space_Dashboard_Controller.cs`

#### Overview

`Space_Dashboard_Controller` is a UI component that displays and manages information about a space within the simulation. It provides real-time updates on the occupancy status of the space, indicating whether it is vacant or occupied, and lists the names of vehicles present.

#### Key Components

- **Fields:**

  - `TextMeshProUGUI Title`: UI element displaying the title of the space dashboard.

  - `TextMeshProUGUI Content`: UI element displaying information about the vehicles in the space.

  - `Transform VeUiInstanceParentTransform`: Parent transform for UI elements associated with the space.

  - `VE_Space VeSpace`: Reference to the `VE_Space` entity that this dashboard represents.

  - `Space_Config SpaceConfig`: Configuration settings for the space.

  - `Color green`: Color used to indicate a vacant space.

  - `Color yellow`: Color used to indicate an occupied space.

- **Methods:**

  - `void SetDependencies(...)`: Sets the required dependencies for the dashboard, including references to the space entity, configuration, UI instance, and system log controller.

  - `override void Init()`: Initializes the dashboard by setting up UI components, loading colors, and activating the UI instance.

  - `void Update()`: Unity's update method, called once per frame. It checks if the dashboard is active and calls `UpdateDashboard()`.

  - `void UpdateDashboard()`: Updates the dashboard UI with the current state of the space and vehicles.

  - `string GetVehicleListText()`: Generates text representing the list of vehicles currently in the space.

#### Implementation Details

- **UI Updates:**

  - The dashboard displays the name of the space and its occupancy status.

  - Uses different colors to represent vacant (green) and occupied (yellow) states.

  - When occupied, it lists the names of all vehicles present in the space.

- **Dependency Management:**

  - The dashboard relies on `VE_Space`, `Space_Config`, and `UI_Controller_SystemLog` for data and logging.

  - Dependencies are injected via `SetDependencies()` to promote modularity.

- **Event Handling:**

  - The dashboard updates in real-time by overriding the `Update()` method and checking the occupancy status each frame.

- **Error Handling:**

  - Logs errors using `SystemLogUiController` if necessary UI components are missing or if the `VeSpace` reference is null.

