using Scripts.Resources;
using System;
using UnityEngine;

namespace Assets.Scripts.Extraction
{
    [Serializable]
    public class MiningResource
    {
        public ResourceType Resource;
        public int LayerFrom;
        public int LayerTo;
        public Material Material;
    }
}
