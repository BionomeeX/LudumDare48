﻿using UnityEngine;

namespace Scripts.Map.Room.ModulableRoom
{
    public class ModularRoomFactory
    {
        public static AModulableRoom BuildModularRoom(RoomType rType, GenericRoom r)
        {
            switch (rType)
            {
                case RoomType.STORAGE:
                    return new StorageRoom(r);

                case RoomType.AIRLOCK:
                    if (r == null)
                        return new AirlockRoom(Vector2Int.zero, 0);
                    return new AirlockRoom(r.GameObject.transform.position, r.Size.x * r.Size.y);

                case RoomType.DEFENSE:
                    return new DefenseRoom();

                case RoomType.FACTORY:
                    return new FactoryRoom(r);

                case RoomType.MINING:
                    return new MiningRoom(r);

                case RoomType.EMPTY:
                    return new EmptyRoom();

                default:
                    throw new System.ArgumentException("Can't build modular room of type " + rType, nameof(rType));
            }
        }

        private ModularRoomFactory()
        { }
    }
}
