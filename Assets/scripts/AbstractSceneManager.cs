using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSceneManager : MonoBehaviour {

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

	public void AllConnected()
	{
		//all connected actions
		// Note to implementers: one of the other events will trigger a frame or two before this one because
		// All connected required 1-2 and/or 2-3 and/or 1-3.
		// Take this into account by making your AllConnected in some way override your individual ones.
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

	}
	public void BoxTwoThreeReleased()
	{

	}
}
