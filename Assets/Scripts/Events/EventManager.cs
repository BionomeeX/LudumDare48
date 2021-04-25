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
