using Scripts.ScriptableObjects;

namespace Scripts.Map.Room.ModulableRoom
{
    public class MiningRoom : AModulableRoom
    {
        public MiningRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.MiningStorageMaxSize);
        }

        public override string GetName()
            => "Mining";

        public override string GetDescription()
            => "Gather materials for you, must be placed close to an ore source";

        public override bool IsMining()
            => true;
    }
}
