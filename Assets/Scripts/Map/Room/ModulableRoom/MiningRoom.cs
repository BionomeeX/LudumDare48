namespace Scripts.Map.Room.ModulableRoom
{
    public class MiningRoom : AModulableRoom
    {
        public override string GetName()
            => "Mining";

        public override string GetDescription()
            => "Gather materials for you, must be placed close to an ore source";
    }
}
