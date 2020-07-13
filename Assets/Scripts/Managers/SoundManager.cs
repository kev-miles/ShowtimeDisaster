using Gameplay.Sound;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SoundManager : MonoBehaviour, ISound
    {
        [SerializeField] private AudioClip _ambientSound;
        [SerializeField] private AudioClip _crowdAudioClip;
        [SerializeField] private AudioClip[] _miscAudioLibrary;
        [SerializeField] private AudioClip[] _buttonAudioLibrary;
        [SerializeField] private AudioClip[] _musicAudioLibrary;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioSource _buttonAudioSource;
        [SerializeField] private AudioSource _miscAudioSource;
        [SerializeField] private AudioSource _crowdAudioSource;
        [SerializeField] private AudioSource _ambientAudioSource;
        [SerializeField] private AudioSource _fanfareAudioSource;
        
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _loseFanfare;
        [SerializeField] private AudioClip _winFanfare;

        private int _currentSong;
        public int CurrentSong => _currentSong;
        
        public bool SoundEnabled
        {
            get => _soundEnabled;
        }

        private bool _soundEnabled = true;

        public void SetSoundEnabled(bool enabled)
        {
            _soundEnabled = enabled;

            if (!_soundEnabled)
            {
                StopSound();
                StopMusic();
            }
            else
            {
                PlayMenuMusic();
            }
        }
        
        public void PlayButtonSound(ButtonSounds sound)
        {
            if (!_soundEnabled)
                return;
            
            var id = (int)sound;
            
            if (id < _buttonAudioLibrary.Length && !_buttonAudioSource.isPlaying)
            {
                _buttonAudioSource.clip = _buttonAudioLibrary[id];
                _buttonAudioSource.Play();
            }
        }

        public void PlayAmbientSound()
        {
            if (!_soundEnabled)
                return;
            _ambientAudioSource.clip = _ambientSound;
            _ambientAudioSource.Play();
        }
        
        public void PlayCrowdSound()
        {
            if (!_soundEnabled)
                return;
            _crowdAudioSource.Play();
        }

        public void PlayWinSound()
        {
            if (!_soundEnabled)
                return;
            _musicAudioSource.Stop();
            _fanfareAudioSource.clip = _winFanfare;
            _fanfareAudioSource.Play();
        }

        public void PlayLoseSound()
        {
            if (!_soundEnabled)
                return;
            _musicAudioSource.Stop();
            _fanfareAudioSource.clip = _loseFanfare;
            _fanfareAudioSource.Play();
        }

        public void PlaySound(MiscSounds sound)
        {
            if (!_soundEnabled)
                return;
            
            var id = (int) sound;
            
            if (id < _miscAudioLibrary.Length && !_buttonAudioSource.isPlaying)
            {
                _miscAudioSource.clip = _miscAudioLibrary[id];
                _miscAudioSource.Play();
            }
        }
        
        public void PlayMenuMusic()
        {
            if (!_soundEnabled)
                return;

            _musicAudioSource.clip = _menuMusic;
            _musicAudioSource.loop = true;
            _musicAudioSource.Play();
        }

        public void StopSound()
        {
            if (_buttonAudioSource.isPlaying)
                _buttonAudioSource.Stop();
            
            if (_miscAudioSource.isPlaying)
                _miscAudioSource.Stop();
            
            if(_ambientAudioSource.isPlaying)
                _ambientAudioSource.Stop();
            
            if(_crowdAudioSource.isPlaying)
                _crowdAudioSource.Stop();
        }
        
        public void StopMusic()
        {
            _musicAudioSource.Stop();
        }

        public void PlayGameplayMusic(int id)
        {
            if (!_soundEnabled)
                return;

            /*if (id > _miscAudioLibrary.Length-1)
                id = 0;*/

            var song = (int)Random.Range(0, 2);
            _currentSong = song;
            _musicAudioSource.clip = _musicAudioLibrary[song];
            _musicAudioSource.Play();
        }

        void Start()
        {
            _crowdAudioSource.clip = _crowdAudioClip;
            _buttonAudioSource.loop = false;
            _miscAudioSource.loop = false;
            _crowdAudioSource.loop = false;
            _ambientAudioSource.loop = true;
            _fanfareAudioSource.loop = false;
        }
    }
}