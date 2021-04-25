using System.Collections;
using System.Collections.Generic;

namespace Scripts.Map.Blueprints
{

    public class MasterBlueprint {

        private List<Blueprint> _blueprints;
        public int Owner;

        public MasterBlueprint(List<Blueprint> bps, int owner) {
            _blueprints = bps;
            Owner = owner;
        }

    }

}
