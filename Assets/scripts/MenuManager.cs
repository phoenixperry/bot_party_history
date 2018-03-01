using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Timeline;

public class MenuManager : AbstractManager
{
    int FRAME_NEXT_SOUND = 0;
    public AudioClip Bot1_Move_Sound, Bot2_Move_Sound, Bot3_Move_Sound;
    public AudioClip[] clips;
    public static int frameCount;
    public int RandomNumber { get; private set; }

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
            playRandomSound();
            FRAME_NEXT_SOUND = Time.frameCount + Random.Range(300, 1500);
        }

    }

    void playRandomSound()
    {
        int randomClip = Random.Range(0, clips.Length);
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clips[randomClip];
        source.Play();
        Destroy(source, clips[randomClip].length);
        //gameObject.GetComponent<AudioSource>().clip = Menu_Sound as AudioClip; //Menu_Sound as AudioClip;
        //gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("Play Me!");
    }

    public override void BoxOneStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot1_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("You've woken up Bot1!");
    }

    public override void BoxTwoStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot2_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("You've woken up Bot2!");
    }

    public override void BoxThreeStartMoving(double speed)
    {
        gameObject.GetComponent<AudioSource>().clip = Bot3_Move_Sound as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("You've woken up Bot3!");
    }
}
