# WifiHelp Service
Auto reconnect wifi when have no internet access (fix lost internet after resume from window sleep).
Run as a service to allow working when not login yet.
If wifi down -> turn on wifi ap that have ssid setup in config.cmd.
If wifi still on but no internet access -> restart wifi with the same ssid.

* REQUIRED: DOT.NET 2.0 or 3 or 4 installed.

* INSTALL SERVICE:
Set SSID and check interval TICK in config.cmd before run install.cmd

* UNINSTALL SERVICE: run uninstall.cmd

# Credit: 
ManagedWifi : https://archive.codeplex.com/?p=managedwifi
