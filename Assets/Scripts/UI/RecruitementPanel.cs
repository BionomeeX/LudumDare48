using Scripts.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class RecruitementPanel : MonoBehaviour
    {
        [SerializeField]
        private Text _unitDetails;

        public void Enable()
        {
            Dictionary<string, int> agents = new Dictionary<string, int>();
            foreach (var a in EventManager.S.GetAgents())
            {
                if (agents.ContainsKey(a.ClassName)) agents[a.ClassName]++;
                else agents.Add(a.ClassName, 1);
            }
            var list = agents.Select(x => (x.Key, x.Value)).OrderBy(x => x.Key);
            _unitDetails.text = string.Join("\n", list.Select(x => x.Key + ": " + x.Value));
        }
    }
}
