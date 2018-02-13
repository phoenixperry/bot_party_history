
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
   
    public void Start() {

    }

	void OnEnable() {
		AbstractReader.OnBotDataReceived += processData;
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
		// To implement: send the actual buttonup/down events
		if (b2.name == "") return;
		if (b1.btn == "1" && b2.btn == "0") {
			doButtonDownFor (b1);
		} else if (b1.btn == "0" && b2.btn == "1") {
			doButtonUpFor (b1);
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
