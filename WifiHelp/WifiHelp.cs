/*
 * Created by SharpDevelop.
 * User: Huy
 * Date: 9/24/2019
 * Time: 8:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
//using System.Threading;
using System.Timers;

namespace WifiHelp
{
	public class WifiHelp : ServiceBase
	{
		public const string MyServiceName = "WifiHelp";
		EventLog eventLog1;
		Timer timer = new Timer();
		const string LOG_SOURCE = "WifiHelp";
		const string LOG_NAME = "Application";
		
		public WifiHelp()
		{
			InitializeComponent();
			
			Utils.parseArgs();
			
			eventLog1 = new System.Diagnostics.EventLog();
			if (!System.Diagnostics.EventLog.SourceExists(LOG_SOURCE))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					LOG_SOURCE, LOG_NAME);
			}
			eventLog1.Source = LOG_SOURCE;
			eventLog1.Log = LOG_NAME;
		}
		
		private void InitializeComponent()
		{
			this.ServiceName = MyServiceName;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			// TODO: Add cleanup code here (if required)
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			
			// TODO: Add start code here (if required) to start your service.
			eventLog1.WriteEntry("In OnStart."+ String.Format("ssid={0} time={1}", Utils.param("ssid"), Utils.param("time")));
			timer.Elapsed += new ElapsedEventHandler(OnTimer);
			timer.Interval = 15000;//Utils.param("time", 15000); //number in milisecinds
			timer.Enabled = true;
		}
		
		public void OnTimer(object sender, ElapsedEventArgs args)
		{
			// TODO: Insert monitoring activities here.		
			Utils.autoReconnectWifi(Utils.param("ssid", "12 Le Thanh Ton"), eventLog1);

		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// TODO: Add tear-down code here (if required) to stop your service.
			 
		}
	}
}
