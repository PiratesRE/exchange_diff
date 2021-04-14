using System;
using System.Globalization;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class WacConfiguration
	{
		private WacConfiguration()
		{
			this.BlockWacViewingThroughUI = new BoolAppSettingsEntry("BlockWacViewingThroughUI", false, ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.AccessTokenExpirationDuration = new TimeSpanAppSettingsEntry("WacAccessTokenExpirationInMinutes", TimeSpanUnit.Minutes, new TimeSpan(8, 0, 0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.AccessTokenCacheTime = new TimeSpanAppSettingsEntry("WacAccessTokenCacheTimeInMinutes", TimeSpanUnit.Minutes, new TimeSpan(0, 30, 0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.UseHttpsForWacUrl = new BoolAppSettingsEntry("UseHttpsForWacUrl", true, ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.DiscoveryDataRefreshInterval = new TimeSpanAppSettingsEntry("WacDiscoveryDataRefreshIntervalInMinutes", TimeSpanUnit.Minutes, TimeSpan.FromHours(8.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.DiscoveryDataRetrievalErrorBaseRefreshInterval = new TimeSpanAppSettingsEntry("WacDiscoveryDataRetrievalErrorBaseRefreshIntervalInMinutes", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(5.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.MdbCacheInterval = new TimeSpanAppSettingsEntry("WacMdbCacheIntervalInMinutes", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(30.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.CobaltStoreCleanupInterval = new TimeSpanAppSettingsEntry("WacCobaltStoreCleanupIntervalInHours", TimeSpanUnit.Hours, TimeSpan.FromHours(24.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.CobaltStoreExpirationInterval = new TimeSpanAppSettingsEntry("WacCobaltStoreExpirationIntervalInHours", TimeSpanUnit.Hours, TimeSpan.FromHours(12.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.AutoSaveInterval = new TimeSpanAppSettingsEntry("WacAutoSaveIntervalInSeconds", TimeSpanUnit.Seconds, TimeSpan.FromSeconds(180.0), ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.DiagnosticsEnabled = new BoolAppSettingsEntry("WacDiagnosticsEnabled", false, ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.EditingEnabled = new BoolAppSettingsEntry("WacEditingEnabled", true, ExTraceGlobals.AttachmentHandlingTracer).Value;
			this.BlobStoreMemoryBudget = new IntAppSettingsEntry("WacCobaltBlobStoreMemoryBudget", 524288, ExTraceGlobals.AttachmentHandlingTracer).Value;
			if (this.BlockWacViewingThroughUI)
			{
				this.WacDiscoveryEndPoint = null;
				string text = "BlockWacViewingThroughUI is set to true";
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_WacConfigurationSetupSuccessful, string.Empty, new object[]
				{
					text
				});
				OwaServerTraceLogger.AppendToLog(new WacAttachmentLogEvent(text));
				return;
			}
			string text2 = new StringAppSettingsEntry("WacUrlHostName", null, ExTraceGlobals.AttachmentHandlingTracer).Value;
			bool flag = false;
			if (string.IsNullOrEmpty(text2) && !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.WacConfigurationFromOrgConfig.Enabled)
			{
				flag = true;
				text2 = WacConfiguration.ReadFromOrganizationConfig();
			}
			if (string.IsNullOrEmpty(text2) || !Uri.TryCreate(text2, UriKind.Absolute, out this.WacDiscoveryEndPoint))
			{
				this.BlockWacViewingThroughUI = true;
				string text3 = string.Format("The WacUrlHostName was invalid. Expected a valid Uri. Actual value was '{0}'. Value read from '{1}'", text2, flag ? "OrganizationConfig" : "web.config");
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_WacConfigurationSetupError, string.Empty, new object[]
				{
					text3
				});
				OwaServerTraceLogger.AppendToLog(new WacAttachmentLogEvent(text3));
				return;
			}
			string value = new StringAppSettingsEntry("WacDataCenterPrefix", null, ExTraceGlobals.AttachmentHandlingTracer).Value;
			if (!string.IsNullOrEmpty(value))
			{
				this.WacDiscoveryEndPoint = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?dcPrefix={1}", new object[]
				{
					this.WacDiscoveryEndPoint,
					value
				}));
			}
			string text4 = string.Format("Wac enabled and configured with {0} as the endpoint. Value was read from {1}.", this.WacDiscoveryEndPoint, flag ? "OrganizationConfig" : "web.config");
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_WacConfigurationSetupSuccessful, string.Empty, new object[]
			{
				text4
			});
			text4 += string.Format(" DiagnosticsEnabled={0}, EditingEnabled={1}", this.DiagnosticsEnabled, this.EditingEnabled);
			OwaServerTraceLogger.AppendToLog(new WacAttachmentLogEvent(text4));
		}

		public static WacConfiguration Instance
		{
			get
			{
				if (WacConfiguration.soleInstance == null)
				{
					WacConfiguration wacConfiguration = new WacConfiguration();
					WacConfiguration.soleInstance = wacConfiguration;
				}
				return WacConfiguration.soleInstance;
			}
		}

		public string WacUrlScheme
		{
			get
			{
				if (!this.UseHttpsForWacUrl)
				{
					return "http";
				}
				return "https";
			}
		}

		private static string ReadFromOrganizationConfig()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 321, "ReadFromOrganizationConfig", "f:\\15.00.1497\\sources\\dev\\clients\\src\\Owa2\\Server\\Core\\attachment\\WacConfiguration.cs");
			Organization orgContainer = topologyConfigurationSession.GetOrgContainer();
			return orgContainer.WACDiscoveryEndpoint;
		}

		public const int DefaultBlobStoreMemoryBudget = 524288;

		public readonly TimeSpan AccessTokenExpirationDuration;

		public readonly TimeSpan AccessTokenCacheTime;

		public readonly TimeSpan DiscoveryDataRefreshInterval;

		public readonly TimeSpan DiscoveryDataRetrievalErrorBaseRefreshInterval;

		public readonly TimeSpan MdbCacheInterval;

		public readonly TimeSpan AutoSaveInterval;

		public readonly TimeSpan CobaltStoreCleanupInterval;

		public readonly TimeSpan CobaltStoreExpirationInterval;

		public readonly bool BlockWacViewingThroughUI;

		public readonly bool UseHttpsForWacUrl;

		public readonly Uri WacDiscoveryEndPoint;

		public readonly bool DiagnosticsEnabled;

		public readonly bool EditingEnabled;

		public readonly int BlobStoreMemoryBudget;

		private static WacConfiguration soleInstance;
	}
}
