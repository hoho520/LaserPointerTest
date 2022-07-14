using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LaserPointer : MonoBehaviourPun, IPunObservable
{
    private Vector3 _receivedPosition;
    private Vector3 _receivedLocalScale;
    private PhotonView _photonView;
    private PhotonLaserPointerView _laserPointerView;
    private RectTransform _rectTransform;
    private Image _icon;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _photonView.ObservedComponents[0] = this;

        _laserPointerView = GetComponent<PhotonLaserPointerView>();
        _laserPointerView.SetLaserPointerObject(gameObject);

        _rectTransform = gameObject.GetComponent<RectTransform>();
        _icon = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        gameObject.SetActive(true);
        HideLaserPoint();
    }

    private void Update()
    {
        if (gameObject.activeSelf == false)
            return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (_photonView.IsMine)
            {
                ShowLaserPointer(false);
            }
            else
            {
                _photonView.RPC("ShowLaserPointer", RpcTarget.Others, false);
            }
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit raycastHit;

            Vector2 pressedPos = Mouse.current.position.ReadValue();
            Vector3 screenPos = CameraManager.Instance.MainCamera.WorldToScreenPoint(pressedPos);

            if (Physics.Raycast(_rectTransform.position, screenPos, out raycastHit, 30))
            {
                _rectTransform.position = Vector3.Lerp(pressedPos, raycastHit.point, 0.5f);

                _rectTransform.LookAt(raycastHit.point);
                _rectTransform.localScale = new Vector3(_rectTransform.localScale.x, _rectTransform.localScale.y, raycastHit.distance);

                if (_photonView.IsMine)
                {
                    ShowLaserPointer(true);
                }
                else
                {
                    _photonView.RPC("ShowLaserPointer", RpcTarget.Others, true);
                }
            }
        }
    }

    private void HideLaserPoint()
    {
        _icon.enabled = false;
    }

    private void ShowLaserPoint()
    {
        _icon.enabled = true;
    }

    [PunRPC]
    public void ShowLaserPointer(bool isActive)
    {
        if (isActive == true)
        {
            ShowLaserPoint();
        }
        else
        {
            HideLaserPoint();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 캐릭터의 데이터를 다른 클라이언트(유저)에게 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
        }
        else
        {
            _receivedPosition = (Vector3)stream.ReceiveNext();
            _receivedLocalScale = (Vector3)stream.ReceiveNext();
        }
    }
}
