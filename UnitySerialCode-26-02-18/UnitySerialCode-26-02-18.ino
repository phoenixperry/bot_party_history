#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_LSM303_U.h>
#define TCAADDR 0x70

/*
  CALIBRATION VARIABLES
  - calibrated: setting this to false turns on the calibration routine.
  - skipBoxes: tk
*/
bool calibrated = true;
bool skipBoxes = false;

// inbetween box touching state bools
bool TOUCH_1_2;
bool TOUCH_1_3;
bool TOUCH_2_3;
bool TOUCH_ALL;

/*
  PIN SETUP
*/
// console buttons
int PIN_MENU_1 = 0;
int PIN_MENU_2 = 1;

// touches
const int PIN_TOUCH_1 = A0;
const int PIN_TOUCH_2 = A1;
const int PIN_TOUCH_3 = A2;

// averages for different touches
int avTOUCH_1_2 = 0;
int avTOUCH_1_3 = 0;
int avTOUCH_2_3 = 0;
int avTOUCH_1_2_3 = 20;
int con = 20;

////bot One vars
int PIN_BUTTON_1 = 2;
int PIN_LED_1 = 8;
int PIN_LED_STRIP_1 = 11; // PP: Change this!
int PIN_IMU_1 = 5;
int PIN_MOTOR_1 = 5; // PP: Change this!
String OUTPUTSTRING_1 = "botOne";
float comp_x_1 = 0;
float comp_y_1 = 0;
float comp_z_1 = 0;
//
////bot Two vars
int PIN_BUTTON_2 = 3;
int PIN_LED_2 = 9;
int PIN_LED_STRIP_2 = 12; // PP: Change this!
int PIN_IMU_2 = 6;
int PIN_MOTOR_2 = 6; // PP: Change this!
String OUTPUTSTRING_2 = "botTwo";
float comp_x_2 = 0;
float comp_y_2 = 0;
float comp_z_2 = 0;
//
//
////bot Three vars
int PIN_BUTTON_3 = 4;
int PIN_LED_3 = 10;
int PIN_LED_STRIP_3 = 13; // PP: Change this!
int PIN_IMU_3 = 7;
int PIN_MOTOR_3 = 7; // PP: Change this!
String OUTPUTSTRING_3 = "botThree";
float comp_x_3 = 0;
float comp_y_3 = 0;
float comp_z_3 = 0;

// pins for testing!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// touches
//  const int box1 = A2;
//  const int box2 = A0;
//  const int box3 = A1;

// //bot One vars
// int btn_1 = 8;
// int led_1 = 9;
// int IMU_1 = 3;
// String botOne = "botOne";
// float comp_x_1 = 0;
// float comp_y_1 = 0;
// float comp_z_1 = 0;

// //bot Two vars
// int btn_2 = 13;
// int led_2 = 11;
// int IMU_2 = 2;
// String botTwo = "botTwo";
// float comp_x_2 = 0;
// float comp_y_2 = 0;
// float comp_z_2 = 0;

// //bot Three vars
// int btn_3 = 12;
// int led_3 = 10;
// int IMU_3 = 4;
// String botThree = "botThree";
// float comp_x_3 = 0;
// float comp_y_3 = 0;
// float comp_z_3 = 0;
// end testing case!!!!!!!!!!!!!!!!!!!

int pulse = 0;
int pulseSpeed = 1;

/* Assign a unique ID to this sensor at the same time */
Adafruit_LSM303_Mag_Unified mag1 = Adafruit_LSM303_Mag_Unified(PIN_IMU_1);
Adafruit_LSM303_Mag_Unified mag2 = Adafruit_LSM303_Mag_Unified(PIN_IMU_2);
Adafruit_LSM303_Mag_Unified mag3 = Adafruit_LSM303_Mag_Unified(PIN_IMU_3);

void sensorRead(int IMU, Adafruit_LSM303_Mag_Unified *mag, float &comp_x, float &comp_y, float &comp_z, String &botID, int &led, int &btn);

void readSerialFull();

bool inRange(int val, int minimum, int maximum);
// launches the calibration routine
void calibrate();

// calibrates the boxes during the calibration routine
int calibrateBoxes(int box_a, int box_b);

