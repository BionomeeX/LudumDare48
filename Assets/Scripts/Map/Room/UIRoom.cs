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

        public GameObject _uiStorage, _receptionStorage;
    }
}
