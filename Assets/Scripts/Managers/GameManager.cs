using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public SoundManager SoundManager { get; private set; }
        public GameplayPresenter GameplayPresenter { get; set; }
        public bool GameOver { get; set; }

        private GameScene _scene;
        private IEnumerator _gameTime;

        private bool _soundEnabled = true;
        
        public void ToggleSound()
        {
            if (_soundEnabled)
                _soundEnabled = false;
            else
                _soundEnabled = true;

            SoundManager.SetSoundEnabled(_soundEnabled);
        }
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            SoundManager = this.gameObject.GetComponent<SoundManager>();
        }

        void Start()
        {
            SoundManager.PlayMenuMusic();
            SceneManager.LoadScene((int)GameScene.MENU);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void MoveToGameplay()
        {
            SoundManager.PlayButtonSound(ButtonSounds.MENU_BUTTON);
            SoundManager.StopMusic();
            SoundManager.PlayAmbientSound();
            SoundManager.PlayGameplayMusic(0);
            _scene = GameScene.GAMEPLAY;
            SceneManager.LoadScene((int)GameScene.GAMEPLAY);
        }
        
        public void BackToMenu()
        {
            GameOver = false;
            SoundManager.PlayButtonSound(ButtonSounds.MENU_BUTTON);
            SoundManager.StopSound();
            SoundManager.StopMusic();
            SoundManager.PlayMenuMusic();
            _scene = GameScene.MENU;
            SceneManager.LoadScene((int)GameScene.MENU);
        }

        public void StartTime()
        {
            _gameTime = GameplayTime();
            StartCoroutine(_gameTime);
        }

        public void StopTime()
        {
            StopCoroutine(_gameTime);
        }

        IEnumerator GameplayTime()
        {
            while (!GameOver)
            {
                yield return new WaitForSeconds(Constants.GAMEPLAY_TIME);
                GameplayPresenter.CheckTime();
            }
        }
    }
}
