using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Agents
{


    public class AgentControler : MonoBehaviour
    {

        public enum Action
        {
            Idle, Building, Mining, Moving,
        }

        private Action _currentAction;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        private ChooseJob( /*??*/ )
        {
            if (_currentAction == Action.Idle && Unity.Random.Range(0f, 1f) < 0.1f)
            {
                // go to building mode
                // 1) choose a room from where to expand
                List<(ARoom room, float weight)> choices = new List<(ARoom room, float weight)>();
                // foreach (ARoom room in ??)
                {
                    if(room.CanExpand()) {
                        choices.Add((room , (float)room.Position.x + 3 * room.Position.y));
                    }
                }

                float total;
                foreach((var _, var weight) in choices) {
                    total += weight;
                }
                foreach((var _, var weight) in choices) {
                    weight /= total;
                }

        }
        }

    }

}
