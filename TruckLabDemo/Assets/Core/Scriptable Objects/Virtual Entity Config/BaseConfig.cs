using UnityEngine;

namespace Core
{
    public abstract class BaseEntityConfig : ScriptableObject
    {
        public int EntityId;
        public string EntityName;
        public GameObject UiPrefab;

        [Header("Ui Configuration")]
        public Vector2 InitialDashboardPosition;
        public bool IsUiVisible = true;
    }
}