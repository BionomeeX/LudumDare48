using Scripts.Map.Room;
using Scripts.Map.Room.ModulableRoom;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Exploration
{
    public class SubmarineManager : MonoBehaviour
    {
        public static SubmarineManager S;

        [HideInInspector]
        public Vector2 Objective;

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
            }
        }

        public void PlaceFlag()
        {
            Cursor.SetCursor(_flagIcon, Vector2.zero, CursorMode.Auto);
            _isPlacementMode = true;
        }

        public void RemoveSubmarinePlacementMode()
        {
            _isPlacementMode = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
