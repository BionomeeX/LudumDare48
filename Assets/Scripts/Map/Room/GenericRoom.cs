using Scripts.Map.Room.ModulableRoom;
using UnityEngine;

namespace Scripts.Map.Room
{
    public class GenericRoom : ARoom
    {
        public GenericRoom(Vector2Int size, Vector2Int position) : base(size, position)
        {
            RoomType = new EmptyRoom();
        }

        public override float GetCost()
            => 1f;

        public AModulableRoom RoomType;
    }
}
