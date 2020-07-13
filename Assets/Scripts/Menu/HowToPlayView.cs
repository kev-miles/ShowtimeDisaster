using System;
using System.Runtime.CompilerServices;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class HowToPlayView : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _howToPlayScreen;
        [SerializeField] private GameObject _menuScreen;

        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _buttonSFX;

        private SoundManager _soundManager;
        private void Start()
        {
            _soundManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().SoundManager;
            _source = gameObject.GetComponent<AudioSource>();
            _source.clip = _buttonSFX;
            _source.loop = false;
            _exitButton.onClick.AddListener(() => CloseHowToPlay());   
        }

        private void CloseHowToPlay()
        {
            if(_soundManager.SoundEnabled)
                _source.Play();
            
            _menuScreen.SetActive(true);
            _howToPlayScreen.SetActive(false);
        }
    }
}
