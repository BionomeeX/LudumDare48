using UnityEngine;

namespace Scripts.Map.Room
{
    public class ReceptionRoom : ARoom
    {
        public ReceptionRoom(Vector2Int size, Vector2Int position) : base(size, position)
        { }

        public override float GetCost()
            => 1f;
    }
}
