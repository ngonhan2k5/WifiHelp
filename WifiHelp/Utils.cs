/*
 * Created by SharpDevelop.
 * User: Huy
 * Date: 9/25/2019
 * Time: 12:21 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
//using System.Management;
using System.Net.NetworkInformation;
using System.Diagnostics;

using System.Runtime ;
using System.Runtime.InteropServices ;

using NativeWifi;
using System.Collections.Generic;

namespace WifiHelp
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class Utils
	{
		private static int status = 0;
		public static void autoReconnectWifi(string ssid, EventLog eventLog){
			// current trouble wifi connected but no internet
			if (Utils.isNetConnected() && !Utils.isConnectedToInternet("8.8.8.8")){
				status = 2;
				logEvent(Utils.reConnect(ssid, true), eventLog, status);
			// wifi disconnected
			}else if (!Utils.isNetConnected()){
				status = 3;
				logEvent(Utils.reConnect(ssid), eventLog, status);
			}else{
				// only log once internet access success
				if (status != 1){
					status = 1;
					eventLog.WriteEntry(String.Format("Internet access Ok", ssid), EventLogEntryType.Information, status);
				}
			}
		}
		
		private static void logEvent(string[] msg, EventLog eventLog, int type){
			eventLog.WriteEntry(String.Join("\n", msg), type==2?EventLogEntryType.Error:EventLogEntryType.Warning, type);
		}
		
		public static bool isConnectedToInternet(string host)
		{
			bool result = false;
			Ping p = new Ping();
			try
			{
				PingReply reply = p.Send(host, 3000);
				if (reply.Status == IPStatus.Success)
					return true;
			}
			catch { }
			return result;
		}
		
		[DllImport("wininet.dll")]
		private extern static bool InternetGetConnectedState( out int Description, int ReservedValue ) ;
		// Creating a function that uses the API function...
		public static bool isNetConnected( )
		{
			int Desc ;
			return InternetGetConnectedState( out Desc, 0 ) ;
		}
		
		public static string[] reConnect(string ssid){
			return reConnect(ssid, false);
		}
		
		public static string[] reConnect(string ssid, bool force ){
			List<string> ret = new List<string>();
			WlanClient client = new WlanClient();
			foreach ( WlanClient.WlanInterface wlanIface in client.Interfaces )
			{
				string currentProfileName = ssid;
				if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected)
					currentProfileName = wlanIface.CurrentConnection.profileName;
				
				string name = "";
				// Retrieves XML configurations of existing profiles.
				// This can assist you in constructing your own XML configuration
				// (that is, it will give you an example to follow).
				foreach ( Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles() )	{
										
					if (profileInfo.profileName == currentProfileName){
						string xml = wlanIface.GetProfileXml( profileInfo.profileName );
						name = profileInfo.profileName;
						if ( wlanIface.InterfaceState == Wlan.WlanInterfaceState.Disconnected ){// wlanIface.CurrentConnection.isState == Wlan.WlanInterfaceState.Disconnected){
							// Wifi disconnected, connect use ssid
							Console.WriteLine( "Connect");
							ret.Add(String.Format("Profile {0} found. Wifi downed -> Connect now", ssid));
							wlanIface.Connect( Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileInfo.profileName );
						
						}else if (force){
							// Disconnect then reconnect
							wlanIface.Disconnect();
							ret.Add(String.Format("Profile {0} found. Wifi up but no internet access -> Force Disconnect and Connect now", ssid));
							System.Threading.Thread.Sleep(1000);
							wlanIface.Connect( Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileInfo.profileName );
							
						}else{
							// Do nothing
							Console.WriteLine("Connected do nothing");
							ret.Add(String.Format("Profile {0} found. Wifi ok", ssid));
						}
						break;
					}
					Console.WriteLine("No profile {0}", ssid );
					ret.Add(String.Format("No Profile {0} found.", ssid));
				}				

			}
			return ret.ToArray();
		}
		private static Dictionary<string, string> args;
		
		public static string param(string key, string def){
			return Utils.args.ContainsKey(key)? args[key]: def;
		}
		
		public static string param(string key){
			return param(key, "");
		}
		
		public static int param(string key, int def){
			try {
				return Utils.args.ContainsKey(key)? Int32.Parse(args[key]): def;
			}catch (FormatException e){
				return def;
			}
		}
		public static Dictionary<string, string> parseArgs(){
			if (args == null) {
				string[] imagePathArgs = Environment.GetCommandLineArgs();
				Dictionary<string, string> map = new Dictionary<string, string>();
				for(int i =1; i< imagePathArgs.Length; i++){
					string arg =imagePathArgs[i];

					string value = arg.Substring(arg.IndexOf("=")+1);
					string key = arg.Substring(2, arg.Length- value.Length-3);
					map.Add(key,value);                
				}
				Utils.args = map;
			}
			return Utils.args;
		}
	}
}
