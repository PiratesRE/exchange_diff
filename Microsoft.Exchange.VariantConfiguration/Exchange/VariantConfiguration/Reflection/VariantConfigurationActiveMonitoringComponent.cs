using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationActiveMonitoringComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationActiveMonitoringComponent() : base("ActiveMonitoring")
		{
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "ProcessIsolationResetIISAppPoolResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "WatsonResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "DirectoryAccessor", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "GetExchangeDiagnosticsInfoResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "PushNotificationsDiscoveryMbx", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "EscalateResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "CafeOfflineRespondersUseClientAccessArray", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "PopImapDiscoveryCommon", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "TraceLogResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "AllowBasicAuthForOutsideInMonitoringMailboxes", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "ActiveSyncDiscovery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "ClearLsassCacheResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "ProcessIsolationRestartServiceResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "SubjectMaintenance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "LocalEndpointManager", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "F1TraceResponder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "RpcProbe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "PushNotificationsDiscoveryCafe", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "AutoDiscoverExternalUrlVerification", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ActiveMonitoring.settings.ini", "PinMonitoringMailboxesToDatabases", typeof(ICmdletSettings), false));
		}

		public VariantConfigurationSection ProcessIsolationResetIISAppPoolResponder
		{
			get
			{
				return base["ProcessIsolationResetIISAppPoolResponder"];
			}
		}

		public VariantConfigurationSection WatsonResponder
		{
			get
			{
				return base["WatsonResponder"];
			}
		}

		public VariantConfigurationSection DirectoryAccessor
		{
			get
			{
				return base["DirectoryAccessor"];
			}
		}

		public VariantConfigurationSection GetExchangeDiagnosticsInfoResponder
		{
			get
			{
				return base["GetExchangeDiagnosticsInfoResponder"];
			}
		}

		public VariantConfigurationSection PushNotificationsDiscoveryMbx
		{
			get
			{
				return base["PushNotificationsDiscoveryMbx"];
			}
		}

		public VariantConfigurationSection EscalateResponder
		{
			get
			{
				return base["EscalateResponder"];
			}
		}

		public VariantConfigurationSection CafeOfflineRespondersUseClientAccessArray
		{
			get
			{
				return base["CafeOfflineRespondersUseClientAccessArray"];
			}
		}

		public VariantConfigurationSection PopImapDiscoveryCommon
		{
			get
			{
				return base["PopImapDiscoveryCommon"];
			}
		}

		public VariantConfigurationSection TraceLogResponder
		{
			get
			{
				return base["TraceLogResponder"];
			}
		}

		public VariantConfigurationSection AllowBasicAuthForOutsideInMonitoringMailboxes
		{
			get
			{
				return base["AllowBasicAuthForOutsideInMonitoringMailboxes"];
			}
		}

		public VariantConfigurationSection ActiveSyncDiscovery
		{
			get
			{
				return base["ActiveSyncDiscovery"];
			}
		}

		public VariantConfigurationSection ClearLsassCacheResponder
		{
			get
			{
				return base["ClearLsassCacheResponder"];
			}
		}

		public VariantConfigurationSection ProcessIsolationRestartServiceResponder
		{
			get
			{
				return base["ProcessIsolationRestartServiceResponder"];
			}
		}

		public VariantConfigurationSection SubjectMaintenance
		{
			get
			{
				return base["SubjectMaintenance"];
			}
		}

		public VariantConfigurationSection LocalEndpointManager
		{
			get
			{
				return base["LocalEndpointManager"];
			}
		}

		public VariantConfigurationSection F1TraceResponder
		{
			get
			{
				return base["F1TraceResponder"];
			}
		}

		public VariantConfigurationSection RpcProbe
		{
			get
			{
				return base["RpcProbe"];
			}
		}

		public VariantConfigurationSection PushNotificationsDiscoveryCafe
		{
			get
			{
				return base["PushNotificationsDiscoveryCafe"];
			}
		}

		public VariantConfigurationSection AutoDiscoverExternalUrlVerification
		{
			get
			{
				return base["AutoDiscoverExternalUrlVerification"];
			}
		}

		public VariantConfigurationSection PinMonitoringMailboxesToDatabases
		{
			get
			{
				return base["PinMonitoringMailboxesToDatabases"];
			}
		}
	}
}
