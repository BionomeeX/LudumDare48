using UnityEngine;

namespace Scripts.ScriptableObjects
{
    public class ConfigManager : MonoBehaviour
    {
        public static ConfigManager S;

        public GameConfig Config;

        private void Awake()
        {
            S = this;
        }
    }
}
