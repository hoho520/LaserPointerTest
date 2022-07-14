using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    private readonly string MAIN_CAMERA = "MainCamera";

    public Camera MainCamera { get; private set; }

    public override void Init()
    {
        base.Init();

        MainCamera = GameObject.FindGameObjectWithTag(MAIN_CAMERA)?.GetComponent<Camera>();
    }

    public override void Release()
    {
        base.Release();

        MainCamera = null;
    }
}
