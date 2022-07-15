using Photon.Realtime;
using StarterAssets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIConnectionStatusPanel : MonoBehaviour
{
    #region SerializeField
    [SerializeField]
    private TextMeshProUGUI _connectionStatusText;
    [SerializeField]
    private TextMeshProUGUI[] _playerListTextArray;
    [SerializeField]
    private TextMeshProUGUI _playerNameText;
    [SerializeField]
    private RectTransform _playerNameRectTransform;
    [SerializeField]
    private Button _logOutButton;
    [SerializeField]
    private Button _screenShareOpenButton;
    [SerializeField]
    private PlaygroundSceneController _sceneController;
    #endregion //SerializeField

    private Transform _nameTextFollowTransform;
    private ThirdPersonController _thirdPersonController;

    private void Awake()
    {
        if (_logOutButton.onClick.GetPersistentEventCount() == 0)
        {
            _logOutButton.onClick.AddListener(OnClickLogOutButton);
        }

        if (_screenShareOpenButton.onClick.GetPersistentEventCount() == 0)
        {
            _screenShareOpenButton.onClick.AddListener(OnClickScreenShareOpenButton);
        }
    }

    private void Start()
    {
        if (_nameTextFollowTransform == null)
        {
            _nameTextFollowTransform = _sceneController != null ? _sceneController.FollowUITargetTransform : null;
        }

        _connectionStatusText.text = PUN2ConnectionManager.Instance.GetConnectionState();
        _playerNameText.text = PUN2ConnectionManager.Instance.PlayerNickName;

        OnRefreshPlayerList(PUN2ConnectionManager.Instance.CurrentConnectedPlayerArray);
    }

    private void OnEnable()
    {
        if (PUN2ConnectionManager.Available)
        {
            PUN2ConnectionManager.Instance.OnRefreshPlayerListCallback += OnRefreshPlayerList;
        }
    }

    private void OnDisable()
    {
        if (PUN2ConnectionManager.Available)
        {
            PUN2ConnectionManager.Instance.OnRefreshPlayerListCallback -= OnRefreshPlayerList;
        }

        if (_thirdPersonController != null)
            _thirdPersonController.OnShowScreenShare -= _sceneController.ShowScreenShare;
    }

    private void FixedUpdate()
    {
        if (CameraManager.Instance.MainCamera == null || _nameTextFollowTransform == null)
            return;

        Vector2 screenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(_nameTextFollowTransform.transform.position);

        _playerNameRectTransform.position = screenPos;
    }

    private void OnDestroy()
    {
        _nameTextFollowTransform = null;
        _logOutButton.onClick.RemoveListener(OnClickLogOutButton);
        _screenShareOpenButton.onClick.RemoveListener(OnClickScreenShareOpenButton);
    }

    public void SetThirdPersonController(ThirdPersonController controller)
    {
        _thirdPersonController = controller;

        _thirdPersonController.OnShowScreenShare -= _sceneController.ShowScreenShare;
        _thirdPersonController.OnShowScreenShare += _sceneController.ShowScreenShare;
    }

    private void OnRefreshPlayerList(Player[] players)
    {
        if (_playerListTextArray != null)
        {
            int count = players.Length;
            for (int i = 0; i < _playerListTextArray.Length; i++)
            {
                bool isPlayerJoined = (i < count);
                _playerListTextArray[i].gameObject.SetActive(isPlayerJoined);
                if (isPlayerJoined)
                {
                    _playerListTextArray[i].text = players[i].NickName;
                }
            }
        }
    }

    private void OnClickLogOutButton()
    {
        PUN2ConnectionManager.Instance.LeaveRoom(() => GameSceneManager.Instance.MoveScene(eSceneType.Lobby));
    }

    private void OnClickScreenShareOpenButton()
    {
        _thirdPersonController.OnClickScreenShareButton();
    }
}
