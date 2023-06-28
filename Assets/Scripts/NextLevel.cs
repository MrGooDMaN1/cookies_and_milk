using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
    public int numberScene;
    public string playerTag;

    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log("!");
        if (col.tag == playerTag)
        {
                SceneManager.LoadScene(numberScene);
        }
    }
}