void displaySensorDetails(Adafruit_LSM303_Mag_Unified *mag)
{
  sensor_t sensor;
  mag->getSensor(&sensor);
  //  Serial.println("------------------------------------");
  //  Serial.print  ("Sensor:       "); Serial.println(sensor.name);
  //  Serial.print  ("Driver Ver:   "); Serial.println(sensor.version);
  //  Serial.print  ("Unique ID:    "); Serial.println(sensor.sensor_id);
  //  Serial.print  ("Max Value:    "); Serial.print(sensor.max_value); Serial.println(" m/s^2");
  //  Serial.print  ("Min Value:    "); Serial.print(sensor.min_value); Serial.println(" m/s^2");
  //  Serial.print  ("Resolution:   "); Serial.print(sensor.resolution); Serial.println(" m/s^2");
  //  Serial.println("------------------------------------");
  //  Serial.println("");
  delay(500);
}

void tcaselect(uint8_t i)
{
  if (i > 7)
    return;

  Wire.beginTransmission(TCAADDR);
  Wire.write(1 << i);
  Wire.endTransmission();
}

float run_compass(Adafruit_LSM303_Mag_Unified *mag, float &compass_x, float &compass_y, float &compass_z)
{
  // compass code
  /* Get a new sensor event */
  sensors_event_t event;
  mag->getEvent(&event);

  float Pi = 3.14159;

  compass_x = event.magnetic.x;
  compass_y = event.magnetic.y;
  compass_z = event.magnetic.z;

  // Calculate the angle of the vector y,x
  float heading = (atan2(compass_y, compass_x) * 180) / Pi;

  // Normalize to 0-360
  if (heading < 0)
  {
    heading = 360 + heading;
  }
  // Serial.print("Compass Heading: ");
  return heading;
}

bool inRange(int val, int minimum, int maximum)
{
  return ((minimum <= val) && (val <= maximum));
}

void setup()
{
  Serial.begin(115200);
  pinMode(PIN_BUTTON_1, INPUT_PULLUP);
  pinMode(PIN_BUTTON_2, INPUT_PULLUP);
  pinMode(PIN_BUTTON_3, INPUT_PULLUP);
  pinMode(PIN_TOUCH_1, INPUT_PULLUP);
  pinMode(PIN_TOUCH_2, INPUT_PULLUP);
  pinMode(PIN_TOUCH_3, INPUT_PULLUP);
  pinMode(PIN_MENU_1, INPUT_PULLUP);
  pinMode(PIN_MENU_2, INPUT_PULLUP);

  //  Serial.println("Hi");
  //  Serial.println("mag Test"); Serial.println("");

  /* Initialise the 1st sensor */
  //  tcaselect(IMU_1);
  //  mag1.begin();
  //  tcaselect(IMU_1);
  //  mag1.enableAutoRange(true);
  //  if (!mag1.begin())
  //  {
  //    /* There was a problem detecting the HMC5883 ... check your connections */
  //    Serial.println("Ooops, no LM303 1 detected ... Check your wiring!");
  //    //while (1);
  //  }
  //
  //  tcaselect(IMU_2);
  //  mag2.begin();
  //  tcaselect(IMU_2);
  //  mag2.enableAutoRange(true);
  //  if (!mag2.begin())
  //  {
  //    /* There was a problem detecting the HMC5883 ... check your connections */
  //    Serial.println("Ooops, no LM303 2 detected ... Check your wiring!");
  //   // while (1);
  //  }
  //
  //  tcaselect(IMU_3);
  //  mag3.begin();
  //  tcaselect(IMU_3);
  //  mag3.enableAutoRange(true);
  //  if (!mag3.begin())
  //  {
  //    /* There was a problem detecting the HMC5883 ... check your connections */
  //    Serial.println("Ooops, no LM303 3 detected ... Check your wiring!");
  //   // while (1);
  //  }

  //  /* Display some basic information on this sensor */
  //  tcaselect(IMU);
  //  displaySensorDetails(&mag1);

  // calls the calibration routine
  calibrate();
}

/* 
  SENSOR FUNCTIONS
  These functions deal with getting sensor data for touch and the accelerometers.
*/
int readTouches(int pin1, int pin2)
{
  int sum = 0;

  for (int i = 0; i < 10; i++)
  {
    pinMode(pin1, OUTPUT);
    digitalWrite(pin1, HIGH);
    // delayMicroseconds(10);
    int high = analogRead(pin2);

    digitalWrite(pin1, LOW);
    // delayMicroseconds(10);
    int low = analogRead(pin2);

    pinMode(pin1, INPUT);
    //   digitalWrite(pin1,LOW);

    sum = sum + high - low;
  }
  // Serial.println("!");
  return (int)((float)sum / 4.0f);
}

