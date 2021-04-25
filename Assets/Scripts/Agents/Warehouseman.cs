using Scripts.Map;
using Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using Scripts.Map.Room;

namespace Scripts.Agents
{

    class Warehousman : AAgent
    {

        public Warehousman() : base("Warehousman")
        {

        }

        public override void OnEventReceived(Events.Event e, object o)
        {
            if (e == Events.Event.RoomCreated)
            {
                if (_actions.Count > 0)
                {
                    List<(List<ARoom> path, Action action)> newactions = new List<(List<ARoom> path, Action action)>();

                    // do not forget to start from the current room and not from the next one !!
                    _actions[0].path.Insert(0, _currentRoom);
                    foreach (var action in _actions)
                    {
                        newactions.Add((
                            Astar.FindPath(action.path[0], action.path[action.path.Count - 1]),
                            action.action
                        ));
                    }
                }
            }
        }

        protected override void DoStartAction()
        {

        }

        private void CheckIfBlueprintNeedResource()
        {
            // check if there any blueprints

            // if there is, reserve a block and ask for its needed resources

            // ask every stocks their amount for the resource

            // do a pathfinding to each stock

            // sort each stocks by distance

            // for each stock generate a path finding from each stocks to the next


        }

        private void CheckIfHighPriorityStockNeedResource()
        {
            // get list of nonfull stocks

            // sort stocks by priority

            // reserve as input

            // search all lowest priority stock if there is any available resources

            // if not, look at the miners

            // if miner or stock, reserve as output

        }

    }
}
