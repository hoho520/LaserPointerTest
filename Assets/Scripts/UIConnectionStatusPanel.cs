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
    #endregion //SerializeField

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
    }

    private void FixedUpdate()
    {
        if (CameraManager.Instance.MainCamera == null || PlaygroundSceneController.Instance.FollowUITargetTransform == null)
            return;

        Vector2 screenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(PlaygroundSceneController.Instance.FollowUITargetTransform.transform.position);

        _playerNameRectTransform.position = screenPos;
    }

    private void OnDestroy()
    {
        _logOutButton.onClick.RemoveListener(OnClickLogOutButton);
        _screenShareOpenButton.onClick.RemoveListener(OnClickScreenShareOpenButton);
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
        PlaygroundSceneController.Instance.ThirdPersonController.OnClickScreenShareButton(true);
    }
}
