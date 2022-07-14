using Cinemachine;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlaygroundSceneController : MonoBehaviour
{    
    [SerializeField]
    private UICanvasControllerInput _canvasControllerInput;
    [SerializeField]
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField]
    private UIConnectionStatusPanel _connectionStatusPanel;
    [SerializeField]
    private UIScreenSharePanel _screenSharePanel;

    public Transform FollowUITargetTransform { get; private set; }

    private ThirdPersonController _thirdPerson;

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

        _thirdPerson = player.GetComponent<ThirdPersonController>();

        FollowUITargetTransform = _thirdPerson.FollowUITarget.transform;

        _cinemachineVirtualCamera.Follow = _thirdPerson.CinemachineCameraTarget.transform;
        _thirdPerson.SetLaserPointerParentPanel(_screenSharePanel.gameObject.transform);
        _canvasControllerInput.SetStarterAssetsInputs(player.GetComponent<StarterAssetsInputs>());
        _connectionStatusPanel.SetThirdPersonController(_thirdPerson);

        _screenSharePanel.SetScreenShareActiveAction(OnSetScreenShareActive);
        _screenSharePanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Cursor.visible)
            Cursor.visible = true;
    }

    public void ShowScreenShare()
    {
        _screenSharePanel.OnShowScreenShare();
    }

    private void OnSetScreenShareActive(bool isActive)
    {
        if (_thirdPerson != null)
            _thirdPerson.SetScreenShareActive(isActive);
    }
}
