using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlaygroundSceneController : MonoSingleton<PlaygroundSceneController>
{    
    [SerializeField]
    private UICanvasControllerInput _canvasControllerInput;
    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField]
    private UIConnectionStatusPanel _connectionStatusPanel;
    [SerializeField]
    private UIScreenSharePanel _screenSharePanel;

    public bool IsScreenShareVisible
    {
        get
        {
            if (_screenSharePanel == null)
                return false;

            return _screenSharePanel.gameObject.activeSelf;
        }
    }        

    public Transform FollowUITargetTransform { get; private set; }

    public ThirdPersonController ThirdPersonController { get; private set; }

    void Start()
    {
        CameraManager.Instance.Init();

        GameObject player = PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity);
        if (player == null)
        {
#if UNITY_EDITOR
            Debug.LogError("PlayerCharacter Prefab is null!!");
#endif
            return;
        }

        ThirdPersonController = player.GetComponent<ThirdPersonController>();

        FollowUITargetTransform = ThirdPersonController.FollowUITarget.transform;

        _cinemachineVirtualCamera.Follow = ThirdPersonController.CinemachineCameraTarget.transform;
        _canvasControllerInput.SetStarterAssetsInputs(player.GetComponent<StarterAssetsInputs>());

        _screenSharePanel.gameObject.SetActive(false);
    }

    public void ShowScreenShare(bool isShow)
    {
        if (isShow)
        {
            Cursor.lockState = CursorLockMode.Confined;

            _screenSharePanel.gameObject.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            _screenSharePanel.SetDefaultCursor();
            _screenSharePanel.gameObject.SetActive(false);
        }
    }

    public void SetLaserPointerTransform(bool isShow, Vector2? position)
    {
        _screenSharePanel.SetLaserPointerTransform(isShow, position);
    }
}
