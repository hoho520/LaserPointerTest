using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUN2ConnectionManager : MonoBehaviourPunCallbacks
{
    public const string CONNECTION_STATE_FORMAT = "Connection : {0}";
    public const string CURRENT_GAME_VERSION = "1.0.0";

    public static bool Available => _instance != null;

    private static PUN2ConnectionManager _instance = null;

    public static PUN2ConnectionManager Instance => _instance;

    public ClientState CurrentClientState { get; private set; }

    public int CurrentPlayerCount => CurrentConnectedPlayerArray.Length;

    public string PlayerNickName
    {
        get { return PhotonNetwork.NickName; }
    }

    public Player[] CurrentConnectedPlayerArray
    {
        get 
        {
            return PhotonNetwork.PlayerList;
        }
    }

    public Action<Player[]> OnRefreshPlayerListCallback { get; set; }

    private readonly Dictionary<string, RoomOptions> _roomOptionsDictionary = new Dictionary<string, RoomOptions>();

    private bool _isConnectedInMasterServer = false;
    private bool _isJoinedRoom = false;
    private int _cachedPlayerCount = 0;
    private Action _connectedToMasterCallback = null;
    private Action _joinOrCreateRoomCallback = null;
    private Action _leaveRoomCallback = null;
    private Action _leaveLobbyCallback = null;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = CURRENT_GAME_VERSION;

        CurrentClientState = PhotonNetwork.NetworkClientState;
    }

    private void Update()
    {
        if (_cachedPlayerCount == CurrentPlayerCount)
        {
            return;
        }

        _cachedPlayerCount = CurrentPlayerCount;

        OnRefreshPlayerListCallback?.Invoke(CurrentConnectedPlayerArray);
    }

    private void OnDestroy()
    {
        _roomOptionsDictionary?.Clear();
        _connectedToMasterCallback = null;
        _joinOrCreateRoomCallback = null;
        _leaveRoomCallback = null;
        _leaveLobbyCallback = null;
    }

    private void OnApplicationQuit()
    {
        if (_isConnectedInMasterServer)
        {
            Disconnect();
        }

        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        _isConnectedInMasterServer = true;

        if (PhotonNetwork.JoinLobby())
        {
#if UNITY_EDITOR
            Debug.Log($"Current Local Player Name : {PhotonNetwork.NickName} Joined In Lobby.");
#endif
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        if (_isJoinedRoom == true)
            return;

        CurrentClientState = PhotonNetwork.NetworkClientState;

#if UNITY_EDITOR
        Debug.Log($"Current Local Player Name : {PhotonNetwork.NickName} Created Room.");
#endif
        _joinOrCreateRoomCallback?.Invoke();

        _isJoinedRoom = true;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        CurrentClientState = PhotonNetwork.NetworkClientState;

        _connectedToMasterCallback?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (_isJoinedRoom == true)
            return;

        CurrentClientState = PhotonNetwork.NetworkClientState;

#if UNITY_EDITOR
        Debug.Log($"Current Local Player Name : {PhotonNetwork.NickName} Joined In Room.");
#endif
        _joinOrCreateRoomCallback?.Invoke();

        _isJoinedRoom = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        _isConnectedInMasterServer = false;
#if UNITY_EDITOR
        Debug.Log("Disconnected Success");
#endif
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

#if UNITY_EDITOR
        Debug.Log("Left Lobby Success");
#endif
        CurrentClientState = PhotonNetwork.NetworkClientState;

        _leaveLobbyCallback?.Invoke();

        PhotonNetwork.Disconnect();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

#if UNITY_EDITOR
        Debug.Log("Left Room Success");
#endif
        CurrentClientState = PhotonNetwork.NetworkClientState;

        _isJoinedRoom = false;

        _leaveRoomCallback?.Invoke();
    }

    public void Connect(string name, Action callback = null)
    {
        PhotonNetwork.NickName = name;

        _connectedToMasterCallback -= callback;

        bool isConnectionSuccess = PhotonNetwork.ConnectUsingSettings(); // 마스터 서버에 접속 시도
        if (isConnectionSuccess)
        {
            _connectedToMasterCallback += callback;

            CurrentClientState = PhotonNetwork.NetworkClientState;            
            return;
        }

#if UNITY_EDITOR
        Debug.LogError("Photon Network Connection Failed!!");
#endif
    }

    public void LeaveRoom(Action callback = null)
    {
        if (PhotonNetwork.IsConnected == false)
        {
#if UNITY_EDITOR
            Debug.LogError("This user didn't connect to Master Server!!");
#endif
            return;
        }

        if (CurrentClientState != ClientState.Joining && CurrentClientState != ClientState.Joined)
        {
            return;
        }

        _leaveRoomCallback -= callback;
        _leaveRoomCallback += callback;

        PhotonNetwork.LeaveRoom();
    }

    public void Disconnect(Action callback = null)
    {
        if (PhotonNetwork.IsConnected == false)
        {
#if UNITY_EDITOR
            Debug.LogError("This user didn't connect to Master Server!!");
#endif
            return;
        }

        if (CurrentClientState != ClientState.JoinedLobby && CurrentClientState != ClientState.JoiningLobby)
        {
            return;
        }

        _leaveLobbyCallback -= callback;
        _leaveLobbyCallback += callback;

        PhotonNetwork.LeaveLobby();
    }

    public void JoinRoom(string roomName, Action callback = null)
    {
        if (PhotonNetwork.IsConnected == false)
        {
#if UNITY_EDITOR
            Debug.LogError("This user didn't connect to Master Server!!");
#endif
            return;
        }

        _joinOrCreateRoomCallback -= callback;
        _joinOrCreateRoomCallback += callback;

        if (_roomOptionsDictionary.TryGetValue(roomName, out RoomOptions roomOptions) == false)
        {
            roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 8;
            _roomOptionsDictionary.Add(roomName, roomOptions);
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public string GetConnectionState()
    {
        string state;

        switch (CurrentClientState)
        {
            case ClientState.JoinedLobby:
            case ClientState.JoiningLobby:
                state = "In Lobby";
                break;
            case ClientState.Joined:
            case ClientState.Joining:
                state = "In Room";
                break;
            default:
                state = "Log Out";
                break;
        }

        return string.Format(CONNECTION_STATE_FORMAT, state);
    }

}
