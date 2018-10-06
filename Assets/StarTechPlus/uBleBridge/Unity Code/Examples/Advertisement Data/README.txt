Import the StrangeIoc Unity package if you want to use uBluetoothLe as a service for Strange Inversion of Control...

To use the service inject the IBluetoothService and BluetoothLeEventSignal

Bind the service

...

if (Application.platform == RuntimePlatform.IPhonePlayer)
	injectionBinder.Bind<IBleBridge> ().To<iOSBleBridge> ().ToSingleton ();
else if (Application.platform == RuntimePlatform.Android)
	injectionBinder.Bind<IBleBridge> ().To<AndroidBleBridge> ().ToSingleton ();
else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
	injectionBinder.Bind<IBleBridge> ().To<OsxBleBridge> ().ToSingleton ();
else
	injectionBinder.Bind<IBleBridge>().To<DummyBleBridge>().ToSingleton();

injectionBinder.Bind<IBluetoothLeService>().To<BluetoothLeService>().ToSingleton();

injectionBinder.Bind<BluetoothLeEventSignal>().ToSingleton();

...

[Inject]
public IBluetoothLeService ble {get; set;}

[Inject]
public BluetoothLeEventSignal bleSignal{ get; set; }

...

add ble listeners

...

public void bleSignalHandler(string key, Dictionary<string, object> data) {

	...	
		
	Debug.Log ("BLE: " + key);

	...
}

...

bleSignal.AddListener (bleSignalHandler);

...

start the service

...

ble.Startup ();

...



See: http://strangeioc.github.io/strangeioc/