void sensorRead(int IMU, Adafruit_LSM303_Mag_Unified *mag, float &comp_x, float &comp_y, float &comp_z, String botID, int led, int btn)
{

  //  sensors_event_t event;
  //  tcaselect(IMU);
  //  mag->getEvent(&event);

  //  int heading = run_compass(mag,comp_x,comp_y,comp_z);
  Serial.print(botID);
  Serial.print(" ");
  //  Serial.print(heading);
  Serial.print(" ");
  Serial.print(int(comp_x));
  Serial.print(" ");
  Serial.print(int(comp_y));
  Serial.print(" ");
  Serial.print(int(comp_z));
  Serial.print(" ");
  int btnVal = digitalRead(btn);
  if (btnVal == 1)
  {
    Serial.println('0');
    // analogWrite(led,255);
  }
  else
  {
    Serial.println('1');
    if (pulse > 255 || pulse < 0)
      pulseSpeed = pulseSpeed * -1;
    // (led,pulse);
    pulse = pulse + pulseSpeed;
  }
}

/* 
  LOOP FUNCTIONS
  Deals with the core read/write loop.

  Variables:
    - LOOP_MAIN_TIME: How much delay between new runs of the touch data being sent.
    - LOOP_READS_PER_LOOP: How many times the serial port is read for new e.g. LED changes between each instance of touch data being sent.
    - LOOP_LED_SKIP_EVERY: How often the LED fades happen, versus runs of the read loop.
    - LOOP_LED_COUNTER: DO NOT TOUCH (loop variable for tracking the above.)
*/
int LOOP_MAIN_TIME = 16;
int LOOP_READS_PER_LOOP = 2;
int LOOP_LED_SKIP_EVERY = 7;

int LOOP_LED_COUNTER = 0;


void loop(void)
{
  if (skipBoxes == false)
  {
    sensorRead(PIN_IMU_1, &mag1, comp_x_1, comp_y_1, comp_z_1, "botOne", PIN_LED_1, PIN_BUTTON_1);
    sensorRead(PIN_IMU_2, &mag2, comp_x_2, comp_y_2, comp_z_2, "botTwo", PIN_LED_2, PIN_BUTTON_2);
    sensorRead(PIN_IMU_3, &mag3, comp_x_3, comp_y_3, comp_z_3, "botThree", PIN_LED_3, PIN_BUTTON_3);

    touchInnerLoop();

    menuInnerLoop();

    writePinInnerLoop();

    int i = 0;
    while (i < LOOP_READS_PER_LOOP)
    {
      readInnerLoop();
      i++;
      delay(LOOP_MAIN_TIME / LOOP_READS_PER_LOOP);
    }


  }
}

void writePinInnerLoop() {
    writeLedOne();
    writeLedTwo();    
    writeLedThree();
    writeMotorOne();
    writeMotorTwo();
    writeMotorThree();
}

void menuInnerLoop() {
  Serial.print("menuButtons ");
  if (digitalRead(PIN_MENU_1) == 1)
  {
    Serial.print('0');
  }
  else
  {
    Serial.print('1');
  }
  Serial.print(" ");
  if (digitalRead(PIN_MENU_2) == 1)
  {
    Serial.println('0');
  }
  else
  {
    Serial.println('1');
  }
}

