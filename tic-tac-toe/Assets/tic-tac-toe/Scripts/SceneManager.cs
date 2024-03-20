using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneManager : MonoSingleton<SceneManager>
{
    // Start is called before the first frame update
    Camera mainCamera;

    public Transform[] cameraPosion;

    int level; //0主菜单、1难度选择、游戏界面

    void Start()
    {
        level=0;
        mainCamera=Camera.main;
        Screen.SetResolution(1920, 1080, true);
    }

    private void Update() {
        mainCamera.transform.position=Vector3.Lerp(mainCamera.transform.position,cameraPosion[level].position+new Vector3(0,0,-10),Time.deltaTime/0.2f);
    }

    public void JumpLevel(int newLevel){
        level=newLevel;
    }

}
