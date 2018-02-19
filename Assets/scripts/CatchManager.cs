using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ButtonState
{ None=0, BtnOne, BtnTwo, BtnThree}; //enum lets you use words to make the code more understandable

public class CatchManager : AbstractManager //referencing the Abstract script for calling events
{
    public static int TO_WIN = 10; //the score to win the game
    bool enabled = true; // game state
    ButtonState target = ButtonState.None; //beginning target
    int score = 0; //starting score
    public AudioClip Sound_Win, Sound_Success, Sound_Fail; //sound files in the game
    float TimerLength = 3.0f;
    float StartTime = 0.0f;
    float CurrentTime = 0.0f;
    float EndTime = 0.0f;
    //float UpdateTime = 0.0f;
    new void OnEnable() //new game, start game
    {
        base.OnEnable();
        score = 0;
        newTarget();
        //LEDs all off to start
    }
    private void Start()
    {
        EndTime = Time.time + TimerLength;
    }

    void Update()
    {
        
        //CurrentTime = Time.time;
       /* if (StartTime + TimerLength > CurrentTime)
        {
          // Debug.Log("Bang " + CurrentTime);
           CurrentTime = StartTime;
        }*/

    }
    
    void win() //what happens when I get 10 points, sound plays
    {
        gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "You win!";
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Stop();
        if (!gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().isPlaying)
        {
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Win as AudioClip;
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();
            gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().loop = true;
        }
    }
    void newTarget() //call for a new target button
    {
        
        ButtonState new_t = target;
        while (new_t == target)
        {
            new_t = (ButtonState)(Random.Range(1, 4));
        }
        target = new_t;
        
        gameObject.transform.Find("TargetText").GetComponent<TextMesh>().text = "Target: " + target;
        
        //LED light on for new target and make sure old LED is off again
        //dimmer LED if available
    }

    void fail() //sound plays when you fail //** now currently don't need fail as if you get one button wrong the game is over
    {
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Fail as AudioClip;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();
    }

    void success() //correct button press means more prizes
    {
        score += 1;
        Debug.Log(score);

        //timer shortens as you progress?
        // StartTime = Time.time;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().clip = Sound_Success as AudioClip;
        gameObject.transform.Find("ButtonSound").GetComponent<AudioSource>().Play();

    }
    bool GameOver = false; //game isnt over
    void checkMatch(ButtonState input) //checking what is happening in the game
    {
           if (!enabled) 
               return;
       // Debug.Log("function ran");
        if (!GameOver)
        {
            //Debug.Log("Target Button: " + input);
            if (input == target && Time.time <= EndTime)     //StartTime + TimerLength >= CurrentTime)
            {
                //Debug.Log("whaaaaat");
                success();
                newTarget();
                
                if (score < TO_WIN)
                {
                    EndTime = Time.time + TimerLength;
                }
                else
                {
                    GameOver = true;
                    Debug.Log("Game Over");
                }
                //turn off LED?

            }
            else if (input != target)
            {
               // GameOver = true;
                Debug.Log(input + "" + target);
            }

            if (score >= TO_WIN)
            {
                enabled = false; //game finished
                win();
                //GameOver = false;
                Debug.Log("yay");
            }

        }
    }// these are from abstract manager to work the events

    public override void BoxOneButtonDown()
    {
      //  Debug.Log("Hello Bot 1!");
        checkMatch(ButtonState.BtnOne);
    }

    public override void BoxTwoButtonDown()
    {
       // Debug.Log("Hello Bot 2!");
        checkMatch(ButtonState.BtnTwo);
    }

    public override void BoxThreeButtonDown()
    {
       // Debug.Log("Hello Bot 3!");
        checkMatch(ButtonState.BtnThree);
    }

    
}

