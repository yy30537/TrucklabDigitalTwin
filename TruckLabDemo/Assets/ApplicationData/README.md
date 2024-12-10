# Application Data

The **Application Data** folder contains data assets used by the application. These assets include event channel instances, virtual entity configurations, prototypes, vehicle actuation input strategies and kinematics strategies, and vehicle reference paths.

## Contents

- **Event Channel Assets**
  - Contains all event channel `ScriptableObject` instances, facilitating communication between systems within the application.
  - Event channels are created using the following classes:
    - `EventChannel_Generic` : `ScriptableObject`
    - `EventChannel_Void` : `ScriptableObject`
    - `EventChannel_UI_Navigation` : `ScriptableObject`
  - The event channels include:
    - `Create_All_VE_Buildings_EC` (`EventChannel_Void`)
    - `Delete_All_VE_Buildings_EC` (`EventChannel_Void`)
    - `Create_VE_Building_EC` (`EventChannel_Void`)
    - `Delete_VE_Building_EC` (`EventChannel_Void`)
    - `Create_All_VE_Spaces_EC` (`EventChannel_Void`)
    - `Delete_All_VE_Spaces_EC` (`EventChannel_Void`)
    - `Create_VE_Space_EC` (`EventChannel_Void`)
    - `Delete_VE_Space_EC` (`EventChannel_Void`)
    - `Create_All_VE_Vehicles_EC` (`EventChannel_Void`)
    - `Delete_All_VE_Vehicles_EC` (`EventChannel_Void`)
    - `Create_VE_Vehicle_EC` (`EventChannel_Void`)
    - `Delete_VE_Vehicle_EC` (`EventChannel_Void`)
    - `UI_Navigation_EC` (`EventChannel_UI_Navigation`)

- **Virtual Entity Configs**
  - Contains configuration assets for virtual entities, created as `ScriptableObject` instances derived from `VE_Config`.
  - **Buildings**
    - Configurations for buildings using `Building_Config` : `VE_Config`.
    - Currently includes one asset for the dock building.
  - **Spaces**
    - Configurations for spaces using `Space_Config` : `VE_Config`.
    - Includes configurations for:
      - Arrival Parking 1, 2, 3
      - Backup Line 439 to 443
      - Departure Parking 1, 2, 3
      - Dock Parking Spaces 438 to 447
      - Incoming Road
      - Outgoing Road
      
  - **Vehicles**
    - Configurations for vehicles using `Vehicle_Config` : `VE_Config`.
    - Includes configurations for:
      - Truck 1 Digital Model
      - Truck 2 Digital Model
      - Truck 3 Digital Model
      - Truck 1 Digital Shadow
      - Truck 2 Digital Shadow
      - Truck 3 Digital Shadow
      - Truck 1 Digital Twin
      - Truck 2 Digital Twin
      - Truck 3 Digital Twin

- **Virtual Entity Prototypes**
  - Contains 3D model prefabs and UI template prefabs for each type of virtual entity.
  - These prototypes are used by the VE creators to instantiate VE instances.
  - **Buildings**
    - Dock Building CAD prefab
    - Dock Building Dashboard UI template prefab
  - **Spaces**
    - Space Dashboard UI template prefab
    - *Note:* Spaces currently do not have a 3D model; ground materials are assigned in the space configs for visual representation.
  - **Vehicles**
    - Tractor, Trailer, and complete Truck CAD models
    - Vehicle Dashboard UI template prefab

- **Vehicle Actuation Input Strategies**
  - Contains `ScriptableObject` assets created from `Actuation_Input_Strategy`.
  - Specific strategies include:
    - **Controller Input Strategies:**
      - `Auto - Simulink Controller (ROS)` (`Controller_Input_Strategy`)
      - `Manual - Game Controller (ROS)` (`Controller_Input_Strategy`)
    - **Keyboard Input Strategies:**
      - `Manual - Keyboard` (`Keyboard_Input_Strategy`)
      - `Manual - No Input` (`Keyboard_Input_Strategy`)

- **Vehicle Kinematics Strategies**
  - Contains kinematics strategy assets created from `Kinematics_Strategy`.
  - Strategies include:
    - `Actuation Mode` (`Forward_Kinematics_Strategy`)
    - `Motion Capture Mode` (`Motion_Capture_Kinematics_Strategy`)

- **Vehicle Reference Paths**
  - Contains vehicle reference paths in JSON format.
  - These paths are used by vehicle dynamics and control researchers for simulation and testing.

---