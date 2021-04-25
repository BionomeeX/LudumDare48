using Scripts.Resources;

namespace Scripts.Map.Room.ModulableRoom
{
    public abstract class AModulableRoom
    {
        public virtual bool IsEmpty()
            => false;

        public abstract string GetName();

        public abstract string GetDescription();

        public ResourceStock Stock;
    }
}
