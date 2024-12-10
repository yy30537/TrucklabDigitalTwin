# Building Controllers

This folder contains scripts that manage the behavior and user interface of building entities. Buildings represent static structures, such as distribution centers or warehouses, within the virtual environment. The controllers in this folder handle interactions related to buildings and provide a user interface for displaying building information.

## Contents

- [`Building_Controller.cs`](#building_controllercs)
- [`Building_Dashboard_Controller.cs`](#building_dashboard_controllercs)

---

### `Building_Controller.cs`

#### Overview

`Building_Controller` manages the behavior and interactions of a building entity. It provides a centralized place for handling logic related to building entities, including initializing building data and managing vehicles associated with the building.

#### Key Components

- **Fields:**

  - `Building_Data BuildingData`: Holds runtime data associated with the building, including its state and configuration.

  - `Vehicle_Creator VehicleCreator`: Reference to the `Vehicle_Creator`, used for managing vehicles related to the building.

- **Methods:**

  - `void Init(Building_Data data, Vehicle_Creator creator)`: Initializes the building controller with the provided building data and vehicle creator.

#### Implementation Details

- **Initialization:**

  - The `Init` method sets up the building controller by assigning the `BuildingData` and `VehicleCreator`.

- **Functionality:**

  - Currently, the `Building_Controller` serves as a placeholder for future expansions and can be extended to include methods for managing building-specific interactions and behaviors.

---

### `Building_Dashboard_Controller.cs`

#### Overview

`Building_Dashboard_Controller` is a UI component that displays and manages information related to a building. It provides a user interface for interacting with building entities and is designed to be extended with additional UI objects as needed.

#### Key Components

- **Fields:**

  - `Transform VeUiInstanceParentTransform`: Parent transform under which UI objects for the building will be instantiated.

  - `Building_Config BuildingConfig`: Configuration data for the building, including UI and behavior settings.

  - `EventChannel_UI_Navigation UiNavigationEc`: Event channel for managing UI navigation events specific to this building.

- **Methods:**

  - `void SetDependencies(...)`: Sets the required dependencies for the building's dashboard, including configuration, UI instance, parent transform, and system log controller.

  - `override void Init()`: Initializes the building's dashboard by activating it and logging its initialization.

  - `void Update()`: Unity's update method, currently used as a placeholder for potential dynamic updates to the dashboard.

  - `void UpdateDashboard()`: Updates the dashboard's UI to reflect the current state of the building.

  - `bool DetectBuildingClick()`: Detects whether the building was clicked by the user (placeholder implementation).

#### Implementation Details

- **User Interface:**

  - The dashboard is designed to display information about the building and can be expanded to include interactive objects for managing building-related tasks.

- **Dependency Management:**

  - Dependencies are injected via the `SetDependencies` method, allowing for modularity and easier maintenance.

- **Event Handling:**

  - Utilizes an `EventChannel_UI_Navigation` for handling UI navigation events specific to the building.

- **Future Development:**

  - Comments within the script indicate plans to implement additional UI objects for scheduling and path planning services once backend support is available.

