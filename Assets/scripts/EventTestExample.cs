using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTestExample : MonoBehaviour {
    //hello friends and my future self which forgets all code, 
    //here's an exmaple of how to subscribe an event! 
 
    public void OnEnable()
    {
        TouchManager.OnBoxOneTwoTouched += testFunctionOneTouch;
        TouchManager.OnBoxOneTwoReleased += testFunctionOneRelease;

        TouchManager.OnBoxOneThreeTouched += testFunctionTwoTouch;
        TouchManager.OnBoxOneThreeReleased += testFunctionTwoRelease;

        TouchManager.OnBoxTwoThreeTouched += testFunction3Touch;
        TouchManager.OnBoxTwoThreeReleased += testFunction3Release;

        TouchManager.OnAllBoxesConnected += testFunction4Touch;
        TouchManager.OnAllBoxesReleased += testFunction4Release;

		BotData.OnBoxOneButtonDown += testBoxOneButtonDown;
		BotData.OnBoxOneButtonUp += testBoxOneButtonUp;

		BotData.OnBoxTwoButtonDown += testBoxTwoButtonDown;
		BotData.OnBoxTwoButtonUp += testBoxTwoButtonUp;

		BotData.OnBoxThreeButtonDown += testBoxThreeButtonDown;
		BotData.OnBoxThreeButtonUp += testBoxThreeButtonUp;

		BotData.OnBoxOneStartMoving += testBoxOneStartMoving;
		BotData.OnBoxTwoStartMoving += testBoxTwoStartMoving;
		BotData.OnBoxThreeStartMoving += testBoxThreeStartMoving;

    }
    //here's how to unsubscibe - if you do one, you must do the other! 
    public void OnDisable() {

        TouchManager.OnBoxOneTwoTouched -= testFunctionOneTouch;
        TouchManager.OnBoxOneTwoReleased -= testFunctionOneRelease;

        TouchManager.OnBoxOneThreeTouched -= testFunctionTwoTouch;
        TouchManager.OnBoxOneThreeReleased -= testFunctionTwoRelease;

        TouchManager.OnBoxTwoThreeTouched -= testFunction3Touch;
        TouchManager.OnBoxTwoThreeReleased -= testFunction3Release;

        TouchManager.OnAllBoxesConnected -= testFunction4Touch;
        TouchManager.OnAllBoxesReleased -= testFunction4Release;

		BotData.OnBoxOneButtonDown -= testBoxOneButtonDown;
		BotData.OnBoxOneButtonUp -= testBoxOneButtonUp;

		BotData.OnBoxTwoButtonDown -= testBoxTwoButtonDown;
		BotData.OnBoxTwoButtonUp -= testBoxTwoButtonUp;

		BotData.OnBoxThreeButtonDown -= testBoxThreeButtonDown;
		BotData.OnBoxThreeButtonUp -= testBoxThreeButtonUp;
    }

    void testFunctionOneTouch() {
    Debug.Log("OnBoxOneTwo touch event works!"); 
    }

    void testFunctionOneRelease()
    {
        Debug.Log("OnBoxOneTwo release event works!");
    }

    void testFunctionTwoTouch()
    {
        Debug.Log("OnBoxOneThree touch event works!");
    }

    void testFunctionTwoRelease()
    {
        Debug.Log("OnBoxOneThree release event works!");
    }

    void testFunction3Touch() {
        Debug.Log("Two three touch event works");
    }

    void testFunction3Release() {
        Debug.Log("two three release event works");
    }

    void testFunction4Touch()
    {
        Debug.Log("all touching event works");
    }
     
    void testFunction4Release()
    {
        Debug.Log("all touching release works");
    }

	void testBoxOneButtonDown()
	{
		Debug.Log ("Button one works.");
	}

	void testBoxTwoButtonDown()
	{
		Debug.Log ("Button two works.");
	}

	void testBoxThreeButtonDown()
	{
		Debug.Log ("Button three works.");
	}

	void testBoxOneButtonUp()
	{
		Debug.Log ("Button one up.");
	}

	void testBoxTwoButtonUp()
	{
		Debug.Log ("Button two up.");
	}

	void testBoxThreeButtonUp()
	{
		Debug.Log ("Button three up.");
	}
		
	void testBoxOneStartMoving(double speed) 
	{
		Debug.Log("Box one moving.");
	}

	void testBoxTwoStartMoving(double speed) 
	{
		Debug.Log ("Box two moving.");
	}

	void testBoxThreeStartMoving(double speed)
	{
		Debug.Log("Box three moving.");
	}
}
