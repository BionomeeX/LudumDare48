using System.Collections.Generic;
using UnityEngine;
using Scripts.Map;
using Scripts.Map.Room;
using Scripts.Resources;
using Scripts.ScriptableObjects;
using System.Collections;

namespace Scripts.Agents
{
    public abstract class AAgent : MonoBehaviour
    {
        protected int _id;

        protected Dictionary<ResourceType, int> _inventory;

        public static int IdRef = 0;
        protected ARoom _currentRoom;

        private string _childClassName;

        protected List<(List<ARoom> path, Action action)> _actions = null;

        protected bool IsIdle = false;

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

        public string ClassName;
        public void Start()
        {
            ClassName = _childClassName;
            name = _childClassName + " " + _id;
            _actions = new List<(List<ARoom> path, Action action)>();
            WhereAmI();
            StartCoroutine(FirstAction(Random.Range(.1f, 1f)));
        }

        public IEnumerator FirstAction(float time)
        {
            yield return new WaitForSeconds(time);
            DoStartAction();
        }

        private void WhereAmI()
        {
            if (_currentRoom == null)
            {
                Debug.Log("  Checking for room");
                Debug.Log("  My pos : " + transform.position.x + ", " + transform.position.y);
                foreach (var room in MapManager.S.MapRooms)
                {
                    Debug.Log("    Room pos :");
                    Debug.Log("      (" + room.Position.x + ", " + room.Position.y + ") (" + (room.Position.x + room.Size.x) + ", " + (room.Position.y + room.Size.y) + ")");

                    if (transform.position.x >= room.Position.x && transform.position.x <= room.Position.x + room.Size.x &&
                       -transform.position.y >= room.Position.y && -transform.position.y <= room.Position.y + room.Size.y
                    )
                    {
                        _currentRoom = room;
                        break;
                    }
                }
                // if _currentRoom is still null ... we are in the water !
                if (_currentRoom == null)
                {
                    Debug.Log("  Still no room => aborting");
                }
            }
        }

        public Action MoveOrGetAction()
        {
            Debug.Log("MoveOrGetAction ON");
            if (_actions.Count > 0)
            {
                Debug.Log("Move");
                if (_actions[0].path.Count > 0)
                {
                    Debug.Log("  to : " + _actions[0].path[0].Position.x + ", " + _actions[0].path[0].Position.y);
                    MoveTo(_actions[0].path[0]);
                    _actions[0].path.RemoveAt(0);
                    Debug.Log("Still moves to do");
                    return Action.Move;
                }
                else
                {
                    Action action = _actions[0].action;
                    _actions.RemoveAt(0);
                    Debug.Log("No move, do the action");
                    return action;
                }
            }
            Debug.Log(this.name + " is Idle");
            return Action.Idle;
        }



        public abstract void DoSpecialAction(Action action);
        public abstract bool ChooseAction();

        public void DoNextAction()
        {
            IsIdle = false;
            Debug.Log("Move Or GetAction");
            Action action = MoveOrGetAction();
            if (action != Action.Move && action != Action.Idle)
            {
                Debug.Log("Do Special Action");
                DoSpecialAction(action);
            }
            if (action == Action.Idle)
            {
                if (!ChooseAction())
                {
                    Debug.Log("Idle set to true");
                    IsIdle = true;
                    return;
                }
            }
            if (action != Action.Move)
            {
                Debug.Log("Do the next Action");
                DoNextAction();
            }
        }

        private void FixedUpdate()
        {
            // Debug.Log("Fixed Update ON");
            // if (_objective != null)
            if(_moving)
            {
                // Debug.Log("Moving");
                transform.position += (_objective - transform.position).normalized * ConfigManager.S.Config.NpcSpeed;
                // Debug.Log("Transformation OK");
                if (Vector2.Distance(transform.position, _objective) < .1f)
                {
                    Debug.Log("NEXT");
                    // _objective = null;
                    _moving = false;
                    DoNextAction();
                }
                // Debug.Log("If OK");
            }
            // Debug.Log("Fixed Update OFF");
        }

        private Vector3 _objective;
        private bool _moving = false;
        public void MoveTo(ARoom room)
        {
            Debug.Log("Moving to called");
            _currentRoom = room;
            _objective = new Vector3(
                _currentRoom.Position.x + 0.5f,
                -_currentRoom.Position.y - 0.9f,
                transform.position.z
            );
            _moving = true;
        }

    }

}
