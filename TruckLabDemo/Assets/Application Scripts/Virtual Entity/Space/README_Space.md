# Space

This folder contains scripts related to the creation, configuration, and management of space entities within the simulation. Spaces are defined areas within the simulation environment that can interact with vehicles, detect their presence, and provide default positions for vehicle placement. The scripts in this folder handle initialization, visualization, data management, and integration with other systems.

## Contents

- [`Space_Config.cs`](#space_configcs)
- [`Space_Creator.cs`](#space_creatorcs)
- [`Space_Data.cs`](#space_datacs)
- [`VE_Space.cs`](#ve_spacecs)

---

### `Space_Config.cs`

#### Overview

Defines the configuration data for a space entity as a scriptable object. Inherits from `VE_Config` and contains all the necessary parameters to instantiate and configure a space within the simulation. This includes space geometry, materials, default vehicle positions, and label positions.

#### Key Components

- **Fields:**

  - **Vertices (Calibration Data):**
    - `Vector3[] SpaceMarkings`: The vertices defining the area of the space.

  - **Ground Materials:**
    - `Material GroundMaterial`: Material used for the space when it is inactive or vacant.
    - `Material GroundMaterialActive`: Material used for the space when it is active or occupied.

  - **Default Vehicle Position:**
    - `float X1`: Default X-coordinate for vehicle placement.
    - `float Y1`: Default Y-coordinate for vehicle placement.
    - `float Psi1Rad`: Default orientation angle (in radians) for the tractor.
    - `float Psi2Rad`: Default orientation angle (in radians) for the trailer.

  - **Default Space Label Position:**
    - `float LabelX`: X-coordinate for the space label.
    - `float LabelY`: Y-coordinate for the space label.
    - `float LabelPsi`: Orientation angle for the space label.

#### Usage

- Create instances of `Space_Config` via the Unity editor to define different spaces within the simulation.
- Assign appropriate vertices, materials, and default positions based on the simulation requirements.
- Used by `Space_Creator` to instantiate and initialize spaces in the simulation.

---

### `Space_Creator.cs`

#### Overview

A factory class responsible for creating and managing space entities within the simulation. Inherits from `VE_Creator<VE_Space, Space_Config>`, allowing it to handle spaces specifically. Provides methods to instantiate spaces based on configurations, register them, and handle their deletion.

#### Key Components

- **Fields:**

  - `GameObject SiteLayout`: Reference to the site layout GameObject for visualizing the region.

  - **Application Dependencies:**
    - `Vehicle_Creator VehicleCreator`: Reference to the `Vehicle_Creator`, used for managing vehicles within the space.
    - `VE_OnClick_Getter VeOnClickGetter`: Utility for detecting mouse click interactions.

- **Methods:**

  - `VE_Space Create_VE(Space_Config space_config)`: Creates a new space entity based on the provided configuration.
  - `void Delete_VE(int ve_id)`: Deletes an existing space entity based on its ID.
  - `void SetLayout(bool status)`: Enables or disables the site layout visualization.

#### Implementation Details

- **Space Creation:**

  - Instantiates a new GameObject for the space.
  - Adds a `VE_Space` component to the GameObject.
  - Sets dependencies and initializes the space.
  - Registers the space in the lookup table and logs the creation event.

- **Space Deletion:**

  - Removes the space from the lookup table based on its ID.
  - Destroys the space's GameObject and associated UI instances.
  - Logs the deletion event.

---

### `Space_Data.cs`

#### Overview

Holds runtime data and configuration for a space entity. Inherits from `MonoBehaviour` and is attached to the space GameObject. Manages vehicles present within the space and provides default positions for vehicle placement.

#### Key Components

- **Fields:**

  - **Configuration:**
    - `Space_Config Config`: Configuration settings for the space entity.

  - **Vehicles in Space:**
    - `Dictionary<int, VE_Vehicle> VehiclesPresentInSpace`: A dictionary mapping vehicle IDs to the vehicles currently present in the space.

  - **Default Vehicle Position:**
    - `float X1`: Default X-coordinate for vehicle placement.
    - `float Y1`: Default Y-coordinate for vehicle placement.
    - `float Psi1Rad`: Default orientation angle (in radians) for the tractor.
    - `float Psi2Rad`: Default orientation angle (in radians) for the trailer.

- **Methods:**

  - `void Init(Space_Config config)`: Initializes the space data using the provided configuration.

#### Implementation Details

- **Initialization:**

  - Assigns the provided configuration to `Config`.
  - Sets default positions and orientations based on the configuration.
  - Initializes the `VehiclesPresentInSpace` dictionary.

---

### `VE_Space.cs`

#### Overview

Represents a space entity within the simulation. Inherits from `VE` and encapsulates all components, controllers, and data associated with a space. Handles initialization, visualization, and interaction with vehicles.

#### Key Components

- **Fields:**

  - **Space Data:**
    - `Space_Config Config`: Configuration for the space entity.
    - `Space_Data SpaceData`: Runtime data associated with the space.

  - **Visualization:**
    - `MeshRenderer SpaceMesh`: Mesh renderer for visualizing the space.
    - `GameObject spaceLabel`: Label GameObject used to display the space name.

  - **Controllers:**
    - `Space_Controller SpaceController`: Manages the space's behavior and interactions.
    - `Space_Dashboard_Controller SpaceDashboardController`: UI dashboard controller for the space.

  - **Application Dependencies:**
    - `Camera_Manager CameraManager`: For handling camera-related operations.
    - `UI_Controller_SystemLog SystemLogUiController`: Controller for logging system events.
    - `VE_OnClick_Getter VeOnClickGetter`: Utility for detecting mouse click interactions.
    - `Vehicle_Creator VehicleCreator`: Reference to the `Vehicle_Creator`, used for managing vehicles within the space.
    - `Transform VeUiInstanceParentTransform`: Parent transform for UI elements.
    - `Transform VeInstanceParentTransform`: Parent transform for the space instance.

- **Methods:**

  - `void SetDependencies(...)`: Sets the dependencies required by the space.
  - `override void Init()`: Initializes the space and its components.
  - `void InitializeCommonProperties()`: Initializes common properties like ID, name, and parent transforms.
  - `void InitializeSpaceData()`: Initializes the space data and links it to the configuration.
  - `void InitializeControllers()`: Initializes the space controllers.
  - `void InitializeDashboard()`: Initializes the UI dashboard for the space.
  - `void CreateMesh()`: Creates the mesh for the space using its configured space markings.
  - `void CreateSpaceLabel()`: Creates a label above the space to display its name.
  - `Vector3 CalculateCenterPoint(Vector3[] vertices)`: Calculates the geometric center point of a set of vertices.

#### Implementation Details

- **Initialization:**

  - Sets up the space's name, tag, and parent transforms.
  - Initializes data and controllers necessary for the space's operation.
  - Creates the mesh and label for visual representation.

- **Mesh Creation:**

  - Calculates the center point of the space markings to position the instance.
  - Adjusts vertices relative to the center point.
  - Generates a mesh using the offset vertices and assigns it to the `MeshFilter`.
  - Sets the default material for the space mesh.

- **Label Creation:**

  - Creates a quad as the label background.
  - Positions and rotates the label according to the configuration.
  - Adds a `TextMeshPro` component to display the space name.
  - Disables collisions for the label.

- **Runtime Updates:**

  - In the `FixedUpdate` method, updates the material of the space's mesh based on the presence of vehicles in the space.

