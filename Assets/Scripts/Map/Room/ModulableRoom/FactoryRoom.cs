using Scripts.ScriptableObjects;

namespace Scripts.Map.Room.ModulableRoom
{
    public class FactoryRoom : AModulableRoom
    {
        public FactoryRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.FactoryStorageMaxSize);
        }

        public override string GetName()
            => "Factory";

        public override string GetDescription()
            => "Convert your ores into refined resources";
    }
}
