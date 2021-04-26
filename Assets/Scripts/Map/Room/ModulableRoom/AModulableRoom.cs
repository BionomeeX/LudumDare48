using Scripts.Resources;
using System;
using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public abstract class AModulableRoom
    {
        public virtual bool IsEmpty()
            => false;

        public abstract string GetName();

        public abstract string GetDescription();

        public virtual GameObject GetDescriptionPanel()
            => null;

        public virtual void SetupConfigPanel(GameObject go)
            => throw new NotImplementedException();

        public virtual bool IsAirlock()
            => false;
        public virtual bool IsMining()
            => false;
        public virtual bool IsFactory()
            => false;

        public ResourceStock Stock;
    }
}
