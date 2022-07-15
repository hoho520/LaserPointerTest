using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIScreenSharePanel : MonoBehaviour
{
    private readonly string ORIGIN_CURSOR_PATH = "Icon/Cursor_Default";
    private const string RED_LASER_POINTER_CURSOR_PATH = "Icon/Red_Laser_Pointer";
    private const string BLUE_LASER_POINTER_CURSOR_PATH = "Icon/Blue_Laser_Pointer";

    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private LaserPointer _laserPointer;

    public bool IsScreenShareActive
    {
        get
        {
            return gameObject.activeSelf == true;
        }
    }

    private bool _isDefaultCursor = false;
    private int _currentLaserCursorIndex = -1;
    private Vector2 _originCursorPoint;
    private Vector2 _laserCursorPoint;
    private Texture2D _originCursorTexture;
    private Texture2D _currentLaserCursorTexture;
    private Texture2D[] _laserCursorTextures = null;

    private void Awake()
    {
        if (_quitButton.onClick.GetPersistentEventCount() == 0)
        {
            _quitButton.onClick.AddListener(OnHideScreenShare);
        }

        _laserCursorTextures = new Texture2D[]
        {
            Resources.Load<Texture2D>(RED_LASER_POINTER_CURSOR_PATH),
            Resources.Load<Texture2D>(BLUE_LASER_POINTER_CURSOR_PATH),
        };

        _originCursorTexture = Resources.Load<Texture2D>(ORIGIN_CURSOR_PATH);
        _currentLaserCursorTexture = _laserCursorTextures[0];

        _originCursorPoint = new Vector2(_originCursorTexture.width / 4, _originCursorTexture.height / 6);
        _laserCursorPoint = new Vector2(_currentLaserCursorTexture.width / 2, _currentLaserCursorTexture.height / 2);

        _laserPointer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (_isDefaultCursor == false)
            {
                _isDefaultCursor = true;

                Cursor.SetCursor(_originCursorTexture, _originCursorPoint, CursorMode.Auto);
            }

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SetLaserCursor(_currentLaserCursorIndex < 0 ? 0 : _currentLaserCursorIndex);

            if (_isDefaultCursor == true)
            {
                _isDefaultCursor = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (_quitButton.onClick.GetPersistentEventCount() > 0)
        {
            _quitButton.onClick.RemoveListener(OnHideScreenShare);
        }
    }

    private void OnHideScreenShare()
    {
        PlaygroundSceneController.Instance.ThirdPersonController.OnClickScreenShareButton(false);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(_originCursorTexture, _originCursorPoint, CursorMode.Auto);
    }

    public void SetLaserCursor(int index)
    {
        if (_currentLaserCursorIndex != index)
        {
            _currentLaserCursorIndex = index;
            _currentLaserCursorTexture = _laserCursorTextures[index];
        }

        Cursor.SetCursor(_currentLaserCursorTexture, _laserCursorPoint, CursorMode.Auto);
    }

    public void SetLaserPointerTransform(bool isShow, Vector2? position = null)
    {
        if (_laserPointer != null)
        {
            _laserPointer.gameObject.SetActive(isShow);

            if (isShow && position.HasValue)
            {
                _laserPointer.SetPosition(position.Value);
            }
        }
    }
}
