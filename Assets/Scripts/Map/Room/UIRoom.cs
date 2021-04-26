using UnityEngine;

namespace Scripts.Map.Room
{
    public class UIRoom : MonoBehaviour
    {
        public static UIRoom S;

        private void Awake()
        {
            S = this;
        }

        public GameObject _uiBlueprint;
        public GameObject _uiStorage, _receptionStorage, _miningStorage, _factoryStorage;
        public GameObject _uiStoragePriority;
    }
}
