using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    public delegate void OnButtonDownDelegate();
    public OnButtonDownDelegate ButtonDown;

    public delegate void OnButtonUpDelegate();
    public OnButtonUpDelegate ButtonUp;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        ButtonDown();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        ButtonUp();
    }
}
