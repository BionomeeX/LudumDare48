namespace Scripts.Map.Room.ModulableRoom
{
    public class ReceptionRoom : AModulableRoom
    {
        public ReceptionRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r);
        }

        public override string GetName()
            => "Reception";

        public override string GetDescription()
            => "Your new agents will come here first";
    }
}
