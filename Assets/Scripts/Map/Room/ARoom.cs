using UnityEngine;

namespace Scripts.Map.Room
{
    public abstract class ARoom
    {
        public ARoom(Vector2Int size, Vector2Int position)
        {
            _size = size;
            _position = position;
            _isBuilt = false;

            RoomUp = null;
            RoomDown = null;
            RoomLeft = null;
            RoomRight = null;
        }

        private bool _isBuilt;
        private Vector2Int _size;
        private Vector2Int _position;

        public ARoom RoomUp, RoomDown, RoomLeft, RoomRight;
    }
}
