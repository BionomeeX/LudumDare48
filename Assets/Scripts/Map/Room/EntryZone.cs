using UnityEngine;

namespace Scripts.Map.Room
{
    public class EntryZone : ARoom
    {
        public EntryZone() : base(new Vector2Int(5, -1), Vector2Int.zero)
        { }

        public override float GetCost()
            => 100f;

        public override string GetName()
            => "Entry zone";

        public override string GetDescription()
            => null; // This room isn't clickable so the description will never be seen
    }
}
