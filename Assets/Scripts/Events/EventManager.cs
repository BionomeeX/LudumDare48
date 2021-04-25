using Scripts.Enemies;
using Scripts.Exploration;
using Scripts.Map;
using Scripts.Map.Room;
using UnityEngine;

namespace Scripts.Events
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager S;

        private void Awake()
        {
            S = this;
        }

        public void NotifyManager(Event e, object o)
        {
            if (e == Event.BlueprintDrawn)
            {
                EnemyManager.S.RecalculateZone();
            }
            else if (e == Event.RoomSetType)
            {
                SubmarineManager.S.UpdateRoom((GenericRoom)o);
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
