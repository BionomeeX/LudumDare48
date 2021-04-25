namespace Scripts.Map.Room.ModulableRoom
{
    public class EmptyRoom : AModulableRoom
    {
        public override bool IsEmpty()
            => true;

        public override string GetName()
            => "Empty";

        public override string GetDescription()
            => "Nothing to see here yet...";
    }
}
