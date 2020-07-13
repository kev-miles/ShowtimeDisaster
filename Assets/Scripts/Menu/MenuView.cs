using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Menu
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _howToButton;
        [SerializeField] private Button _exitGameButton;
        [SerializeField] private GameObject _howToPlayScreen;
        [SerializeField] private GameObject _menuScreen;
        
        private MenuPresenter _presenter;
        private bool soundEnabled;
        
        [SerializeField] private Sprite[] soundButtonSprites;

        private void Awake()
        {
            _presenter = new MenuPresenter(this);
            soundEnabled = GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<GameManager>()
                .SoundManager.SoundEnabled;
        }

        private void Start()
        {
            StopAllCoroutines();
            _startGameButton.onClick.AddListener(() => _presenter.LoadGameplayScene());
            _optionsButton.onClick.AddListener( () => _presenter.ToggleSound());
            _optionsButton.onClick.AddListener(ShiftSoundSprite);
            _exitGameButton.onClick.AddListener(() =>_presenter.ExitGame());
            _howToButton.onClick.AddListener(OpenHowToPlay);
#if UNITY_WEBGL
            _exitGameButton.gameObject.SetActive(false);
#endif
            SetSoundSprite();
        }
        
        private void OpenHowToPlay()
        {
            _presenter.PlayButtonSound(ButtonSounds.MENU_BUTTON);
            _howToPlayScreen.SetActive(true);
            _menuScreen.SetActive(false);
        }

        private void ShiftSoundSprite()
        {
            if (soundEnabled)
                _optionsButton.image.sprite = soundButtonSprites[1];
            else
                _optionsButton.image.sprite = soundButtonSprites[0];

            soundEnabled = !soundEnabled;
        }

        private void SetSoundSprite()
        {
            if(soundEnabled)
                _optionsButton.image.sprite = soundButtonSprites[0];
            else
                _optionsButton.image.sprite = soundButtonSprites[1];
        }
    }
}
