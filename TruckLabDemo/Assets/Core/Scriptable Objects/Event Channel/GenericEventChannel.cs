using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    [CreateAssetMenu(menuName = "Event Channel Generic", fileName = "GenericEventChannel")]
    public abstract class GenericEventChannel<T> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction<T> onEventRaised;
 
        public void RaiseEvent(T parameter)
        {

            if (onEventRaised == null)
                return;

            onEventRaised.Invoke(parameter);

        }
    }
}