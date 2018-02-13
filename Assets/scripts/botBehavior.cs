using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class botBehavior : MonoBehaviour {


    public Button btn;

    // Use this for initialization
    public int xpos, ypos, zpos, compass = 0;
    public int btnState = 0;
    public string name; 

    private void Start()
    {
        gameObject.GetComponent<AudioSource>().playOnAwake = false;
    }

    public void OnEnable() {
		Debug.Log ("Enable");
    }
    public void OnDisable()
    {
        
    }

    
    public void setData(Bot b_) {
        name = b_.name;
        int.TryParse(b_.compass, out compass);
		int.TryParse(b_.xpos, out xpos);
		int.TryParse(b_.ypos, out ypos);
		int.TryParse(b_.zpos, out zpos);
		int.TryParse(b_.btn, out btnState);
    }
    public void setBtn(UnityEngine.UI.Button _btn) {

        btn = _btn;
        if (btnState == 1) {

            var colors = btn.colors;
            colors.normalColor = Color.green;
            btn.colors = colors;
        } 

    } 

    public void rotateBot(int xpos, int ypos, int zpos) {

        //TODO: make this a reasonable animation in the future 
        Vector3 angles = new Vector3((float)xpos, (float)ypos, (float)zpos);
        transform.Rotate(angles);     
    }


    public void updateDisplay() {
        btn.onClick.Invoke();
    }
    public void triggerSound(AudioClip sound) {

        gameObject.GetComponent<AudioSource>().clip  = sound;
        if (!gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

}
