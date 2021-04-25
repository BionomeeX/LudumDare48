using Scripts.Map.Room.ModulableRoom;
using Scripts.ScriptableObjects;
using UnityEngine;

namespace Scripts.Exploration
{
    public class Submarine : MonoBehaviour
    {
        private AirlockRoom _base;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            SubmarineManager.S.RegisterSubmarine(this);
        }

        public void GetNewAirlock(AirlockRoom airlock)
        {
            if (_base == null)
            {
                for (int i = 0; i < airlock.Emplacements.Length; i++)
                {
                    if (airlock.Emplacements[i] == null)
                    {
                        airlock.Emplacements[i] = this;
                        _base = airlock;
                        break;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            Vector2? objective = null;
            if (SubmarineManager.S.Objective.HasValue)
            {
                objective = SubmarineManager.S.Objective.Value;
            }
            if (objective == null)
            {
                objective = _base?.Position;
            }
            if (objective != null)
            {
                _rb.velocity =
                    (objective.Value - (Vector2)transform.position) * ConfigManager.S.Config.SubmarineSpeed;
            }
        }
    }
}
