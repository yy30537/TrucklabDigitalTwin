using UnityEngine;
using UnityEngine.Events;

namespace Application_Scripts.Event_Channel
{
    /// <summary>
    /// Represents a generic ScriptableObject-based event channel.
    /// Allows listeners to subscribe and receive notifications with a parameter of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the parameter passed with the event.</typeparam>
    [CreateAssetMenu(menuName = "Event Channel Generic", fileName = "EventChannel_Generic")]
    public abstract class EventChannel_Generic<T> : ScriptableObject
    {
        /// <summary>
        /// The UnityAction that listeners subscribe to for this event.
        /// Listeners will be invoked with a parameter of type <typeparamref name="T"/>.
        /// </summary>
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction.")]
        public UnityAction<T> OnEventRaised;

        /// <summary>
        /// Raises the event, invoking all subscribed listeners with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of type <typeparamref name="T"/> to pass to the listeners.</param>
        public void RaiseEvent(T parameter)
        {
            // Check if there are any listeners subscribed to the event.
            if (OnEventRaised == null)
                return;

            // Invoke all subscribed listeners, passing the parameter.
            OnEventRaised.Invoke(parameter);
        }
    }
}