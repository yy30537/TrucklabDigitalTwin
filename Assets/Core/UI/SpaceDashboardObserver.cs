using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class SpaceDashboardObserver : MonoBehaviour
    {
        public SpaceProduct spaceProduct;
        public GameObject dashboardInstance;
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;
        public bool isUpdating = false;

        private Transform mainCanvasParent;
        private int offset = 300;

        
        public void Initialize(SpaceProduct product, Transform canvasParent)
        {
            spaceProduct = product;
            mainCanvasParent = canvasParent;
            spaceProduct.RegisterObserver(this);
            InitSpaceDashboard();
        }

        private void OnDestroy()
        {
            if (spaceProduct != null)
            {
                spaceProduct.RemoveObserver(this);
            }
        }

        private void Update()
        {
            UpdateDashboard();
        }

        public void InitSpaceDashboard()
        {
            dashboardInstance = this.gameObject;
            title = dashboardInstance.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            content = dashboardInstance.transform.Find("Content").GetComponent<TextMeshProUGUI>();
            dashboardInstance.SetActive(true);
        }

        public void UpdateDashboard()
        {
            title.text = $"{spaceProduct.productName}\n";
            content.text = "";
            foreach (var vehicle in spaceProduct.vehiclesInside.Values)
            {
                content.text += $"[{vehicle.productName}] \n";
            }
        }

        private void OnToggleUI()
        {
            isUpdating = !isUpdating;
        }
    }
}
