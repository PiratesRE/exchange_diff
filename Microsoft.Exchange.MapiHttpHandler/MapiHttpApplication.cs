using System;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	public class MapiHttpApplication : HttpApplication
	{
		static MapiHttpApplication()
		{
			MapiHttpApplication.fileSearchAssemblyResolver.FileNameFilter = new Func<string, bool>(MapiHttpApplication.AssemblyFileNameFilter);
			MapiHttpApplication.fileSearchAssemblyResolver.SearchPaths = new string[]
			{
				ExchangeSetupContext.InstallPath
			};
			MapiHttpApplication.fileSearchAssemblyResolver.Recursive = true;
			MapiHttpApplication.fileSearchAssemblyResolver.Install();
		}

		private static bool AssemblyFileNameFilter(string fileName)
		{
			if (AssemblyResolver.ExchangePrefixedAssembliesOnly(fileName))
			{
				return true;
			}
			foreach (string text in MapiHttpApplication.approvedAssemblies)
			{
				if (text.EndsWith("."))
				{
					if (fileName.StartsWith(text, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				else if (string.Compare(fileName, text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private void Application_Start(object sender, EventArgs e)
		{
			this.InitializePerformanceCounter();
			SettingOverrideSync.Instance.Start(true);
		}

		private void Application_End(object sender, EventArgs e)
		{
			try
			{
				MapiHttpHandler.ShutdownHandler();
				SettingOverrideSync.Instance.Stop();
			}
			catch (Exception)
			{
			}
		}

		private void Application_Error(object sender, EventArgs e)
		{
		}

		private void InitializePerformanceCounter()
		{
			Globals.InitializeMultiPerfCounterInstance("MapiHttp");
		}

		private static FileSearchAssemblyResolver fileSearchAssemblyResolver = new FileSearchAssemblyResolver();

		private static string[] approvedAssemblies = new string[]
		{
			"Microsoft.",
			"System."
		};
	}
}