void touchInnerLoop() {
   int ar = readTouches(PIN_TOUCH_1, PIN_TOUCH_2);

    ar = map(ar, 0, 1014, 0, 10);
    TOUCH_1_2 = inRange(ar, avTOUCH_1_2 - 5, avTOUCH_1_2 + 5);

    Serial.print("BoxOneTwo ");
    if (TOUCH_1_2 == 0)
      Serial.println(1);
    else
      Serial.println(0);
    // Serial.println(TOUCH_1_2);

    ar = readTouches(PIN_TOUCH_1, PIN_TOUCH_3);
    ar = map(ar, 0, 1024, 0, 10);
    TOUCH_1_3 = inRange(ar, avTOUCH_1_3 - 5, avTOUCH_1_3 + 5);
    Serial.print("BoxOneThree ");
    if (TOUCH_1_3 == 0)
      Serial.println(1);
    else
      Serial.println(0);
    //       Serial.println(TOUCH_1_3);

    ar = readTouches(PIN_TOUCH_2, PIN_TOUCH_3);
    ar = map(ar, 0, 1024, 0, 10);
    TOUCH_2_3 = inRange(ar, avTOUCH_2_3 - 5, avTOUCH_2_3 + 5);

    Serial.print("BoxTwoThree ");
    if (TOUCH_2_3 == 0)
      Serial.println(1);
    else
      Serial.println(0);

    if (!TOUCH_1_2 && !TOUCH_1_3 && !TOUCH_2_3)
    {
      TOUCH_ALL = true;
    }
    else
    {
      TOUCH_ALL = false;
    }

    Serial.print("AllBoxes ");
    Serial.println(TOUCH_ALL);
}

void readInnerLoop() {
  
    readSerialFull();
    if ((LOOP_LED_COUNTER = ++LOOP_LED_COUNTER % LOOP_LED_SKIP_EVERY) == 0)
    {
      fadeLedOne();
      fadeLedTwo();
      fadeLedThree();
    }
}

/* 
  LED VARIABLES
  - ALL VARIABLES: DO NOT TOUCH (loop/management of LED variables.)
  
  LED modes: 0 ("set to value"), 1 ("fade on to 255"), 2 ("fade off to 0")
*/
int LED_1_VALUE = 0;
int LED_1_MODE = 0;
int LED_1_PARAMETER = 0;

int LED_2_VALUE = 0;
int LED_2_MODE = 0;
int LED_2_PARAMETER = 0;

int LED_3_VALUE = 0;
int LED_3_MODE = 0;
int LED_3_PARAMETER = 0;

/* 
  MOTOR VARIABLES
  - MOTOR_BASE_VALUE: the number of inner loops the motor turns on for
  - MOTOR_1_VALUE, MOTOR_2_VALUE, MOTOR_3_VALUE: DO NOT TOUCH (loop variables)
*/
int MOTOR_BASE_VALUE = 3;
int MOTOR_1_VALUE = 0;
int MOTOR_2_VALUE = 0;
int MOTOR_3_VALUE = 0;

/*
  READ SERIAL FUNCTIONS
  These functions handle the incoming serial data.
*/
void readSerialFull()
{
  String data;
  if (Serial.available() >= 6)
  {
    data = Serial.readString();
    Serial.print("GOT " + data);

    int length = data.length();
    int i = 0;

    while (i < length)
    {
      if (data[i] == 'X')
      {
        readSerialOne(data.substring(i + 1, i + 6));
        i = i + 6;
      }
      else
      {
        i++;
      }
    }
  }
}

bool readSerialOne(String instruction)
{
  int led = instruction.substring(0, 1).toInt();
  int parameter = instruction.substring(2, 5).toInt();
  char mode = instruction[1];
  if (mode == 'Y')
  {
    ledOn(led, parameter);
  }
  else if (mode == 'N')
  {
    ledOff(led, parameter);
  }
  else if (mode == 'S')
  {
    ledSetTo(led, parameter);
  }
  else if (mode == 'F')
  {
    ledFadeOn(led, parameter);
  }
  else if (mode == 'f')
  {
    ledFadeOff(led, parameter);
  }
  return true;
}


