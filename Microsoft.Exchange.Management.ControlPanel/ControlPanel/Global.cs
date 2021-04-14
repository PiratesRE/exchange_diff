using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Web;
using System.Web.UI;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class Global : HttpApplication
	{
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyName assemblyName = new AssemblyName(args.Name);
			if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
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

		static Global()
		{
			AppDomain.CurrentDomain.AssemblyResolve += Global.CurrentDomain_AssemblyResolve;
		}

		private void Application_Start(object sender, EventArgs e)
		{
			EcpEventLogConstants.Tuple_EcpStarted.LogEvent(new object[]
			{
				AppDomain.CurrentDomain.FriendlyName
			});
			if (!Util.IsDataCenter)
			{
				ServiceDiscovery.ADNotificationScope = ExchangeTopologyScope.ADAndExchangeServerAndSiteAndVirtualDirectoryTopology;
			}
			Globals.InitializeMultiPerfCounterInstance("ECP");
			foreach (ExPerformanceCounter exPerformanceCounter in EcpPerfCounters.AllCounters)
			{
				exPerformanceCounter.RawValue = 0L;
			}
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				EcpPerfCounters.PID.RawValue = (long)currentProcess.Id;
			}
			ADSession.DisableAdminTopologyMode();
			BaseSprite.IsDataCenter = Util.IsMicrosoftHostedOnly;
			BaseSprite.GetSpriteImageSrc = new GetSpriteImageSrcDelegate(Util.GetSpriteImageSrc);
			VirtualDirectoryConfiguration.EcpVirtualDirectoryAnonymousAuthenticationEnabled = VirtualDirectoryConfiguration.GetEcpAnonymousAuthenticationStatus();
			string identification = ConfigurationManager.AppSettings["ProvisioningCacheIdentification"];
			ProvisioningCache.InitializeAppRegistrySettings(identification);
			this.InvokeExtendedApplicationStart(sender, e);
			ScriptManager.ScriptResourceMapping.AddDefinition("MicrosoftAjax.js", new ScriptResourceDefinition
			{
				Path = ThemeResource.ScriptPath + "microsoftajax.js",
				DebugPath = ThemeResource.ScriptPath + "microsoftajax.debug.js"
			});
		}

		private void Application_End(object sender, EventArgs e)
		{
			EcpEventLogConstants.Tuple_EcpDisposed.LogEvent(new object[]
			{
				AppDomain.CurrentDomain.FriendlyName
			});
			this.InvokeExtendedApplicationEnd(sender, e);
		}

		private void InvokeExtendedApplicationStart(object sender, EventArgs e)
		{
			foreach (object obj in this.GetExtendedApplicationInstances())
			{
				MethodInfo method = obj.GetType().GetMethod("Application_Start", new Type[]
				{
					typeof(object),
					typeof(EventArgs)
				});
				if (method != null)
				{
					method.Invoke(obj, new object[]
					{
						sender,
						e
					});
				}
			}
		}

		private void InvokeExtendedApplicationEnd(object sender, EventArgs e)
		{
			foreach (object obj in this.GetExtendedApplicationInstances())
			{
				MethodInfo method = obj.GetType().GetMethod("Application_End", new Type[]
				{
					typeof(object),
					typeof(EventArgs)
				});
				if (method != null)
				{
					method.Invoke(obj, new object[]
					{
						sender,
						e
					});
				}
			}
		}

		private IEnumerable<object> GetExtendedApplicationInstances()
		{
			List<object> list = new List<object>();
			string text = ConfigurationManager.AppSettings["ExtendedApplications"];
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length > 0)
				{
					foreach (string text2 in array)
					{
						string[] array3 = text2.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (array3 == null || array3.Length != 2)
						{
							throw new Exception("Invalid extended application configuration '" + text2 + "' found.");
						}
						ObjectHandle objectHandle = Activator.CreateInstance(array3[1].Trim(), array3[0].Trim());
						if (objectHandle != null)
						{
							list.Add(objectHandle.Unwrap());
						}
					}
				}
			}
			return list;
		}

		private static readonly string[] BinSearchFolders = ConfigurationManager.AppSettings["BinSearchFolders"].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
	}
}
