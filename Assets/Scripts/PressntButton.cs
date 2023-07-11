using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressntButton : MonoBehaviour
{
    private bool isPressed;
    public bool IsPressed
    { 
        get { return isPressed; }
    }

    public void OnPointerDown()
    {
        isPressed = true;
    }
    public void OnPointerUp()
    {
        isPressed = false;
    }
}
