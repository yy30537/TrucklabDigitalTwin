# Application Scripts

This folder contains all the C# source scripts for the application space. The scripts are organized into subfolders based on their functionality and the components they manage. Each subfolder includes a README file with documentation on their contents, classes, and implementation specifics.

## Contents

- [Virtual Entity](#virtual-entity)
- [UI Controller](#ui-controller)
- [Manager](#manager)
- [Event Channel](#event-channel)

---

### Virtual Entity

The **Virtual Entity** folder contains the base classes and utilities for creating and managing Virtual Entities (VEs) within the application. Virtual Entities are abstract representations of objects within the virtual environment, such as vehicles, buildings, and spaces. The scripts in this folder provide the foundational structures, configurations, and mechanisms for instantiating, initializing, and interacting with these entities.

- **Subfolders:**
  - `Building/`
    - Contains scripts for building entities.
  - `Space/`
    - Contains scripts for space entities.
  - `Vehicle/`
    - Contains scripts for vehicle entities.

**For detailed information, refer to [README_Virtual_Entity.md](Virtual%20Entity/README_Virtual_Entity.md).**

---

### UI Controller

The **UI Controller** folder contains scripts that manage the user interface components of the application. The UI controllers handle visibility, interaction, and behavior of various UI objects associated with virtual entities such as vehicles, spaces, and buildings. They facilitate user interaction with the virtual environment, providing mechanisms to display dashboards, respond to user input, and integrate with other systems.

- **Subfolders:**
  - `Application UI/`
    - Contains scripts for main application UI components.
  - `Virtual Entity UI/`
    - Contains scripts for UI components specific to virtual entities.

**For detailed information, refer to [README_UI_Controller.md](UI%20Controller/README_UI_Controller.md).**

---

### Manager

The **Manager** folder contains scripts that manage core functionalities and components in the application. The Manager scripts handle tasks including camera control, drone synchronization, and vehicle reference path management.

- **Subfolders:**
  - `Path_Manager/`
    - Contains scripts responsible for managing vehicle reference paths.

**For detailed information, refer to [README_Manager.md](Manager/README_Manager.md).**

---

### Event Channel

The **Event Channel** folder contains scripts that implement event channels using the `ScriptableObject` architecture in Unity. Event channels are a design pattern used to facilitate decoupled communication between different systems or components within the application. They allow for events to be raised and listened to without the sender needing to know about the receiver.

**For detailed information, refer to [README_Event_Channel.md](Event%20Channel/README_Event_Channel.md).**

---

## Overview of Subfolders

### Virtual Entity

- **Purpose:** Provides the foundational classes and utilities for managing virtual entities in the virtual environment.
- **Components:**
  - **Base Classes:** Abstract classes `VE`, `VE_Config`, and `VE_Creator` define common interfaces and structures.
  - **Utilities:** `VE_OnClick_Getter` for interaction detection.
  - **Entity Types:** Subfolders for `Building`, `Space`, and `Vehicle`, each containing entity-specific scripts and controllers.

### UI Controller

- **Purpose:** Manages the user interface components, handles interaction between the user and the virtual environment.
- **Components:**
  - **Base UI Controller:** Abstract classes like `Base_UI_Controller` providing common UI management functionalities.
  - **Draggable UI:** Scripts enabling draggable UI objects.
  - **Application UI Controllers:** Scripts managing main menu, service menus, system logs, and virtual entity creators.
  - **Virtual Entity UI Controllers:** Scripts handling dashboards and UI interactions for buildings, spaces, and vehicles.

### Manager

- **Purpose:** Centralizes management of components including cameras, drones, and vehicle paths.
- **Components:**
  - **Camera Manager:** Handles camera views, user controls, and integration with virtual entities.
  - **Drone Manager:** Synchronizes drone objects with external tracking systems.
  - **Path Manager:** Manages loading, recording, saving, and visualization of vehicle paths.

### Event Channel

- **Purpose:** Implements a event system using Unity's `ScriptableObject`.
- **Components:**
  - **Generic Event Channels:** Abstract classes for creating custom event channels with parameters.
  - **Void Event Channel:** For events that do not require parameters.
  - **UI Navigation Event Channel:** Specific implementation for UI navigation events with parameter of type string.

---


## Getting Started

To understand the architecture and functionality of the application scripts, it is recommended to read through the README files in each subfolder:

- [Virtual Entity README](Virtual%20Entity/README_Virtual_Entity.md)
- [UI Controller README](UI%20Controller/README_UI_Controller.md)
- [Manager README](Manager/README_Manager.md)
- [Event Channel README](Event%20Channel/README_Event_Channel.md)
