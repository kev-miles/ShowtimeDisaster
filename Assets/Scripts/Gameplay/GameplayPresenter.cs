using System.Net;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Gameplay
{
    public class GameplayPresenter
    {
        private GameplayView _view;
        private int _clockStep = 0;

        public bool GameOver { get; set; }
        private GameManager _gameManager;

        private SoundManager _soundManager;

        private Camera.Camera _activeCamera;
        [SerializeField] Camera.Camera[] cameras;

        private float _mood = 1;
        private readonly float _moodDecrease = Constants.MOOD_DECREASE;
    
        public GameplayPresenter(GameplayView view)
        {
            _view = view;
            _gameManager = GameObject.FindWithTag("MainCamera").GetComponent<GameManager>();
            _gameManager.GameplayPresenter = this;
            _gameManager.StartTime();
            _soundManager = _gameManager.SoundManager;

            cameras = _view.CameraObjects;
            SetPresenterOnCameras();
        }

        public void LoadMenu()
        {
            _gameManager.BackToMenu();
        }

        public void CheckTime()
        {
            if (!GameOver)
            {
                if (_clockStep < 5)
                    _clockStep++;
                else
                {
                    _clockStep++;
                    TriggerWin();
                }
                
                _soundManager.PlayCrowdSound();
                _soundManager.PlaySound(MiscSounds.CLOCK);
                _view.AdvanceClock(_clockStep);

                /*var currentSongID = _soundManager.CurrentSong;
                _soundManager.PlayGameplayMusic(currentSongID++);*/
            }
        }

        public void SwitchCamera(Cameras camera)
        {
            PlayButtonSound();
            
            foreach (var cam in cameras)
            {
                if (cam.cameraType == camera)
                {
                    //Deactivate previous camera
                    if(_activeCamera != null)
                        _activeCamera.Dectivate();
                    
                    //Activate new one
                    _activeCamera = cam;
                    cam.Activate();
                    break;
                }
            }   
        }

        public void FixIssue(GameplayEvents fix)
        {
            if (fix == GameplayEvents.TECHNICAL && _activeCamera.IssueType == GameplayEvents.DISCONNECT)
            {
                _activeCamera.FixDisconnect();
            }
            
            bool fixMatchesIssue = _activeCamera.IssueType == fix;

            if (fixMatchesIssue)
            {
                _activeCamera.FixIssue();
            }
        }

        public void CallTruck()
        {
            _mood += Constants.TRUCK_MOOD_EFFECT;
            _view.ShowMood(_mood);
        }

        public void AffectMood()
        {
            if ((_mood -= _moodDecrease) > 0)
            {
                _mood -= _moodDecrease;
                _view.ShowMood(_mood);
            }
            else
            {
                _view.DepleteMood();
                GameOver = true;
            }

            if (GameOver)
            {
                TriggerLoss();
                _soundManager.StopSound();
                _gameManager.GameOver = GameOver;
            }
        }

        void TriggerWin()
        {
            _gameManager.GameOver = GameOver = true;
            _view.ShowWinPopUp();
            _gameManager.StopTime();
            _soundManager.StopSound();
            foreach (var cam in cameras)
            {
                cam.StopSound();
                cam.ShutdownRoutines();
            }
            _soundManager.PlayWinSound();
        }

        void TriggerLoss()
        {
            _gameManager.GameOver = GameOver = true;
            _view.ShowLosePopUp();
            _gameManager.StopTime();
            foreach (var cam in cameras)
            {
                cam.StopSound();
                cam.ShutdownRoutines();
            }
            _soundManager.StopSound();
            _soundManager.PlayLoseSound();
        }

        void SetPresenterOnCameras()
        {
            foreach(var cam in cameras)
                cam.SetGameplayPresenter(this);
        }
        
        void PlayButtonSound()
        {
            _soundManager.PlayButtonSound(ButtonSounds.GAMEPLAY_BUTTON);
        }
    }
}
