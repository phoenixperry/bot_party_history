﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractManager : MonoBehaviour {

	/* 
	 * Class: AbstractSceneManager
	 * Extend AbstractSceneManager with your own class for your own scene.
	 * Each of the methods of this class (except OnEnable and OnDisable)
	 * Are events that are sent from the bots as certain things occur.
	 * Your concrete implementation of a SceneManager should use this events
	 * In order to produce whatever you're trying to do game-wise.
	 * You should not modify any other code other than to create a subclass
	 * of this class.
	 */

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

		BotData.OnBoxOneButtonDown += BoxOneButtonDown;
		BotData.OnBoxOneButtonUp += BoxOneButtonUp;

		BotData.OnBoxTwoButtonDown += BoxTwoButtonDown;
		BotData.OnBoxTwoButtonUp += BoxTwoButtonUp;

		BotData.OnBoxThreeButtonDown += BoxThreeButtonDown;
		BotData.OnBoxThreeButtonUp += BoxThreeButtonUp;

		BotData.OnBoxOneStartMoving += BoxOneStartMoving;
		BotData.OnBoxOneContinueMoving += BoxOneContinueMoving;
		BotData.OnBoxOneStopMoving += BoxOneStopMoving;

		BotData.OnBoxTwoStartMoving += BoxTwoStartMoving;
		BotData.OnBoxTwoContinueMoving += BoxTwoContinueMoving;
		BotData.OnBoxTwoStopMoving += BoxTwoStopMoving;

		BotData.OnBoxThreeStartMoving += BoxThreeStartMoving;
		BotData.OnBoxThreeContinueMoving += BoxThreeContinueMoving;
		BotData.OnBoxThreeStopMoving += BoxThreeStopMoving;

		BotData.OnBoxOneStartRotating += BoxOneStartRotating;
		BotData.OnBoxOneContinueRotating += BoxOneContinueRotating;
		BotData.OnBoxOneStopRotating += BoxOneStopRotating;

		BotData.OnBoxTwoStartRotating += BoxTwoStartRotating;
		BotData.OnBoxTwoContinueRotating += BoxTwoContinueRotating;
		BotData.OnBoxTwoStopRotating += BoxTwoStopRotating;

		BotData.OnBoxThreeStartRotating += BoxThreeStartRotating;
		BotData.OnBoxThreeContinueRotating += BoxThreeContinueRotating;
		BotData.OnBoxThreeStopRotating += BoxThreeStopRotating;
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

		BotData.OnBoxOneButtonDown -= BoxOneButtonDown;
		BotData.OnBoxOneButtonUp -= BoxOneButtonUp;

		BotData.OnBoxTwoButtonDown -= BoxTwoButtonDown;
		BotData.OnBoxTwoButtonUp -= BoxTwoButtonUp;

		BotData.OnBoxThreeButtonDown -= BoxThreeButtonDown;
		BotData.OnBoxThreeButtonUp -= BoxThreeButtonUp;

		BotData.OnBoxOneStartMoving -= BoxOneStartMoving;
		BotData.OnBoxOneContinueMoving -= BoxOneContinueMoving;
		BotData.OnBoxOneStopMoving -= BoxOneStopMoving;

		BotData.OnBoxTwoStartMoving -= BoxTwoStartMoving;
		BotData.OnBoxTwoContinueMoving -= BoxTwoContinueMoving;
		BotData.OnBoxTwoStopMoving -= BoxTwoStopMoving;

		BotData.OnBoxThreeStartMoving -= BoxThreeStartMoving;
		BotData.OnBoxThreeContinueMoving -= BoxThreeContinueMoving;
		BotData.OnBoxThreeStopMoving -= BoxThreeStopMoving;

		BotData.OnBoxOneStartRotating -= BoxOneStartRotating;
		BotData.OnBoxOneContinueRotating -= BoxOneContinueRotating;
		BotData.OnBoxOneStopRotating -= BoxOneStopRotating;

		BotData.OnBoxTwoStartRotating -= BoxTwoStartRotating;
		BotData.OnBoxTwoContinueRotating -= BoxTwoContinueRotating;
		BotData.OnBoxTwoStopRotating -= BoxTwoStopRotating;

		BotData.OnBoxThreeStartRotating -= BoxThreeStartRotating;
		BotData.OnBoxThreeContinueRotating -= BoxThreeContinueRotating;
		BotData.OnBoxThreeStopRotating -= BoxThreeStopRotating;

	}

	/* This section contains all of the events for boxes touching eachother.
	 * Each combination of boxes (One-Two, One-Three, Two-Three, All)
	 * Has a corresponding Connected and Released event */
	public virtual void AllConnected()
	{
		// Note to implementers: one of the other events will trigger a frame or two before this one because
		// All connected required 1-2 and/or 2-3 and/or 1-3.
		// Take this into account by making your AllConnected in some way override your individual ones.
	}

	public virtual void AllReleased()
	{
	}

	public virtual void BoxOneThreeConnected()
	{

	}
	public virtual void BoxOneThreeReleased()
	{

	}

	public virtual void BoxOneTwoConnected()
	{

	}
	public virtual void BoxOneTwoReleased()
	{

	}

	public virtual void BoxTwoThreeConnected()
	{

	}
	public virtual void BoxTwoThreeReleased()
	{

	}

	/* This section contains all of the data from the button presses.
	 * Each box (1, 2 and 3) has an event for their button being pressed and released.
	 */
	public virtual void BoxOneButtonDown() {

	}
	public virtual void BoxOneButtonUp() {

	}

	public virtual void BoxTwoButtonDown() {

	}
	public virtual void BoxTwoButtonUp() {

	}

	public virtual void BoxThreeButtonDown() {

	}
	public virtual void BoxThreeButtonUp() {

	}

	// Accelerometer events
	public virtual void BoxOneStartMoving(double speed) {
		
	}
	public virtual void BoxOneContinueMoving(double speed) {

	}
	public virtual void BoxOneStopMoving() {

	}

	public virtual void BoxTwoStartMoving(double speed) {

	}
	public virtual void BoxTwoContinueMoving(double speed) {

	}
	public virtual void BoxTwoStopMoving() {

	}

	public virtual void BoxThreeStartMoving(double speed) {

	}
	public virtual void BoxThreeContinueMoving(double speed) {

	}
	public virtual void BoxThreeStopMoving() {

	}

	// Rotating events
	public virtual void BoxOneStartRotating(double angular_speed) {

	}
	public virtual void BoxOneContinueRotating(double angular_speed) {

	}
	public virtual void BoxOneStopRotating() {

	}

	public virtual void BoxTwoStartRotating(double angular_speed) {

	}
	public virtual void BoxTwoContinueRotating(double angular_speed) {

	}
	public virtual void BoxTwoStopRotating() {

	}

	public virtual void BoxThreeStartRotating(double angular_speed) {

	}
	public virtual void BoxThreeContinueRotating(double angular_speed) {

	}
	public virtual void BoxThreeStopRotating() {

	}

	public delegate void LEDChange(int led, LED_CHANGES type, int parameter);
	public static event LEDChange DoLEDChange;
	public void TurnOnLEDOne() {
		DoLEDChange (1, LED_CHANGES.On, 0);
	}
	public void TurnOnLEDTwo() {
		DoLEDChange (2, LED_CHANGES.On, 0);
	}
	public void TurnOnLEDThree() {
		DoLEDChange (3, LED_CHANGES.On, 0);
	}

	public void TurnOffLEDOne() {
		DoLEDChange (1, LED_CHANGES.Off, 0);
	}
	public void TurnOffLEDTwo() {
		DoLEDChange (2, LED_CHANGES.Off, 0);
	}
	public void TurnOffLEDThree() {
		DoLEDChange (3, LED_CHANGES.Off, 0);
	}
}
