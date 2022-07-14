using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eSceneType
{
    Login,
    Lobby,
    Playground,
}

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    public void MoveScene(eSceneType sceneType)
    {
        MoveScene(sceneType.ToString());
    }

    public void MoveScene(string name)
    {
        Photon.Pun.PhotonNetwork.LoadLevel(name);
    }
}
