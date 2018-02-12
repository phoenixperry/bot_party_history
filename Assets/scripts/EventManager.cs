using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {


    //these are the events you can resister functions to if you want to listen for touch events. 
    /*If you want to use them, you need to use the EventManager Object. Go into the class where the methodd you want to have run with the event occurs and add an OnEnable and OnDisable function subscripting to these events 
     * Here's a sample
     * public class sampleClass:MonoBehaviour
     *  {
     *      public void onEnable()
     *      {
     *          TouchData.OnBoxOneTwoTouched += myFunction;  
     *      }
     *      public void onDisable
     *      {
     *          TouchData.OnBoxOneTwoTouched -= myFunctions; 
     *      }
     *      void myFunction() {
     *          //nonsense to have happen when box one and two are touched. 
     *      } 
     *  }
     *  
     */


    /*
     * If you want to trigger an event from this class, all you have to do is this 
     *  if(OnBoxOneTwoTouched != null) //a null check is required - if you have an event with no subscribers, it will cause issues in the codebase. 
     *      OnBoxOneTwoTouched(); 
     */
    //here are all the events you can subscribe to elsewhere in the code 

   




    public delegate void


} 
