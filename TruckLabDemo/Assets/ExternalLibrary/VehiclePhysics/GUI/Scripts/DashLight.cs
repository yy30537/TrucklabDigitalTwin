using UnityEngine;
using UnityEngine.UI;

namespace VehiclePhysics.GUI.Scripts
{
    public class DashLight : MonoBehaviour
    {
        private bool active;

        private Image icon;
        private Color originalColor;
        private Color activeColor = Color.white;

        void Start()
        {
            icon = GetComponent<Image>();
            originalColor = icon.color;
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;

                if(active)
                {
                    icon.color = activeColor;
                }
                else
                {
                    icon.color = originalColor;
                }
            }
        }
    }
}

