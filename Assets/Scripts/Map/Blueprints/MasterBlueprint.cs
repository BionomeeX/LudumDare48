using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Events;
using Scripts.Map.Room;


namespace Scripts.Map.Blueprints
{

    public class MasterBlueprint
    {

        public List<Blueprint> Blueprints;
        public int Owner;

        public MasterBlueprint(List<Blueprint> bps, int owner)
        {
            Blueprints = bps;
            Owner = owner;
        }

        public bool Contains(ARoom room)
        {
            return Blueprints.Select(bp => bp.RoomRef).Contains(room);
        }
        public void NotifyBlueprintFinished(ARoom room)
        {
            var bp = Blueprints.First(bp => bp.RoomRef == room);
            Blueprints.Remove(bp);
            if(Blueprints.Count == 0){
                EventManager.S.NotifyManager(Events.Event.MasterBlueprintFinished, this);
            }
        }

    }

}
