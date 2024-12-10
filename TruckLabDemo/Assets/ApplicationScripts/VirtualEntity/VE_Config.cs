using ApplicationScripts.EventChannel;
using UnityEngine;

namespace ApplicationScripts.VirtualEntity
{
    /// <summary>
    /// Abstract base class for configuration objects of Virtual Entities (VE).
    /// Defines shared properties for UI integration and identification.
    /// </summary>
    public abstract class VE_Config : ScriptableObject
    {
        /// <summary>
        /// Unique identifier for the configuration.
        /// </summary>
        public int Id;

        /// <summary>
        /// Display name or descriptive title for the configuration.
        /// </summary>
        public string Name;

        /// <summary>
        /// Reference to the UI template GameObject associated with this configuration.
        /// Used for visual representation in the user interface.
        /// </summary>
        public GameObject UiTemplate;

        /// <summary>
        /// Reference to the event channel for UI navigation events.
        /// Facilitates communication between the UI and other systems.
        /// </summary>
        public EventChannel_UI_Navigation UiNavigationEventChannel;
    }
}