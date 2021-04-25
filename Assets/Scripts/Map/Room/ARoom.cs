using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Map.Room
{
    [System.Serializable]
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

        public GameObject GameObject;

        public ARoom[] GetNeighborhood()
        {
            List<ARoom> rooms = new List<ARoom>();
            if (RoomUp != null) rooms.Add(RoomUp);
            if (RoomDown != null) rooms.Add(RoomDown);
            if (RoomLeft != null) rooms.Add(RoomLeft);
            if (RoomRight != null) rooms.Add(RoomRight);

            return rooms.ToArray();
        }

        public abstract float GetCost();

        public static float GetDistance(ARoom a, ARoom b)
        {
            return Vector2Int.Distance(a.Position, b.Position);
        }

        public override string ToString()
        {
            return $"Room {Size.x}_{Size.y} at ({Position.x};{Position.y})";
        }
    }
}
