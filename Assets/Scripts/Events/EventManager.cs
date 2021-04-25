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

    }
}
