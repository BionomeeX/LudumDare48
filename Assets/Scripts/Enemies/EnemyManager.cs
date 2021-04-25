using Scripts.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        private List<int> _baseZone = new List<int>();

        private void Start()
        {
            RecalculateZone();
        }

        public void RecalculateZone()
        {
            var rooms = MapManager.S.MapRooms;
            int minY = rooms.Max(x => x.Position.y + x.Size.y);
            for (int y = 0; y < minY; y++)
            {
                var r = rooms
                    .Where((x) => {
                        return y >= x.Position.y && y <= x.Position.y + x.Size.y;
                    })
                    .Max(x => x.Position.x + x.Size.x);
                Debug.Log(r);
                _baseZone.Add(r);
            }
        }

        private void OnDrawGizmos()
        {
            if (_baseZone.Count == 0)
                return;
            Gizmos.color = Color.black;
            int i = 0;
            for (; i < _baseZone.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(_baseZone[i] + .1f, -i), new Vector2(_baseZone[i + 1] + .1f, -i - 1));
            }
            Gizmos.DrawLine(new Vector2(_baseZone[i] + .1f, -i), new Vector2(_baseZone[i] + .1f, -i - 1));
        }
    }
}
