using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmScriptingTest : MonoBehaviour {

    // Use this for initialization
	void Start () {

       
    }

    public void OnEnable()
    {
        gameObject.GetComponent<AudioHelm.Sequencer>().OnBeat += ChangeColor;
    }

    public void OnDisable()
    {
        gameObject.GetComponent<AudioHelm.Sequencer>().OnBeat -= ChangeColor;
    }

    // Update is called once per frame
    void Update () {
		
	}
//here's what it looks like when you subscribe a function to an event.   
   public void ChangeColor(int note) {

        Camera.main.backgroundColor = new Color(note/14.0f, 0.0f, note/14.0f);
        Debug.Log("i'm firing on the beat and the note was "  +note);
    } 




}
