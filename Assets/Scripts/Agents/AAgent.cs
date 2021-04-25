using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Map;
using Scripts.Map.Room;
using Scripts.Resources;
using Scripts.Events;

namespace Scripts.Agents
{
    public abstract class AAgent : MonoBehaviour
    {

        protected int _id;

        protected Dictionary<ResourceType, int> _inventory;

        public static int IdRef = 0;
        protected ARoom _currentRoom;

        private string _childClassName;

        protected List<(List<ARoom> path, Action action)> _actions;

        public enum Action
        {
            Idle,
            Move,
            TakeRessource,
            DropRessource,
            GenerateBlueprint,
        }

        public AAgent(string childClassName)
        {
            _childClassName = childClassName;
            _id = ++IdRef;
            _inventory = new Dictionary<ResourceType, int>();

        }
        public abstract void OnEventReceived(Events.Event e, object o);
        protected abstract void DoStartAction();

        public void Start()
        {
            name = _childClassName + " " + _id;
            DoStartAction();
        }

        // private abstract void

        public void MoveTo(ARoom room)
        {
            _currentRoom = room;
            transform.position = new Vector3(
                _currentRoom.Position.x + 0.5f,
                -_currentRoom.Position.y + 0.1f,
                0f
            );
        }

        public Action MoveOrGetAction()
        {
            if (_actions.Count > 0)
            {
                if (_actions[0].path.Count > 0)
                {
                    MoveTo(_actions[0].path[0]);
                    _actions[0].path.RemoveAt(0);
                    return Action.Move;
                }
                else
                {
                    Action action = _actions[0].action;
                    _actions.RemoveAt(0);
                    return action;
                }
            }
            return Action.Idle;
        }

        public void TakeRessource()
        {
            // get the ResourceStock assiociated with the current room
            ResourceStock rs = ((GenericRoom)_currentRoom).RoomType.Stock;
            var resourceAndAmount = rs.GetResource(_id);
            if(!_inventory.ContainsKey(resourceAndAmount.resourceType)) {
                _inventory.Add(
                    resourceAndAmount.resourceType,
                    resourceAndAmount.amount
                );
            } else {
                _inventory[resourceAndAmount.resourceType] += resourceAndAmount.amount;
            }
        }

        public void DropRessource()
        {
            // check what ressources the current room need
            //List<(ResourceType type, int amount)> needs = new List<(ResourceType type, int amount)>();
            // ???????????????????????????????
        }



    }

}
