# Building

This folder contains scripts related to the creation, configuration, and management of building entities. Buildings are static structures that serve as environments or interaction points for other entities, such as vehicles. The scripts in this folder handle the instantiation, configuration, data management, and visualization of buildings in the main scene.

## Contents

- [`Building_Config.cs`](#building_configcs)
- [`Building_Creator.cs`](#building_creatorcs)
- [`Building_Data.cs`](#building_datacs)
- [`VE_Building.cs`](#ve_buildingcs)

---

### `Building_Config.cs`

#### Overview

Defines the configuration data for a building entity as a scriptable object. Inherits from `VE_Config` and contains parameters required to instantiate and configure a building entity, including its model, position, and orientation.

#### Key Components

- **Fields:**

  - **Prototype:**
    - `GameObject BuildingPrefab`: Prefab representing the building model.

  - **Data:**
    - `Vector3 BuildingPosition`: Position of the building in the virtual environment.
    - `float BuildingOrientation`: Orientation angle (in degrees) of the building.

#### Usage

- Create instances of `Building_Config` via the Unity editor to define different building entities.
- Assign the appropriate prefab containing the model and set the desired position and orientation.
- Used by `Building_Creator` to instantiate and initialize buildings in the virtual environment.

---

### `Building_Creator.cs`

#### Overview

A factory class responsible for creating and managing building entities. Inherits from `VE_Creator<VE_Building, Building_Config>`, allowing it to handle buildings specifically. Provides methods to instantiate buildings based on configurations, register them, and handle their deletion.

#### Key Components

- **Fields:**

  - **Application Dependencies:**
    - `Vehicle_Creator VehicleCreator`: Reference to the `Vehicle_Creator`, used for managing vehicles associated with the building.
    - `VE_OnClick_Getter VeOnClickGetter`: Utility for detecting mouse click interactions.

- **Methods:**

  - `VE_Building Create_VE(Building_Config building_config)`: Creates a new building entity based on the provided configuration.
  - `void Delete_VE(int ve_id)`: Deletes an existing building entity based on its ID.

#### Implementation Details

- **Building Creation:**

  - Instantiates the building prefab defined in the `Building_Config`.
  - Adds a `VE_Building` component to the instantiated object.
  - Sets dependencies and initializes the building.
  - Registers the building in the lookup table and logs the creation event.

- **Building Deletion:**

  - Removes the building from the lookup table based on its ID.
  - Destroys the building's GameObject and associated UI instances.
  - Logs the deletion event.

#### Usage

- Used by higher-level systems or UI components to create and manage buildings.
- Listens to events and provides methods to add or remove buildings during runtime.

---

### `Building_Data.cs`

#### Overview

Holds runtime data and configuration for a building entity. Inherits from `MonoBehaviour` and is attached to the building GameObject. Manages vehicles associated with the building and provides storage for dynamic data related to the building's state.

#### Key Components

- **Fields:**

  - **Configuration:**
    - `Building_Config Config`: Configuration settings for the building.

  - **Vehicles:**
    - `Dictionary<int, VE_Vehicle> Vehicles`: A dictionary mapping vehicle IDs to vehicles currently associated with the building.

- **Methods:**

  - `void Init(Building_Config config)`: Initializes the building data with the provided configuration.

#### Implementation Details

- **Initialization:**

  - Assigns the provided configuration to `Config`.
  - Initializes the `Vehicles` dictionary to track vehicles associated with the building.

- **Functionality:**

  - Provides a central place to access and update runtime data related to the building.
  - Can be extended to include additional data as needed for building operations.

---

### `VE_Building.cs`

#### Overview

Represents a building entity. Inherits from `VE` and encapsulates all components, controllers, and data associated with a building. Handles initialization, positioning, and interaction with other entities, such as vehicles.

#### Key Components

- **Fields:**

  - **Building Data:**
    - `Building_Config Config`: Configuration data for the building.
    - `Building_Data BuildingData`: Runtime data associated with the building.

  - **Controllers:**
    - `Building_Controller BuildingController`: Manages the building's behavior and interactions.
    - `Building_Dashboard_Controller BuildingDashboardController`: UI dashboard controller for the building.

  - **Application Dependencies:**
    - `Camera_Manager CameraManager`: Manages camera operations.
    - `UI_Controller_SystemLog SystemLogUiController`: Handles logging of system events.
    - `Vehicle_Creator VehicleCreator`: Manages vehicles associated with the building.
    - `VE_OnClick_Getter VeOnClickGetter`: Utility for detecting mouse click interactions.
    - `Transform VeUiInstanceParentTransform`: Parent transform for UI objects.
    - `Transform VeInstanceParentTransform`: Parent transform for the building instance.

- **Methods:**

  - `void SetDependencies(...)`: Sets the dependencies required for the building entity.
  - `override void Init()`: Initializes the building entity, including data, controllers, and UI dashboard.
  - `void SetBuildingPosition()`: Sets the position and orientation of the building in the scene based on its configuration.

#### Implementation Details

- **Initialization:**

  - Sets up the building's name, tag, and assigns it to the appropriate parent transform.
  - Initializes `BuildingData` and `BuildingController`.
  - Instantiates and initializes the `Building_Dashboard_Controller`.
  - Positions the building in the main scene according to the configuration.

- **Controllers:**

  - **Building Controller:** Manages behavior and interactions specific to the building.
  - **Dashboard Controller:** Provides a UI interface for interacting with the building.

- **Dependencies:**

  - Dependencies such as `VehicleCreator`, `CameraManager`, and `SystemLogUiController` are injected to facilitate interaction with other parts of the application.

#### Usage

- Instantiated by the `Building_Creator` when creating a new building.
- Interacts with other systems through its controllers and dependencies.
- Provides methods for initialization to ensure proper setup.

