using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Timeline;

public class MenuManager : MonoBehaviour
{
    //float Timer = 0.0F;
    int FRAME_NEXT_SOUND = 0;
    public AudioClip Menu_Sound;
    //int Time.frameCount = 0;
    public static int frameCount;
    //public int Time.frameCount { get; }
    public int RandomNumber { get; private set; }

    // Use this for initialization
    // time related

    public void FreePlay()
    {
		SceneManager.LoadScene("FreePlay", LoadSceneMode.Single);
    }
    
    public void TouchTouchRevolution()
    {
        SceneManager.LoadScene("TouchTouchRevolution", LoadSceneMode.Single);
    }

    void Update()
        {

            if (Time.frameCount >= FRAME_NEXT_SOUND)
            {
            playSound();
            FRAME_NEXT_SOUND = Time.frameCount + Random.Range(300, 1500);
            }

        }
       
    void playSound()
    {
        //gameObject.transform.Find("RandomSound").GetComponent<AudioSource>().clip = Menu_Sound as AudioClip;
        //gameObject.transform.Find("RandomSound").GetComponent<AudioSource>().Play();
        gameObject.GetComponent<AudioSource>().clip = Menu_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("Play Me!");
    }

     void BotMovement()
     {
         //this function will play a random sound if one of the bots are moved
     }
}

