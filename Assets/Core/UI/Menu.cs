using UnityEngine;

namespace Core
{
    
    public abstract class Menu : MonoBehaviour
    {
        public string menuName;
        [Header("UI Event Channels")]
        public MenuNavigationEventChannel ecToggle;

        [Header("UI Components")]
        public GameObject menuObject;
        
        public virtual void Init()
        {
            ecToggle.onEventRaised += Toggle;
            menuObject.SetActive(false);
        }
        
        public void Toggle(string menuNameToNavigateTo)
        {
            if (menuName == menuNameToNavigateTo)
            {
                menuObject.SetActive(true);
            }
            else
            {
                menuObject.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            if (ecToggle != null)
            {
                ecToggle.onEventRaised -= Toggle;
            }
        }
        
    }
}
