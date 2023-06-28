using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GD : MonoBehaviour
{
    public bool isGrounded; //Против двойного прыжка
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }
}
