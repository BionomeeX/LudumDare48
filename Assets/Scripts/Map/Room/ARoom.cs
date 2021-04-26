using Scripts.Map.Blueprints;
using Scripts.UI.RoomUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Scripts.Map.Room
{
    [System.Serializable]
    public abstract class ARoom
    {
        public static int id = 0;
        public int Myid;

        public ARoom(Vector2Int size, Vector2Int position)
        {
            Myid = ++id;
            Size = size;
            Position = position;
            IsBuilt = false;

            RoomUp = null;
            RoomDown = null;
            RoomLeft = null;
            RoomRight = null;
        }

        public string GetName()
        {
            if (Requirement != null)
                return "Blueprint";
            if (!IsBuilt)
                return "Construction";
            return GetNameInternal();
        }

        public string GetDescription()
        {
            if (Requirement != null)
                return "The room was ordered by a commander and is waiting for its materials";
            if (!IsBuilt)
                return "The room is being build";
            return GetDescriptionInternal();
        }

        protected abstract string GetNameInternal();
        protected abstract string GetDescriptionInternal();

        public bool IsBuilt { set; get; }
        public Vector2Int Size { private set; get; }
        public Vector2Int Position { private set; get; }

        public ARoom RoomUp, RoomDown, RoomLeft, RoomRight;

        public GameObject GameObject;

        public GameObject Sign;

        public Requirement Requirement;

        public ARoom[] GetNeighborhood()
        {
            List<ARoom> rooms = new List<ARoom>();
            if (RoomUp != null) rooms.Add(RoomUp);
            if (RoomDown != null) rooms.Add(RoomDown);
            if (RoomLeft != null) rooms.Add(RoomLeft);
            if (RoomRight != null) rooms.Add(RoomRight);

            return rooms.ToArray();
        }

        public abstract float GetCost();

        public static float GetDistance(ARoom a, ARoom b)
        {
            return Vector2Int.Distance(a.Position, b.Position);
        }

        public override string ToString()
        {
            return $"Room {Size.x}_{Size.y} at ({Position.x};{Position.y}) num neighbor : {GetNeighborhood().Length}";
        }

        public GameObject GetDescriptionPanel()
            => UIRoom.S._uiBlueprint;

        public void SetupConfigPanel(GameObject go)
        {
            var c = go.GetComponent<ReceptionUI>();
            c.StorageInfoText.text = string.Join("\n", Requirement.GetMissingResources().Select((x) =>
            {
                return x.Item1 + ": " + x.Item2;
            }));
        }
    }
}
