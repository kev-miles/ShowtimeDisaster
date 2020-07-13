using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using Utilities;

namespace Gameplay
{
    public class GameplayView : MonoBehaviour
    {
        public Camera.Camera[] CameraObjects = new Camera.Camera[5];
        
        #region CameraButtons
        [SerializeField] private Button _stageCameraButton;
        [SerializeField] private Button _firstRowCameraButton;
        [SerializeField] private Button _publicCameraButton;
        [SerializeField] private Button _backStageCameraButton;
        [SerializeField] private Button _venueAccessCameraButton;
        #endregion

        #region ActionButtons
        [SerializeField] private Button _securityButton;
        [SerializeField] private Button _medicalButton;
        [SerializeField] private Button _fireCrewButton;
        [SerializeField] private Button _technicalAssistButton;
        [SerializeField] private Button _hydrantTruckButton;
        
        [SerializeField] private GameObject _secButtonOverlay;
        [SerializeField] private GameObject _medButtonOverlay;
        [SerializeField] private GameObject _fireButtonOverlay;
        [SerializeField] private GameObject _techButtonOverlay;
        [SerializeField] private GameObject _hydrantTruckButtonOverlay;
        
        [SerializeField] private Text _secButtonCD;
        [SerializeField] private Text _medButtonCD;
        [SerializeField] private Text _fireButtonCD;
        [SerializeField] private Text _techButtonCD;

        private Coroutine _secRoutine;
        private Coroutine _medRoutine;
        private Coroutine _fireRoutine;
        private Coroutine _techRoutine;
        #endregion

        [SerializeField] private Button _backToMenuButton;
    
        [SerializeField] private Image _moodBarForeground;
        
        [SerializeField] private Image _clock;
        [SerializeField] private Sprite[] _clockHours = new Sprite[7];
    
        [SerializeField] private GameObject _losePopUp;
        [SerializeField] private GameObject _winPopUp;

        private GameplayPresenter _presenter;

        public void AdvanceClock(int step)
        {
            _clock.sprite = _clockHours[step];
        }

        public void ShowWinPopUp()
        {
            DisableAllButtons();
            StopAllCoroutines();
            _winPopUp.SetActive(true);
        }

        public void ShowLosePopUp()
        {
            DisableAllButtons();
            StopAllCoroutines();
            _losePopUp.SetActive(true);
        }

        public void ShowMood(float amount)
        {
            _moodBarForeground.fillAmount = amount;
        }

        public void DepleteMood()
        {
            _moodBarForeground.fillAmount = 0;
        }
    
        void Awake()
        {
            _presenter = new GameplayPresenter(this);
            _clock.sprite = _clockHours[0];
        }
    
        void Start()
        {
            _backToMenuButton.onClick.AddListener(() => _presenter.LoadMenu());
            _backToMenuButton.onClick.AddListener(StopAllCoroutines);
            BindCameraButtons();
            BindActionButtons();
            _presenter.SwitchCamera(Cameras.STAGE);
        }

        void DisableAllButtons()
        {
            _stageCameraButton.interactable = false;
            _firstRowCameraButton.interactable = false;
            _publicCameraButton.interactable = false;
            _backStageCameraButton.interactable = false;
            _venueAccessCameraButton.interactable = false;
            
            _securityButton.interactable = false;
            _medicalButton.interactable = false;
            _fireCrewButton.interactable = false;
            _technicalAssistButton.interactable = false;
            _hydrantTruckButton.interactable = false;
        }
        
        void BindCameraButtons()
        {
            _stageCameraButton.onClick.AddListener(()=> _presenter.SwitchCamera(Cameras.STAGE));
            _firstRowCameraButton.onClick.AddListener(()=> _presenter.SwitchCamera(Cameras.FIRST_ROW));
            _publicCameraButton.onClick.AddListener(()=> _presenter.SwitchCamera(Cameras.PUBLIC));
            _backStageCameraButton.onClick.AddListener(()=> _presenter.SwitchCamera(Cameras.BACKSTAGE));
            _venueAccessCameraButton.onClick.AddListener(()=> _presenter.SwitchCamera(Cameras.VENUE_ACCESS));
        }

        void BindActionButtons()
        {
            _securityButton.onClick.AddListener(()=> _presenter.FixIssue(GameplayEvents.SECURITY));
            //_securityButton.onClick.AddListener(()=> DisableButton(GameplayEvents.SECURITY));
            
            _medicalButton.onClick.AddListener(()=> _presenter.FixIssue(GameplayEvents.MEDICAL));
            //_securityButton.onClick.AddListener(()=> DisableButton(GameplayEvents.MEDICAL));
            
            _fireCrewButton.onClick.AddListener(()=> _presenter.FixIssue(GameplayEvents.FIRE));
            //_securityButton.onClick.AddListener(()=> DisableButton(GameplayEvents.FIRE));
            
            _technicalAssistButton.onClick.AddListener(()=> _presenter.FixIssue(GameplayEvents.TECHNICAL));
            //_securityButton.onClick.AddListener(()=> DisableButton(GameplayEvents.TECHNICAL));
            
            _hydrantTruckButton.onClick.AddListener(() => _presenter.CallTruck());
            _hydrantTruckButton.onClick.AddListener(DisableTruckButton);
        }

        void DisableButton(GameplayEvents e)
        {
            switch(e)
            {
                case GameplayEvents.SECURITY:
                    _secRoutine = StartCoroutine(ButtonCooldown(_securityButton, _secButtonOverlay, _secButtonCD, GameplayEvents.SECURITY));
                    break;
                case GameplayEvents.MEDICAL:
                    _medRoutine = StartCoroutine(ButtonCooldown(_medicalButton, _medButtonOverlay, _medButtonCD, GameplayEvents.MEDICAL));
                    break;
                case GameplayEvents.FIRE:
                    _fireRoutine = StartCoroutine(ButtonCooldown(_fireCrewButton, _fireButtonOverlay, _fireButtonCD, GameplayEvents.FIRE));
                    break;
                case GameplayEvents.TECHNICAL:
                    _techRoutine = StartCoroutine(ButtonCooldown(_technicalAssistButton, _techButtonOverlay, _techButtonCD, GameplayEvents.TECHNICAL));
                    break;
            }
        }

        void DisableTruckButton()
        {
            _hydrantTruckButtonOverlay.SetActive(true);
            _hydrantTruckButton.interactable = false;
        }

        IEnumerator ButtonCooldown(Button button, GameObject overlay, Text cd, GameplayEvents e)
        {
            button.interactable = false;
            overlay.SetActive(true);
            
            var count = 5;
            cd.text = count.ToString();
            
            while (count < Constants.BUTTON_COOLDOWN)
            {
                yield return new WaitForSeconds(1);
                count--;
                cd.text = count.ToString();
            }

            button.interactable = true;
            overlay.SetActive(false);
            
            TerminateCDRoutine(e);
        }

        void TerminateCDRoutine(GameplayEvents e)
        {
            switch(e)
            {
                case GameplayEvents.SECURITY:
                    if(_secRoutine != null)
                        StopCoroutine(_secRoutine);
                    break;
                case GameplayEvents.MEDICAL:
                    if(_medRoutine != null)
                        StopCoroutine(_medRoutine);
                    break;
                case GameplayEvents.FIRE:
                    if(_fireRoutine != null)
                        StopCoroutine(_fireRoutine);
                    break;
                case GameplayEvents.TECHNICAL:
                    if(_techRoutine != null)
                        StopCoroutine(_techRoutine);
                    break;
            }
        }
    }
}
