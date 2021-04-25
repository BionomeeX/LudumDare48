using Scripts.Map;
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
                if (Vector2.Distance(transform.position, objective.Value) < 1f)
                    _rb.velocity = Vector2.zero;
                else
                {
                    _rb.velocity =
                        (objective.Value - (Vector2)transform.position).normalized * ConfigManager.S.Config.SubmarineSpeed;
                    var r = ConfigManager.S.Config.SubmarineRadius;
                    for (int x = -r; x <= r; x++)
                    {
                        for (int y = -r; y <= r; y++)
                        {
                            MapManager.S.DiscoverTile(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(-transform.position.y) + y);
                        }
                    }
                }
            }
        }
    }
}
