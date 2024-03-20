using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform background;
    public RectTransform rectTransform;

    Camera UICamera;
    void Start()
    {
        UICamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
       Vector2 screenPoint = Camera.main.WorldToScreenPoint(background.position);
       Vector2 UiPos;
       transform.position=screenPoint;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, UICamera, out UiPos);
    }
}
