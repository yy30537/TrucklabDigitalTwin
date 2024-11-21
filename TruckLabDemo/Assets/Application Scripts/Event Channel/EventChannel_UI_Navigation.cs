using UnityEngine;

namespace Application_Scripts.Event_Channel
{
    /// <summary>
    /// Represents an event channel for UI navigation.
    /// Allows listeners to respond to navigation events triggered with a string parameter (e.g., the target UI name).
    /// Inherits from <see cref="EventChannel_Generic{T}"/> with <see cref="string"/> as the generic type.
    /// </summary>
    [CreateAssetMenu(menuName = "UI Navigation Event Channel", fileName = "EventChannel_UI_Navigation")]
    public class EventChannel_UI_Navigation : EventChannel_Generic<string>
    {
        // This class serves as a specific implementation of EventChannel_Generic<string>.
        // It can be used to handle events where the parameter is a string, such as navigating to specific UI elements.
    }
}