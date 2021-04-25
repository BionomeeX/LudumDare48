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

        [Header("Construction")]
        [Tooltip("How many resources can an agent move in one travel")]
        public int NbOfResourcePerTransportation;
        [Tooltip("Resources required to build one bloc (ex: a room of 2x1 need twice the requirement)")]
        public ResourceInfo[] RequirementPerBloc;
        public float BuildingTime;

        [Header("Exploration")]
        public float SubmarineSpeed;
    }
}
