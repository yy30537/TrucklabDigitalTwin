# Vehicle Controllers

This folder contains scripts that manage various aspects of vehicle behavior within the virtual environment. The vehicle controllers are responsible for handling the vehicle's animation, user interface dashboard, drag-and-drop functionality, and kinematics. These controllers enable interaction with the vehicle, visual representation, and the underlying physics that govern its movement.

## Contents

- [`Vehicle\_Animation\_Controller.cs`](#vehicle\_animation\_controllercs)
- [`Vehicle\_Dashboard\_Controller.cs`](#vehicle\_dashboard\_controllercs)
- [`Vehicle\_Dragger.cs`](#vehicle\_draggercs)
- [`Vehicle\_Kinematics\_Controller.cs`](#vehicle\_kinematics\_controllercs)
- [`Vehicle\_Collision\_Controller.cs`](#vehicle\_collision\_controllercs)

---

### `Vehicle\_Animation\_Controller.cs`

#### Overview

Handles the visual representation and animation of the vehicle (`VeVehicle`). Manages wheel rotation, vehicle transforms, and other visual components to provide a realistic depiction of the vehicle's movement.

#### Key Components

- **Fields:**

  - `bool IsActive`: Indicates whether the animation controller is active.
  - `VE\_Vehicle VeVehicle`: Reference to the associated vehicle.
  - `Vehicle\_Data VehicleData`: Reference to the vehicle's data.
  - **Transform Components:**
    - `Transform VehicleTransform`: Main vehicle transform.
    - `Transform TractorTransform`: Transform of the tractor part.
    - `Transform TrailerTransform`: Transform of the trailer part.
    - **Wheel Transforms:**
      - `Transform SteeringWheel`
      - `Transform TractorWheelFrontLeft`
      - `Transform TractorWheelFrontRight`
      - `Transform TractorWheelRearLeft`
      - `Transform TractorWheelRearRight`
      - `Transform TrailerWheelFrontLeft`
      - `Transform TrailerWheelFrontRight`
      - `Transform TrailerWheelCenterLeft`
      - `Transform TrailerWheelCenterRight`
      - `Transform TrailerWheelRearLeft`
      - `Transform TrailerWheelRearRight`
  - `GameObject TractorTrajectory`: Visual representation of the tractor's trail.
  - **Wheel Rotation Values:**
    - `float TractorWheelFrontLeftRot`
    - `float TractorWheelFrontRightRot`
    - `float TractorWheelRearLeftRot`
    - `float TractorWheelRearRightRot`
    - `float TrailerWheelFrontLeftRot`
    - `float TrailerWheelFrontRightRot`
    - `float TrailerWheelCenterLeftRot`
    - `float TrailerWheelCenterRightRot`
    - `float TrailerWheelRearLeftRot`
    - `float TrailerWheelRearRightRot`
  - **Timing and Synchronization:**
    - `float GainTime`: Used for smoothing animations.
    - `float TimeCount`: Counter for timing purposes.

- **Methods:**

  - `void FixedUpdate()`: Called at fixed intervals to update the vehicle's animation if active.
  - `void UpdateVehicleAnimation()`: Updates the visual animation of the vehicle, including wheels and transforms.
  - `void Init(VE\_Vehicle vehicle, Vehicle\_Data data)`: Initializes the animation controller with references to the associated vehicle and its data.
  - `void InitializeWheelTransforms()`: Finds and assigns references to all relevant wheel transform components.
  - `void InitializeWheelRotationData()`: Resets all wheel rotation values to zero.
  - `void UpdateWheelRotation()`: Calculates and updates the rotation values for all wheels based on the vehicle's velocity and angular velocity.
  - `void UpdateWheelLocalRotation(Transform wheelTransform, float rotation)`: Sets the local rotation of a specific wheel transform based on its rotation value.
  - `void UpdateVehicleTransforms()`: Interpolates and updates the position and orientation of the tractor and trailer.
  - `void UpdateSteeringWheel()`: Smoothly rotates the steering wheel based on the vehicle's steering input angle.

#### Implementation Details

- **Wheel Rotation:**
  - Wheel rotation is calculated based on the vehicle's velocity (`V1`, `V2`) and angular velocity (`Psi1dot`, `Psi2dot`).
  - Each wheel's rotation is updated individually to reflect the differential rotation due to turning.

- **Vehicle Transforms:**
  - The positions and orientations of the tractor and trailer are updated using linear interpolation (`Vector3.Lerp`) and quaternion interpolation (`Quaternion.Lerp`) for smooth animations.

- **Steering Wheel:**
  - The steering wheel rotation reflects the driver's input, adjusted by a factor of 10 for visual effect.

#### Usage

- The `Vehicle\_Animation\_Controller` is attached to the vehicle object and initialized with the vehicle's data.
- It requires references to all relevant transforms, which are assigned during initialization.
- The animation controller updates automatically during the `FixedUpdate` cycle when `IsActive` is `true`.

---

### `Vehicle\_Dashboard\_Controller.cs`

#### Overview

A UI component for displaying and managing vehicle information. The dashboard provides interactive controls and displays real-time data about the vehicle's state, such as kinematics, configuration, and detected obstacles.

#### Key Components

- **Fields:**

  - `bool FloatAboveVehicle`: Determines whether the dashboard floats above the vehicle in the scene.
  - `VE\_Vehicle VeVehicle`: Reference to the associated vehicle.
  - `Vehicle\_Data VehicleData`: Reference to the vehicle's data.
  - `Transform VeUiInstanceParentTransform`: Parent transform for the UI objects.
  - **UI Objects:**
    - `GameObject TopBar`: Top bar of the dashboard for dragging and displaying the vehicle's name.
    - `TextMeshProUGUI TopBarText`: Text component for the top bar.
    - `TextMeshProUGUI KinematicsMode`: Text displaying the current kinematics mode.
    - `GameObject[] Pages`: Array of page objects in the dashboard UI.
    - `TextMeshProUGUI[] PageContents`: Array of content texts for each page.
    - `Button[] PageButtons`: Buttons for navigating between pages.
    - `Button CloseButton`: Button to close the dashboard.
    - `TMP\_Dropdown ActuationSourceDropdown`: Dropdown for selecting the actuation source.
    - `TMP\_Dropdown SpaceDropdown`: Dropdown for selecting a space to position the vehicle.
    - `Toggle TractorTrailToggle`: Toggle for enabling/disabling the tractor trail visualization.
    - `Button SetPositionButton`: Button to set the vehicle's position based on the selected space.
    - `Toggle PinDashboardToggle`: Toggle for pinning the dashboard in a fixed position.
  - **Constants:**
    - `int PageCount = 4`: Number of pages in the dashboard.
    - `int OffsetY = 150`: Vertical offset when floating above the vehicle.

- **Methods:**

  - `void Update()`: Updates the dashboard content and position.
  - `void SetDependencies(...)`: Sets up dependencies for the dashboard controller.
  - `override void Init()`: Initializes the dashboard UI and its components.
  - `void InitializeVehicleDashboardUiObjects()`: Initializes all UI objects and configurations.
  - `void InitializeTopBar()`: Initializes the top bar UI object.
  - `void InitializeKinematicsModeText()`: Initializes the kinematics mode text.
  - `void InitializePages()`: Configures the pages in the dashboard UI.
  - `void InitializeButtons()`: Sets up the buttons in the dashboard UI.
  - `void InitializeDropdowns()`: Configures dropdown UI objects.
  - `void InitializeToggle()`: Configures toggle UI objects.
  - `void InitializeDashboardPinToggle()`: Configures the toggle for pinning the dashboard.
  - `void SetupEventListeners()`: Attaches event listeners to UI objects.
  - `void UpdateDashboard()`: Updates the dashboard content and UI objects.
  - `void UpdateDashboardPosition()`: Updates the dashboard position to float above the vehicle.
  - `void UpdateTopBar()`: Updates the top bar text with the vehicle's name.
  - `void UpdatePage1()`: Updates the first page with kinematics mode.
  - `void UpdatePage2()`: Updates the second page with vehicle configuration.
  - `void UpdatePage3()`: Updates the third page with vehicle state.
  - `void UpdatePage4()`: Updates the fourth page with detected obstacles.
  - `void ShowPage(int pageIndex)`: Displays the selected page.
  - `void PopulateInputDropdown()`: Populates the actuation source dropdown.
  - `void PopulateSpaceDropdown()`: Populates the space dropdown.
  - `void SetVehiclePositionToSpace()`: Sets the vehicle's position based on the selected space.
  - `void OnToggleTractorTrail(bool status)`: Toggles the tractor trail visualization.
  - `void OnToggleDashboardPin(bool status)`: Toggles the dashboard's pin state.
  - `void OnActuationSourceChanged(int index)`: Changes the actuation source.
  - `protected override void OnDestroy()`: Cleans up event listeners when the dashboard is destroyed.

#### Implementation Details

- **UI Interaction:**
  - The dashboard allows users to interact with the vehicle's settings and view real-time data.
  - Event listeners are attached to UI objects to handle user interactions.

- **Data Display:**
  - The dashboard displays vehicle kinematics, configuration parameters, and obstacle detection information.

- **Positioning:**
  - The dashboard can float above the vehicle or be pinned in a fixed position based on user preference.

#### Usage

- The `Vehicle\_Dashboard\_Controller` is instantiated as part of the vehicle's UI.
- It requires references to the vehicle and its data, which are set via `SetDependencies`.
- The dashboard updates automatically during the `Update` cycle when `IsActive` is `true`.

---

### `Vehicle\_Dragger.cs`

#### Overview

Handles the drag-and-drop operations for vehicle instances in the main scene. It provides functionality for selecting, moving, and adjusting the vehicle's position and orientation based on user interaction with the mouse and scroll wheel. This helper function provides a mechanism to conviniently position vehicles to set up simulation scenarios.

#### Key Components

- **Fields:**

  - `bool IsDraggingEnabled`: Indicates whether drag-and-drop functionality is enabled.
  - `bool IsDragging`: Tracks whether a vehicle is currently being dragged.
  - `float ScrollInput`: Captures scroll wheel input.
  - `float ScrollSensitivity`: Sensitivity for adjusting vehicle angles.
  - `UI\_Controller\_SystemLog UiControllerSystemLog`: Reference for logging events.
  - **Selected Objects:**
    - `GameObject ClickedVehicle`: The currently selected vehicle.
    - `GameObject ClickedTrailerObject`: Reference to the clicked trailer object.
    - `GameObject ClickedTractorObject`: Reference to the clicked tractor object.
  - **Dependencies:**
    - `VE\_OnClick\_Getter VeOnClickGetterGetter`: Utility for identifying clicked objects.
    - `Camera\_Manager CameraManager`: Reference for raycasting operations.
  - **UI objects:**
    - `Toggle Toggle`: UI toggle controlling whether dragging is enabled.

- **Methods:**

  - `void Awake()`: Initializes the state by clearing selections and disabling dragging.
  - `void Update()`: Checks if dragging is enabled and processes drag-and-drop operations.
  - `void OnMenuToggle()`: Toggles the drag-and-drop functionality based on the UI toggle.
  - `void HandleDragAndDrop()`: Handles the logic for selecting, moving, and releasing vehicles.
  - `void MoveSelectedVehicle(VE\_Vehicle veVehicle)`: Updates the position of the selected vehicle.
  - `void HandleScrollInput(VE\_Vehicle veVehicle)`: Processes scroll input to adjust vehicle angles.
  - `void AdjustVehicleAngles(VE\_Vehicle veVehicle)`: Adjusts the tractor or trailer angles.
  - `void ResetSelections()`: Resets selected references and stops dragging.

#### Implementation Details

- **Selection:**
  - Vehicles are selected by clicking on the tractor or trailer components.
  - Collision detection is disabled during dragging to prevent interference.

- **Movement:**
  - The vehicle's position is updated based on mouse position using raycasting.
  - The scroll wheel adjusts the vehicle's orientation.

- **User Interface:**
  - The drag-and-drop functionality is controlled via a UI toggle.
  - Events are logged to the system log for user feedback.

#### Usage

- The `Vehicle\_Dragger` is used to interactively reposition vehicles.
- It must be properly initialized with references to the required components and UI objects.
- Dragging can be enabled or disabled based on user preference.

---

### `Vehicle\_Kinematics\_Controller.cs`

#### Overview

Controls the kinematics of the vehicle, managing its motion, steering, and actuation inputs. It utilizes kinematics and input strategies to update the vehicle's state over time, handling both user input and predefined paths.

#### Key Components

- **Fields:**

  - `VE\_Vehicle VeVehicle`: Reference to the associated vehicle.
  - `Vehicle\_Data VehicleData`: Reference to the vehicle's data.
  - `bool IsActive`: Indicates whether the kinematics controller is active.
  - `Kinematics\_Strategy.Kinematics\_Strategy KinematicStrategy`: The selected kinematics strategy for motion.
  - `Actuation\_Input\_Strategy.Actuation\_Input\_Strategy ActiveInputStrategy`: The current actuation input strategy.
  - `float InputVelocity`: Current input velocity of the vehicle.
  - `float InputSteerAngle`: Current input steering angle.
  - `bool isBraking`: Indicates whether the vehicle is currently braking.

- **Methods:**

  - `void FixedUpdate()`: Processes kinematics updates and input strategies.
  - `void Init(VE\_Vehicle vehicle, Vehicle\_Data data)`: Initializes the controller with the vehicle and data.
  - `string GetCurrentInputStrategyName()`: Retrieves the name of the active input strategy.
  - `bool ChangeInputStrategy(string strategyName)`: Changes the input strategy by name.
  - `void SetInputStrategy(Actuation\_Input\_Strategy inputStrategy)`: Sets the active input strategy.
  - `void CalculateIntermediateStates(Vehicle\_Data data, float deltaTime, float inputVelocity, float inputSteerAngle)`: Calculates intermediate kinematic states.
  - `void SetVehiclePosition(float x, float y, float psi1Degrees, float psi2Degrees)`: Sets the vehicle's position and orientation.
  - `void SetTractorAngle(float inputValue, float angleAdjustmentRate)`: Adjusts the tractor's angle.
  - `void SetTrailerAngle(float inputValue, float angleAdjustmentRate)`: Adjusts the trailer's angle.
  - `void InitPathReplay(Path path)`: Initializes the vehicle's position based on a path.
  - `void StartPathReplaying(Path path)`: Starts replaying the vehicle's motion along a path.
  - `void StopPathReplaying(Path path)`: Stops replaying the vehicle's motion.
  - `IEnumerator PathReplayCoroutine(Path path)`: Coroutine for path replaying.
  - `void Brake()`: Executes braking logic.
  - `void ApplyBrakes()`: Activates braking.
  - `void ReleaseBrakes()`: Deactivates braking.

#### Implementation Details

- **Kinematics Strategy:**
  - Utilizes a selected kinematics strategy to update the vehicle's position and orientation.
  - Strategies can be switched dynamically based on simulation requirements.

- **Actuation Input:**
  - The controller retrieves input from the active input strategy, which could be user input or external data.
  - Supports changing input strategies at runtime.

- **Braking Mechanism:**
  - Provides methods to apply and release brakes.
  - Braking logic can be expanded for more advanced behavior.

- **Path Replay:**
  - Supports replaying vehicle motion along predefined paths using coroutines.
  - Allows for simulation of recorded or planned movements.

#### Usage

- The `Vehicle\_Kinematics\_Controller` is attached to the vehicle and initialized with the necessary data.
- It requires a kinematics strategy and input strategy to function.
- The controller updates automatically during the `FixedUpdate` cycle when `IsActive` is `true`.

---

## Notes

- The vehicle controllers are designed to work together to provide comprehensive management of vehicle behavior.
- Modularity is emphasized, allowing for strategies and components to be swapped or extended as needed.
- Proper initialization and configuration are crucial for the controllers to function correctly.
- Event handling and user interaction are integral parts of the controllers, enabling real-time control and feedback.

### `Vehicle\_Collision\_Controller.cs`

#### Overview

Manages collision detection and avoidance for the vehicle using sensors (e.g., colliders). It tracks detected obstacles, updates their information, and triggers actions such as braking when obstacles are within certain thresholds.

#### Key Components

- **Fields:**
  - `VE\_Vehicle VeVehicle`: Reference to the associated vehicle object.
  - `Vehicle\_Data VehicleData`: Reference to the vehicle's data.
  - `Collider TractorBoxCollider`: Collider for the tractor section.
  - `Collider TrailerBoxCollider`: Collider for the trailer section.
  - `SphereCollider SensorSphereCollider`: Sphere collider used as an obstacle detection sensor.
  - `Vector3 VehicleObstacleSensorCenter`: Current center position of the vehicle's obstacle detection sensor.
  - `bool IsActive`: Indicates whether collision detection is active.
  - `bool IsObstacleDetected`: Indicates if an obstacle is currently detected.
  - `float Range`: Maximum detection range of the obstacle sensor.
  - `int NumRays`: Number of rays used in obstacle detection (if applicable).
  - `List<Obstacle\_Info> DetectedObstacles`: List of currently detected obstacles.
  - `float BrakingThreshold`: Distance threshold to trigger braking.

- **Methods:**
  - `Init(VE\_Vehicle vehicle, Vehicle\_Data data)`: Initializes the collision controller with the associated vehicle and data.
  - `FixedUpdate()`: Updates the sensor position and obstacle data at fixed intervals.
  - `OnChildTriggerEnter(GameObject child, Collider other)`: Handles trigger enter events from child objects, detecting new obstacles.
  - `OnChildTriggerExit(GameObject child, Collider other)`: Handles trigger exit events from child objects, removing obstacles from the detection list.
  - `UpdateDetectedObstacles()`: Updates information for detected obstacles, including distance and angle.
  - `IsSelfCollision(Collider collider)`: Determines if a collision is with the vehicle itself.
  - `AttachCollisionForwarder(GameObject child)`: Attaches a collision forwarder to a specified child object.

#### Functionality

- **Obstacle Detection:**
  - Uses a `SphereCollider` as a sensor to detect obstacles within a certain range around the vehicle.
  - When an obstacle enters the sensor's trigger area, an `Obstacle\_Info` instance is created and added to the `DetectedObstacles` list.
  - The sensor ignores collisions with the vehicle itself and specific tags (e.g., ground plane).

- **Obstacle Tracking:**
  - Continuously updates the position of the sensor based on the vehicle's current position.
  - Updates the distance and angle of each detected obstacle relative to the vehicle.
  - Removes obstacles from the list when they exit the sensor's trigger area or are destroyed.

- **Collision Response:**
  - Checks if any detected obstacle is within the `BrakingThreshold`.
  - If an obstacle is within the threshold, commands the vehicle's kinematics controller to apply brakes.
  - Releases brakes when no obstacles are within the threshold.

#### Usage

- Initialize the collision controller by calling `Init` with the appropriate vehicle and data references.
- Ensure that the vehicle's tractor and trailer have colliders and that `Vehicle\_Children\_Collision\_Forwarder` is attached to them.
- Set the `IsActive` flag to enable or disable collision detection as needed.

---