using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Gameplay.Camera
{
    public class Camera : MonoBehaviour, ICamera
    {
        public string cameraName;
        public Cameras cameraType;
        public GameplayEvents IssueType => _currentEvent;
        
        [SerializeField] private Image _screen;
        [SerializeField] private Sprite _screenImage;
        
        [SerializeField] private Text _cameraNameText;
        [SerializeField] private GameObject _disabledImage;
        
        [SerializeField] private Text _issueText;
        [SerializeField] private GameObject _issueFeedback;
        private Image _issueFeedbackImage;

        private Coroutine _generatorTimer;
        private Coroutine _issueTimer;
        private GameplayPresenter _presenter;

        private GameplayEvents _currentEvent;
        private bool _issueActive = false;
        private bool _activeCamera = false;
        private bool _soundEnabled;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _sfx;

        [SerializeField] private Sprite[] frames;
        
        [SerializeField] private Sprite[] issueIcons;
        
        public void Activate()
        {
            _activeCamera = true;
            _cameraNameText.text = cameraName;
            _screen.sprite = _screenImage;
            if (_issueActive)
            {
                _issueText.text = EventToText(IssueType);
                if (_currentEvent == GameplayEvents.DISCONNECT)
                {
                    TriggerDisconnect();
                }
                else
                {
                    ShowEventFeedback(_currentEvent);
                }
            }
            else
            {
                HideEventFeedback();
                HideDisconnect();
                _issueText.text = "AREA STATUS: OK".ToLower();
            }
        }
        
        public void Dectivate()
        {
            _issueFeedback.SetActive(false);
            _disabledImage.SetActive(false);
            _activeCamera = false;
        }

        public void ShutdownRoutines()
        {
            StopAllCoroutines();
        }
        
        public void FixIssue()
        {
            if(_issueTimer != null)
                StopCoroutine(_issueTimer);
            
            _issueActive = false;
            _currentEvent = GameplayEvents.NONE;
            HideEventFeedback();
        }

        public void FixDisconnect()
        {
            if(_issueTimer != null)
                StopCoroutine(_issueTimer);
            
            _issueActive = false;
            _currentEvent = GameplayEvents.NONE;
            HideDisconnect();
        }

        private void Start()
        {
            StartCoroutine(Animate());
            _audioSource = gameObject.GetComponent<AudioSource>();
            _audioSource.loop = false;
            _issueFeedbackImage = _issueFeedback.GetComponent<Image>();
            _soundEnabled = GameObject.FindGameObjectWithTag("MainCamera")
                .GetComponent<GameManager>().SoundManager.SoundEnabled;
        }
        
        public void SetGameplayPresenter(GameplayPresenter presenter)
        {
            _presenter = presenter;
            _generatorTimer = StartCoroutine(EventTimer());
        }

        public void HideDisconnect()
        {
            _disabledImage.SetActive(false);
            if (_activeCamera)
                _issueText.text = "AREA STATUS: OK".ToLower();
        }

        public void TriggerDisconnect()
        {
            if(_activeCamera)
                _disabledImage.SetActive(true);
        }

        void ShowEventFeedback(GameplayEvents e)
        {
            if (_activeCamera)
            {
                _issueFeedback.SetActive(true);
                _issueFeedbackImage.sprite = issueIcons[(int) e];
                _issueText.text = EventToText(e);
            }
        }

        void HideEventFeedback()
        {
            _generatorTimer = StartCoroutine(EventTimer());
            _issueFeedback.SetActive(false);
        }

        #region Sound

        public void PlaySound(MiscSounds id)
        {
            if (!_soundEnabled)
                return;
            
            _audioSource.clip = _sfx[(int)id];
            _audioSource.Play();
        }

        public void StopSound()
        {
            _audioSource.Stop();
        }
        #endregion
        
        
        #region GameplayEvents
        IEnumerator EventTimer()
        {
            while (!_presenter.GameOver)
            {
                yield return new WaitForSeconds(Constants.CAMERA_EVENT_TIMER);
                var chance = UnityEngine.Random.Range(0, 100);
                if (chance >= Constants.EVENT_CHANCE && !_issueActive)
                {
                    GenerateEvent();
                }
            }
        }

        void GenerateEvent()
        {
            if(_generatorTimer!=null)
                StopCoroutine(_generatorTimer);
            
            var issue = UnityEngine.Random.Range(0, (int)GameplayEvents.NONE);
            _issueActive = true;
            ConstructEvent(issue);
        }

        void ConstructEvent(int e)
        {
            switch (e)
            {
                case (int)GameplayEvents.SECURITY:
                    _currentEvent = GameplayEvents.SECURITY;
                    ShowEventFeedback(GameplayEvents.SECURITY);
                    PlaySound(MiscSounds.ANGRY_CROWD);
                    break;
                case (int)GameplayEvents.MEDICAL:
                    _currentEvent = GameplayEvents.MEDICAL;
                    ShowEventFeedback(GameplayEvents.MEDICAL);
                    PlaySound(MiscSounds.DEBRIS);
                    break;
                case (int)GameplayEvents.FIRE:
                    _currentEvent = GameplayEvents.FIRE;
                    ShowEventFeedback(GameplayEvents.FIRE);
                    PlaySound(MiscSounds.FIRE);
                    break;
                case (int)GameplayEvents.TECHNICAL:
                    _currentEvent = GameplayEvents.TECHNICAL;
                    ShowEventFeedback(GameplayEvents.TECHNICAL);
                    PlaySound(MiscSounds.MALFUNCTION);
                    break;
                case (int)GameplayEvents.DISCONNECT:
                    _currentEvent = GameplayEvents.DISCONNECT;
                    TriggerDisconnect();
                    PlaySound(MiscSounds.DISCONNECT);
                    break;
                default:
                    return;
            }

            _issueTimer = StartCoroutine(IssueTimer());
        }

        string EventToText(GameplayEvents e)
        {
            switch (e)
            {
                case GameplayEvents.SECURITY:
                    return "SECURITY NEEDED!".ToLower();
                case GameplayEvents.MEDICAL:
                    return "MEDICAL AID NEEDED!".ToLower();
                case GameplayEvents.FIRE:
                    return "A FIRE BROKE OUT!".ToLower();
                case GameplayEvents.TECHNICAL:
                    return "TECHNICAL ASSISTANCE NEEDED!".ToLower();
                case GameplayEvents.DISCONNECT:
                    return "TECHNICAL ASSISTANCE NEEDED!".ToLower();
                default:
                    return "AREA STATUS: OK".ToLower();
            }
        }
        
        IEnumerator IssueTimer()
        {
            while (_issueActive)
            {
                yield return new WaitForSeconds(Constants.CAMERA_ISSUE_TIMER);
                _presenter.AffectMood();
            }
        }
        #endregion
        
        IEnumerator Animate()
        {
            var frameID = 0;
            
            while (!_presenter.GameOver)
            {
                yield return new WaitForSeconds(0.25f);

                frameID++;
                
                if (frameID < frames.Length)
                {
                    _screenImage = frames[frameID];
                    if(_activeCamera)
                        _screen.sprite = _screenImage;
                }
                else
                {
                    frameID = 0;
                    _screenImage = frames[frameID];
                    if(_activeCamera)
                        _screen.sprite = _screenImage;
                }
            }
        }
    }
}