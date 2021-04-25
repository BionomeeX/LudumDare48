using UnityEngine;

namespace Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameConfig", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Storage")]
        public int NormalStorageMaxSize, FactoryStorageMaxSize, MiningStorageMaxSize;

        [Header("Starting info")]
        public ResourceInfo[] StartingResources;
    }
}
