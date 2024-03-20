using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInput : MonoSingleton<MouseInput>
{
    public event Action<Vector2> OnLeftButtonDown, OnLeftButtonUp;
    private Vector2 MousePos;
 

    private void Update()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0)&&!IsOnUI()) { OnLeftButtonDown?.Invoke(MousePos); }
        if (Input.GetMouseButtonUp(0)&&!IsOnUI()) {  OnLeftButtonUp?.Invoke(MousePos); }
    }

    public Vector2 GetMousePosition()
    {
        return MousePos;
    }


    public bool IsOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


}
