using Scripts.Exploration;

namespace Scripts.Map.Room.ModulableRoom
{
    public class AirlockRoom : AModulableRoom
    {
        public AirlockRoom(int size) : base()
        {
            Emplacements = new Submarine[size];
        }

        public override string GetName()
            => "Airlock";

        public override string GetDescription()
            => "Launch explorators to discovers new zones and making them buildable";

        public override bool IsAirlock()
            => true;

        public Submarine[] Emplacements;
    }
}
