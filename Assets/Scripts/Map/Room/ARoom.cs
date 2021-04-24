using System.Collections.Generic;
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

        public ARoom[] GetNeighborhood()
        {
            List<ARoom> rooms = new List<ARoom>();
            if (RoomUp != null) rooms.Add(RoomUp);
            if (RoomDown != null) rooms.Add(RoomDown);
            if (RoomLeft != null) rooms.Add(RoomLeft);
            if (RoomRight != null) rooms.Add(RoomRight);

            return rooms.ToArray();
        }

        public static float GetDistance(ARoom a, ARoom b)
        {
            return Vector2Int.Distance(a.Position, b.Position);
        }
    }
}
