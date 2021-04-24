using UnityEngine;

namespace Scripts.Map.Room
{
    public abstract class ARoom
    {
        public ARoom(Vector2Int size, Vector2Int position)
        {
            Size = size;
            Position = position;
            _isBuilt = false;
        }

        private bool _isBuilt;
        public Vector2Int Size {
            private set;
            get;
        }

        public Vector2Int Position {
            private set;
            get;
        }
    }
}
