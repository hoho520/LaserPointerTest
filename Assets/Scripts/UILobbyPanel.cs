using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyPanel : MonoBehaviour
{
    private readonly string ROOM_FORMAT = "Room {0}";

    [SerializeField]
    private TextMeshProUGUI _connectionStatusText;
    [SerializeField]
    private Button[] _buttons;
    [SerializeField]
    private UILoadingPanel _loadingPanel;

    private void Start()
    {
        _loadingPanel.gameObject.SetActive(false);

        _connectionStatusText.text = PUN2ConnectionManager.Instance.GetConnectionState();
    }

    public void OnClickRoomButton(int roomIndex)
    {
        this.gameObject.SetActive(false);
        _loadingPanel.gameObject.SetActive(true);

        string roomName = string.Format(ROOM_FORMAT, roomIndex.ToString());
        PUN2ConnectionManager.Instance.JoinRoom(roomName, () => 
        {
#if UNITY_EDITOR
            Debug.Log($"Joined in {roomName}");
#endif
            GameSceneManager.Instance.MoveScene(eSceneType.Playground);
        });
    }

    public void OnClickExitLobbyButton()
    {
        PUN2ConnectionManager.Instance.Disconnect(() => GameSceneManager.Instance.MoveScene(eSceneType.Login));
    }
}
