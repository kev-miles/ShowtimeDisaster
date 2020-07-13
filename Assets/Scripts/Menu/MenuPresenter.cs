using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Menu
{
    public class MenuPresenter
    {
        private GameManager _gameManager;
        private MenuView _view;
    
        public MenuPresenter(MenuView view)
        {
            _gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
            _view = view;
        }

        public void ExitGame()
        {
            _gameManager.ExitGame();
        }
        
        public void LoadGameplayScene()
        {
            _gameManager.MoveToGameplay();
        }

        public void ToggleSound()
        {
            _gameManager.ToggleSound();
        }

        public void PlayButtonSound(ButtonSounds sound)
        {
            _gameManager.SoundManager.PlayButtonSound(sound);
        }
    }
}
