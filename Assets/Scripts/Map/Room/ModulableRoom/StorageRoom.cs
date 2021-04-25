namespace Scripts.Map.Room.ModulableRoom
{
    public class StorageRoom : AModulableRoom
    {
        public StorageRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r);
        }

        public override string GetName()
            => "Storage";

        public override string GetDescription()
            => "All your materials are stored here, without it you won't be able to mine or build anything";
    }
}
