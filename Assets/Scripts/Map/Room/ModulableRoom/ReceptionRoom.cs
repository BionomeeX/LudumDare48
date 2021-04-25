using Scripts.ScriptableObjects;
using System.Linq;

namespace Scripts.Map.Room.ModulableRoom
{
    public class ReceptionRoom : AModulableRoom
    {
        public ReceptionRoom(GenericRoom r) : base()
        {
            Stock = new Resources.ResourceStock(r, ConfigManager.S.Config.StartingResources.Select(x => x.Amount).Sum());
        }

        public override string GetName()
            => "Reception";

        public override string GetDescription()
            => "Your new agents will come here first";
    }
}
