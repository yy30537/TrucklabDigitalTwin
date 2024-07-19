### README.md

# TruckLab Digital Twin Unity Virtual Application Space

## Table of Contents

1. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Installation](#installation)
2. [Running the Project](#running-the-project)
3. [Using the Application](#using-the-application)
   - [Main Menu](#main-menu)
   - [Service Menu](#service-menu)
     - [Vehicle Factory](#vehicle-factory)
     - [Path Simulation](#path-simulation)
   - [Vehicle UI](#vehicle-ui)
   - [Regions UI](#regions-ui)
   - [System Log](#system-log)
   - [DC Schedule](#dc-schedule)
   - [Using the Camera](#using-the-camera)
   - [Free Look](#free-look)
4. [Path Manager](#path-manager)
   - [Adding Paths](#adding-paths)
   - [JSON Path File Format](#json-path-file-format)
   - [Recording New Paths](#recording-new-paths)
5. [Common Issues](#common-issues)
6. [Project Structure](#project-structure)
7. [License](#license)

## Getting Started

### Prerequisites

- Unity 2022.3.16f1
- RosSharp
- OptiTrack Motive
- Turtlebot
- MATLAB 2020a

Simulation of the Digital Twins is enabled only within the TruckLab Proving Ground where the IoT Network is available. Standalone simulation of Digital Models can still be performed within the Unity Application Space.

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/trucklab-digital-twin.git
   cd trucklab-digital-twin
   ```

2. Open the project in Unity:
   - Launch Unity Hub
   - Click on "Add" and select the `Unity Application Space\TruckLabDemo` folder

## Running the Project

1. Open the project in Unity.
2. Load the `MainScene` from the `Scenes` folder.
3. Press the Play button in Unity to start the application.

## Using the Application

### Main Menu

The main menu is located at the bottom of the screen and includes the following:
- **System Log**: Toggle the system log.
- **Service Menu**: Toggle the service menu.
- **DC Schedule**: Toggle the distribution center schedule board.
- **Region States**: Toggle the status interface of environment regions of interest.
- **Camera Dropdown**: Switch between available camera views (Main, Overhead, Side, and Dock views).
- **Free Look**: Toggle free look control for the main camera.
- **Drag & Drop Vehicles**: Toggle the ability to drag and drop vehicle models if available.

### Service Menu

The service menu currently has Vehicle Factory and Path Simulation pages. Planned features include scheduler service integration between Unity Application Space and Matlab controller blocks.

#### Vehicle Factory

The Vehicle Factory in the TruckLab Digital Twin application allows users to create and manage vehicle products within the simulation. To define a new vehicle, users need to create a new `VehicleConfig` scriptable object and update the list of configs in the factory menu.

##### Creating a New VehicleConfig

1. **Locate the Vehicle Config Folder**:
   - In the Unity Editor, navigate to the `Assets` folder.
   - Go to `Core > Scriptable Objects > Virtual Entity Config > Instances > Vehicle`.

2. **Create a New VehicleConfig**:
   - Right-click within the `Vehicle` folder.
   - Select `Create > ScriptableObject > VehicleConfig`.
   - Name the new `VehicleConfig` (e.g., `NewVehicleConfig`).

3. **Configure the New VehicleConfig**:
   - Select the newly created `VehicleConfig` scriptable object.
   - In the `Inspector` window, configure the parameters for the vehicle:
     - **Entity ID**: Unique identifier for the vehicle.
     - **Entity Name**: Name of the vehicle.
     - **Vehicle Prototype Prefab**: Assign the prefab for the vehicle.
     - **Parameters**:
       - **Scale**: Scale of the vehicle.
       - **Length**: Length of the vehicle components.
       - **Width**: Width of the vehicle components.
     - **Mocap Configuration**: Configure the motion capture settings if applicable.
     - **ROS Configuration**: Configure ROS settings if the vehicle uses ROS integration.
     - **Vehicle Control**: Set kinematics and actuation input sources.
     - **Initial Vehicle States**: Define the initial states like position, orientation, and velocity.

4. **Save the Config**:
   - Ensure all changes are saved.

##### Updating the Factory Menu

1. **Open the Factory Menu**:
   - In the Unity Editor, navigate to `MainScene > Interface > Canvas > Explorer > Service Menu > Menu Pages > Factory`.

2. **Update the Vehicle Config List**:
   - Select the `Factory` object.
   - In the `Inspector` window, find the `Factory Menu` script component.
   - Locate the `Vehicle Configs` list under the `Factories` section.
   - Add the newly created `VehicleConfig` by increasing the `Size` of the list and dragging the `VehicleConfig` into the new element slot.

3. **Save the Scene**:
   - Ensure the scene is saved with the updated factory menu configuration.

##### Using the Factory Menu

1. **Create a Vehicle**:
   - Open the application and navigate to the `Service Menu`.
   - Click on the `Factory Menu`.
   - Select a vehicle from the dropdown.
   - Click "Create Vehicle" to instantiate the vehicle in the simulation.

2. **Delete a Vehicle**:
   - Select the vehicle from the dropdown in the `Factory Menu`.
   - Click "Delete Vehicle" to remove the vehicle from the simulation.

By following these steps, users can create new vehicle configurations, update the factory menu, and manage vehicles within the application.

#### Path Simulation

If vehicles are created, they will appear in the vehicle dropdown in the simulation menu.

- **Select Vehicle**: Choose a vehicle from the dropdown to perform simulation.
- **Select Path**: Choose a path from the dropdown to visualize.
- **Simulate**: Visualize the selected path for the selected vehicle.
- **Start Path Recorder**: Start recording a new path.
- **Stop Path Recorder**: Stop recording the current path.

### Path Manager

Located under `Services > Simulation`, the `Path Manager` component allows you to manage and visualize paths. 

#### Adding Paths

1. In the Scene, locate the `Path Manager` under `Simulation` in `Services`.
2. Go to `Window > Panels > Project`, locate the JSON path files under `Assets > Core > Vehicle Components > Reference Paths`.
3. Drag and drop selected JSON path files into the fields of the Path Manager.

#### JSON Path File Format

The JSON path files are created by the vehicle dynamics and control researchers at TruckLab. The structure is shown below:

```json
{
  "pathName": "string",
  "pathID": int,
  "frontaxle": [{"x": float, "y": float}, ...],
  "pivot": [{"x": float, "y": float}, ...],
  "rearaxle": [{"x": float, "y": float}, ...],
  "traileraxle": [{"x": float, "y": float}, ...],
  "psi": [{"x": float, "y": float}, ...],
  "startPose": [float, float, float, float],
  "endPose": [float, float, float, float],
  "time": [float, ...],
  "velocities": [float, ...],
  "steeringAngles": [float, ...],
  "simInput": {
    "steer_input": [[float, ...], [float, ...]],
    "Vx_input": [[float, ...], [float, ...]],
    "Vx": float,
    "tmax": float
  }
}
```

Each JSON file represents a path with information about the vehicle's movement, including axle positions, pose, velocity, steering angles, and simulation input data.

#### Recording New Paths

1. **Start Recording**: 
   - Open the Service Menu and navigate to the Path Simulation page.
   - Select a vehicle from the dropdown in the `Path Simulation` service menu page to record from.
   - Select actuation input source in the selected vehicle UI.
   - Click "Start Path Recorder" to begin recording a new path.
   
2. **Stop Recording**: 
   - Click "Stop Path Recorder" to finish recording. The path will be saved and added to the list of available paths in the Path Manager.

3. **View Recorded Path**:
   - The newly recorded path will be saved in `Assets/Core/Vehicle Components/Reference Paths` and can be added to the simulation by dragging the JSON file into the Path Manager again.

### Vehicle UI

When vehicles are created, a UI appears floating above each vehicle. It includes the following pages:

#### P1: Vehicle Sources

- **Kinematic Source**: Select the kinematic source (Actuation / Motion Capture).
- **Actuation Source**: Select the actuation source (Controller / ThrustMaster / Keyboard).
- **Trail Renderer**: Toggle the trail object for the vehicle.

#### P2: Vehicle Attributes

Displays vehicle parameters (scale, lengths, and widths).

#### P3: Vehicle Position and Kinematics Information

Shows vehicle information including velocity, steering change, and position and orientation of the tractor and trailer.
- **Place Vehicle**: Choose a region

 from the dropdown to place the vehicle.

#### P4: Collision Information

Shows detected objects and their position information.

### Regions UI

Located above the system explorer at the bottom of the screen, this UI shows the vehicles that regions are detecting.

### System Log

Located in the lower-left corner, this log displays real-time system events.

### DC Schedule

Located at the top of the screen, this schedule is yet to be implemented and currently serves as a template. Once the backend scheduler microservice is implemented, the DC dock schedule UI should be updated.  

### Using the Camera

The camera system in the TruckLab Digital Twin application allows you to switch between camera views and cameras associated with vehicles.

The initial predefined camera views can be selected from the cameras dropdown menu at the bottom of the screen. The available predefined views are:
- **Main Camera**: The default camera used in the simulation. 
- **DC Overhead View**: An overhead view of the distribution center.
- **DC Side View**: A side view of the distribution center.
- **DC Dock View**: A view focused on the docking area.

When vehicles are created, their associated cameras are also added to the dropdown menu, including:
- **Tractor Left Mirror Cam**.
- **Tractor Right Mirror Cam**.
- **Driver Seat Cam**.
- **Trailer Mirror Cam**.
- **Overhead Cam**.

To switch between these views:
1. Click on the dropdown menu at the bottom labeled "Main Camera".
2. Select the desired camera view from the list. This will change the active camera to the selected view.

### Free Look

To enable Free Look:
1. Click the "Free Look" button located at the bottom menu of the screen. This will toggle the Free Look mode on or off.
2. When Free Look is enabled, the camera can be controlled using the mouse and keyboard inputs.

Once Free Look is enabled, use the following controls to navigate the scene:

- **Mouse Rotation**:
  - **Right Mouse Button (Hold)**: Click and hold the right mouse button to rotate the camera.
  - **Mouse Movement**: Move the mouse while holding the right mouse button to rotate the camera view. Vertical movement rotates the camera up and down, while horizontal movement rotates it left and right.

- **Camera Movement**:
  - **Middle Mouse Button (Hold)**: Click and hold the middle mouse button to drag the camera.
  - **Mouse Movement**: Move the mouse while holding the middle mouse button to move the camera along the x and z axes.

- **WASD Movement**:
  - **W**: Move the camera forward.
  - **A**: Move the camera left.
  - **S**: Move the camera backward.
  - **D**: Move the camera right.
  - **Arrow Keys**: The arrow keys can also be used for similar movements as WASD keys.

- **Vertical Movement**:
  - **Spacebar**: Hold the spacebar to raise the camera.
  - **Ctrl**: Hold the Ctrl key to lower the camera.

- **Zoom**:
  - **Mouse Scroll Wheel**: Scroll the mouse wheel up to zoom in and down to zoom out. This adjusts the camera's field of view for a closer or wider perspective.

#### Adjusting Sensitivity and Speed

The Free Look controls can be customized by adjusting the following parameters in the CameraManager script:
- **Movement Speed** (`movementSpeed`): The speed at which the camera moves using the WASD keys.
- **Camera Vertical Speed** (`cameraVerticalSpeed`): The speed at which the camera moves up and down using the spacebar and Ctrl key.
- **Camera Drag Speed** (`cameraDragSpeed`): The speed at which the camera moves when dragging with the middle mouse button.
- **Mouse Sensitivity** (`mouseSensitivity`): The sensitivity of the mouse for rotating the camera.
- **Zoom Sensitivity** (`zoomSensitivity`): The sensitivity of the mouse scroll wheel for zooming in and out.

These settings can be found in the CameraManager script under the "Camera Control Properties" section and can be adjusted in the Unity Inspector.

To adjust the camera control properties during runtime:
1. Ensure the scene is running.
2. In the Unity Editor, locate the `Camera Manager` object in the `Hierarchy` window. It can be found under `MainScene > Utility > Camera Manager`.
3. Select the `Camera Manager` object.
4. In the `Inspector` window, you will see the `CameraManager` script component. Here you can adjust the following properties:
   - **Movement Speed**
   - **Camera Vertical Speed**
   - **Camera Drag Speed**
   - **Mouse Sensitivity**
   - **Zoom Sensitivity**

### External Systems

For ROS and MATLAB/Simulink integration, ensure the respective topics are configured.

## Project Structure

The project is organized into the following main directories:

- `Assets/`
  - `Core/`
    - `Factory/`
      - `BaseFactory.cs`
      - `DockFactory.cs`
      - `RegionFactory.cs`
      - `VehicleFactory.cs`
    - `Interface/`
      - `Menu/`
        - `BaseMenu.cs`
        - `FactoryMenu.cs`
        - `SimulationMenu.cs`
        - `SystemMenuController.cs`
        - `SystemLogWindow.cs`
      - `Virtual Entity Observer/`
        - `BaseObserver.cs`
        - `DockObserver.cs`
        - `RegionObserver.cs`
        - `VehicleObserver.cs`
      - `Virtual Entity UI/`
        - `BaseUI.cs`
        - `DockUI.cs`
        - `RegionUI.cs`
        - `VehicleUI.cs`
    - `Product/`
      - `BaseProduct.cs`
      - `DockProduct.cs`
      - `IProduct.cs`
      - `RegionProduct.cs`
      - `VehicleProduct.cs`
    - `Scriptable Objects/`
      - `Event Channel/`
        - `GenericEventChannel.cs`
        - `MenuNavigationEventChannel.cs`
        - `VoidEventChannel.cs`
      - `Virtual Entity Config/`
        - `BaseConfig.cs`
        - `DockConfig.cs`
        - `RegionConfig.cs`
        - `VehicleConfig.cs`
    - `Services/`
      - `Path Simulation/`
        - `Path.cs`
        - `PathDrawer.cs`
        - `PathManager.cs`
    - `Utility/`
      - `CameraManager.cs`
      - `DragVehicle.cs`
      - `Drone.cs`
      - `GetClickedObject.cs`
      - `Vector2Converter.cs`
    - `Vehicle Components/`
      - `VehicleAnimation.cs`
      - `VehicleCollisionController.cs`
      - `VehicleComponent.cs`
      - `VehicleData.cs`
      - `VehicleKinematics.cs`
  - `ExternalLibraries/`
    - `OptiTrack/`
    - `RosSharp/`
    - `TextMesh Pro/`
    - `VehiclePhysics/`
  - `Prototypes/`
    - `Distribution Center/`
    - `UI Prototypes/`
    - `Vehicle/`
  - `Resources/`
  - `Scenes/`
    - `MainScene`

## License

Include the license information here.