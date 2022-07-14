// ----------------------------------------------------------------------------
// <copyright file="PhotonLaserPointerView.cs" company="LG Uplus Future Technology Lab">
//   PhotonNetwork Framework for Unity - Copyright (C) 2022 LG Uplus
// </copyright>
// <summary>
//   Component to synchronize Laser Pointer Object via PUN2.
//   Referenced By Exit Games GmbH Codes.
// </summary>
// <author>Choi_Kyungho_wiznine@lguplus.co.kr</author>
// ----------------------------------------------------------------------------

namespace Photon.Pun
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Photon Networking/Photon Laser Pointer View")]
    public class PhotonLaserPointerView : MonoBehaviourPun, IPunObservable
    {
        [SerializeField]
        private float _syncSmoothingDelay = 5f;

        public enum SynchronizeType
        {
            Disabled = 0,
            Discrete = 1,
            Continuous = 2,
        }

        // received Photon View Information
        private Vector3 _receivedLocalPosition = Vector3.zero;
        private Vector3 _receivedLocalScale = Vector3.one;
        private GameObject _laserPointer = null;

        /// 레이저 포인터를 키고(Turn On) 끌 것(Turn Off)인 지 여부는 RPC로 보낸다.

        private void Update()
        {
            if (_laserPointer == null)
            {
                return;
            }
            
            if (photonView.IsMine == false)
            {
                _laserPointer.transform.localPosition = Vector3.Lerp(_laserPointer.transform.position, _receivedLocalPosition, Time.deltaTime * _syncSmoothingDelay);
                _laserPointer.transform.localScale = Vector3.Lerp(_laserPointer.transform.localScale, _receivedLocalScale, Time.deltaTime * _syncSmoothingDelay);
                return;
            }
        }

        public void SetLaserPointerObject(GameObject laserObject)
        {
            _laserPointer = laserObject;
        }

        /// <summary>
        /// 레이저 포인터의 Transform(Position, Scale) 값은 SerializeView로 보낸다.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (_laserPointer == null || stream == null)
            {
                return;
            }

            // 로컬 캐릭터의 데이터를 다른 클라이언트(유저)에게 송신
            if (stream.IsWriting == true)
            {
                stream.SendNext(_laserPointer.transform.localPosition);
                stream.SendNext(_laserPointer.transform.localScale);
            }
            else
            {
                _receivedLocalPosition = (Vector3)stream.ReceiveNext();
                _receivedLocalScale = (Vector3)stream.ReceiveNext();
            }
        }
    }

}