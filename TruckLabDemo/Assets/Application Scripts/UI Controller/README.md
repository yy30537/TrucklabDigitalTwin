# UI Controller

This folder contains scripts that manage the user interface components of the simulation application. The UI controllers handle visibility, interaction, and behavior of various UI elements associated with virtual entities such as vehicles, spaces, and buildings. They facilitate user interaction with the simulation environment, providing mechanisms to display dashboards, respond to user input, and integrate with other systems.

## Contents

- [`Base_UI_Controller.cs`](#base_ui_controllercs)
- [`Draggable_UI.cs`](#draggable_uics)
- **Virtual Entity UI:**
  - [`UI_Controller_BuildingDashboards.cs`](#ui_controller_buildingdashboardscs)
  - [`UI_Controller_SpaceDashboards.cs`](#ui_controller_spacedashboardscs)
  - [`UI_Controller_VehicleDashboards.cs`](#ui_controller_vehicledashboardscs)

---

### `Base_UI_Controller.cs`

#### Overview

`Base_UI_Controller` serves as an abstract base class for all UI controllers within the application. It provides common properties and methods for managing UI elements, such as showing, hiding, and toggling visibility. This class ensures consistency across different UI components and facilitates easier management and extension of UI functionalities.

#### Key Components

- **Fields:**

  - `bool IsActive`: Indicates whether the UI controller is active.
  - `string UiName`: Name of the UI element.
  - `GameObject UiInstance`: The instance of the UI GameObject.
  - `EventChannel_UI_Navigation UiNavigationEventChannel`: Event channel for UI navigation events.
  - `UI_Controller_SystemLog SystemLogUiController`: Reference to the system log UI controller.

- **Methods:**

  - `virtual void Init()`: Initializes the UI controller.
  - `public void ShowUi()`: Makes the UI visible.
  - `public void HideUi()`: Hides the UI.
  - `public void ToggleUi()`: Toggles the visibility of the UI.
  - `protected bool IsDashboardVisible()`: Checks if the dashboard is visible.

---

### `Draggable_UI.cs`

#### Overview

`Draggable_UI` allows UI elements to be draggable within the application window. It enables users to click and drag UI panels or windows to reposition them on the screen, enhancing the user experience by providing flexibility in arranging UI components.

#### Key Components

- **Fields:**

  - `RectTransform rectTransform`: Reference to the RectTransform component of the UI element.
  - `Canvas canvas`: Reference to the Canvas component.

- **Methods:**

  - `public void OnBeginDrag(PointerEventData eventData)`: Called when dragging begins.
  - `public void OnDrag(PointerEventData eventData)`: Called during dragging.
  - `public void OnEndDrag(PointerEventData eventData)`: Called when dragging ends.

