# Virtual Entity

This folder contains the base classes and utilities for creating and managing Virtual Entities (VEs) in the application space. Virtual Entities are abstract representations of objects within the virtual environment, including vehicles, buildings, and spaces. The scripts in this folder provide the foundational structures, configurations, and mechanisms for instantiating, initializing, and interacting with these entities.

## Contents

- [`VE.cs`](#vecs)
- [`VE_Config.cs`](#ve_configcs)
- [`VE_Creator.cs`](#ve_creatorcs)
- [`VE_OnClick_Getter.cs`](#ve_onclick_gettercs)

---

### `VE.cs`

#### Overview

`VE` is an abstract base class representing a Virtual Entity. It defines common properties and an initialization method that must be implemented by all derived entities. This class serves as a template for creating concrete types of entities by providing the interface and structure.

#### Key Components

- **Fields:**

  - `int Id`: Unique identifier for the virtual entity.
  - `string Name`: Display name or description of the virtual entity.
  - `GameObject Instance`: Reference to the GameObject instance in the main scene representing this entity.

- **Methods:**

  - `abstract void Init()`: Abstract method to initialize the virtual entity. Must be implemented by derived classes to define entity-specific initialization behavior.

#### Implementation Details

- **Inheritance:**

  - Derived classes inherit from `VE` and implement the `Init` method to handle their specific initialization logic.

- **Common Properties:**

  - Provides common identifiers and references that are shared across all virtual entities, ensuring consistency and facilitating management in the application.

---

### `VE_Config.cs`

#### Overview

`VE_Config` is an abstract base class for configuration objects for the Virtual Entities. It defines shared properties for UI integration, identification, and event handling. This scriptable object class allows for creation and management of entity configuration assets conveniently in the Unity editor.

#### Key Components

- **Fields:**

  - `int Id`: Unique identifier for the configuration.
  - `string Name`: Display name or descriptive title for the configuration.
  - `GameObject UiTemplate`: Reference to the UI template GameObject associated with this configuration.
  - `EventChannel_UI_Navigation UiNavigationEventChannel`: Reference to the event channel for UI navigation events.

#### Implementation Details

- **Scriptable Object:**

  - Inherits from `ScriptableObject`, allowing configurations to be created as assets in the Unity editor.

- **UI Integration:**

  - The `UiTemplate` field facilitates linking a UI representation with the entity, enabling dynamic UI generation based on the configuration.

- **Event Handling:**

  - The `UiNavigationEventChannel` provides a way to handle UI navigation events, promoting decoupled communication between UI components and entities.

---

### `VE_Creator.cs`

#### Overview

`VE_Creator` is an abstract generic class responsible for creating and managing Virtual Entities within the virtual environment. It provides common methods and properties for creation, deletion, registration, and lookup of entities. This class serves as a factory and manager for entities, ensuring they are properly instantiated and tracked.

#### Key Components

- **Fields:**

  - `Transform VeInstanceParentTransform`: Parent transform for newly instantiated VE instances.
  - `Transform VeUiParentTransform`: Parent transform for VE-related UI objects.
  - `Camera_Manager CameraManager`: Camera manager responsible for camera-related operations.
  - `UI_Controller_SystemLog SystemLogUiController`: Reference to the system log UI controller.
  - `Dictionary<int, VE_Type> LookupTable`: Stores all VE instances created by this factory, indexed by their unique ID.

- **Methods:**

  - `abstract VE_Type Create_VE(VE_Config_Type ve_config)`: Abstract method to create and return a new VE based on the provided configuration.
  - `abstract void Delete_VE(int ve_id)`: Abstract method to delete a VE instance from the factory.
  - `void Register_VE(VE_Type ve, VE_Config_Type ve_config)`: Registers a VE instance in the lookup table.
  - `VE_Type Lookup_VE(int ve_id)`: Retrieves a VE instance from the lookup table by its unique ID.

#### Implementation Details

- **Generics:**

  - Utilizes generics to allow flexibility in the types of entities (`VE_Type`) and configurations (`VE_Config_Type`) it can handle.

- **Entity Management:**

  - Provides a centralized mechanism for creating, registering, and deleting entities, ensuring consistency and ease of management.

- **Lookup Table:**

  - Maintains a dictionary of all created entities, facilitating quick access and management based on unique identifiers.

- **Dependencies:**

  - Injects dependencies such as `CameraManager` and `SystemLogUiController` to enable interaction with other systems within the application.

---

### `VE_OnClick_Getter.cs`

#### Overview

`VE_OnClick_Getter` handles the detection of objects clicked by the user within the scene. It utilizes raycasting from the active camera to determine which object under the mouse pointer has been clicked. This utility class is essential for enabling user interaction with entities in the virtual environment.

#### Key Components

- **Fields:**

  - `Camera_Manager CameraManager`: Reference to the camera manager, used to access the currently active camera for raycasting.
  - `LayerMask LayerMask`: Layer mask used to filter the objects that can be clicked.

- **Methods:**

  - `GameObject ReturnClickedObject()`: Casts a ray from the mouse pointer position to detect clicked objects in the scene.

#### Implementation Details

- **Raycasting:**

  - Uses Unity's `Physics.Raycast` method to perform raycasting from the mouse position into the scene, detecting objects on specified layers.

- **Layer Filtering:**

  - The `LayerMask` allows specifying which layers should be considered for click detection, improving performance and ensuring only relevant objects are interactable.

- **Camera Integration:**

  - Relies on the `CameraManager` to obtain the currently active camera, ensuring that the raycast aligns with the user's view.

- **Usage Considerations:**

  - This class should be attached to a GameObject within the scene and requires proper assignment of the `CameraManager` and `LayerMask` to function correctly.

---

## Notes

- The scripts in this folder establish the foundational architecture for Virtual Entities within the virtual environment.
- By utilizing abstract classes and generics, the design promotes extensibility and reusability, allowing new types of entities to be added.
- Proper dependency injection and event handling facilitate decoupled communication between entities and other systems, which promotes modularity and maintainability.

