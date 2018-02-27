using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    float Timer = 0.0F;

    public int RandomNumber { get; private set; }

    // Use this for initialization
    // time related

    public void FreePlay()
    {
        SceneManager.LoadScene("FreePlay", LoadSceneMode.Additive);
    }
    
    public void TouchTouchRevolution()
    {
        SceneManager.LoadScene("TouchTouchRevolution", LoadSceneMode.Additive);
    }

   /* void Update()
    {
        Debug.Log(Random.Range(1, 20));
        if (int = 2)
        {
            gameObject.GetComponent<AudioSource>
        }
        //this function will play a sound at random intervals
        // time.time + random = play sound
    }

    void BotMovement()
    {
        //this function will play a random sound if one of the bots are moved
    }*/
}

