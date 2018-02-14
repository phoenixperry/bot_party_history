
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotData : MonoBehaviour

{

    //this class deals with all bot data 

    public static string botName;
    public static ArrayList compass = new ArrayList();
    public static int btn;
    private static string[] sensors;
	public GameObject bot1;
	public GameObject bot2;
	public GameObject bot3;

	public Bot bot1_data;
	public Bot bot2_data;
	public Bot bot3_data;

    public int integratedCompass;
    int btn1Down = 0; //saves btn down state 
    int btn2Down = 0;
    int btn3Down = 0;

	public delegate void BoxOneButtonDown();
	public static event BoxOneButtonDown OnBoxOneButtonDown;
	public delegate void BoxOneButtonUp();
	public static event BoxOneButtonUp OnBoxOneButtonUp;

	public delegate void BoxTwoButtonDown();
	public static event BoxTwoButtonDown OnBoxTwoButtonDown;
	public delegate void BoxTwoButtonUp();
	public static event BoxTwoButtonUp OnBoxTwoButtonUp;

	public delegate void BoxThreeButtonDown();
	public static event BoxThreeButtonDown OnBoxThreeButtonDown;
	public delegate void BoxThreeButtonUp();
	public static event BoxThreeButtonUp OnBoxThreeButtonUp;

	public delegate void BoxOneStartMoving(double speed);
	public static event BoxOneStartMoving OnBoxOneStartMoving;
	public delegate void BoxOneContinueMoving(double speed);
	public static event BoxOneContinueMoving OnBoxOneContinueMoving;
	public delegate void BoxOneStopMoving ();
	public static event BoxOneStopMoving OnBoxOneStopMoving;

	public delegate void BoxTwoStartMoving(double speed);
	public static event BoxTwoStartMoving OnBoxTwoStartMoving;
	public delegate void BoxTwoContinueMoving(double speed);
	public static event BoxTwoContinueMoving OnBoxTwoContinueMoving;
	public delegate void BoxTwoStopMoving ();
	public static event BoxTwoStopMoving OnBoxTwoStopMoving;

	public delegate void BoxThreeStartMoving(double speed);
	public static event BoxThreeStartMoving OnBoxThreeStartMoving;
	public delegate void BoxThreeContinueMoving(double speed);
	public static event BoxThreeContinueMoving OnBoxThreeContinueMoving;
	public delegate void BoxThreeStopMoving ();
	public static event BoxThreeStopMoving OnBoxThreeStopMoving;
   
    public void Start() {

    }

	void OnEnable() {
        AbstractInputReader.OnBotDataReceived += processData;
	}

	private void processData(Bot b) {
		if (b.name == "botOne") {
			processBotDifference (b, bot1_data);
			bot1_data = b;
		} else if (b.name == "botTwo") {
			processBotDifference (b, bot2_data);
			bot2_data = b;
		} else if (b.name == "botThree") {
			processBotDifference (b, bot3_data);
			bot3_data = b;
		}
	}

	private void processBotDifference(Bot b1, Bot b2)
	{
		if (b2.name == "") return;

		// Deals with button presses
		if (b1.btn == "1" && b2.btn == "0") {
			doButtonDownFor (b1);
		} else if (b1.btn == "0" && b2.btn == "1") {
			doButtonUpFor (b1);
		}

		processAccelerometer (b2, b1);

	}
	public static double DELTA = 0.25;
	public static double THRESHOLD = 5;
	public static double CAP = 10;
	public static double STICKINESS = 0.5;
	public double bot1_mv_avg = 0.0;
	public double bot2_mv_avg = 0.0;
	public double bot3_mv_avg = 0.0;
	public bool box1_moving = false;
	public bool box2_moving = false;
	public bool box3_moving = false;

	/*
	 * This works by taking a weighted moving average of the speed
	 * v(t+1) = new * d + (1-d) * v(t)
	 * if v(t+1) > T and v(t) < T then Move event
	 * if v(t+1) < T and v(t) > T then Stop Move event
	 * If new > CAP then new = cap (to stop large movement frames from skewing the data too much)
	 */
	private void processAccelerometer(Bot b1, Bot b2)
	{
		int b1x, b2x, b1y, b2y, b1z, b2z;
		int.TryParse(b1.xpos, out b1x); 		int.TryParse(b2.xpos, out b2x);
		int.TryParse(b1.ypos, out b1y); 		int.TryParse(b2.ypos, out b2y);
		int.TryParse(b1.zpos, out b1z); 		int.TryParse(b2.zpos, out b2z);
		double magnitude_change = Mathf.Sqrt (Mathf.Pow((b1x - b2x),2) + Mathf.Pow((b1y - b2y),2) + Mathf.Pow((b1z - b2z), 2));
		if (magnitude_change > CAP) {
			magnitude_change = CAP;
		}
		double new_mv_avg;			
		if (b1.name == "botOne") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot1_mv_avg;
			if (!box1_moving && new_mv_avg > THRESHOLD) {
				box1_moving = true;
				OnBoxOneStartMoving (new_mv_avg);
			} else if (box1_moving && new_mv_avg < THRESHOLD - STICKINESS) {
				box1_moving = false;
				OnBoxOneStopMoving ();
			} else if (box1_moving) {
				OnBoxOneContinueMoving (new_mv_avg);
			}
			bot1_mv_avg = new_mv_avg;
		} else if (b1.name == "botTwo") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot2_mv_avg;
			if (!box2_moving && new_mv_avg > THRESHOLD) {
				box2_moving = true;
				OnBoxTwoStartMoving (new_mv_avg);
			} else if (box2_moving && new_mv_avg < THRESHOLD-STICKINESS) {
				box2_moving = false;
				OnBoxTwoStopMoving ();
			} else if (box2_moving) {
				OnBoxTwoContinueMoving (new_mv_avg);
			}
			bot2_mv_avg = new_mv_avg;
		} else if (b1.name == "botThree") {
			new_mv_avg = DELTA * magnitude_change + (1 - DELTA) * bot3_mv_avg;
			if (!box3_moving && new_mv_avg > THRESHOLD) {
				box3_moving = true;
				OnBoxThreeStartMoving (new_mv_avg);
			} else if (box3_moving && new_mv_avg < THRESHOLD-STICKINESS) {
				box3_moving = false;
				OnBoxThreeStopMoving ();
			} else if (box3_moving) {
				OnBoxThreeContinueMoving (new_mv_avg);
			}
			bot3_mv_avg = new_mv_avg;
		}
	}

	private void doButtonUpFor(Bot b1)
	{
		if (b1.name == "botOne")
			OnBoxOneButtonUp ();
		else if (b1.name == "botTwo")
			OnBoxTwoButtonUp ();
		else if (b1.name == "botThree")
			OnBoxThreeButtonUp ();
	}

	private void doButtonDownFor(Bot b1){
		if (b1.name == "botOne")
			OnBoxOneButtonDown ();
		else if (b1.name == "botTwo")
			OnBoxTwoButtonDown ();
		else if (b1.name == "botThree")
			OnBoxThreeButtonDown ();
	}
    
    public void updateData(string values)

    {
        compass.Clear();
        sensors = values.Split(' '); //split the array at every space. we use a space to deliminate our values from Arduino 
        botName = sensors[0]; //get which bot we're dealing with, which is saved in the 0 position 
        // Debug.Log(name);
        compass.Add(sensors[1]); //integrated compass 
        compass.Add(sensors[2]); //x 
        compass.Add(sensors[3]); //y
        compass.Add(sensors[4]); //z  
        int.TryParse(sensors[5], out btn); //btn

        //Debug.Log("Bot Parsed: " + botName + " btn " + btn + "Compass vals" + compass[0] + " " + compass[1] + " " + compass[2] + " " + compass[3]);
        //Debug.Log(botName+ botName.Length); 
        routeData();
    }
    public void routeData()
    {
        if (botName == "botOne")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 

            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);
            //send the accelerometer vals to the bot using them  
            bot1.GetComponent<botBehavior>().rotateBot(xpos, ypos, zpos); // move the bot by the x, y, z positions 
            //if the btn is down  & and it was not down in the last frame  
            if (btn == 1 && btn1Down == 0)
            {
                Debug.Log("btn 1 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn1Down = 1;
                //bot1.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn1.colors;
                //colors.normalColor = Color.red;
                //btn1.colors = colors;
            }
            else if (btn == 0)
            {
                btn1Down = 0; //reset the btn flag so the button can fire again 
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn1.colors;
                //colors.normalColor = Color.green;
                //btn1.colors = colors;
            }

        } else if (botName == "botTwo")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 
            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);
            //send the accelerometer vals to the bot using them  
            bot2.GetComponent<botBehavior>().rotateBot(xpos, ypos, zpos); // move the bot by the x, y, z positions 

            if (btn == 1 && btn2Down == 0)
            {
                Debug.Log("btn 2 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn2Down = 1;
                //bot2.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn2.colors;
                //colors.normalColor = Color.red;
                //btn2.colors = colors;
            } else if (btn == 0)
            {
                btn2Down = 0; //reset the btn flag so the button can fire again
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn2.colors;
                //colors.normalColor = Color.green;
                //btn2.colors = colors;
            }
        } else if (botName == "botThree")
        {
            //route teh compass values 
            string val = compass[0] as string;
            int.TryParse(val, out integratedCompass);
            //eventually, the compass data should be passed to the functions that need it here 

            //route the accelerometer values 
            val = compass[1] as string;
            int xpos;
            int.TryParse(val, out xpos);
            val = compass[2] as string;
            int ypos;
            int.TryParse(val, out ypos);
            val = compass[3] as string;
            int zpos;
            int.TryParse(val, out zpos);
            //send the accelerometer vals to the bot using them  
            bot3.GetComponent<botBehavior>().rotateBot(xpos, ypos, zpos); // move the bot by the x, y, z positions 

            if (btn == 1 && btn3Down == 0)
            {
                Debug.Log("btn 3 fired");
                //flip on the bool that checks if it is down on so it does not trigger repeatedly. 
                btn3Down = 1;
                //bot3.GetComponent<botBehavior>().triggerSound(); //trigger the sound 
                //var colors = btn3.colors;
                //colors.normalColor = Color.red;
                //btn3.colors = colors;
            } else if (btn == 0)
            {
                btn3Down = 0; //reset the btn flag so the button can fire again 
                              //create a color then assign it through to the btn to show it's off now 
                //var colors = btn3.colors;
                //colors.normalColor = Color.green;
                //btn3.colors = colors;
            }
      }
    
    }
 }
