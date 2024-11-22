# Manager

This folder contains scripts that manage core functionalities and components within the simulation environment. The Manager scripts handle tasks such as camera control, drone synchronization, and other essential operations that facilitate user interaction and system behavior. They provide centralized management of these components, ensuring consistent performance and integration within the application.

## Contents

- [`Camera_Manager.cs`](#camera_managercs)
- [`Drone_Manager.cs`](#drone_managercs)

---

### `Camera_Manager.cs`

#### Overview

`Camera_Manager` is responsible for managing the cameras within the simulation scene. It allows users to switch between different camera views, control the main camera with free-look capabilities, and integrate cameras associated with virtual entities such as vehicles. The manager handles user inputs for camera movement, rotation, and zoom, and provides a user interface for selecting active cameras.

#### Key Components

- **Fields:**

  - **Target Cameras:**
    - `Camera MainCamera`: The main camera used for free-look and primary operations.
    - `Camera EnvOverheadCamera`: Overhead camera for viewing the environment from above.
    - `Camera EnvSideCamera`: Side camera for lateral perspectives.
    - `Camera EnvDockCamera`: Camera positioned for viewing dock areas.
    - `Camera EnvOverviewCamera`: Overview camera for displaying the entire environment.

  - **UI Objects:**
    - `TMP_Dropdown CameraDropdown`: Dropdown UI element for selecting the active camera.
    - `Toggle MenuToggle`: Toggle UI element for enabling or disabling free-look mode.

  - **Event Channels:**
    - `EventChannel_Void VeRegistrationEvent`: Triggered when a vehicle is registered, to add its cameras.
    - `EventChannel_Void VeDeletionEvent`: Triggered when a vehicle is deleted, to remove its cameras.

  - **Camera Control Properties:**
    - `bool IsControlActive`: Indicates whether free-look mode is active.
    - `float MovementSpeed`: Speed of camera movement during free-look.
    - `float CameraVerticalSpeed`: Speed of vertical camera movement.
    - `float CameraDragSpeed`: Speed of camera panning with the middle mouse button.
    - `float MouseSensitivity`: Sensitivity of camera rotation using the mouse.
    - `float ZoomSensitivity`: Sensitivity of camera zoom with the scroll wheel.

  - **Application Dependencies:**
    - `Vehicle_Creator VehicleCreator`: Reference to the vehicle creator for accessing vehicle cameras.
    - `UI_Controller_SystemLog UiControllerSystemLog`: Reference to the system log controller.

- **Methods:**

  - `void Start()`: Initializes the camera manager, sets up the camera dropdown, and subscribes to events.
  - `void Update()`: Handles camera controls if free-look mode is active and interpolates camera movement.
  - `void HandleCameraControl()`: Processes user inputs for camera movement, rotation, and zoom.
  - `void InitializeDropdown()`: Initializes the camera dropdown with available cameras.
  - `void AddVehicleCameras(VE_Vehicle veVehicle)`: Adds cameras from a vehicle to the camera list.
  - `void UpdateDropdownOptions()`: Updates the camera dropdown options.
  - `void OnDropdownValueChanged(int index)`: Handles changes in the camera dropdown selection.
  - `void ActivateCamera(Camera cameraToActivate)`: Activates the specified camera and disables others.
  - `void On_VehicleVE_Registered()`: Adds cameras when a new vehicle is registered.
  - `void On_Vehicle_VE_Deleted()`: Removes cameras when a vehicle is deleted.
  - `void OnMenuToggle()`: Toggles free-look mode and logs the event.

#### Implementation Details

- **Camera Management:**

  - Maintains a dictionary `allCameras` mapping camera names to camera instances.
  - Initializes with predefined cameras and updates the list when vehicles are added or removed.
  - Provides a dropdown UI for selecting the active camera.

- **User Input Handling:**

  - In `HandleCameraControl()`, processes mouse and keyboard inputs for camera control:
    - Mouse right-click and drag for rotation.
    - WASD keys for movement.
    - Space and Shift keys for vertical movement.
    - Mouse scroll wheel for zoom.
    - Middle mouse button and drag for panning.

- **Event Handling:**

  - Subscribes to `VeRegistrationEvent` and `VeDeletionEvent` to manage cameras associated with vehicles.
  - Updates the camera list and UI when vehicles are added or removed.

- **Camera Activation:**

  - `ActivateCamera()` ensures only one camera is active at a time.
  - Updates the `ActiveCamera` reference and adjusts the camera dropdown selection.

- **Free-Look Mode:**

  - Controlled by `IsControlActive`, toggled via the `MenuToggle` UI element.
  - Logs changes to free-look mode using the system log controller.

---

### `Drone_Manager.cs`

#### Overview

`Drone_Manager` synchronizes a drone object's position and rotation within the simulation based on data from an external tracking system (Optitrack). It updates the drone's transform in real-time, ensuring that the simulated drone accurately reflects the tracked physical drone's movements.

#### Key Components

- **Fields:**

  - `OptitrackRigidBody DroneRigidbody`: Reference to the Optitrack rigid body providing position and orientation data.
  - `Transform Drone`: The transform of the drone object to be updated in the scene.

- **Time / Sync Parameters:**

  - `float GainTime`: Controls the responsiveness of synchronization; higher values result in faster updates.
  - `float TimeCount`: Internal time count for synchronization logic (currently unused).

- **Private Variables:**

  - `float scale`: Scaling factor applied to the position data for proper mapping (set to 13).

- **Methods:**

  - `void FixedUpdate()`: Updates the drone's position and rotation every fixed time step.

#### Implementation Details

- **Data Mapping:**

  - Transforms Optitrack coordinate system to Unity's coordinate system:
    - Optitrack Z-axis maps to Unity's X-axis.
    - Optitrack X-axis maps to Unity's Z-axis (negated).

- **Position and Rotation Synchronization:**

  - Uses `Vector3.Lerp()` to smoothly interpolate the drone's position.
  - Uses `Quaternion.Lerp()` to smoothly interpolate the drone's rotation.
  - Applies the `GainTime` factor to control the interpolation speed.

- **Scaling:**

  - Applies a scaling factor to position data to match the simulation's world scale.

- **Update Loop:**

  - `FixedUpdate()` ensures updates occur at consistent intervals, important for physics calculations and smooth motion.

---

## Notes

- The Manager scripts are central to the operation of the simulation, providing essential control over cameras and synchronization with external tracking systems.
- Proper handling of user inputs and event subscriptions ensures a responsive and intuitive user experience.
- The `Camera_Manager` integrates seamlessly with other components, such as vehicles and UI controllers, highlighting the modularity of the application's design.
- The `Drone_Manager` demonstrates integration with external hardware, allowing for real-time interaction between physical devices and the simulation.

