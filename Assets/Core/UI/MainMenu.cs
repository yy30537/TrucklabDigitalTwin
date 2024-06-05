using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class MainMenu : MonoBehaviour, IDragHandler
    {
        public GameObject mainMenuObject;
        private bool isActive = false;
        
        private void Start()
        {
            mainMenuObject.SetActive(false);
        }

        public void Toggle()
        {
            isActive = !isActive;
            mainMenuObject.SetActive(isActive);
        }
        
        public void OnDrag(PointerEventData data)
        {
            float x = data.position.x;
            float y = data.position.y;
            mainMenuObject.transform.position = new Vector3(x, y, 0f);
        }

    }
}
