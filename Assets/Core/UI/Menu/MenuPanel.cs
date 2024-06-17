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
    public class MenuPanel : MonoBehaviour, IDragHandler
    {
        public GameObject panel;
        private bool isActive = true;
        
        private void Start()
        {
            panel.SetActive(true);
        }

        public void Toggle()
        {
            isActive = !isActive;
            panel.SetActive(isActive);
        }
        
        public void OnDrag(PointerEventData data)
        {
            float x = data.position.x;
            float y = data.position.y;
            panel.transform.position = new Vector3(x, y, 0f);
        }

    }
}
