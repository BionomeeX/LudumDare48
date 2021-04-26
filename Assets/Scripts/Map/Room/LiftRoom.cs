using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class LiftRoom : ARoom
    {
        public LiftRoom(Vector2Int size, Vector2Int position) : base(size, position)
        { }

        public override float GetCost()
            => .1f;

        protected override string GetNameInternal()
            => "Lift";

        protected override string GetDescriptionInternal()
            => "Can be used to go up and down ... Weeeeeeeeeeeeeeeeeeeeeeeeeee!";
    }
}
