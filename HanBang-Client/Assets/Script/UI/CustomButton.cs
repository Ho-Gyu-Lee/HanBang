using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    public delegate void OnButtonEnterDelegate();
    public OnButtonEnterDelegate ButtonEnter;

    public delegate void OnButtonUpDelegate();
    public OnButtonUpDelegate ButtonUp;

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        ButtonUp();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }
}
