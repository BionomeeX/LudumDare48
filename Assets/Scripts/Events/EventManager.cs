using Scripts.Enemies;
using Scripts.Exploration;
using Scripts.Map;
using Scripts.Map.Room;
using UnityEngine;
using Scripts.Agents;
using System.Collections.Generic;
using Scripts.Sounds;
using System.Collections.ObjectModel;

namespace Scripts.Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager S;

        private List<AAgent> _agents;
        public ReadOnlyCollection<AAgent> GetAgents()
            => _agents.AsReadOnly();
        private void Awake()
        {
            S = this;
            _agents = new List<AAgent>();
        }

        public void Subscribe(AAgent agent){
            Debug.Log("Subscribe : " + agent.name);
            _agents.Add(agent);
        }

        public void NotifyManager(Event e, object o)
        {
            if (e == Event.BlueprintDrawn)
            {
                Debug.Log("Blueprint Drawn event received");
                EnemyManager.S.RecalculateZone();
                foreach(var agent in _agents){
                    agent.OnEventReceived(e, o);
                }
            }
            else if (e == Event.RoomSetType)
            {
                SubmarineManager.S.UpdateRoom((GenericRoom)o);
            }
            AudioManager.S.ReceiveEvent(e);
        }

        public void ResetAll()
        {
            foreach (var r in MapManager.S.MapRooms)
            {
                if (r.Requirement != null)
                {
                    r.Requirement.ResetAll();
                }
                if (r is GenericRoom gRoom)
                {
                    gRoom.RoomType.Stock?.ResetAll();
                }
            }
        }
    }
}
