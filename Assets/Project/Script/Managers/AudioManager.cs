using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Audio;

namespace WordSearch
{
    public class AudioManager : MonoBehaviour
    {
        public enum SFXClip { Click, }

        [SerializeField] private AudioSource _sfxAS, _gameplaySfxAS;
        [SerializeField] private AudioMixerGroup _mainMixer;
        [SerializeField] private AudioClip[] _sfxClips;

        public bool MuteEnabled;

        private const string MASTER_VOLUME = "MasterVolume";

        public void OnDestroy()
        {
            GameEvents.OnButtonClick -= PlayButtonClick;
            GameEvents.OnGPOneShotSFXReqAsync -= PlayOneShotSFX;
            GameEvents.OnGPSFXChangeReq -= PlayGameplaySFX;
            GameEvents.OnToggleMute -= ToggleMute;
        }

        public void Initialize()
        {
            MuteEnabled = false;

            GameEvents.OnButtonClick += PlayButtonClick;
            GameEvents.OnGPOneShotSFXReqAsync += PlayOneShotSFX;
            GameEvents.OnGPSFXChangeReq += PlayGameplaySFX;
            GameEvents.OnToggleMute += ToggleMute;
        }

        private void ToggleMute(Action<bool> onMuteToggled)
        {
            MuteEnabled = !MuteEnabled;
            if (MuteEnabled)
                _mainMixer.audioMixer.SetFloat(MASTER_VOLUME, -80.0f);
            else
                _mainMixer.audioMixer.SetFloat(MASTER_VOLUME, 0.0f);
            onMuteToggled?.Invoke(MuteEnabled);
        }

        private void PlayButtonClick()
        {
            if (MuteEnabled) return;
            _sfxAS.PlayOneShot(_sfxClips[(int)SFXClip.Click]);
        }

        // This wouldnt be used with each missile having an audiosource
        private async void PlayOneShotSFX(int clipIndex, float delayInSec)
        {
            if (MuteEnabled) return;
            if (delayInSec != 0)
                await Task.Delay((int)(delayInSec * 1000));

            _gameplaySfxAS.PlayOneShot(_sfxClips[clipIndex]);
        }

        private void PlayGameplaySFX(int clipIndex, bool enable = true)
        {
            if (!enable)
            {
                _gameplaySfxAS.Pause();
                return;
            }
            else if (clipIndex == -1)
            {
                _gameplaySfxAS.Play();
                return;
            }

            // _gameplaySfxAS.clip = _sfxClips[1];                   // TEST

            _gameplaySfxAS.clip = _sfxClips[clipIndex + 1];          // Offset by 1, as 1 is Click
            _gameplaySfxAS.Play();
        }
    }
}