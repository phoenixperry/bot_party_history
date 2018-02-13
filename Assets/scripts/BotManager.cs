using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BotManager : MonoBehaviour {


    public Button btn;

    // Use this for initialization

    public GameObject bot1;
    public GameObject bot2;
    public GameObject bot3;


    public void OnEnable()
    {
        TouchManager.OnBoxOneTwoTouched += BoxOneTwoConnected;
        TouchManager.OnBoxOneTwoReleased += BoxOneTwoReleased;

        TouchManager.OnBoxOneThreeTouched += BoxOneThreeConnected;
        TouchManager.OnBoxOneThreeReleased += BoxOneThreeReleased;

        TouchManager.OnBoxTwoThreeTouched += BoxTwoThreeConnected;
        TouchManager.OnBoxTwoThreeReleased += BoxTwoThreeReleased;

        TouchManager.OnAllBoxesConnected += AllConnected;
        TouchManager.OnAllBoxesReleased += AllReleased;
    }
    //here's how to unsubscibe - if you do one, you must do the other! 
    public void OnDisable()
    {
        TouchManager.OnBoxOneTwoTouched -= BoxOneTwoConnected;
        TouchManager.OnBoxOneTwoReleased -= BoxOneTwoReleased;

        TouchManager.OnBoxOneThreeTouched -= BoxOneThreeConnected;
        TouchManager.OnBoxOneThreeReleased -= BoxOneThreeReleased;

        TouchManager.OnBoxTwoThreeTouched -= BoxTwoThreeConnected;
        TouchManager.OnBoxTwoThreeReleased -= BoxTwoThreeReleased;

        TouchManager.OnAllBoxesConnected -= AllConnected;
        TouchManager.OnAllBoxesReleased -= AllReleased;

    }

    public void setBtn(UnityEngine.UI.Button _btn)
    {

        btn = _btn;
    }
    public void rotateBot(int xpos, int ypos, int zpos)
    {

        //TODO: make this a reasonable animation in the future 
        Vector3 angles = new Vector3((float)xpos, (float)ypos, (float)zpos);
        transform.Rotate(angles);
    }

    public void triggerSound()
    {

        btn.onClick.Invoke();

    }

    public void AllConnected()
    {
        //all connected actions 
    }

    public void AllReleased()
    {
        //all connected actions 
    }

    public void BoxOneThreeConnected()
    {

    }
    public void BoxOneThreeReleased()
    {

    }
    public void BoxOneTwoConnected()
    {

	}
    public void BoxOneTwoReleased()
    {

    }
    public void BoxTwoThreeConnected()
    {
		Debug.Log ("BOX TWO AND THREE TOUCH!");
    }
    public void BoxTwoThreeReleased()
    {
		Debug.Log ("BOX TWO AND THREE RELEASED.");
    }
}

