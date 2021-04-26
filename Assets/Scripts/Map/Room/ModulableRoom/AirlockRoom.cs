using Scripts.Exploration;
using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class AirlockRoom : AModulableRoom
    {
        public AirlockRoom(Vector2 position, int size) : base()
        {
            Emplacements = new Submarine[size];
            Position = position;
        }

        public override string GetName()
            => "Airlock";

        public override string GetDescription()
            => "Launch explorators to discovers new zones and making them buildable";

        public override bool IsAirlock()
            => true;

        public Submarine[] Emplacements;

        public Vector2 Position;
    }
}
