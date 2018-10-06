The OSX plugin is still in testing... you can enable it with something like this:

//Determine which native IBleBridge to use based on the runtime platform; Android or iOS
switch(Application.platform)
{
case RuntimePlatform.Android:
	bleBridge = new AndroidBleBridge();
	break;
case RuntimePlatform.IPhonePlayer:
	bleBridge = new iOSBleBridge();
	break;
case RuntimePlatform.OSXEditor:
	bleBridge = new OsxBleBridge ();
	break;
case RuntimePlatform.OSXPlayer:
	bleBridge = new OsxBleBridge();
	break;
default:
	bleBridge = new DummyBleBridge(); //modify this class if you want to emulate ble interaction in the editor...
	break;
}