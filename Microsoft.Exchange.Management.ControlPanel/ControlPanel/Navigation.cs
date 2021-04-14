using System;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("Navigation", "Microsoft.Exchange.Management.ControlPanel.Client.Navigation.js")]
	public class Navigation : ScriptComponent
	{
		private int HybridSyncTimeoutInSeconds
		{
			get
			{
				return Navigation.hybridSyncTimeoutInSeconds.Value;
			}
		}

		private bool DisableHybridSyncCheck
		{
			get
			{
				return Navigation.disableHybridEndDate.Value > DateTime.UtcNow;
			}
		}

		public bool HasHybridParameter { get; set; }

		public NavigationTreeNode NavigationTree { get; set; }

		public string CloudServer { get; set; }

		public string ServerVersion { get; set; }

		private bool CloseWindowOnLogout
		{
			get
			{
				if (!this.IsLegacyLogOff || Util.IsDataCenter)
				{
					return false;
				}
				VdirConfiguration instance = VdirConfiguration.Instance;
				return !instance.FormBasedAuthenticationEnabled && !HttpContext.Current.Request.IsAuthenticatedByAdfs() && (instance.BasicAuthenticationEnabled || instance.WindowsAuthenticationEnabled || instance.DigestAuthenticationEnabled);
			}
		}

		private bool IsLegacyLogOff
		{
			get
			{
				return LogOnSettings.IsLegacyLogOff;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddScriptProperty("NavTree", this.NavigationTree.ToJsonString(null));
			descriptor.AddProperty("CloudServer", this.CloudServer, true);
			descriptor.AddProperty("ServerVersion", this.ServerVersion);
			if (this.HasHybridParameter)
			{
				descriptor.AddProperty("DisableHybridSyncCheck", this.DisableHybridSyncCheck);
				descriptor.AddProperty("HybridSyncTimeoutInSeconds", this.HybridSyncTimeoutInSeconds);
			}
			descriptor.AddProperty("CmdletLoggingEnabled", EacFlightUtility.GetSnapshotForCurrentUser().Eac.CmdletLogging.Enabled);
			descriptor.AddProperty("ShowPerfConsole", StringComparer.OrdinalIgnoreCase.Equals("true", WebConfigurationManager.AppSettings["ShowPerformanceConsole"]), true);
			descriptor.AddScriptProperty("RbacData", ClientRbac.GetRbacData().ToJsonString(null));
			descriptor.AddProperty("CloseWindowOnLogout", this.CloseWindowOnLogout, true);
			descriptor.AddProperty("IsLegacyLogOff", this.IsLegacyLogOff, true);
		}

		private static Lazy<DateTime> disableHybridEndDate = new Lazy<DateTime>(delegate()
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime t = utcNow.AddDays(30.0);
			string configStringValue = AppConfigLoader.GetConfigStringValue("DisableHybridSyncCheckEndDate", null);
			DateTime minValue = DateTime.MinValue;
			if (!string.IsNullOrEmpty(configStringValue))
			{
				if (!DateTime.TryParse(configStringValue, out minValue))
				{
					minValue = DateTime.MinValue;
				}
				if (minValue > t || minValue < utcNow)
				{
					EcpEventLogConstants.Tuple_ConfigurableValueOutOfRange.LogEvent(new object[]
					{
						"DisableHybridSyncCheckEndDate",
						utcNow.ToString("s"),
						t.ToString("s")
					});
					minValue = DateTime.MinValue;
				}
			}
			return minValue;
		});

		private static Lazy<int> hybridSyncTimeoutInSeconds = new Lazy<int>(() => AppConfigLoader.GetConfigIntValue("HybridSyncTimeoutInSeconds", 0, int.MaxValue, 90));
	}
}
