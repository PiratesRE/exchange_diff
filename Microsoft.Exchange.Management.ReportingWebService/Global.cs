using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	public class Global : HttpApplication
	{
		static Global()
		{
			AppDomain.CurrentDomain.AssemblyResolve += Global.CurrentDomain_AssemblyResolve;
		}

		public static bool GetAppSettingAsBool(string key, bool defaultValue)
		{
			string value = ConfigurationManager.AppSettings[key];
			bool result;
			if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static int GetAppSettingAsInteger(string key, int defaultValue)
		{
			string text = ConfigurationManager.AppSettings[key];
			int result;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName assemblyName = new AssemblyName(args.Name);
			foreach (string path in Global.BinSearchFolders)
			{
				string text = Path.Combine(path, assemblyName.Name) + ".dll";
				if (File.Exists(text))
				{
					return Assembly.LoadFrom(text);
				}
			}
			return null;
		}

		private void Application_Start(object sender, EventArgs e)
		{
			this.InitializeWatsonReporting();
			this.InitializePerformanceCounter();
		}

		private void InitializeWatsonReporting()
		{
			bool appSettingAsBool = Global.GetAppSettingAsBool("WatsonEnabled", true);
			ServiceDiagnostics.InitializeWatsonReporting(appSettingAsBool);
		}

		private void InitializePerformanceCounter()
		{
			Globals.InitializeMultiPerfCounterInstance("RWS");
			foreach (ExPerformanceCounter exPerformanceCounter in RwsPerfCounters.AllCounters)
			{
				exPerformanceCounter.RawValue = 0L;
			}
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				RwsPerfCounters.PID.RawValue = (long)currentProcess.Id;
			}
		}

		private const string AppSettingSendWatsonReport = "WatsonEnabled";

		private static readonly string[] BinSearchFolders = ConfigurationManager.AppSettings["BinSearchFolders"].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
	}
}
