All of the Bluetooth Low Energy interaction is handled in the IBleBridge Interface. See  the "IBleBridge.cs" file it is thoroughly commented in "StarTechPlus -> uBleBridge -> Scripts"...

The basic usage is to create a script and attach it to a GameObject.  In that script instantiate an IBleBridge object then call the various IBleBridge functions with appropriate Action() callbacks to interact with Ble Devices.

On iOS be sure to add the CoreBluetooth.framework to the Xcode project, then on successive build just Append to the project when asked by Unity... 


There are two example Scenes one for controlling a Blue-Rx 16 channel servo controller and one for reading a TI SensorTag.  There are README.txt files located with each example. "SensorTag.cs" and "BlueRx.cs" are also thoroughly commented...

If you have questions don't hesitate to email me, Jason Peterson. Also if you find a bug or improvement please let me know so I can keep the plugin updated!

Thanks!

For more info on Blue-Rx see:

For more information see:
http://repo.startechplus.com/bluerx

To purchase a Blue-Rx goto:
http://www.gigamint.com

For more info on the SensorTag see:
http://www.ti.com/ww/en/wireless_connectivity/sensortag

Email:
Jason Peterson
jason@startechplus.com