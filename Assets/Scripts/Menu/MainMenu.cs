using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void Start()
        {
            foreach (var i in Images)
                i.SetActive(false);
        }

        public GameObject[] Images;
        public void LoadImage(int index)
        {
            foreach (var i in Images)
                i.SetActive(false);
            Images[index].SetActive(true);
        }
    }
}
