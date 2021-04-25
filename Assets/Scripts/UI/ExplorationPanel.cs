using Scripts.Map;
using Scripts.Map.Room;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ExplorationPanel : MonoBehaviour
    {
        [SerializeField]
        private Text _warningText;

        [SerializeField]
        private GameObject _panelButtons;

        public void Enable()
        {
            if (MapManager.S.MapRooms.Any((x) =>
            {
                return x is GenericRoom gRoom && gRoom.RoomType.IsAirlock();
            }))
            {
                _warningText.gameObject.SetActive(false);
                _panelButtons.SetActive(true);
            }
            else
            {
                _warningText.gameObject.SetActive(true);
                _panelButtons.SetActive(false);
            }
        }
    }
}
