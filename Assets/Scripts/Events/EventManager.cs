using Scripts.Enemies;
using Scripts.Map;
using Scripts.Map.Room;
using UnityEngine;
using Scripts.Agents;
using System.Collections;
using System.Collections.Generic;

namespace Scripts.Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager S;

        private List<AAgent> _agents;
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
