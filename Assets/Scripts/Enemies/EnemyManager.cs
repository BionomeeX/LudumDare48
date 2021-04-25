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
            Debug.Log(minY);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
        }
    }
}
