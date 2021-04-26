using Scripts.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Extraction
{
    public class MiningManager : MonoBehaviour
    {
        private List<GameObject> _metals = new List<GameObject>();

        [SerializeField]
        private GameObject _metalPrefab;

        private void Start()
        {
            for (int i = 0; i < 4; i++)
            {
                foreach (var m in ConfigManager.S.Config.MiningResources)
                {
                    if (m.LayerFrom >= i && m.LayerTo <= i)
                    {
                        var amount = Random.Range(ConfigManager.S.Config.MinMetalPerLayer, ConfigManager.S.Config.MaxMetalPerLayer);
                        var go = Instantiate(_metalPrefab, new Vector2(0f,
                            Random.Range(i * ConfigManager.S.Config.LayerYSize,
                            (i + 1) * ConfigManager.S.Config.LayerYSize)), Quaternion.identity);
                        go.GetComponent<MeshRenderer>().material = m.Material;
                    }
                }
            }
        }
    }
}
