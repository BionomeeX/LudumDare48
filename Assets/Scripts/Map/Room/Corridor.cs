using UnityEngine;

namespace Scripts.Map.Room
{
    public class Corridor : ARoom
    {
        public Corridor(Vector2Int size, Vector2Int position) : base(size, position)
        { }

        public override float GetCost()
            => .1f;

        public override string GetName()
            => "Corridor";

        public override string GetDescription()
            => "Long corridor allowing you to move from one room to another";
    }
}
