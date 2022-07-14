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
    private Action<bool> _onScreenShareActiveCallback;
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
        _quitButton.onClick.RemoveListener(OnHideScreenShare);
    }

    public void SetScreenShareActiveAction(Action<bool> callback)
    {
        _onScreenShareActiveCallback = callback;
    }

    public void OnShowScreenShare()
    {
        Cursor.lockState = CursorLockMode.Confined;

        gameObject.SetActive(true);
        _onScreenShareActiveCallback?.Invoke(true);        
    }

    public void OnHideScreenShare()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(_originCursorTexture, _originCursorPoint, CursorMode.Auto);

        gameObject.SetActive(false);
        _onScreenShareActiveCallback?.Invoke(false);       
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
}
