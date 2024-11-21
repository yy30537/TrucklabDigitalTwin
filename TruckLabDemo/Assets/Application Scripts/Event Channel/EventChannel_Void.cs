using UnityEngine;
using UnityEngine.Events;

namespace Application_Scripts.Event_Channel
{
    /// <summary>
    /// Represents a ScriptableObject-based event channel for void events.
    /// Allows listeners to subscribe to and be notified when the event is raised.
    /// </summary>
    [CreateAssetMenu(menuName = "Event Channel Void", fileName = "EventChannel_Void")]
    public class EventChannel_Void : ScriptableObject
    {
        /// <summary>
        /// The UnityAction that listeners subscribe to for this event.
        /// </summary>
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction.")]
        public UnityAction OnEventRaised;

        /// <summary>
        /// Raises the event, invoking all subscribed listeners.
        /// </summary>
        public void RaiseEvent()
        {
            // Check if there are any listeners subscribed to the event.
            if (OnEventRaised == null)
                return;

            // Invoke the subscribed listeners.
            OnEventRaised.Invoke();
        }
    }
}