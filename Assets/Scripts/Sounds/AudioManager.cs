using UnityEngine;

namespace Scripts.Sounds
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager S;

        private void Awake()
        {
            S = this;
        }

        public void ReceiveEvent(Events.Event e)
        {
            if (e == Events.Event.ExplorationFlagSet)
            {
                PlayAudio(_explorationSet);
            }
            else if (e == Events.Event.ExplorationFlagUnset)
            {
                PlayAudio(_yes);
            }
        }

        public void PlayAudio(AudioClip[] audio)
        {
            _audioSource.clip = audio[Random.Range(0, audio.Length)];
            _audioSource.Play();
        }

        [SerializeField]
        private AudioClip[] _yes, _no, _explorationSet;

        [SerializeField]
        private AudioSource _audioSource;
    }
}
