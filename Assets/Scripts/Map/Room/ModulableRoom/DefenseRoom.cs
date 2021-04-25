namespace Scripts.Map.Room.ModulableRoom
{
    public class DefenseRoom : AModulableRoom
    {
        public override string GetName()
            => "Defense";

        public override string GetDescription()
            => "Will automatically fire at any enemy at range";
    }
}
