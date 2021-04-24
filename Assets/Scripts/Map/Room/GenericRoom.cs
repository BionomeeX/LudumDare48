using UnityEngine;

namespace Scripts.Map.Room
{
    public class GenericRoom : ARoom
    {
        public GenericRoom(Vector2Int size, Vector2Int position) : base(size, position)
        {
            RoomType = RoomType.EMPTY;
        }

        public override float GetCost()
            => 1f;

        public RoomType RoomType;
    }
}