/*
  LED SETUP FUNCTIONS
  These functions set up LED (and motor) actions to then be processed by the WRITE FUNCTIONS.
*/
void ledFadeOn(int led, int parameter)
{
  if (led == 1)
  {
    LED_1_MODE = 1;
    LED_1_PARAMETER = parameter;
    MOTOR_1_VALUE = MOTOR_BASE_VALUE;
  }
  else if (led == 2)
  {
    LED_2_MODE = 1;
    LED_2_PARAMETER = parameter;
    MOTOR_2_VALUE = MOTOR_BASE_VALUE;
  }
  else if (led == 3)
  {
    LED_3_MODE = 1;
    LED_3_PARAMETER = parameter;
    MOTOR_3_VALUE = MOTOR_BASE_VALUE;
  }
}
void ledFadeOff(int led, int parameter)
{
  if (led == 1)
  {
    LED_1_MODE = 2;
    LED_1_PARAMETER = parameter;
    MOTOR_1_VALUE = MOTOR_BASE_VALUE;
  }
  else if (led == 2)
  {
    LED_2_MODE = 2;
    LED_2_PARAMETER = parameter;
    MOTOR_2_VALUE = MOTOR_BASE_VALUE;
  }
  else if (led == 3)
  {
    LED_3_MODE = 2;
    LED_3_PARAMETER = parameter;
    MOTOR_3_VALUE = MOTOR_BASE_VALUE;
  }
}
void ledSetTo(int led, int parameter)
{
  if (led == 1)
  {
    LED_1_VALUE = parameter;
    LED_1_MODE = 0;
    LED_1_PARAMETER = 0;
  }
  else if (led == 2)
  {
    LED_2_VALUE = parameter;
    LED_2_MODE = 0;
    LED_2_PARAMETER = 0;
  }
  else if (led == 3)
  {
    LED_3_VALUE = parameter;
    LED_3_MODE = 0;
    LED_3_PARAMETER = 0;
  }
}
void ledOn(int led, int parameter)
{
  ledSetTo(led, 255);
}
void ledOff(int led, int parameter) {
  ledSetTo(led, 0);
}

/*
  LED FADE FUNCTIONS
  These functions deal with LED fading.
*/
void fadeLedOne() {
  if (LED_1_MODE == 1)
  {
    // Fade on
    if (LED_1_PARAMETER == 0)
    {
      ledOn(1, 0);
    }
    else
    {
      LED_1_VALUE = LED_1_VALUE + ((255 - LED_1_VALUE) / LED_1_PARAMETER);
      if (LED_1_VALUE > 255)
      {
        LED_1_VALUE = 255;
      }
      LED_1_PARAMETER -= 1;
    }
  }
  else if (LED_1_MODE == 2)
  {
    if (LED_1_PARAMETER == 0)
    {
      ledOff(1, 0);
    }
    else
    {
      LED_1_VALUE = LED_1_VALUE - ((LED_1_VALUE) / LED_1_PARAMETER);
      if (LED_1_VALUE < 0)
      {
        LED_1_VALUE = 0;
      }
      LED_1_PARAMETER -= 1;
    }
}
}

void fadeLedTwo() {
  if (LED_2_MODE == 1)
  {
    // Fade on
    if (LED_2_PARAMETER == 0)
    {
      ledOn(2, 0);
    }
    else
    {
      LED_2_VALUE = LED_2_VALUE + ((255 - LED_2_VALUE) / LED_2_PARAMETER);
      if (LED_2_VALUE > 255)
      {
        LED_2_VALUE = 255;
      }
      LED_2_PARAMETER -= 1;
    }
  }
  else if (LED_2_MODE == 2)
  {
    if (LED_2_PARAMETER == 0)
    {
      ledOff(2, 0);
    }
    else
    {
      LED_2_VALUE = LED_2_VALUE - ((LED_2_VALUE) / LED_2_PARAMETER);
      if (LED_2_VALUE < 0)
      {
        LED_2_VALUE = 0;
      }
      LED_2_PARAMETER -= 1;
    }
  }
}

void fadeLedThree() {
  if (LED_3_MODE == 1)
  {
    // Fade on
    if (LED_3_PARAMETER == 0)
    {
      ledOn(3, 0);
    }
    else
    {
      LED_3_VALUE = LED_3_VALUE + ((255 - LED_3_VALUE) / LED_3_PARAMETER);
      if (LED_3_VALUE > 255)
      {
        LED_3_VALUE = 255;
      }
      LED_3_PARAMETER -= 1;
    }
  }
  else if (LED_3_MODE == 2)
  {
    if (LED_3_PARAMETER == 0)
    {
      ledOff(3, 0);
    }
    else
    {
      LED_3_VALUE = LED_3_VALUE - ((LED_3_VALUE) / LED_3_PARAMETER);
      if (LED_3_VALUE < 0)
      {
        LED_3_VALUE = 0;
      }
      LED_3_PARAMETER -= 1;
    }
  }
}

