# Path Manager

This folder contains scripts responsible for managing vehicle paths within the simulation. The Path Manager handles the loading, recording, saving, and visualization of paths that vehicles can follow. It facilitates path-based simulations by providing mechanisms to deserialize path data from JSON files, record new paths during runtime, and render paths within the scene for visual feedback.

## Contents

- [`Path_Manager.cs`](#path_managercs)
- [`Path_Previewer.cs`](#path_previewercs)
- [`Path.cs`](#pathcs)
- [`Vector2_Converter.cs`](#vector2_convertercs)

---

### `Path_Manager.cs`

#### Overview

`Path_Manager` manages paths for vehicle simulation, including loading predefined paths from JSON files, recording new paths, and saving them to disk. It maintains a list of available paths and provides functionality to start and stop path recording for vehicles within the simulation.

#### Key Components

- **Fields:**

  - `List<TextAsset> JsonFiles`: List of JSON files containing predefined paths, assignable in the Unity Inspector.
  - `List<Path> Paths`: List of deserialized paths available for simulation.

- **Private Variables:**

  - `Path recordingPath`: The current path being recorded.
  - `bool isRecording`: Indicates whether a path recording is in progress.
  - `float recordStartTime`: Time when the recording started.
  - `VE_Vehicle currentVeVehicle`: Reference to the vehicle being recorded.
  - `Vehicle_Data vehicleData`: Cached reference to the vehicle's data.

- **Methods:**

  - `void Awake()`: Initializes the `Path_Manager` by deserializing paths from JSON files.
  - `void StartPathRecording(VE_Vehicle veVehicle)`: Starts recording a new path for the specified vehicle.
  - `void StopPathRecording()`: Stops recording the current path and saves it to disk as a JSON file.
  - `void FixedUpdate()`: Updates the recording process during the FixedUpdate loop.
  - `void RecordPath()`: Records the current state of the vehicle during the recording process.

#### Implementation Details

- **Path Loading:**

  - In `Awake()`, the manager iterates over the assigned JSON files, deserializes them into `Path` objects, and adds them to the `Paths` list.

- **Path Recording:**

  - `StartPathRecording()` initializes a new `Path` object and begins recording the vehicle's state.
  - `RecordPath()` captures the vehicle's positions, orientations, velocities, and steering angles at each time step.
  - `StopPathRecording()` finalizes the recording, serializes the `Path` object to JSON, and adds it to the `Paths` list.

---

### `Path_Previewer.cs`

#### Overview

`Path_Previewer` handles the visualization of vehicle paths using `LineRenderer` components. It provides methods to draw paths within the simulation scene, allowing users to see the routes that vehicles will follow.

#### Key Components

- **Fields:**

  - `LineRenderer FrontAxleRenderer`: LineRenderer used for visualizing the front axle path of the vehicle.

- **Methods:**

  - `void DrawPath(Path path)`: Draws the specified path using LineRenderers.
  - `void DrawLine(List<Vector2> points, LineRenderer lineRenderer)`: Draws a line between a series of points using the specified LineRenderer.
  - `void Toggle(bool status)`: Toggles the visibility of the path visualization.

#### Implementation Details

- **Path Visualization:**

  - `DrawPath()` activates the `FrontAxleRenderer` and uses `DrawLine()` to render the path based on the `FrontAxle` data from the `Path` object.
  - `DrawLine()` converts 2D points to 3D positions (with a slight elevation) and assigns them to the LineRenderer.
  - `Toggle()` enables or disables the LineRenderer's GameObject to show or hide the path.

---

### `Path.cs`

#### Overview

`Path` represents a vehicle path within the simulation, containing data such as axle positions, orientations, velocities, and steering inputs. It can be deserialized from a JSON string and used for simulating vehicle movements along predefined routes.

#### Key Components

- **Fields:**

  - `string PathName`: Name of the path for identification.
  - `int PathId`: Unique identifier for the path.
  - `List<Vector2> FrontAxle`: Positions of the front axle throughout the path.
  - `List<Vector2> Pivot`: Positions of the pivot point throughout the path.
  - `List<Vector2> RearAxle`: Positions of the rear axle throughout the path.
  - `List<Vector2> TrailerAxle`: Positions of the trailer axle throughout the path.
  - `List<Vector2> Psi`: Yaw angles (orientations) of the vehicle throughout the path.
  - `List<float> StartPose`: Starting pose of the vehicle.
  - `List<float> EndPos
