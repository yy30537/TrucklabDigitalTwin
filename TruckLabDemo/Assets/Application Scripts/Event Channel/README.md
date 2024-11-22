# Event Channel

This folder contains scripts that implement event channels using the `ScriptableObject` architecture in Unity. Event channels are a design pattern used to facilitate decoupled communication between different systems or components within the application. They allow for events to be raised and listened to without the sender needing to know about the receiver, promoting loose coupling and modularity in the application's architecture.

## Contents

- [`EventChannel_Generic.cs`](#eventchannel_genericcs)
- [`EventChannel_Void.cs`](#eventchannel_voidcs)
- [`EventChannel_UI_Navigation.cs`](#eventchannel_ui_navigationcs)

---

### `EventChannel_Generic.cs`

#### Overview

`EventChannel_Generic<T>` is an abstract class that represents a generic event channel using Unity's `ScriptableObject`. It allows for events to be raised with a parameter of a specified type `T`, and for listeners to subscribe and receive notifications with that parameter. This facilitates decoupled communication between different parts of the application.

#### Key Components

- **Type Parameter:**

  - `<T>`: The type of the parameter that will be passed with the event.

- **Fields:**

  - `UnityAction<T> OnEventRaised`: The action that listeners can subscribe to. When the event is raised, all subscribed listeners are invoked with a parameter of type `T`.

- **Methods:**

  - `void RaiseEvent(T parameter)`: Raises the event by invoking all subscribed listeners with the provided parameter.

#### Implementation Details

- **ScriptableObject:**

  - By inheriting from `ScriptableObject`, instances of `EventChannel_Generic<T>` can be created as assets in Unity, allowing for easy assignment and management within the Unity Editor.

- **Event Invocation:**

  - The `RaiseEvent` method checks if there are any listeners subscribed before invoking them to prevent null reference exceptions.

---

### `EventChannel_Void.cs`

#### Overview

`EventChannel_Void` represents an event channel for events that do not require any parameters. It allows listeners to subscribe to and be notified when the event is raised. It is useful for signaling events where no additional data needs to be passed.

#### Key Components

- **Fields:**

  - `UnityAction OnEventRaised`: The action that listeners can subscribe to. When the event is raised, all subscribed listeners are invoked.

- **Methods:**

  - `void RaiseEvent()`: Raises the event by invoking all subscribed listeners.

#### Implementation Details

- **ScriptableObject:**

  - As with other event channels, `EventChannel_Void` inherits from `ScriptableObject` to enable easy creation and management within the Unity Editor.

- **Event Invocation:**

  - The `RaiseEvent` method includes a null check before invoking listeners to ensure safety.

---

### `EventChannel_UI_Navigation.cs`

#### Overview

`EventChannel_UI_Navigation` is a specific implementation of `EventChannel_Generic<string>` designed for UI navigation events. It allows listeners to respond to navigation events triggered with a `string` parameter, such as navigating to specific UI elements by name.

#### Key Components

- **Inheritance:**

  - Inherits from `EventChannel_Generic<string>`.

- **Usage:**

  - Can be used to handle events where the parameter is a string identifier for a UI element or menu.

#### Implementation Details

- **ScriptableObject:**

  - By creating instances of this class as assets, UI navigation events can be managed and assigned in the Unity Editor.

- **Event Handling:**

  - Since it inherits from `EventChannel_Generic<string>`, it utilizes the same mechanisms for raising events and notifying listeners.

---

## Notes

- **Event Channels Pattern:**

  - The use of event channels promotes decoupled communication, making the system more modular and easier to maintain.

- **UnityAction Delegates:**

  - Listeners subscribe to events using `UnityAction` delegates, which are part of the `UnityEngine.Events` namespace.

- **ScriptableObject Assets:**

  - By utilizing `ScriptableObject`, event channels can be created and assigned in the Unity Editor without requiring instances to be attached to GameObjects.

- **Null Checks:**

  - The event raising methods include null checks to prevent errors when no listeners are subscribed.

