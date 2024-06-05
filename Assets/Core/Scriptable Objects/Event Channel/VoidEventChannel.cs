using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    [CreateAssetMenu(menuName = "Event Channel Void", fileName = "VoidEventChannel")]
    public class VoidEventChannel : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction")]
        public UnityAction onEventRaised;
    
        
        // method to raise an event in this channel
        public void RaiseEvent()
        {
            if (onEventRaised == null)
                return;
            onEventRaised.Invoke();
        }
    }
}