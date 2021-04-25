using Scripts.Map;
using Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using Scripts.Map.Room;
using Scripts.Resources;
using System.Linq;
using Scripts.ScriptableObjects;
using UnityEngine;


namespace Scripts.Agents
{

    class Warehouseman : AAgent
    {

        public Warehouseman() : base("Warehousman")
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
            if (e == Events.Event.RoomDestroyed)
            {
                // oups
            }
            if (e == Events.Event.BlueprintDrawn)
            {
                Debug.Log("Blueprint Drawn event received for warehouseman");
                if (IsIdle)
                {
                    Debug.Log("I'm available");
                    ChooseAction();
                    DoNextAction();
                }
            }
        }

        public override void ChooseAction()
        {
            if (CheckIfTurretNeedResource())
            {
                return;
            }
            if (CheckIfFactoryNeedResource())
            {
                return;
            }
            if (CheckIfBlueprintNeedResource())
            {
                Debug.Log("CheckIfBlueprint !");
                return;
            }
            if (CheckIfHighPriorityStockNeedResource())
            {
                return;
            }
        }

        public override void DoSpecialAction(Action action)
        {
            if (action == Action.TakeRessource)
            {
                TakeRessource();
            }
            if (action == Action.DropRessource)
            {
                DropRessource();
            }
        }

        public void TakeRessource()
        {
            // get the ResourceStock assiociated with the current room
            var resourceAndAmount = ((GenericRoom)_currentRoom).RoomType.Stock.GetResource(_id);
            if (!_inventory.ContainsKey(resourceAndAmount.resourceType))
            {
                _inventory.Add(
                    resourceAndAmount.resourceType,
                    resourceAndAmount.amount
                );
            }
            else
            {
                _inventory[resourceAndAmount.resourceType] += resourceAndAmount.amount;
            }
        }

        public void DropRessource()
        {
            // check what ressources the current room need
            //List<(ResourceType type, int amount)> needs = new List<(ResourceType type, int amount)>();
            // ???????????????????????????????
        }

        protected override void DoStartAction()
        {
            DoNextAction();
        }

        private bool CheckIfTurretNeedResource()
        {
            // Get list of all turrets that need ressources
            // List<ARoom> turrets = MapManager.S.GetAllTurrets().Where(t => t.NeedRessource(ResourceType.AMMO)).ToList();

            // // for each turret, pathfind and sort by distance
            // List<(List<Map.ARoom> path, float distance)> orderedTurretPaths = turrets.Select(
            //     t => Astar.FindPath(_currentRoom, t)
            // ).Select(
            //     path => (path, Astar.ComputePathWeight(path))
            // ).OrderBy(pf => pf.Item2).ToList();

            // // Lock the turret for filling and reserve the first one
            // ARoom turretTarget = orderedTurretPaths[0];

            // List<ARoom> stocks = MapManager.S.GetAllStockRoom().Where(s => ((GenericRoom)s).RoomType.Stock.CheckResourceByType(ResourceType.AMMO));



            //

            return false;
        }
        private bool CheckIfFactoryNeedResource()
        {
            return false;
        }

        private bool CheckIfBlueprintNeedResource()
        {
            Debug.Log("Any blueprints need some loving ?");
            // check if there any blueprints
            var blueprints = MapManager.S.GetAllAccessibleBlueprint().Select(
                b => Astar.FindPath(_currentRoom, b)
            ).Select(
                path => (path, Astar.ComputePathWeight(path))
            ).OrderBy(pf => pf.Item2).ToList();

            Debug.Log("blueprints found : " + blueprints.Count);
            // if there is, reserve a block and ask for its needed resources
            if (blueprints.Count > 0)
            {
                foreach (var blueprint in blueprints)
                {
                    // check the ressource needed for the blueprint
                    List<(ResourceType type, int amount)> resourcesNeeded = blueprint.path.Last().Requirement.GetRequirements().ToList();
                    Debug.Log("Resources needed : " + resourcesNeeded.Count);
                    foreach (var resourceAmount in resourcesNeeded)
                    {
                        Debug.Log("  Need " + resourceAmount.amount);
                        if (FindPathForResource(resourceAmount.type, resourceAmount.amount, 0))
                        {
                            // register
                            // add path from the last element to the blueprint
                            _actions.Add(
                                (Astar.FindPath(_actions.Last().path.Last(), blueprint.path.Last()), Action.DropRessource)
                            );
                            return true;
                        }
                    }
                }
            }
            Debug.Log("No resources for the blueprints ...");
            return false;
        }
        private bool CheckIfHighPriorityStockNeedResource()
        {
            // get list of nonfull stocks

            // sort stocks by priority

            // reserve as input

            // search all lowest priority stock if there is any available resources

            // if not, look at the miners

            // if miner or stock, reserve as output

            return false;
        }

        private bool FindPathForResource(ResourceType resource, int amount, int priority)
        // hypothesis:
        // 1) inventory is empty
        // 2) actions list is empty
        {
            var stocks = MapManager.S.GetAllStockRoom().Select(
                room => (room, ((GenericRoom)room).RoomType.Stock.CheckResourceByType(resource))
            ).Where(
                roomAmount => roomAmount.Item2 > 0
            ).Select(
                roomAmount => (roomAmount.Item1, roomAmount.Item2, Astar.FindPath(_currentRoom, roomAmount.Item1))
            ).Select(
                rap => (rap.Item1, rap.Item2, rap.Item3, Astar.ComputePathWeight(rap.Item3))
            ).OrderBy(
                rapw => rapw.Item4
            ).ToList();

            // go in ascending order to fill my inventory
            int myamount = ConfigManager.S.Config.NbOfResourcePerTransportation;
            bool first = true;
            foreach (var rapw in stocks)
            {
                if (rapw.Item2 >= myamount)
                {
                    ((GenericRoom)rapw.Item1).RoomType.Stock.ReserveResource(resource, myamount, _id);
                    _actions.Add((first ? rapw.Item3 : Astar.FindPath(_actions.Last().path.Last(), rapw.Item1), Action.TakeRessource));
                    return true;
                }
                else
                {
                    ((GenericRoom)rapw.Item1).RoomType.Stock.ReserveResource(resource, rapw.Item2, _id);
                    myamount -= rapw.Item2;
                    _actions.Add((first ? rapw.Item3 : Astar.FindPath(_actions.Last().path.Last(), rapw.Item1), Action.TakeRessource));
                    first = false;
                }
            }
            if (myamount > 0)
            {
                // failled to get all resources => we empty everything
                foreach (var action in _actions)
                {
                    ((GenericRoom)action.path.Last()).RoomType.Stock.CancelAllReservations(_id);
                }
                _actions.Clear();
                return false;
            }
            return true;
        }

    }
}
