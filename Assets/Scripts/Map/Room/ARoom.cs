using UnityEngine;

namespace Scripts.Map.Room
{
    public abstract class ARoom
    {
        public ARoom(Vector2Int size, Vector2Int position)
        {
            Size = size;
            Position = position;
            IsBuilt = false;

            RoomUp = null;
            RoomDown = null;
            RoomLeft = null;
            RoomRight = null;
        }

        public bool IsBuilt { set; get; }
        public Vector2Int Size { private set; get; }
        public Vector2Int Position { private set; get; }

        public ARoom RoomUp, RoomDown, RoomLeft, RoomRight;
    }
}