/*
  WRITE FUNCTIONS
  These functions directly write to the LED and Motor pins, using data from the LED SETUP FUNCTIONS.
*/
void writeLedOne()
{
  analogWrite(PIN_LED_1, LED_1_VALUE);
  analogWrite(PIN_LED_STRIP_1, LED_1_VALUE);
}
void writeLedTwo()
{
  analogWrite(PIN_LED_2, LED_2_VALUE);
  analogWrite(PIN_LED_STRIP_2, LED_2_VALUE);
}
void writeLedThree()
{
  analogWrite(PIN_LED_3, LED_3_VALUE);
  analogWrite(PIN_LED_STRIP_3, LED_3_VALUE);
}
void writeMotorOne()
{
  if (MOTOR_1_VALUE > 0)
  {
    digitalWrite(PIN_MOTOR_1, HIGH);
    MOTOR_1_VALUE--;
  }
  else
  {
    digitalWrite(PIN_MOTOR_1, LOW);
  }
}
void writeMotorTwo()
{
  if (MOTOR_2_VALUE > 0)
  {
    digitalWrite(PIN_MOTOR_2, HIGH);
    MOTOR_2_VALUE--;
  }
  else
  {
    digitalWrite(PIN_MOTOR_2, LOW);
  }
}
void writeMotorThree()
{
  if (MOTOR_3_VALUE > 0)
  {
    digitalWrite(PIN_MOTOR_3, HIGH);
    MOTOR_3_VALUE--;
  }
  else
  {
    digitalWrite(PIN_MOTOR_3, LOW);
  }
}

/* 
  CALIBRATION FUNCTIONS
*/
bool started = false;

bool TOUCH_2_3_calibrated = false;
bool TOUCH_1_3_calibrated = false;
bool TOUCH_1_2_calibrated = false;

void calibrate()
{
  while (!calibrated)
  {
    // put your main code here, to run repeatedly:
    String incoming = Serial.readStringUntil('\n');

    if (started == false)

    {
      Serial.println("touch boxes 1 & 2 then type ! in the above text box & enter");
    }

    if (incoming.length() > 0)
      ;
    {
      // note incoming includes a return character and new line (two bytes)

      if (incoming == "!" && TOUCH_1_2_calibrated == false)
      {
        started = true;
        avTOUCH_1_2 = calibrateBoxes(PIN_TOUCH_1, PIN_TOUCH_2);
        Serial.println("Done calibrating boxes 1&2");
        Serial.print("Their average is saved ");
        Serial.println(avTOUCH_1_2);
        Serial.println("Now touch box1 and box3 and type @ & enter to proceed");
        TOUCH_1_2_calibrated = true;
      }
    }
    /// now calibrating boxes 1&3
    if (incoming == "@" && TOUCH_1_2_calibrated)
    {
      avTOUCH_1_3 = calibrateBoxes(PIN_TOUCH_1, PIN_TOUCH_3);
      Serial.println("Done calibrating boxes 1&3");
      Serial.print("Their average is saved ");
      Serial.println(avTOUCH_1_3);
      Serial.println("Now touch box2 and box3 and type # & enter to proceed");
      TOUCH_1_3_calibrated = true;
    }
    /// now calibrating boxes 2&3
    if (incoming == "#" && TOUCH_1_3_calibrated)
    {
      avTOUCH_2_3 = calibrateBoxes(PIN_TOUCH_2, PIN_TOUCH_3);
      Serial.println("Done calibrating boxes 2&3");
      Serial.print("Their average is saved ");
      Serial.println(avTOUCH_2_3);
      Serial.println("That's it! We're about to start up data sending!");
      TOUCH_2_3_calibrated = true;
      calibrated = true;
      delay(1500);
    }
  }
}
int calibrateBoxes(int box_a, int box_b)
{
  Serial.println("You've got 2 seconds to touch boxes! Starting up");
  delay(2000);
  double startTime = millis();
  double endTime = startTime + 5000;

  int total = 0;
  int samples = 0;

  while (startTime < endTime)
  {
    Serial.println("reading for 5 seconds");
    int theseBoxes = readTouches(box_a, box_b);
    theseBoxes = map(theseBoxes, 0, 1024, 0, 10);

    Serial.println(theseBoxes);
    startTime = millis();

    // safe guarding against bad touches.
    if (theseBoxes > 2)
    {
      total = theseBoxes + total;
      samples = samples + 1;
    }

    delay(100);
  }

  return total / samples;
}