# UI Controller

This folder contains scripts that manage the user interface objects of the application. The UI controllers handle visibility, interaction, and behavior of UI objects associated with virtual entities such as vehicles, spaces, and buildings. They facilitate user interaction with the virtual environment, providing mechanisms to display dashboards, respond to user input, and integrate with other systems.

## Contents

- [`Base\_UI\_Controller.cs`](#base\_ui\_controllercs)
- [`Draggable\_UI.cs`](#draggable\_uics)
- **Virtual Entity UI:**
  - [`UI\_Controller\_BuildingDashboards.cs`](#ui\_controller\_buildingdashboardscs)
  - [`UI\_Controller\_SpaceDashboards.cs`](#ui\_controller\_spacedashboardscs)
  - [`UI\_Controller\_VehicleDashboards.cs`](#ui\_controller\_vehicledashboardscs)

---

### `Base\_UI\_Controller.cs`

#### Overview

`Base\_UI\_Controller` is an abstract base class for all UI controllers within the application. It provides common properties and methods for managing UI objects, such as showing, hiding, and toggling visibility. 

#### Key Components

- **Fields:**

  - `bool IsActive`: Indicates whether the UI controller is active.
  - `string UiName`: Name of the UI object.
  - `GameObject UiInstance`: The instance of the UI GameObject.
  - `EventChannel\_UI\_Navigation UiNavigationEventChannel`: Event channel for UI navigation events.
  - `UI\_Controller\_SystemLog SystemLogUiController`: Reference to the system log UI controller.

- **Methods:**

  - `virtual void Init()`: Initializes the UI controller.
  - `public void ShowUi()`: Makes the UI visible.
  - `public void HideUi()`: Hides the UI.
  - `public void ToggleUi()`: Toggles the visibility of the UI.
  - `protected bool IsDashboardVisible()`: Checks if the dashboard is visible.

---

### `Draggable\_UI.cs`

#### Overview

`Draggable\_UI` allows UI objects to be draggable within the application window. It enables users to click and drag UI panels or windows to reposition them on the screen, enhancing the user experience by providing flexibility in arranging UI objects.

#### Key Components

- **Fields:**

  - `RectTransform rectTransform`: Reference to the RectTransform component of the UI object.
  - `Canvas canvas`: Reference to the Canvas component.

- **Methods:**

  - `public void OnBeginDrag(PointerEventData eventData)`: Called when dragging begins.
  - `public void OnDrag(PointerEventData eventData)`: Called during dragging.
  - `public void OnEndDrag(PointerEventData eventData)`: Called when dragging ends.

