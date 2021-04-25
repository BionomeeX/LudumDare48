using Scripts.Events;
using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Exploration
{
    public class SubmarineManager : MonoBehaviour
    {
        public static SubmarineManager S;

        [HideInInspector]
        public Vector2? Objective;

        [SerializeField]
        private GameObject _flagPrefab;

        private GameObject _flagInstance;

        private bool _isPlacementMode;

        private List<Submarine> _submarines = new List<Submarine>();

        public void RegisterSubmarine(Submarine s)
            => _submarines.Add(s);

        [SerializeField]
        private Texture2D _flagIcon;

        private void Awake()
        {
            S = this;
        }

        public void UpdateRoom(GenericRoom room)
        {
            if (room.RoomType.IsAirlock())
            {
                var airlock = (AirlockRoom)room.RoomType;
                if (airlock.Emplacements.Any(x => x == null))
                {
                    foreach (var sub in _submarines)
                    {
                        sub.GetNewAirlock(airlock);
                    }
                }
            }
        }

        public void PlaceFlag()
        {
            Cursor.SetCursor(_flagIcon, Vector2.zero, CursorMode.Auto);
            _isPlacementMode = true;
        }

        public void RemoveFlag()
        {
            Objective = null;
            if (_flagInstance != null)
            {
                Destroy(_flagInstance);
                EventManager.S.NotifyManager(Events.Event.ExplorationFlagUnset, null);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _isPlacementMode)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = -Camera.main.transform.position.z;
                var pos = Camera.main.ScreenToWorldPoint(mousePos);
                Objective = pos;
                if (_flagInstance == null)
                {
                    _flagInstance = Instantiate(_flagPrefab);
                }
                _flagInstance.transform.position = pos;
                EventManager.S.NotifyManager(Events.Event.ExplorationFlagSet, null);
                RemoveSubmarinePlacementMode();
            }
        }

        public void RemoveSubmarinePlacementMode()
        {
            _isPlacementMode = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
