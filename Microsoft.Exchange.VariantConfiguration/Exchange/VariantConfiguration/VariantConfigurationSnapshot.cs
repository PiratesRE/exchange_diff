using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Assistants;
using Microsoft.Exchange.AutoDiscover;
using Microsoft.Exchange.Calendar;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.HolidayCalendars;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.MessageDepot;
using Microsoft.Exchange.Search;
using Microsoft.Exchange.TextProcessing.Boomerang;
using Microsoft.Exchange.VariantConfiguration.Settings;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Search.Platform.Parallax;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class VariantConfigurationSnapshot
	{
		public VariantConfigurationSnapshot.ActiveMonitoringSettingsIni ActiveMonitoring
		{
			get
			{
				return new VariantConfigurationSnapshot.ActiveMonitoringSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.ActiveSyncSettingsIni ActiveSync
		{
			get
			{
				return new VariantConfigurationSnapshot.ActiveSyncSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.ADSettingsIni AD
		{
			get
			{
				return new VariantConfigurationSnapshot.ADSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.AutodiscoverSettingsIni Autodiscover
		{
			get
			{
				return new VariantConfigurationSnapshot.AutodiscoverSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.BoomerangSettingsIni Boomerang
		{
			get
			{
				return new VariantConfigurationSnapshot.BoomerangSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.CafeSettingsIni Cafe
		{
			get
			{
				return new VariantConfigurationSnapshot.CafeSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.CalendarLoggingSettingsIni CalendarLogging
		{
			get
			{
				return new VariantConfigurationSnapshot.CalendarLoggingSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.ClientAccessRulesCommonSettingsIni ClientAccessRulesCommon
		{
			get
			{
				return new VariantConfigurationSnapshot.ClientAccessRulesCommonSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.CmdletInfraSettingsIni CmdletInfra
		{
			get
			{
				return new VariantConfigurationSnapshot.CmdletInfraSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.CompliancePolicySettingsIni CompliancePolicy
		{
			get
			{
				return new VariantConfigurationSnapshot.CompliancePolicySettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.DataStorageSettingsIni DataStorage
		{
			get
			{
				return new VariantConfigurationSnapshot.DataStorageSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.DiagnosticsSettingsIni Diagnostics
		{
			get
			{
				return new VariantConfigurationSnapshot.DiagnosticsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.DiscoverySettingsIni Discovery
		{
			get
			{
				return new VariantConfigurationSnapshot.DiscoverySettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.E4ESettingsIni E4E
		{
			get
			{
				return new VariantConfigurationSnapshot.E4ESettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.EacSettingsIni Eac
		{
			get
			{
				return new VariantConfigurationSnapshot.EacSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.EwsSettingsIni Ews
		{
			get
			{
				return new VariantConfigurationSnapshot.EwsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.GlobalSettingsIni Global
		{
			get
			{
				return new VariantConfigurationSnapshot.GlobalSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.HighAvailabilitySettingsIni HighAvailability
		{
			get
			{
				return new VariantConfigurationSnapshot.HighAvailabilitySettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.HolidayCalendarsSettingsIni HolidayCalendars
		{
			get
			{
				return new VariantConfigurationSnapshot.HolidayCalendarsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.HxSettingsIni Hx
		{
			get
			{
				return new VariantConfigurationSnapshot.HxSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.ImapSettingsIni Imap
		{
			get
			{
				return new VariantConfigurationSnapshot.ImapSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.InferenceSettingsIni Inference
		{
			get
			{
				return new VariantConfigurationSnapshot.InferenceSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.IpaedSettingsIni Ipaed
		{
			get
			{
				return new VariantConfigurationSnapshot.IpaedSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MailboxAssistantsSettingsIni MailboxAssistants
		{
			get
			{
				return new VariantConfigurationSnapshot.MailboxAssistantsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MailboxPlansSettingsIni MailboxPlans
		{
			get
			{
				return new VariantConfigurationSnapshot.MailboxPlansSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MailboxTransportSettingsIni MailboxTransport
		{
			get
			{
				return new VariantConfigurationSnapshot.MailboxTransportSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MalwareAgentSettingsIni MalwareAgent
		{
			get
			{
				return new VariantConfigurationSnapshot.MalwareAgentSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MessageTrackingSettingsIni MessageTracking
		{
			get
			{
				return new VariantConfigurationSnapshot.MessageTrackingSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MexAgentsSettingsIni MexAgents
		{
			get
			{
				return new VariantConfigurationSnapshot.MexAgentsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.MrsSettingsIni Mrs
		{
			get
			{
				return new VariantConfigurationSnapshot.MrsSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.NotificationBrokerServiceSettingsIni NotificationBrokerService
		{
			get
			{
				return new VariantConfigurationSnapshot.NotificationBrokerServiceSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OABSettingsIni OAB
		{
			get
			{
				return new VariantConfigurationSnapshot.OABSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OfficeGraphSettingsIni OfficeGraph
		{
			get
			{
				return new VariantConfigurationSnapshot.OfficeGraphSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OwaClientSettingsIni OwaClient
		{
			get
			{
				return new VariantConfigurationSnapshot.OwaClientSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OwaClientServerSettingsIni OwaClientServer
		{
			get
			{
				return new VariantConfigurationSnapshot.OwaClientServerSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OwaServerSettingsIni OwaServer
		{
			get
			{
				return new VariantConfigurationSnapshot.OwaServerSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.OwaDeploymentSettingsIni OwaDeployment
		{
			get
			{
				return new VariantConfigurationSnapshot.OwaDeploymentSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.PopSettingsIni Pop
		{
			get
			{
				return new VariantConfigurationSnapshot.PopSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.RpcClientAccessSettingsIni RpcClientAccess
		{
			get
			{
				return new VariantConfigurationSnapshot.RpcClientAccessSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.SearchSettingsIni Search
		{
			get
			{
				return new VariantConfigurationSnapshot.SearchSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.SharedCacheSettingsIni SharedCache
		{
			get
			{
				return new VariantConfigurationSnapshot.SharedCacheSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.SharedMailboxSettingsIni SharedMailbox
		{
			get
			{
				return new VariantConfigurationSnapshot.SharedMailboxSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.TestSettingsIni Test
		{
			get
			{
				return new VariantConfigurationSnapshot.TestSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.Test2SettingsIni Test2
		{
			get
			{
				return new VariantConfigurationSnapshot.Test2SettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.TransportSettingsIni Transport
		{
			get
			{
				return new VariantConfigurationSnapshot.TransportSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.UCCSettingsIni UCC
		{
			get
			{
				return new VariantConfigurationSnapshot.UCCSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.UMSettingsIni UM
		{
			get
			{
				return new VariantConfigurationSnapshot.UMSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.VariantConfigSettingsIni VariantConfig
		{
			get
			{
				return new VariantConfigurationSnapshot.VariantConfigSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.WorkingSetSettingsIni WorkingSet
		{
			get
			{
				return new VariantConfigurationSnapshot.WorkingSetSettingsIni(this);
			}
		}

		public VariantConfigurationSnapshot.WorkloadManagementSettingsIni WorkloadManagement
		{
			get
			{
				return new VariantConfigurationSnapshot.WorkloadManagementSettingsIni(this);
			}
		}

		internal VariantConfigurationSnapshot(VariantObjectStore store, int rotationHash, string rampId, bool evaluateFlights, VariantConfigurationSnapshotProvider snapshotProvider)
		{
			this.store = store;
			this.rotationHash = rotationHash;
			this.rampId = rampId;
			this.snapshotProvider = snapshotProvider;
			this.evaluateFlights = evaluateFlights;
		}

		public KeyValuePair<string, string>[] Constraints
		{
			get
			{
				return this.store.DefaultContext.GetVariantFilters();
			}
		}

		public string[] Flights
		{
			get
			{
				return (from pair in this.Constraints
				where pair.Key.StartsWith("flt.", StringComparison.OrdinalIgnoreCase)
				select pair.Key.Substring("flt.".Length)).ToArray<string>();
			}
		}

		public T GetObject<T>(string config, string section) where T : class, ISettings
		{
			if (string.IsNullOrEmpty(config))
			{
				throw new ArgumentNullException("config");
			}
			if (string.IsNullOrEmpty(section))
			{
				throw new ArgumentNullException("section");
			}
			if (VariantConfiguration.TestOverride != null)
			{
				ISettings settings = VariantConfiguration.TestOverride(config, section);
				if (settings != null)
				{
					return (T)((object)settings);
				}
			}
			this.LoadDataSourceAndCreateNewSnapshot(config);
			T @object;
			try
			{
				@object = this.store.GetResolvedObjectProvider(config).GetObject<T>(section);
			}
			catch (DataSourceNotFoundException)
			{
				throw new KeyNotFoundException(config + " could not be found.");
			}
			return @object;
		}

		public T GetObject<T>(string config, object id1, params object[] ids) where T : class, ISettings
		{
			if (string.IsNullOrEmpty(config))
			{
				throw new ArgumentNullException("config");
			}
			if (id1 == null)
			{
				throw new ArgumentNullException("id1");
			}
			string text = id1.ToString();
			if (ids != null && ids.Length > 0)
			{
				text = text + "-" + string.Join("-", ids);
			}
			return this.GetObject<T>(config, text);
		}

		public IDictionary<string, T> GetObjectsOfType<T>(string config) where T : class, ISettings
		{
			if (string.IsNullOrEmpty(config))
			{
				throw new ArgumentNullException("config");
			}
			this.LoadDataSourceAndCreateNewSnapshot(config);
			IDictionary<string, T> objectsOfType;
			try
			{
				objectsOfType = this.store.GetResolvedObjectProvider(config).GetObjectsOfType<T>();
			}
			catch (DataSourceNotFoundException)
			{
				throw new KeyNotFoundException(config + " could not be found.");
			}
			return objectsOfType;
		}

		private void LoadDataSourceAndCreateNewSnapshot(string config)
		{
			if (this.store.DataSourceNames.Contains(config))
			{
				return;
			}
			lock (this.storeLock)
			{
				if (!this.store.DataSourceNames.Contains(config))
				{
					string[] array = new string[]
					{
						config
					};
					this.snapshotProvider.DataLoader.LoadIfNotLoaded(array);
					VariantObjectStore variantObjectStore = this.store;
					VariantObjectStore currentSnapshot = this.snapshotProvider.Container.GetCurrentSnapshot();
					currentSnapshot.DefaultContext.InitializeFrom(variantObjectStore.DefaultContext);
					if (this.evaluateFlights)
					{
						this.snapshotProvider.AddFlightsToStoreContext(currentSnapshot, array, this.rotationHash, this.rampId);
					}
					this.store = currentSnapshot;
				}
			}
		}

		private const string SectionIdSeparator = "-";

		private const string FlightPrefix = "flt.";

		private readonly VariantConfigurationSnapshotProvider snapshotProvider;

		private readonly object storeLock = new object();

		private readonly int rotationHash;

		private readonly string rampId;

		private readonly bool evaluateFlights;

		private VariantObjectStore store;

		public struct ActiveMonitoringSettingsIni
		{
			internal ActiveMonitoringSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("ActiveMonitoring.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ActiveMonitoring.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ActiveMonitoring.settings.ini", id1, ids);
			}

			public IFeature ProcessIsolationResetIISAppPoolResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "ProcessIsolationResetIISAppPoolResponder");
				}
			}

			public IFeature WatsonResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "WatsonResponder");
				}
			}

			public IFeature DirectoryAccessor
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "DirectoryAccessor");
				}
			}

			public IFeature GetExchangeDiagnosticsInfoResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "GetExchangeDiagnosticsInfoResponder");
				}
			}

			public IFeature PushNotificationsDiscoveryMbx
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "PushNotificationsDiscoveryMbx");
				}
			}

			public IFeature EscalateResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "EscalateResponder");
				}
			}

			public IFeature CafeOfflineRespondersUseClientAccessArray
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "CafeOfflineRespondersUseClientAccessArray");
				}
			}

			public IFeature PopImapDiscoveryCommon
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "PopImapDiscoveryCommon");
				}
			}

			public IFeature TraceLogResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "TraceLogResponder");
				}
			}

			public IFeature AllowBasicAuthForOutsideInMonitoringMailboxes
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "AllowBasicAuthForOutsideInMonitoringMailboxes");
				}
			}

			public IFeature ActiveSyncDiscovery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "ActiveSyncDiscovery");
				}
			}

			public IFeature ClearLsassCacheResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "ClearLsassCacheResponder");
				}
			}

			public IFeature ProcessIsolationRestartServiceResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "ProcessIsolationRestartServiceResponder");
				}
			}

			public IFeature SubjectMaintenance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "SubjectMaintenance");
				}
			}

			public IFeature LocalEndpointManager
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "LocalEndpointManager");
				}
			}

			public IFeature F1TraceResponder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "F1TraceResponder");
				}
			}

			public IFeature RpcProbe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "RpcProbe");
				}
			}

			public IFeature PushNotificationsDiscoveryCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "PushNotificationsDiscoveryCafe");
				}
			}

			public IFeature AutoDiscoverExternalUrlVerification
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveMonitoring.settings.ini", "AutoDiscoverExternalUrlVerification");
				}
			}

			public ICmdletSettings PinMonitoringMailboxesToDatabases
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("ActiveMonitoring.settings.ini", "PinMonitoringMailboxesToDatabases");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct ActiveSyncSettingsIni
		{
			internal ActiveSyncSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("ActiveSync.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ActiveSync.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ActiveSync.settings.ini", id1, ids);
			}

			public IFeature SyncStateOnDirectItems
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "SyncStateOnDirectItems");
				}
			}

			public IMdmSupportedPlatformsSettings MdmSupportedPlatforms
			{
				get
				{
					return this.snapshot.GetObject<IMdmSupportedPlatformsSettings>("ActiveSync.settings.ini", "MdmSupportedPlatforms");
				}
			}

			public IFeature GlobalCriminalCompliance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "GlobalCriminalCompliance");
				}
			}

			public IFeature ConsumerOrganizationUser
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "ConsumerOrganizationUser");
				}
			}

			public IFeature HDPhotos
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "HDPhotos");
				}
			}

			public IFeature MailboxLoggingVerboseMode
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "MailboxLoggingVerboseMode");
				}
			}

			public IFeature ActiveSyncClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "ActiveSyncClientAccessRulesEnabled");
				}
			}

			public IFeature ForceSingleNameSpaceUsage
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "ForceSingleNameSpaceUsage");
				}
			}

			public IMdmNotificationSettings MdmNotification
			{
				get
				{
					return this.snapshot.GetObject<IMdmNotificationSettings>("ActiveSync.settings.ini", "MdmNotification");
				}
			}

			public IFeature ActiveSyncDiagnosticsLogABQPeriodicEvent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "ActiveSyncDiagnosticsLogABQPeriodicEvent");
				}
			}

			public IFeature RedirectForOnBoarding
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "RedirectForOnBoarding");
				}
			}

			public IFeature CloudMdmEnrolled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "CloudMdmEnrolled");
				}
			}

			public IFeature UseOAuthMasterSidForSecurityContext
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "UseOAuthMasterSidForSecurityContext");
				}
			}

			public IFeature EnableV160
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "EnableV160");
				}
			}

			public IFeature EasPartialIcsSync
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "EasPartialIcsSync");
				}
			}

			public IFeature DisableCharsetDetectionInCopyMessageContents
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "DisableCharsetDetectionInCopyMessageContents");
				}
			}

			public IFeature GetGoidFromCalendarItemForMeetingResponse
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "GetGoidFromCalendarItemForMeetingResponse");
				}
			}

			public IFeature SyncStatusOnGlobalInfo
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ActiveSync.settings.ini", "SyncStatusOnGlobalInfo");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct ADSettingsIni
		{
			internal ADSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("AD.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("AD.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("AD.settings.ini", id1, ids);
			}

			public IDelegatedSetupRoleGroupSettings DelegatedSetupRoleGroupValue
			{
				get
				{
					return this.snapshot.GetObject<IDelegatedSetupRoleGroupSettings>("AD.settings.ini", "DelegatedSetupRoleGroupValue");
				}
			}

			public IFeature DisplayNameMustContainReadableCharacter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("AD.settings.ini", "DisplayNameMustContainReadableCharacter");
				}
			}

			public IFeature MailboxLocations
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("AD.settings.ini", "MailboxLocations");
				}
			}

			public IFeature EnableUseIsDescendantOfForRecipientViewRoot
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("AD.settings.ini", "EnableUseIsDescendantOfForRecipientViewRoot");
				}
			}

			public IFeature UseGlobalCatalogIsSetToFalse
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("AD.settings.ini", "UseGlobalCatalogIsSetToFalse");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct AutodiscoverSettingsIni
		{
			internal AutodiscoverSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Autodiscover.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Autodiscover.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Autodiscover.settings.ini", id1, ids);
			}

			public IFeature AnonymousAuthentication
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "AnonymousAuthentication");
				}
			}

			public IFeature EnableMobileSyncRedirectBypass
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "EnableMobileSyncRedirectBypass");
				}
			}

			public IFeature ParseBinarySecretHeader
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "ParseBinarySecretHeader");
				}
			}

			public IFeature SkipServiceTopologyDiscovery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "SkipServiceTopologyDiscovery");
				}
			}

			public IFeature StreamInsightUploader
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "StreamInsightUploader");
				}
			}

			public IFeature LoadNegoExSspNames
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "LoadNegoExSspNames");
				}
			}

			public IFeature NoADLookupForUser
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "NoADLookupForUser");
				}
			}

			public IFeature NoCrossForestDiscover
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "NoCrossForestDiscover");
				}
			}

			public IFeature EcpInternalExternalUrl
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "EcpInternalExternalUrl");
				}
			}

			public IFeature MapiHttpForOutlook14
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "MapiHttpForOutlook14");
				}
			}

			public IOWAUrl OWAUrl
			{
				get
				{
					return this.snapshot.GetObject<IOWAUrl>("Autodiscover.settings.ini", "OWAUrl");
				}
			}

			public IFeature AccountInCloud
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "AccountInCloud");
				}
			}

			public IFeature ConfigurePerformanceCounters
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "ConfigurePerformanceCounters");
				}
			}

			public IFeature RedirectOutlookClient
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "RedirectOutlookClient");
				}
			}

			public IFeature WsSecurityEndpoint
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "WsSecurityEndpoint");
				}
			}

			public IFeature UseMapiHttpADSetting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "UseMapiHttpADSetting");
				}
			}

			public IFeature NoAuthenticationTokenToNego
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "NoAuthenticationTokenToNego");
				}
			}

			public IFeature RestrictedSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "RestrictedSettings");
				}
			}

			public IFeature MapiHttp
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "MapiHttp");
				}
			}

			public IFeature LogonViaStandardTokens
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Autodiscover.settings.ini", "LogonViaStandardTokens");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct BoomerangSettingsIni
		{
			internal BoomerangSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Boomerang.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Boomerang.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Boomerang.settings.ini", id1, ids);
			}

			public IBoomerangSettings BoomerangSettings
			{
				get
				{
					return this.snapshot.GetObject<IBoomerangSettings>("Boomerang.settings.ini", "BoomerangSettings");
				}
			}

			public IFeature BoomerangMessageId
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Boomerang.settings.ini", "BoomerangMessageId");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct CafeSettingsIni
		{
			internal CafeSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Cafe.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Cafe.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Cafe.settings.ini", id1, ids);
			}

			public IFeature CheckServerOnlineForActiveServer
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CheckServerOnlineForActiveServer");
				}
			}

			public IFeature ExplicitDomain
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "ExplicitDomain");
				}
			}

			public IFeature UseExternalPopIMapSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "UseExternalPopIMapSettings");
				}
			}

			public IFeature NoServiceTopologyTryGetServerVersion
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "NoServiceTopologyTryGetServerVersion");
				}
			}

			public IFeature NoFormBasedAuthentication
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "NoFormBasedAuthentication");
				}
			}

			public IFeature RUMUseADCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "RUMUseADCache");
				}
			}

			public IFeature PartitionedRouting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "PartitionedRouting");
				}
			}

			public IFeature DownLevelServerPing
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "DownLevelServerPing");
				}
			}

			public IFeature UseResourceForest
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "UseResourceForest");
				}
			}

			public IFeature TrustClientXForwardedFor
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "TrustClientXForwardedFor");
				}
			}

			public IFeature MailboxServerSharedCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "MailboxServerSharedCache");
				}
			}

			public IFeature LoadBalancedPartnerRouting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "LoadBalancedPartnerRouting");
				}
			}

			public IFeature CompositeIdentity
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CompositeIdentity");
				}
			}

			public IFeature CafeV2
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CafeV2");
				}
			}

			public IFeature RetryOnError
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "RetryOnError");
				}
			}

			public IFeature PreferServersCacheForRandomBackEnd
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "PreferServersCacheForRandomBackEnd");
				}
			}

			public IFeature AnchorMailboxSharedCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "AnchorMailboxSharedCache");
				}
			}

			public IFeature CafeV1RUM
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CafeV1RUM");
				}
			}

			public IFeature DebugResponseHeaders
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "DebugResponseHeaders");
				}
			}

			public IFeature SyndicatedAdmin
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "SyndicatedAdmin");
				}
			}

			public IFeature EnableTls11
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "EnableTls11");
				}
			}

			public IFeature ConfigurePerformanceCounters
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "ConfigurePerformanceCounters");
				}
			}

			public IFeature EnableTls12
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "EnableTls12");
				}
			}

			public IFeature ServersCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "ServersCache");
				}
			}

			public IFeature NoCrossForestServerLocate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "NoCrossForestServerLocate");
				}
			}

			public IFeature SiteNameFromServerFqdnTranslation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "SiteNameFromServerFqdnTranslation");
				}
			}

			public IFeature CacheLocalSiteLiveE15Servers
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CacheLocalSiteLiveE15Servers");
				}
			}

			public IFeature EnforceConcurrencyGuards
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "EnforceConcurrencyGuards");
				}
			}

			public IFeature NoVDirLocationHint
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "NoVDirLocationHint");
				}
			}

			public IFeature NoCrossSiteRedirect
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "NoCrossSiteRedirect");
				}
			}

			public IFeature CheckServerLocatorServersForMaintenanceMode
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "CheckServerLocatorServersForMaintenanceMode");
				}
			}

			public IFeature UseExchClientVerInRPS
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "UseExchClientVerInRPS");
				}
			}

			public IFeature RUMLegacyRoutingEntry
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Cafe.settings.ini", "RUMLegacyRoutingEntry");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct CalendarLoggingSettingsIni
		{
			internal CalendarLoggingSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("CalendarLogging.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CalendarLogging.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CalendarLogging.settings.ini", id1, ids);
			}

			public IFeature FixMissingMeetingBody
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CalendarLogging.settings.ini", "FixMissingMeetingBody");
				}
			}

			public IFeature CalendarLoggingIncludeSeriesMeetingMessagesInCVS
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CalendarLogging.settings.ini", "CalendarLoggingIncludeSeriesMeetingMessagesInCVS");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct ClientAccessRulesCommonSettingsIni
		{
			internal ClientAccessRulesCommonSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("ClientAccessRulesCommon.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ClientAccessRulesCommon.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("ClientAccessRulesCommon.settings.ini", id1, ids);
			}

			public IFeature ImplicitAllowLocalClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("ClientAccessRulesCommon.settings.ini", "ImplicitAllowLocalClientAccessRulesEnabled");
				}
			}

			public ICacheExpiryTimeInMinutes ClientAccessRulesCacheExpiryTime
			{
				get
				{
					return this.snapshot.GetObject<ICacheExpiryTimeInMinutes>("ClientAccessRulesCommon.settings.ini", "ClientAccessRulesCacheExpiryTime");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct CmdletInfraSettingsIni
		{
			internal CmdletInfraSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("CmdletInfra.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CmdletInfra.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CmdletInfra.settings.ini", id1, ids);
			}

			public ICmdletSettings NewTransportRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-TransportRule");
				}
			}

			public IFeature ReportingWebService
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ReportingWebService");
				}
			}

			public IFeature PrePopulateCacheForMailboxBasedOnDatabase
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "PrePopulateCacheForMailboxBasedOnDatabase");
				}
			}

			public ICmdletSettings SetMailboxImportRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-MailboxImportRequest");
				}
			}

			public ICmdletSettings SetHoldComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-HoldComplianceRule");
				}
			}

			public ICmdletSettings GetHistoricalSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-HistoricalSearch");
				}
			}

			public ICmdletSettings GetSPOOneDriveForBusinessFileActivityReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOOneDriveForBusinessFileActivityReport");
				}
			}

			public ICmdletSettings RemoveDataClassification
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-DataClassification");
				}
			}

			public IFeature SetPasswordWithoutOldPassword
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SetPasswordWithoutOldPassword");
				}
			}

			public ICmdletSettings NewAuditConfigurationPolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-AuditConfigurationPolicy");
				}
			}

			public ICmdletSettings NewMailboxSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MailboxSearch");
				}
			}

			public IFeature SiteMailboxCheckSharePointUrlAgainstTrustedHosts
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SiteMailboxCheckSharePointUrlAgainstTrustedHosts");
				}
			}

			public ICmdletSettings SetDataClassification
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-DataClassification");
				}
			}

			public ICmdletSettings NewAuditConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-AuditConfigurationRule");
				}
			}

			public IFeature LimitNameMaxlength
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "LimitNameMaxlength");
				}
			}

			public IFeature CmdletMonitoring
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "CmdletMonitoring");
				}
			}

			public ICmdletSettings SetReportSchedule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-ReportSchedule");
				}
			}

			public ICmdletSettings GetHoldComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-HoldComplianceRule");
				}
			}

			public IFeature GlobalAddressListAttrbutes
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "GlobalAddressListAttrbutes");
				}
			}

			public ICmdletSettings GetComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ComplianceSearch");
				}
			}

			public ICmdletSettings AddMailbox
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Add-Mailbox");
				}
			}

			public ICmdletSettings InstallUnifiedCompliancePrerequisite
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Install-UnifiedCompliancePrerequisite");
				}
			}

			public IFeature ServiceAccountForest
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ServiceAccountForest");
				}
			}

			public ICmdletSettings GetSPOSkyDriveProStorageReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOSkyDriveProStorageReport");
				}
			}

			public IFeature InactiveMailbox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "InactiveMailbox");
				}
			}

			public ICmdletSettings NewDlpComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-DlpComplianceRule");
				}
			}

			public ICmdletSettings RemoveHoldComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-HoldComplianceRule");
				}
			}

			public IFeature Psws
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "Psws");
				}
			}

			public ICmdletSettings RemoveReportSchedule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-ReportSchedule");
				}
			}

			public ICmdletSettings GetClientAccessRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ClientAccessRule");
				}
			}

			public IFeature SetDefaultProhibitSendReceiveQuota
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SetDefaultProhibitSendReceiveQuota");
				}
			}

			public ICmdletSettings SetMailbox
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-Mailbox");
				}
			}

			public ICmdletSettings GetExternalActivityReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ExternalActivityReport");
				}
			}

			public ICmdletSettings GetDlpCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-DlpCompliancePolicy");
				}
			}

			public IFeature ReportToOriginator
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ReportToOriginator");
				}
			}

			public ICmdletSettings SetMailUser
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-MailUser");
				}
			}

			public ICmdletSettings NewMailboxExportRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MailboxExportRequest");
				}
			}

			public ICmdletSettings TestClientAccessRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Test-ClientAccessRule");
				}
			}

			public ICmdletSettings GetExternalActivitySummaryReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ExternalActivitySummaryReport");
				}
			}

			public ICmdletSettings NewClientAccessRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-ClientAccessRule");
				}
			}

			public ICmdletSettings GetExternalActivityByUserReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ExternalActivityByUserReport");
				}
			}

			public ICmdletSettings GetFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-FolderMoveRequest");
				}
			}

			public ICmdletSettings StopHistoricalSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Stop-HistoricalSearch");
				}
			}

			public ICmdletSettings NewDlpCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-DlpCompliancePolicy");
				}
			}

			public ICmdletSettings SetDeviceConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-DeviceConfigurationRule");
				}
			}

			public IFeature WinRMExchangeDataUseTypeNamedPipe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "WinRMExchangeDataUseTypeNamedPipe");
				}
			}

			public ICmdletSettings GetReportScheduleHistory
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ReportScheduleHistory");
				}
			}

			public ICmdletSettings RemoveFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-FolderMoveRequest");
				}
			}

			public IFeature RecoverMailBox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "RecoverMailBox");
				}
			}

			public IFeature SiteMailboxProvisioningInExecutingUserOUEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SiteMailboxProvisioningInExecutingUserOUEnabled");
				}
			}

			public ICmdletSettings GetListedIPWrapper
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ListedIPWrapper");
				}
			}

			public ICmdletSettings SetClientAccessRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-ClientAccessRule");
				}
			}

			public ICmdletSettings GetExternalActivityByDomainReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ExternalActivityByDomainReport");
				}
			}

			public ICmdletSettings NewDeviceConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-DeviceConfigurationRule");
				}
			}

			public ICmdletSettings GetCsClientDeviceReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-CsClientDeviceReport");
				}
			}

			public ICmdletSettings GetDlpComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-DlpComplianceRule");
				}
			}

			public ICmdletSettings GetDeviceConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-DeviceConfigurationRule");
				}
			}

			public ICmdletSettings RemoveHoldCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-HoldCompliancePolicy");
				}
			}

			public IFeature ShowFismaBanner
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ShowFismaBanner");
				}
			}

			public IFeature UseDatabaseQuotaDefaults
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "UseDatabaseQuotaDefaults");
				}
			}

			public ICmdletSettings GetAuditConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-AuditConfigurationRule");
				}
			}

			public IFeature WriteEventLogInEnglish
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "WriteEventLogInEnglish");
				}
			}

			public ICmdletSettings SetDlpCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-DlpCompliancePolicy");
				}
			}

			public IFeature SupportOptimizedFilterOnlyInDDG
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SupportOptimizedFilterOnlyInDDG");
				}
			}

			public ICmdletSettings RemoveComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-ComplianceSearch");
				}
			}

			public IFeature DepthTwoTypeEntry
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "DepthTwoTypeEntry");
				}
			}

			public ICmdletSettings SetAuditConfig
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-AuditConfig");
				}
			}

			public ICmdletSettings NewDataClassification
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-DataClassification");
				}
			}

			public ICmdletSettings NewMigrationEndpoint
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MigrationEndpoint");
				}
			}

			public ICmdletSettings SetMailboxExportRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-MailboxExportRequest");
				}
			}

			public IFeature ValidateExternalEmailAddressInAcceptedDomain
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ValidateExternalEmailAddressInAcceptedDomain");
				}
			}

			public ICmdletSettings EnableEOPMailUser
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Enable-EOPMailUser");
				}
			}

			public ICmdletSettings GetOMEConfiguration
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-OMEConfiguration");
				}
			}

			public ICmdletSettings NewFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-FolderMoveRequest");
				}
			}

			public IFeature EmailAddressPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "EmailAddressPolicy");
				}
			}

			public IFeature SkipPiiRedactionForForestWideObject
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SkipPiiRedactionForForestWideObject");
				}
			}

			public ICmdletSettings GetPartnerClientExpiringSubscriptionReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-PartnerClientExpiringSubscriptionReport");
				}
			}

			public IFeature PiiRedaction
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "PiiRedaction");
				}
			}

			public IFeature ValidateFilteringOnlyUser
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ValidateFilteringOnlyUser");
				}
			}

			public IFeature SoftDeleteObject
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SoftDeleteObject");
				}
			}

			public ICmdletSettings SetMailboxSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-MailboxSearch");
				}
			}

			public ICmdletSettings GetSPOOneDriveForBusinessUserStatisticsReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOOneDriveForBusinessUserStatisticsReport");
				}
			}

			public ICmdletSettings SetFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-FolderMoveRequest");
				}
			}

			public ICmdletSettings AddDelistIP
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Add-DelistIP");
				}
			}

			public IFeature GenerateNewExternalDirectoryObjectId
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "GenerateNewExternalDirectoryObjectId");
				}
			}

			public ICmdletSettings NewComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-ComplianceSearch");
				}
			}

			public IFeature IncludeFBOnlyForCalendarContributor
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "IncludeFBOnlyForCalendarContributor");
				}
			}

			public IFeature ValidateEnableRoomMailboxAccount
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ValidateEnableRoomMailboxAccount");
				}
			}

			public ICmdletSettings SetDlpComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-DlpComplianceRule");
				}
			}

			public ICmdletSettings RemoveDlpComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-DlpComplianceRule");
				}
			}

			public IFeature PswsCmdletProxy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "PswsCmdletProxy");
				}
			}

			public ICmdletSettings SetHoldCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-HoldCompliancePolicy");
				}
			}

			public IFeature LegacyRegCodeSupport
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "LegacyRegCodeSupport");
				}
			}

			public ICmdletSettings SetOMEConfiguration
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-OMEConfiguration");
				}
			}

			public ICmdletSettings GetSPOActiveUserReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOActiveUserReport");
				}
			}

			public ICmdletSettings RemoveAuditConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-AuditConfigurationRule");
				}
			}

			public ICmdletSettings GetSPOSkyDriveProDeployedReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOSkyDriveProDeployedReport");
				}
			}

			public ICmdletSettings SetTransportRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-TransportRule");
				}
			}

			public ICmdletSettings NewFingerprint
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-Fingerprint");
				}
			}

			public ICmdletSettings GetReputationOverride
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ReputationOverride");
				}
			}

			public ICmdletSettings NewReportSchedule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-ReportSchedule");
				}
			}

			public ICmdletSettings NewMailbox
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-Mailbox");
				}
			}

			public IFeature InstallModernGroupsAddressList
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "InstallModernGroupsAddressList");
				}
			}

			public IFeature GenericExchangeSnapin
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "GenericExchangeSnapin");
				}
			}

			public ICmdletSettings SetMigrationBatch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-MigrationBatch");
				}
			}

			public ICmdletSettings RemoveAuditConfigurationPolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-AuditConfigurationPolicy");
				}
			}

			public ICmdletSettings SetAuditConfigurationRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-AuditConfigurationRule");
				}
			}

			public ICmdletSettings RemoveClientAccessRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-ClientAccessRule");
				}
			}

			public IFeature OverWriteElcMailboxFlags
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "OverWriteElcMailboxFlags");
				}
			}

			public IFeature MaxAddressBookPolicies
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "MaxAddressBookPolicies");
				}
			}

			public ICmdletSettings StartComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Start-ComplianceSearch");
				}
			}

			public ICmdletSettings TestMigrationServerAvailability
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Test-MigrationServerAvailability");
				}
			}

			public IFeature WinRMExchangeDataUseAuthenticationType
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "WinRMExchangeDataUseAuthenticationType");
				}
			}

			public IFeature RpsClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "RpsClientAccessRulesEnabled");
				}
			}

			public ICmdletSettings StopComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Stop-ComplianceSearch");
				}
			}

			public ICmdletSettings ResumeFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Resume-FolderMoveRequest");
				}
			}

			public ICmdletSettings RemoveDlpCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-DlpCompliancePolicy");
				}
			}

			public ICmdletSettings RemoveMailbox
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-Mailbox");
				}
			}

			public ICmdletSettings GetSPOTeamSiteDeployedReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOTeamSiteDeployedReport");
				}
			}

			public ICmdletSettings NewHoldComplianceRule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-HoldComplianceRule");
				}
			}

			public IFeature PswsClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "PswsClientAccessRulesEnabled");
				}
			}

			public ICmdletSettings RemoveReputationOverride
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Remove-ReputationOverride");
				}
			}

			public ICmdletSettings GetAuditConfigurationPolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-AuditConfigurationPolicy");
				}
			}

			public ICmdletSettings GetDnsBlocklistInfo
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-DnsBlocklistInfo");
				}
			}

			public ICmdletSettings GetFolderMoveRequestStatistics
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-FolderMoveRequestStatistics");
				}
			}

			public ICmdletSettings StartHistoricalSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Start-HistoricalSearch");
				}
			}

			public IFeature CheckForDedicatedTenantAdminRoleNamePrefix
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "CheckForDedicatedTenantAdminRoleNamePrefix");
				}
			}

			public ICmdletSettings SuspendFolderMoveRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Suspend-FolderMoveRequest");
				}
			}

			public ICmdletSettings NewMailboxImportRequest
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MailboxImportRequest");
				}
			}

			public ICmdletSettings NewMigrationBatch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MigrationBatch");
				}
			}

			public ICmdletSettings SetComplianceSearch
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Set-ComplianceSearch");
				}
			}

			public ICmdletSettings GetSPOTeamSiteStorageReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOTeamSiteStorageReport");
				}
			}

			public ICmdletSettings GetHoldCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-HoldCompliancePolicy");
				}
			}

			public ICmdletSettings GetDlpSensitiveInformationType
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-DlpSensitiveInformationType");
				}
			}

			public ICmdletSettings GetReportScheduleList
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ReportScheduleList");
				}
			}

			public ICmdletSettings GetMailbox
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-Mailbox");
				}
			}

			public ICmdletSettings GetSPOTenantStorageMetricReport
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-SPOTenantStorageMetricReport");
				}
			}

			public ICmdletSettings NewMailUser
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-MailUser");
				}
			}

			public ICmdletSettings GetReportSchedule
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-ReportSchedule");
				}
			}

			public IFeature SetActiveArchiveStatus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "SetActiveArchiveStatus");
				}
			}

			public ICmdletSettings GetAuditConfig
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "Get-AuditConfig");
				}
			}

			public IFeature WsSecuritySymmetricAndX509Cert
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "WsSecuritySymmetricAndX509Cert");
				}
			}

			public IFeature ProxyDllUpdate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CmdletInfra.settings.ini", "ProxyDllUpdate");
				}
			}

			public ICmdletSettings NewHoldCompliancePolicy
			{
				get
				{
					return this.snapshot.GetObject<ICmdletSettings>("CmdletInfra.settings.ini", "New-HoldCompliancePolicy");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct CompliancePolicySettingsIni
		{
			internal CompliancePolicySettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("CompliancePolicy.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CompliancePolicy.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("CompliancePolicy.settings.ini", id1, ids);
			}

			public IFeature ProcessForestWideOrgEtrs
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CompliancePolicy.settings.ini", "ProcessForestWideOrgEtrs");
				}
			}

			public IFeature ShowSupervisionPredicate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CompliancePolicy.settings.ini", "ShowSupervisionPredicate");
				}
			}

			public IFeature ValidateTenantOutboundConnector
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CompliancePolicy.settings.ini", "ValidateTenantOutboundConnector");
				}
			}

			public IFeature RuleConfigurationAdChangeNotifications
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CompliancePolicy.settings.ini", "RuleConfigurationAdChangeNotifications");
				}
			}

			public IFeature QuarantineAction
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("CompliancePolicy.settings.ini", "QuarantineAction");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct DataStorageSettingsIni
		{
			internal DataStorageSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("DataStorage.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("DataStorage.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("DataStorage.settings.ini", id1, ids);
			}

			public IFeature CheckForRemoteConnections
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CheckForRemoteConnections");
				}
			}

			public IFeature PeopleCentricConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "PeopleCentricConversation");
				}
			}

			public IFeature UseOfflineRms
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "UseOfflineRms");
				}
			}

			public IFeature CalendarUpgrade
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CalendarUpgrade");
				}
			}

			public IFeature IgnoreInessentialMetaDataLoadErrors
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "IgnoreInessentialMetaDataLoadErrors");
				}
			}

			public IFeature ModernMailInfra
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "ModernMailInfra");
				}
			}

			public IFeature CalendarView
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CalendarView");
				}
			}

			public IFeature GroupsForOlkDesktop
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "GroupsForOlkDesktop");
				}
			}

			public IFeature FindOrgMailboxInMultiTenantEnvironment
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "FindOrgMailboxInMultiTenantEnvironment");
				}
			}

			public IFeature DeleteGroupConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "DeleteGroupConversation");
				}
			}

			public IFeature ModernConversationPrep
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "ModernConversationPrep");
				}
			}

			public IFeature CheckLicense
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CheckLicense");
				}
			}

			public IFeature LoadHostedMailboxLimits
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "LoadHostedMailboxLimits");
				}
			}

			public IFeature RepresentRemoteMailbox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "RepresentRemoteMailbox");
				}
			}

			public ICalendarUpgradeSettings CalendarUpgradeSettings
			{
				get
				{
					return this.snapshot.GetObject<ICalendarUpgradeSettings>("DataStorage.settings.ini", "CalendarUpgradeSettings");
				}
			}

			public IFeature CrossPremiseDelegate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CrossPremiseDelegate");
				}
			}

			public ICalendarIcalConversionSettings CalendarIcalConversionSettings
			{
				get
				{
					return this.snapshot.GetObject<ICalendarIcalConversionSettings>("DataStorage.settings.ini", "CalendarIcalConversionSettings");
				}
			}

			public IFeature CalendarViewPropertyRule
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CalendarViewPropertyRule");
				}
			}

			public IFeature CheckR3Coexistence
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CheckR3Coexistence");
				}
			}

			public IFeature XOWAConsumerSharing
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "XOWAConsumerSharing");
				}
			}

			public IFeature UserConfigurationAggregation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "UserConfigurationAggregation");
				}
			}

			public IFeature StorageAttachmentImageAnalysis
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "StorageAttachmentImageAnalysis");
				}
			}

			public IFeature LogIpEndpoints
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "LogIpEndpoints");
				}
			}

			public IFeature CheckExternalAccess
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "CheckExternalAccess");
				}
			}

			public IFeature ThreadedConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("DataStorage.settings.ini", "ThreadedConversation");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct DiagnosticsSettingsIni
		{
			internal DiagnosticsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Diagnostics.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Diagnostics.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Diagnostics.settings.ini", id1, ids);
			}

			public IFeature TraceToHeadersLogger
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Diagnostics.settings.ini", "TraceToHeadersLogger");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct DiscoverySettingsIni
		{
			internal DiscoverySettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Discovery.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Discovery.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Discovery.settings.ini", id1, ids);
			}

			public ISettingsValue DiscoveryServerLookupConcurrency
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryServerLookupConcurrency");
				}
			}

			public ISettingsValue DiscoveryMaxAllowedExecutorItems
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMaxAllowedExecutorItems");
				}
			}

			public ISettingsValue DiscoveryKeywordsBatchSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryKeywordsBatchSize");
				}
			}

			public ISettingsValue DiscoveryExecutesInParallel
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryExecutesInParallel");
				}
			}

			public IFeature UrlRebind
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Discovery.settings.ini", "UrlRebind");
				}
			}

			public ISettingsValue DiscoveryDisplaySearchPageSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryDisplaySearchPageSize");
				}
			}

			public ISettingsValue DiscoveryLocalSearchConcurrency
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryLocalSearchConcurrency");
				}
			}

			public ISettingsValue SearchTimeout
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "SearchTimeout");
				}
			}

			public ISettingsValue ServiceTopologyTimeout
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "ServiceTopologyTimeout");
				}
			}

			public ISettingsValue DiscoveryDisplaySearchBatchSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryDisplaySearchBatchSize");
				}
			}

			public ISettingsValue DiscoveryDefaultPageSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryDefaultPageSize");
				}
			}

			public ISettingsValue DiscoveryServerLookupBatch
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryServerLookupBatch");
				}
			}

			public ISettingsValue DiscoveryMaxAllowedResultsPageSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMaxAllowedResultsPageSize");
				}
			}

			public IFeature SearchScale
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Discovery.settings.ini", "SearchScale");
				}
			}

			public ISettingsValue MailboxServerLocatorTimeout
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "MailboxServerLocatorTimeout");
				}
			}

			public ISettingsValue DiscoveryADPageSize
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryADPageSize");
				}
			}

			public ISettingsValue DiscoveryMailboxMaxProhibitSendReceiveQuota
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMailboxMaxProhibitSendReceiveQuota");
				}
			}

			public ISettingsValue DiscoveryFanoutConcurrency
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryFanoutConcurrency");
				}
			}

			public ISettingsValue DiscoveryExcludedFolders
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryExcludedFolders");
				}
			}

			public ISettingsValue DiscoveryUseFastSearch
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryUseFastSearch");
				}
			}

			public ISettingsValue DiscoveryFanoutBatch
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryFanoutBatch");
				}
			}

			public ISettingsValue DiscoveryLocalSearchIsParallel
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryLocalSearchIsParallel");
				}
			}

			public ISettingsValue DiscoveryAggregateLogs
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryAggregateLogs");
				}
			}

			public ISettingsValue DiscoveryMailboxMaxProhibitSendQuota
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMailboxMaxProhibitSendQuota");
				}
			}

			public ISettingsValue DiscoveryMaxAllowedMailboxQueriesPerRequest
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMaxAllowedMailboxQueriesPerRequest");
				}
			}

			public ISettingsValue DiscoveryMaxMailboxes
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryMaxMailboxes");
				}
			}

			public ISettingsValue DiscoveryADLookupConcurrency
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryADLookupConcurrency");
				}
			}

			public ISettingsValue DiscoveryExcludedFoldersEnabled
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("Discovery.settings.ini", "DiscoveryExcludedFoldersEnabled");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct E4ESettingsIni
		{
			internal E4ESettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("E4E.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("E4E.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("E4E.settings.ini", id1, ids);
			}

			public IFeature OTP
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("E4E.settings.ini", "OTP");
				}
			}

			public IVersion Version
			{
				get
				{
					return this.snapshot.GetObject<IVersion>("E4E.settings.ini", "Version");
				}
			}

			public IFeature LogoffViaOwa
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("E4E.settings.ini", "LogoffViaOwa");
				}
			}

			public IFeature MsodsGraphQuery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("E4E.settings.ini", "MsodsGraphQuery");
				}
			}

			public IFeature E4E
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("E4E.settings.ini", "E4E");
				}
			}

			public IFeature TouchLayout
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("E4E.settings.ini", "TouchLayout");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct EacSettingsIni
		{
			internal EacSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Eac.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Eac.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Eac.settings.ini", id1, ids);
			}

			public IFeature ManageMailboxAuditing
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "ManageMailboxAuditing");
				}
			}

			public IFeature UnifiedPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UnifiedPolicy");
				}
			}

			public IFeature DiscoverySearchStats
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "DiscoverySearchStats");
				}
			}

			public IFeature AllowRemoteOnboardingMovesOnly
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "AllowRemoteOnboardingMovesOnly");
				}
			}

			public IFeature DlpFingerprint
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "DlpFingerprint");
				}
			}

			public IFeature GeminiShell
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "GeminiShell");
				}
			}

			public IFeature DevicePolicyMgmtUI
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "DevicePolicyMgmtUI");
				}
			}

			public IFeature UnifiedAuditPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UnifiedAuditPolicy");
				}
			}

			public IFeature EACClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "EACClientAccessRulesEnabled");
				}
			}

			public IFeature RemoteDomain
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "RemoteDomain");
				}
			}

			public IFeature CmdletLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "CmdletLogging");
				}
			}

			public IFeature UnifiedComplianceCenter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UnifiedComplianceCenter");
				}
			}

			public IFeature Office365DIcon
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "Office365DIcon");
				}
			}

			public IFeature DiscoveryPFSearch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "DiscoveryPFSearch");
				}
			}

			public IFeature ModernGroups
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "ModernGroups");
				}
			}

			public IFeature OrgIdADSeverSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "OrgIdADSeverSettings");
				}
			}

			public IFeature UCCPermissions
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UCCPermissions");
				}
			}

			public IFeature AllowMailboxArchiveOnlyMigration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "AllowMailboxArchiveOnlyMigration");
				}
			}

			public IFeature DiscoveryDocIdHint
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "DiscoveryDocIdHint");
				}
			}

			public IUrl AdminHomePage
			{
				get
				{
					return this.snapshot.GetObject<IUrl>("Eac.settings.ini", "AdminHomePage");
				}
			}

			public IFeature CrossPremiseMigration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "CrossPremiseMigration");
				}
			}

			public IFeature UCCAuditReports
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UCCAuditReports");
				}
			}

			public IFeature UnlistedServices
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "UnlistedServices");
				}
			}

			public IFeature BulkPermissionAddRemove
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Eac.settings.ini", "BulkPermissionAddRemove");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct EwsSettingsIni
		{
			internal EwsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Ews.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Ews.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Ews.settings.ini", id1, ids);
			}

			public IFeature AutoSubscribeNewGroupMembers
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "AutoSubscribeNewGroupMembers");
				}
			}

			public IFeature OnlineArchive
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "OnlineArchive");
				}
			}

			public IFeature UserPasswordExpirationDate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "UserPasswordExpirationDate");
				}
			}

			public IFeature InstantSearchFoldersForPublicFolders
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "InstantSearchFoldersForPublicFolders");
				}
			}

			public IFeature LinkedAccountTokenMunging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "LinkedAccountTokenMunging");
				}
			}

			public IFeature EwsServiceCredentials
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "EwsServiceCredentials");
				}
			}

			public IFeature ExternalUser
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "ExternalUser");
				}
			}

			public IFeature UseInternalEwsUrlForExtensionEwsProxyInOwa
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "UseInternalEwsUrlForExtensionEwsProxyInOwa");
				}
			}

			public IFeature EwsClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "EwsClientAccessRulesEnabled");
				}
			}

			public IFeature LongRunningScenarioThrottling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "LongRunningScenarioThrottling");
				}
			}

			public IFeature HttpProxyToCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "HttpProxyToCafe");
				}
			}

			public IFeature OData
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "OData");
				}
			}

			public IFeature EwsHttpHandler
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "EwsHttpHandler");
				}
			}

			public IFeature WsPerformanceCounters
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "WsPerformanceCounters");
				}
			}

			public IFeature CreateUnifiedMailbox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ews.settings.ini", "CreateUnifiedMailbox");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct GlobalSettingsIni
		{
			internal GlobalSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Global.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Global.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Global.settings.ini", id1, ids);
			}

			public IFeature GlobalCriminalCompliance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Global.settings.ini", "GlobalCriminalCompliance");
				}
			}

			public IFeature WindowsLiveID
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Global.settings.ini", "WindowsLiveID");
				}
			}

			public IFeature DistributedKeyManagement
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Global.settings.ini", "DistributedKeyManagement");
				}
			}

			public IFeature MultiTenancy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Global.settings.ini", "MultiTenancy");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct HighAvailabilitySettingsIni
		{
			internal HighAvailabilitySettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("HighAvailability.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("HighAvailability.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("HighAvailability.settings.ini", id1, ids);
			}

			public IActiveManagerSettings ActiveManager
			{
				get
				{
					return this.snapshot.GetObject<IActiveManagerSettings>("HighAvailability.settings.ini", "ActiveManager");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct HolidayCalendarsSettingsIni
		{
			internal HolidayCalendarsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("HolidayCalendars.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("HolidayCalendars.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("HolidayCalendars.settings.ini", id1, ids);
			}

			public IHostSettings HostConfiguration
			{
				get
				{
					return this.snapshot.GetObject<IHostSettings>("HolidayCalendars.settings.ini", "HostConfiguration");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct HxSettingsIni
		{
			internal HxSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Hx.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Hx.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Hx.settings.ini", id1, ids);
			}

			public IFeature SmartSyncWebSockets
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Hx.settings.ini", "SmartSyncWebSockets");
				}
			}

			public IFeature EnforceDevicePolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Hx.settings.ini", "EnforceDevicePolicy");
				}
			}

			public IFeature Irm
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Hx.settings.ini", "Irm");
				}
			}

			public IFeature ServiceAvailable
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Hx.settings.ini", "ServiceAvailable");
				}
			}

			public IFeature ClientSettingsPane
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Hx.settings.ini", "ClientSettingsPane");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct ImapSettingsIni
		{
			internal ImapSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Imap.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Imap.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Imap.settings.ini", id1, ids);
			}

			public IFeature RfcIDImap
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "RfcIDImap");
				}
			}

			public IFeature IgnoreNonProvisionedServers
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "IgnoreNonProvisionedServers");
				}
			}

			public IFeature UseSamAccountNameAsUsername
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "UseSamAccountNameAsUsername");
				}
			}

			public IFeature SkipAuthOnCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "SkipAuthOnCafe");
				}
			}

			public IFeature AllowPlainTextConversionWithoutUsingSkeleton
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "AllowPlainTextConversionWithoutUsingSkeleton");
				}
			}

			public IFeature RfcIDImapCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "RfcIDImapCafe");
				}
			}

			public IFeature GlobalCriminalCompliance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "GlobalCriminalCompliance");
				}
			}

			public IFeature CheckOnlyAuthenticationStatus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "CheckOnlyAuthenticationStatus");
				}
			}

			public IFeature RfcMoveImap
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "RfcMoveImap");
				}
			}

			public IFeature RefreshSearchFolderItems
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "RefreshSearchFolderItems");
				}
			}

			public IFeature ReloadMailboxBeforeGettingSubscriptionList
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "ReloadMailboxBeforeGettingSubscriptionList");
				}
			}

			public IFeature EnforceLogsRetentionPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "EnforceLogsRetentionPolicy");
				}
			}

			public IFeature AppendServerNameInBanner
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "AppendServerNameInBanner");
				}
			}

			public IFeature UsePrimarySmtpAddress
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "UsePrimarySmtpAddress");
				}
			}

			public IFeature ImapClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "ImapClientAccessRulesEnabled");
				}
			}

			public IFeature DontReturnLastMessageForUInt32MaxValue
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "DontReturnLastMessageForUInt32MaxValue");
				}
			}

			public IFeature LrsLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "LrsLogging");
				}
			}

			public IFeature AllowKerberosAuth
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "AllowKerberosAuth");
				}
			}

			public IFeature RfcMoveImapCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Imap.settings.ini", "RfcMoveImapCafe");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct InferenceSettingsIni
		{
			internal InferenceSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Inference.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Inference.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Inference.settings.ini", id1, ids);
			}

			public IFeature ActivityLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "ActivityLogging");
				}
			}

			public IInferenceTrainingConfigurationSettings InferenceTrainingConfigurationSettings
			{
				get
				{
					return this.snapshot.GetObject<IInferenceTrainingConfigurationSettings>("Inference.settings.ini", "InferenceTrainingConfigurationSettings");
				}
			}

			public IFeature InferenceGroupingModel
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceGroupingModel");
				}
			}

			public IFeature InferenceLatentLabelModel
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceLatentLabelModel");
				}
			}

			public IFeature InferenceClutterInvitation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceClutterInvitation");
				}
			}

			public IFeature InferenceEventBasedAssistant
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceEventBasedAssistant");
				}
			}

			public IFeature InferenceAutoEnableClutter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceAutoEnableClutter");
				}
			}

			public IClutterModelConfigurationSettings InferenceClutterModelConfigurationSettings
			{
				get
				{
					return this.snapshot.GetObject<IClutterModelConfigurationSettings>("Inference.settings.ini", "InferenceClutterModelConfigurationSettings");
				}
			}

			public IClutterDataSelectionSettings InferenceClutterDataSelectionSettings
			{
				get
				{
					return this.snapshot.GetObject<IClutterDataSelectionSettings>("Inference.settings.ini", "InferenceClutterDataSelectionSettings");
				}
			}

			public IFeature InferenceClutterAutoEnablementNotice
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceClutterAutoEnablementNotice");
				}
			}

			public IFeature InferenceModelComparison
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceModelComparison");
				}
			}

			public IFeature InferenceFolderBasedClutter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceFolderBasedClutter");
				}
			}

			public IFeature InferenceStampTracking
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Inference.settings.ini", "InferenceStampTracking");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct IpaedSettingsIni
		{
			internal IpaedSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Ipaed.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Ipaed.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Ipaed.settings.ini", id1, ids);
			}

			public IFeature ProcessedByUnjournal
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "ProcessedByUnjournal");
				}
			}

			public IFeature ProcessForestWideOrgJournal
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "ProcessForestWideOrgJournal");
				}
			}

			public IFeature MoveDeletionsToPurges
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "MoveDeletionsToPurges");
				}
			}

			public IFeature InternalJournaling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "InternalJournaling");
				}
			}

			public IFeature IncreaseQuotaForOnHoldMailboxes
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "IncreaseQuotaForOnHoldMailboxes");
				}
			}

			public IFeature AdminAuditLocalQueue
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "AdminAuditLocalQueue");
				}
			}

			public IFeature AdminAuditCmdletBlockList
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "AdminAuditCmdletBlockList");
				}
			}

			public IFeature AdminAuditEventLogThrottling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "AdminAuditEventLogThrottling");
				}
			}

			public IFeature AuditConfigFromUCCPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "AuditConfigFromUCCPolicy");
				}
			}

			public IFeature PartitionedMailboxAuditLogs
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "PartitionedMailboxAuditLogs");
				}
			}

			public IFeature MailboxAuditLocalQueue
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "MailboxAuditLocalQueue");
				}
			}

			public IFeature RemoveMailboxFromJournalRecipients
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "RemoveMailboxFromJournalRecipients");
				}
			}

			public IFeature MoveClearNrn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "MoveClearNrn");
				}
			}

			public IFeature FolderBindExtendedThrottling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "FolderBindExtendedThrottling");
				}
			}

			public IFeature PartitionedAdminAuditLogs
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "PartitionedAdminAuditLogs");
				}
			}

			public IFeature AdminAuditExternalAccessCheckOnDedicated
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "AdminAuditExternalAccessCheckOnDedicated");
				}
			}

			public IFeature LegacyJournaling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "LegacyJournaling");
				}
			}

			public IFeature EHAJournaling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Ipaed.settings.ini", "EHAJournaling");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MailboxAssistantsSettingsIni
		{
			internal MailboxAssistantsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MailboxAssistants.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxAssistants.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxAssistants.settings.ini", id1, ids);
			}

			public IFeature FlagPlus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "FlagPlus");
				}
			}

			public IFeature ApprovalAssistantCheckRateLimit
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ApprovalAssistantCheckRateLimit");
				}
			}

			public IMailboxAssistantSettings StoreUrgentMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "StoreUrgentMaintenanceAssistant");
				}
			}

			public IMailboxAssistantSettings SharePointSignalStoreAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "SharePointSignalStoreAssistant");
				}
			}

			public IMailboxAssistantSettings StoreOnlineIntegrityCheckAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "StoreOnlineIntegrityCheckAssistant");
				}
			}

			public IFeature DirectoryProcessorTenantLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "DirectoryProcessorTenantLogging");
				}
			}

			public ICalendarRepairLoggerSettings CalendarRepairAssistantLogging
			{
				get
				{
					return this.snapshot.GetObject<ICalendarRepairLoggerSettings>("MailboxAssistants.settings.ini", "CalendarRepairAssistantLogging");
				}
			}

			public IFeature CalendarNotificationAssistantSkipUserSettingsUpdate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "CalendarNotificationAssistantSkipUserSettingsUpdate");
				}
			}

			public IFeature ElcAssistantTryProcessEhaMigratedMessages
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ElcAssistantTryProcessEhaMigratedMessages");
				}
			}

			public IMailboxAssistantSettings InferenceDataCollectionAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "InferenceDataCollectionAssistant");
				}
			}

			public IMailboxAssistantSettings SearchIndexRepairAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "SearchIndexRepairAssistant");
				}
			}

			public IFeature TimeBasedAssistantsMonitoring
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "TimeBasedAssistantsMonitoring");
				}
			}

			public IMailboxAssistantSettings TestTBA
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "TestTBA");
				}
			}

			public IFeature OrgMailboxCheckScaleRequirements
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "OrgMailboxCheckScaleRequirements");
				}
			}

			public IMailboxAssistantSettings PeopleRelevanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "PeopleRelevanceAssistant");
				}
			}

			public IMailboxAssistantSettings PublicFolderAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "PublicFolderAssistant");
				}
			}

			public IMailboxAssistantSettings ElcAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "ElcAssistant");
				}
			}

			public IFeature ElcRemoteArchive
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ElcRemoteArchive");
				}
			}

			public IFeature PublicFolderSplit
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "PublicFolderSplit");
				}
			}

			public IMailboxAssistantSettings InferenceTrainingAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "InferenceTrainingAssistant");
				}
			}

			public IFeature SharePointSignalStore
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "SharePointSignalStore");
				}
			}

			public IMailboxAssistantSettings ProbeTimeBasedAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "ProbeTimeBasedAssistant");
				}
			}

			public IFeature SharePointSignalStoreInDatacenter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "SharePointSignalStoreInDatacenter");
				}
			}

			public IFeature GenerateGroupPhoto
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "GenerateGroupPhoto");
				}
			}

			public IMailboxAssistantSettings StoreMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "StoreMaintenanceAssistant");
				}
			}

			public IMailboxAssistantSettings CalendarSyncAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "CalendarSyncAssistant");
				}
			}

			public IFeature EmailReminders
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "EmailReminders");
				}
			}

			public IFeature DelegateRulesLogger
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "DelegateRulesLogger");
				}
			}

			public IMailboxAssistantSettings OABGeneratorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "OABGeneratorAssistant");
				}
			}

			public IMailboxAssistantSettings StoreScheduledIntegrityCheckAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "StoreScheduledIntegrityCheckAssistant");
				}
			}

			public IFeature MwiAssistantGetUMEnabledUsersFromDatacenter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "MwiAssistantGetUMEnabledUsersFromDatacenter");
				}
			}

			public IMailboxAssistantSettings MailboxProcessorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "MailboxProcessorAssistant");
				}
			}

			public IMailboxAssistantSettings PeopleCentricTriageAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "PeopleCentricTriageAssistant");
				}
			}

			public IMailboxAssistantServiceSettings MailboxAssistantService
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantServiceSettings>("MailboxAssistants.settings.ini", "MailboxAssistantService");
				}
			}

			public IFeature ElcAssistantApplyLitigationHoldDuration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ElcAssistantApplyLitigationHoldDuration");
				}
			}

			public IMailboxAssistantSettings TopNWordsAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "TopNWordsAssistant");
				}
			}

			public IMailboxAssistantSettings SiteMailboxAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "SiteMailboxAssistant");
				}
			}

			public IMailboxAssistantSettings UMReportingAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "UMReportingAssistant");
				}
			}

			public IMailboxAssistantSettings DarTaskStoreAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "DarTaskStoreAssistant");
				}
			}

			public IMailboxAssistantSettings GroupMailboxAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "GroupMailboxAssistant");
				}
			}

			public IFeature QuickCapture
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "QuickCapture");
				}
			}

			public IMailboxAssistantSettings JunkEmailOptionsCommitterAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "JunkEmailOptionsCommitterAssistant");
				}
			}

			public IMailboxAssistantSettings DirectoryProcessorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "DirectoryProcessorAssistant");
				}
			}

			public IMailboxAssistantSettings CalendarRepairAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "CalendarRepairAssistant");
				}
			}

			public IMailboxAssistantSettings MailboxAssociationReplicationAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "MailboxAssociationReplicationAssistant");
				}
			}

			public IMailboxAssistantSettings StoreDSMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "StoreDSMaintenanceAssistant");
				}
			}

			public IFeature ElcAssistantDiscoveryHoldSynchronizer
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ElcAssistantDiscoveryHoldSynchronizer");
				}
			}

			public IMailboxAssistantSettings SharingPolicyAssistant
			{
				get
				{
					return this.snapshot.GetObject<IMailboxAssistantSettings>("MailboxAssistants.settings.ini", "SharingPolicyAssistant");
				}
			}

			public IFeature ElcAssistantAlwaysProcessMailbox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "ElcAssistantAlwaysProcessMailbox");
				}
			}

			public IFeature CalendarRepairAssistantReliabilityLogger
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "CalendarRepairAssistantReliabilityLogger");
				}
			}

			public IFeature UnifiedPolicyHold
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "UnifiedPolicyHold");
				}
			}

			public IFeature PerformRecipientDLExpansion
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxAssistants.settings.ini", "PerformRecipientDLExpansion");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MailboxPlansSettingsIni
		{
			internal MailboxPlansSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MailboxPlans.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxPlans.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxPlans.settings.ini", id1, ids);
			}

			public IFeature CloneLimitedSetOfMailboxPlanProperties
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxPlans.settings.ini", "CloneLimitedSetOfMailboxPlanProperties");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MailboxTransportSettingsIni
		{
			internal MailboxTransportSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MailboxTransport.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxTransport.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MailboxTransport.settings.ini", id1, ids);
			}

			public ISettingsValue ParkedMeetingMessagesRetentionPeriod
			{
				get
				{
					return this.snapshot.GetObject<ISettingsValue>("MailboxTransport.settings.ini", "ParkedMeetingMessagesRetentionPeriod");
				}
			}

			public IFeature MailboxTransportSmtpIn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "MailboxTransportSmtpIn");
				}
			}

			public IFeature DeliveryHangRecovery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "DeliveryHangRecovery");
				}
			}

			public IFeature InferenceClassificationAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "InferenceClassificationAgent");
				}
			}

			public IFeature UseParticipantSmtpEmailAddress
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "UseParticipantSmtpEmailAddress");
				}
			}

			public IFeature CheckArbitrationMailboxCapacity
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "CheckArbitrationMailboxCapacity");
				}
			}

			public IFeature ProcessSeriesMeetingMessages
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "ProcessSeriesMeetingMessages");
				}
			}

			public IFeature UseFopeReceivedSpfHeader
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "UseFopeReceivedSpfHeader");
				}
			}

			public IFeature OrderSeriesMeetingMessages
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MailboxTransport.settings.ini", "OrderSeriesMeetingMessages");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MalwareAgentSettingsIni
		{
			internal MalwareAgentSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MalwareAgent.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MalwareAgent.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MalwareAgent.settings.ini", id1, ids);
			}

			public IFeature MalwareAgentV2
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MalwareAgent.settings.ini", "MalwareAgentV2");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MessageTrackingSettingsIni
		{
			internal MessageTrackingSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MessageTracking.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MessageTracking.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MessageTracking.settings.ini", id1, ids);
			}

			public IFeature StatsLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MessageTracking.settings.ini", "StatsLogging");
				}
			}

			public IFeature QueueViewerDiagnostics
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MessageTracking.settings.ini", "QueueViewerDiagnostics");
				}
			}

			public IFeature AllowDebugMode
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MessageTracking.settings.ini", "AllowDebugMode");
				}
			}

			public IFeature UseBackEndLocator
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MessageTracking.settings.ini", "UseBackEndLocator");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MexAgentsSettingsIni
		{
			internal MexAgentsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("MexAgents.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MexAgents.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("MexAgents.settings.ini", id1, ids);
			}

			public IFeature TrustedMailAgents_CrossPremisesHeadersPreserved
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MexAgents.settings.ini", "TrustedMailAgents_CrossPremisesHeadersPreserved");
				}
			}

			public IFeature TrustedMailAgents_AcceptAnyRecipientOnPremises
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MexAgents.settings.ini", "TrustedMailAgents_AcceptAnyRecipientOnPremises");
				}
			}

			public IFeature TrustedMailAgents_StampOriginatorOrgForMsitConnector
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MexAgents.settings.ini", "TrustedMailAgents_StampOriginatorOrgForMsitConnector");
				}
			}

			public IFeature TrustedMailAgents_HandleCrossPremisesProbe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MexAgents.settings.ini", "TrustedMailAgents_HandleCrossPremisesProbe");
				}
			}

			public IFeature TrustedMailAgents_CheckOutboundDeliveryTypeSmtpConnector
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("MexAgents.settings.ini", "TrustedMailAgents_CheckOutboundDeliveryTypeSmtpConnector");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct MrsSettingsIni
		{
			internal MrsSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Mrs.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Mrs.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Mrs.settings.ini", id1, ids);
			}

			public IFeature MigrationMonitor
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "MigrationMonitor");
				}
			}

			public IFeature PublicFolderMailboxesMigration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "PublicFolderMailboxesMigration");
				}
			}

			public IFeature UseDefaultValueForCheckInitialProvisioningForMovesParameter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "UseDefaultValueForCheckInitialProvisioningForMovesParameter");
				}
			}

			public IFeature SlowMRSDetector
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "SlowMRSDetector");
				}
			}

			public IFeature CheckProvisioningSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "CheckProvisioningSettings");
				}
			}

			public IFeature TxSyncMrsImapExecute
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "TxSyncMrsImapExecute");
				}
			}

			public IFeature TxSyncMrsImapCopy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "TxSyncMrsImapCopy");
				}
			}

			public IFeature AutomaticMailboxLoadBalancing
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Mrs.settings.ini", "AutomaticMailboxLoadBalancing");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct NotificationBrokerServiceSettingsIni
		{
			internal NotificationBrokerServiceSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("NotificationBrokerService.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("NotificationBrokerService.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("NotificationBrokerService.settings.ini", id1, ids);
			}

			public IFeature Service
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("NotificationBrokerService.settings.ini", "Service");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OABSettingsIni
		{
			internal OABSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OAB.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OAB.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OAB.settings.ini", id1, ids);
			}

			public IFeature LinkedOABGenMailboxes
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "LinkedOABGenMailboxes");
				}
			}

			public IFeature SkipServiceTopologyDiscovery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "SkipServiceTopologyDiscovery");
				}
			}

			public IFeature EnforceManifestVersionMatch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "EnforceManifestVersionMatch");
				}
			}

			public IFeature SharedTemplateFiles
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "SharedTemplateFiles");
				}
			}

			public IFeature GenerateRequestedOABsOnly
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "GenerateRequestedOABsOnly");
				}
			}

			public IFeature OabHttpClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OAB.settings.ini", "OabHttpClientAccessRulesEnabled");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OfficeGraphSettingsIni
		{
			internal OfficeGraphSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OfficeGraph.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OfficeGraph.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OfficeGraph.settings.ini", id1, ids);
			}

			public IFeature OfficeGraphGenerateSignals
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OfficeGraph.settings.ini", "OfficeGraphGenerateSignals");
				}
			}

			public IFeature OfficeGraphAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OfficeGraph.settings.ini", "OfficeGraphAgent");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OwaClientSettingsIni
		{
			internal OwaClientSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OwaClient.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaClient.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaClient.settings.ini", id1, ids);
			}

			public IFeature TopNSuggestions
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "TopNSuggestions");
				}
			}

			public IFeature O365ShellPlus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "O365ShellPlus");
				}
			}

			public IFeature XOWABirthdayCalendar
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWABirthdayCalendar");
				}
			}

			public IFeature CalendarSearchSurvey
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "CalendarSearchSurvey");
				}
			}

			public IFeature LWX
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "LWX");
				}
			}

			public IFeature FlagPlus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "FlagPlus");
				}
			}

			public IFeature PALDogfoodEnforcement
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "PALDogfoodEnforcement");
				}
			}

			public IFeature EnableFBL
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "EnableFBL");
				}
			}

			public IFeature ModernGroupsQuotedText
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ModernGroupsQuotedText");
				}
			}

			public IFeature InstantEventCreate
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "InstantEventCreate");
				}
			}

			public IFeature BuildGreenLightSurveyFlight
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "BuildGreenLightSurveyFlight");
				}
			}

			public IFeature XOWAShowPersonaCardOnHover
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWAShowPersonaCardOnHover");
				}
			}

			public IFeature CalendarEventSearch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "CalendarEventSearch");
				}
			}

			public IFeature InstantSearch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "InstantSearch");
				}
			}

			public IFeature Like
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "Like");
				}
			}

			public IFeature iOSSharePointRichTextEditor
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "iOSSharePointRichTextEditor");
				}
			}

			public IFeature ModernGroupsTrendingConversations
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ModernGroupsTrendingConversations");
				}
			}

			public IFeature AttachmentsHub
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "AttachmentsHub");
				}
			}

			public IFeature LocationReminder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "LocationReminder");
				}
			}

			public IFeature OWADiagnostics
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OWADiagnostics");
				}
			}

			public IFeature DeleteGroupConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "DeleteGroupConversation");
				}
			}

			public IFeature Oops
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "Oops");
				}
			}

			public IFeature DisableAnimations
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "DisableAnimations");
				}
			}

			public IFeature UnifiedMailboxUI
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "UnifiedMailboxUI");
				}
			}

			public IFeature XOWAUnifiedForms
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWAUnifiedForms");
				}
			}

			public IFeature O365ShellCore
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "O365ShellCore");
				}
			}

			public IFeature XOWAFrequentContacts
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWAFrequentContacts");
				}
			}

			public IFeature InstantPopout
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "InstantPopout");
				}
			}

			public IFeature Water
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "Water");
				}
			}

			public IFeature EmailReminders
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "EmailReminders");
				}
			}

			public IFeature ProposeNewTime
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ProposeNewTime");
				}
			}

			public IFeature EnableAnimations
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "EnableAnimations");
				}
			}

			public IFeature SuperMailLink
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SuperMailLink");
				}
			}

			public IFeature OwaFlow
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OwaFlow");
				}
			}

			public IFeature OptionsLimited
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OptionsLimited");
				}
			}

			public IFeature XOWACalendar
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWACalendar");
				}
			}

			public IFeature SuperSwipe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SuperSwipe");
				}
			}

			public IFeature XOWASuperCommand
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWASuperCommand");
				}
			}

			public IFeature OWADelayedBinding
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OWADelayedBinding");
				}
			}

			public IFeature SharePointOneDrive
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SharePointOneDrive");
				}
			}

			public IFeature SendLinkClickedSignalToSP
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SendLinkClickedSignalToSP");
				}
			}

			public IFeature XOWAAwesomeReadingPane
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWAAwesomeReadingPane");
				}
			}

			public IFeature OrganizationBrowser
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OrganizationBrowser");
				}
			}

			public IFeature O365Miniatures
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "O365Miniatures");
				}
			}

			public IFeature ModernGroupsSurveyGroupA
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ModernGroupsSurveyGroupA");
				}
			}

			public IFeature OwaPublicFolderFavorites
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OwaPublicFolderFavorites");
				}
			}

			public IFeature MailSatisfactionSurvey
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "MailSatisfactionSurvey");
				}
			}

			public IFeature QuickCapture
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "QuickCapture");
				}
			}

			public IFeature OwaLinkPrefetch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "OwaLinkPrefetch");
				}
			}

			public IFeature Options
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "Options");
				}
			}

			public IFeature SuppressPushNotificationsWhenOof
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SuppressPushNotificationsWhenOof");
				}
			}

			public IFeature AndroidCED
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "AndroidCED");
				}
			}

			public IFeature InstantPopout2
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "InstantPopout2");
				}
			}

			public IFeature LanguageQualitySurvey
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "LanguageQualitySurvey");
				}
			}

			public IFeature O365Panorama
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "O365Panorama");
				}
			}

			public IFeature ShowClientWatson
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ShowClientWatson");
				}
			}

			public IFeature HelpPanel
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "HelpPanel");
				}
			}

			public IFeature InstantSearchAlpha
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "InstantSearchAlpha");
				}
			}

			public IFeature MowaInternalFeedback
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "MowaInternalFeedback");
				}
			}

			public IFeature XOWATasks
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWATasks");
				}
			}

			public IFeature XOWAEmoji
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "XOWAEmoji");
				}
			}

			public IFeature ContextualApps
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ContextualApps");
				}
			}

			public IFeature SuperZoom
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "SuperZoom");
				}
			}

			public IFeature AgavePerformance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "AgavePerformance");
				}
			}

			public IFeature ComposeBread1
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "ComposeBread1");
				}
			}

			public IFeature WorkingSetAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClient.settings.ini", "WorkingSetAgent");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OwaClientServerSettingsIni
		{
			internal OwaClientServerSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OwaClientServer.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaClientServer.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaClientServer.settings.ini", id1, ids);
			}

			public IFeature FolderBasedClutter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "FolderBasedClutter");
				}
			}

			public IFeature FlightsView
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "FlightsView");
				}
			}

			public IFeature O365Header
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "O365Header");
				}
			}

			public IFeature OwaVersioning
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "OwaVersioning");
				}
			}

			public IFeature AutoSubscribeNewGroupMembers
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "AutoSubscribeNewGroupMembers");
				}
			}

			public IFeature XOWAHolidayCalendars
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "XOWAHolidayCalendars");
				}
			}

			public IFeature AttachmentsFilePicker
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "AttachmentsFilePicker");
				}
			}

			public IFeature GroupRegionalConfiguration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "GroupRegionalConfiguration");
				}
			}

			public IFeature DocCollab
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "DocCollab");
				}
			}

			public IFeature OwaPublicFolders
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "OwaPublicFolders");
				}
			}

			public IFeature O365ParityHeader
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "O365ParityHeader");
				}
			}

			public IFeature ModernMail
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ModernMail");
				}
			}

			public IFeature SmimeConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "SmimeConversation");
				}
			}

			public IFeature ActiveViewConvergence
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ActiveViewConvergence");
				}
			}

			public IFeature ModernGroupsWorkingSet
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ModernGroupsWorkingSet");
				}
			}

			public IFeature InlinePreview
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "InlinePreview");
				}
			}

			public IFeature PeopleCentricTriage
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "PeopleCentricTriage");
				}
			}

			public IFeature ChangeLayout
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ChangeLayout");
				}
			}

			public IFeature SuperStart
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "SuperStart");
				}
			}

			public IFeature SuperNormal
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "SuperNormal");
				}
			}

			public IFeature FasterPhoto
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "FasterPhoto");
				}
			}

			public IFeature NotificationBroker
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "NotificationBroker");
				}
			}

			public IFeature ModernGroupsNewArchitecture
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ModernGroupsNewArchitecture");
				}
			}

			public IFeature SuperSort
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "SuperSort");
				}
			}

			public IFeature AutoSubscribeSetByDefault
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "AutoSubscribeSetByDefault");
				}
			}

			public IFeature SafeHtml
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "SafeHtml");
				}
			}

			public IFeature Weather
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "Weather");
				}
			}

			public IFeature ModernGroups
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ModernGroups");
				}
			}

			public IFeature ModernAttachments
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "ModernAttachments");
				}
			}

			public IFeature OWAPLTPerf
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "OWAPLTPerf");
				}
			}

			public IFeature O365G2Header
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaClientServer.settings.ini", "O365G2Header");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OwaServerSettingsIni
		{
			internal OwaServerSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OwaServer.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaServer.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaServer.settings.ini", id1, ids);
			}

			public IFeature OwaMailboxSessionCloning
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaMailboxSessionCloning");
				}
			}

			public IFeature PeopleCentricConversation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "PeopleCentricConversation");
				}
			}

			public IFeature OwaSessionDataPreload
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaSessionDataPreload");
				}
			}

			public IFeature ShouldSkipAdfsGroupReadOnFrontend
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "ShouldSkipAdfsGroupReadOnFrontend");
				}
			}

			public IFeature XOWABirthdayAssistant
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "XOWABirthdayAssistant");
				}
			}

			public IInlineExploreSettings InlineExploreSettings
			{
				get
				{
					return this.snapshot.GetObject<IInlineExploreSettings>("OwaServer.settings.ini", "InlineExploreSettings");
				}
			}

			public IFeature InferenceUI
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "InferenceUI");
				}
			}

			public IFeature OwaHttpHandler
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaHttpHandler");
				}
			}

			public IFeature FlightFormat
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "FlightFormat");
				}
			}

			public IFeature AndroidPremium
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "AndroidPremium");
				}
			}

			public IFeature ModernConversationPrep
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "ModernConversationPrep");
				}
			}

			public IFeature OptimizedParticipantResolver
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OptimizedParticipantResolver");
				}
			}

			public IFeature OwaHostNameSwitch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaHostNameSwitch");
				}
			}

			public IFeature OwaVNext
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaVNext");
				}
			}

			public IFeature OWAEdgeMode
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OWAEdgeMode");
				}
			}

			public IFeature OwaCompositeSessionData
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaCompositeSessionData");
				}
			}

			public IFeature ReportJunk
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "ReportJunk");
				}
			}

			public IFeature OwaClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaClientAccessRulesEnabled");
				}
			}

			public IFeature OwaServerLogonActivityLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "OwaServerLogonActivityLogging");
				}
			}

			public IFeature InlineExploreUI
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaServer.settings.ini", "InlineExploreUI");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct OwaDeploymentSettingsIni
		{
			internal OwaDeploymentSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("OwaDeployment.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaDeployment.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("OwaDeployment.settings.ini", id1, ids);
			}

			public IFeature PublicFolderTreePerTenanant
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "PublicFolderTreePerTenanant");
				}
			}

			public IFeature ExplicitLogonAuthFilter
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "ExplicitLogonAuthFilter");
				}
			}

			public IFeature Places
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "Places");
				}
			}

			public IFeature IncludeAccountAccessDisclaimer
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "IncludeAccountAccessDisclaimer");
				}
			}

			public IFeature FilterETag
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "FilterETag");
				}
			}

			public IFeature CacheUMCultures
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "CacheUMCultures");
				}
			}

			public IFeature RedirectToServer
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "RedirectToServer");
				}
			}

			public IFeature UseAccessProxyForInstantMessagingServerName
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UseAccessProxyForInstantMessagingServerName");
				}
			}

			public IFeature UseBackendVdirConfiguration
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UseBackendVdirConfiguration");
				}
			}

			public IFeature OneDriveProProviderAvailable
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "OneDriveProProviderAvailable");
				}
			}

			public IFeature LogTenantInfo
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "LogTenantInfo");
				}
			}

			public IFeature RedirectToLogoffPage
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "RedirectToLogoffPage");
				}
			}

			public IFeature IsBranded
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "IsBranded");
				}
			}

			public IFeature SkipPushNotificationStorageTenantId
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "SkipPushNotificationStorageTenantId");
				}
			}

			public IFeature UseVdirConfigForInstantMessaging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UseVdirConfigForInstantMessaging");
				}
			}

			public IFeature RenderPrivacyStatement
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "RenderPrivacyStatement");
				}
			}

			public IFeature UseRootDirForAppCacheVdir
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UseRootDirForAppCacheVdir");
				}
			}

			public IFeature IsLogonFormatEmail
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "IsLogonFormatEmail");
				}
			}

			public IFeature WacConfigurationFromOrgConfig
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "WacConfigurationFromOrgConfig");
				}
			}

			public IFeature MrsConnectedAccountsSync
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "MrsConnectedAccountsSync");
				}
			}

			public IFeature UsePersistedCapabilities
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UsePersistedCapabilities");
				}
			}

			public IFeature CheckFeatureRestrictions
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "CheckFeatureRestrictions");
				}
			}

			public IFeature HideInternalUrls
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "HideInternalUrls");
				}
			}

			public IFeature IncludeImportContactListButton
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "IncludeImportContactListButton");
				}
			}

			public IFeature UseThemeStorageFolder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "UseThemeStorageFolder");
				}
			}

			public IFeature ConnectedAccountsSync
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("OwaDeployment.settings.ini", "ConnectedAccountsSync");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct PopSettingsIni
		{
			internal PopSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Pop.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Pop.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Pop.settings.ini", id1, ids);
			}

			public IFeature PopClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "PopClientAccessRulesEnabled");
				}
			}

			public IFeature IgnoreNonProvisionedServers
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "IgnoreNonProvisionedServers");
				}
			}

			public IFeature UseSamAccountNameAsUsername
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "UseSamAccountNameAsUsername");
				}
			}

			public IFeature SkipAuthOnCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "SkipAuthOnCafe");
				}
			}

			public IFeature GlobalCriminalCompliance
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "GlobalCriminalCompliance");
				}
			}

			public IFeature CheckOnlyAuthenticationStatus
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "CheckOnlyAuthenticationStatus");
				}
			}

			public IFeature EnforceLogsRetentionPolicy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "EnforceLogsRetentionPolicy");
				}
			}

			public IFeature AppendServerNameInBanner
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "AppendServerNameInBanner");
				}
			}

			public IFeature UsePrimarySmtpAddress
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "UsePrimarySmtpAddress");
				}
			}

			public IFeature LrsLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Pop.settings.ini", "LrsLogging");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct RpcClientAccessSettingsIni
		{
			internal RpcClientAccessSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("RpcClientAccess.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("RpcClientAccess.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("RpcClientAccess.settings.ini", id1, ids);
			}

			public IFeature FilterModernCalendarItemsMomtIcs
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtIcs");
				}
			}

			public IFeature BlockInsufficientClientVersions
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "BlockInsufficientClientVersions");
				}
			}

			public IFeature StreamInsightUploader
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "StreamInsightUploader");
				}
			}

			public IFeature FilterModernCalendarItemsMomtSearch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtSearch");
				}
			}

			public IFeature RpcHttpClientAccessRulesEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "RpcHttpClientAccessRulesEnabled");
				}
			}

			public IFeature DetectCharsetAndConvertHtmlBodyOnSave
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "DetectCharsetAndConvertHtmlBodyOnSave");
				}
			}

			public IFeature MimumResponseSizeEnforcement
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "MimumResponseSizeEnforcement");
				}
			}

			public IFeature XtcEndpoint
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "XtcEndpoint");
				}
			}

			public IFeature IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty");
				}
			}

			public IFeature ServerInformation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "ServerInformation");
				}
			}

			public IFeature FilterModernCalendarItemsMomtView
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("RpcClientAccess.settings.ini", "FilterModernCalendarItemsMomtView");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct SearchSettingsIni
		{
			internal SearchSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Search.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Search.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Search.settings.ini", id1, ids);
			}

			public ITransportFlowSettings TransportFlowSettings
			{
				get
				{
					return this.snapshot.GetObject<ITransportFlowSettings>("Search.settings.ini", "TransportFlowSettings");
				}
			}

			public IFeature RequireMountedForCrawl
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "RequireMountedForCrawl");
				}
			}

			public IFeature RemoveOrphanedCatalogs
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "RemoveOrphanedCatalogs");
				}
			}

			public IIndexStatusSettings IndexStatusInvalidateInterval
			{
				get
				{
					return this.snapshot.GetObject<IIndexStatusSettings>("Search.settings.ini", "IndexStatusInvalidateInterval");
				}
			}

			public IFeature ProcessItemsWithNullCompositeId
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "ProcessItemsWithNullCompositeId");
				}
			}

			public IInstantSearch InstantSearch
			{
				get
				{
					return this.snapshot.GetObject<IInstantSearch>("Search.settings.ini", "InstantSearch");
				}
			}

			public IFeature CrawlerFeederUpdateCrawlingStatusResetCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "CrawlerFeederUpdateCrawlingStatusResetCache");
				}
			}

			public ILanguageDetection LanguageDetection
			{
				get
				{
					return this.snapshot.GetObject<ILanguageDetection>("Search.settings.ini", "LanguageDetection");
				}
			}

			public IFeature CachePreWarmingEnabled
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "CachePreWarmingEnabled");
				}
			}

			public IFeature MonitorDocumentValidationFailures
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "MonitorDocumentValidationFailures");
				}
			}

			public IFeature UseAlphaSchema
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "UseAlphaSchema");
				}
			}

			public IFeature EnableIndexPartsCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableIndexPartsCache");
				}
			}

			public IFeature SchemaUpgrading
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "SchemaUpgrading");
				}
			}

			public IFeature EnableGracefulDegradation
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableGracefulDegradation");
				}
			}

			public IFeature EnableIndexStatusTimestampVerification
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableIndexStatusTimestampVerification");
				}
			}

			public IFeature EnableDynamicActivationPreference
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableDynamicActivationPreference");
				}
			}

			public IFeature UseExecuteAndReadPage
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "UseExecuteAndReadPage");
				}
			}

			public ICompletions Completions
			{
				get
				{
					return this.snapshot.GetObject<ICompletions>("Search.settings.ini", "Completions");
				}
			}

			public IFeature UseBetaSchema
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "UseBetaSchema");
				}
			}

			public IFeature ReadFlag
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "ReadFlag");
				}
			}

			public IFeature EnableSingleValueRefiners
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableSingleValueRefiners");
				}
			}

			public IDocumentFeederSettings DocumentFeederSettings
			{
				get
				{
					return this.snapshot.GetObject<IDocumentFeederSettings>("Search.settings.ini", "DocumentFeederSettings");
				}
			}

			public IFeature CrawlerFeederCollectDocumentsVerifyPendingWatermarks
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "CrawlerFeederCollectDocumentsVerifyPendingWatermarks");
				}
			}

			public IMemoryModelSettings MemoryModel
			{
				get
				{
					return this.snapshot.GetObject<IMemoryModelSettings>("Search.settings.ini", "MemoryModel");
				}
			}

			public IFeature EnableTopN
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableTopN");
				}
			}

			public IFeederSettings FeederSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeederSettings>("Search.settings.ini", "FeederSettings");
				}
			}

			public IFeature WaitForMountPoints
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "WaitForMountPoints");
				}
			}

			public IFeature EnableInstantSearch
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Search.settings.ini", "EnableInstantSearch");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct SharedCacheSettingsIni
		{
			internal SharedCacheSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("SharedCache.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("SharedCache.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("SharedCache.settings.ini", id1, ids);
			}

			public IFeature UsePersistenceForCafe
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("SharedCache.settings.ini", "UsePersistenceForCafe");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct SharedMailboxSettingsIni
		{
			internal SharedMailboxSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("SharedMailbox.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("SharedMailbox.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("SharedMailbox.settings.ini", id1, ids);
			}

			public IFeature SharedMailboxSentItemCopy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("SharedMailbox.settings.ini", "SharedMailboxSentItemCopy");
				}
			}

			public IFeature SharedMailboxSentItemsRoutingAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("SharedMailbox.settings.ini", "SharedMailboxSentItemsRoutingAgent");
				}
			}

			public IFeature SharedMailboxSentItemsDeliveryAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("SharedMailbox.settings.ini", "SharedMailboxSentItemsDeliveryAgent");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct TestSettingsIni
		{
			internal TestSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Test.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Test.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Test.settings.ini", id1, ids);
			}

			public IFeature TestSettingsEnterprise
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test.settings.ini", "TestSettingsEnterprise");
				}
			}

			public IFeature TestSettings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test.settings.ini", "TestSettings");
				}
			}

			public IFeature TestSettingsOn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test.settings.ini", "TestSettingsOn");
				}
			}

			public IFeature TestSettingsOff
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test.settings.ini", "TestSettingsOff");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct Test2SettingsIni
		{
			internal Test2SettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Test2.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Test2.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Test2.settings.ini", id1, ids);
			}

			public IFeature Test2SettingsOn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test2.settings.ini", "Test2SettingsOn");
				}
			}

			public IFeature Test2Settings
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test2.settings.ini", "Test2Settings");
				}
			}

			public IFeature Test2SettingsEnterprise
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test2.settings.ini", "Test2SettingsEnterprise");
				}
			}

			public IFeature Test2SettingsOff
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Test2.settings.ini", "Test2SettingsOff");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct TransportSettingsIni
		{
			internal TransportSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("Transport.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Transport.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("Transport.settings.ini", id1, ids);
			}

			public IFeature VerboseLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "VerboseLogging");
				}
			}

			public IFeature TargetAddressRoutingForRemoteGroupMailbox
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "TargetAddressRoutingForRemoteGroupMailbox");
				}
			}

			public IMessageDepotSettings MessageDepot
			{
				get
				{
					return this.snapshot.GetObject<IMessageDepotSettings>("Transport.settings.ini", "MessageDepot");
				}
			}

			public IFeature SelectHubServersForClientProxy
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SelectHubServersForClientProxy");
				}
			}

			public IFeature TestProcessingQuota
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "TestProcessingQuota");
				}
			}

			public IFeature SystemMessageOverrides
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SystemMessageOverrides");
				}
			}

			public IFeature DirectTrustCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "DirectTrustCache");
				}
			}

			public IFeature UseNewConnectorMatchingOrder
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "UseNewConnectorMatchingOrder");
				}
			}

			public IFeature TargetAddressDistributionGroupAsExternal
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "TargetAddressDistributionGroupAsExternal");
				}
			}

			public IFeature ConsolidateAdvancedRouting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "ConsolidateAdvancedRouting");
				}
			}

			public IFeature ClientAuthRequireMailboxDatabase
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "ClientAuthRequireMailboxDatabase");
				}
			}

			public IFeature UseTenantPartitionToCreateOrganizationId
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "UseTenantPartitionToCreateOrganizationId");
				}
			}

			public IFeature LimitTransportRules
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "LimitTransportRules");
				}
			}

			public IFeature SmtpAcceptAnyRecipient
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SmtpAcceptAnyRecipient");
				}
			}

			public IFeature RiskBasedCounters
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "RiskBasedCounters");
				}
			}

			public IFeature DefaultTransportServiceStateToInactive
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "DefaultTransportServiceStateToInactive");
				}
			}

			public IFeature LimitRemoteDomains
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "LimitRemoteDomains");
				}
			}

			public IFeature IgnoreArbitrationMailboxForModeratedRecipient
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "IgnoreArbitrationMailboxForModeratedRecipient");
				}
			}

			public IFeature TransferAdditionalTenantDataThroughXATTR
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "TransferAdditionalTenantDataThroughXATTR");
				}
			}

			public IFeature ADExceptionHandling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "ADExceptionHandling");
				}
			}

			public IFeature EnforceProcessingQuota
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "EnforceProcessingQuota");
				}
			}

			public IFeature SystemProbeDropAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SystemProbeDropAgent");
				}
			}

			public IFeature SetMustDeliverJournalReport
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SetMustDeliverJournalReport");
				}
			}

			public IFeature SendUserAddressInXproxyCommand
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SendUserAddressInXproxyCommand");
				}
			}

			public IFeature UseAdditionalTenantDataFromXATTR
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "UseAdditionalTenantDataFromXATTR");
				}
			}

			public IFeature DelayDsn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "DelayDsn");
				}
			}

			public IFeature ExplicitDeletedObjectNotifications
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "ExplicitDeletedObjectNotifications");
				}
			}

			public IFeature EnforceQueueQuota
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "EnforceQueueQuota");
				}
			}

			public IFeature OrganizationMailboxRouting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "OrganizationMailboxRouting");
				}
			}

			public IFeature StringentHeaderTransformationMode
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "StringentHeaderTransformationMode");
				}
			}

			public IFeature SmtpReceiveCountersStripServerName
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SmtpReceiveCountersStripServerName");
				}
			}

			public IFeature ClientSubmissionToDelivery
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "ClientSubmissionToDelivery");
				}
			}

			public IFeature SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn");
				}
			}

			public IFeature EnforceOutboundConnectorAndAcceptedDomainsRestriction
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "EnforceOutboundConnectorAndAcceptedDomainsRestriction");
				}
			}

			public IFeature TenantThrottling
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "TenantThrottling");
				}
			}

			public IFeature SystemProbeLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("Transport.settings.ini", "SystemProbeLogging");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct UCCSettingsIni
		{
			internal UCCSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("UCC.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("UCC.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("UCC.settings.ini", id1, ids);
			}

			public IFeature UCC
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UCC.settings.ini", "UCC");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct UMSettingsIni
		{
			internal UMSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("UM.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("UM.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("UM.settings.ini", id1, ids);
			}

			public IFeature UMDataCenterLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "UMDataCenterLogging");
				}
			}

			public IFeature VoicemailDiskSpaceDatacenterLimit
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "VoicemailDiskSpaceDatacenterLimit");
				}
			}

			public IFeature DatacenterUMGrammarTenantCache
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "DatacenterUMGrammarTenantCache");
				}
			}

			public IFeature DirectoryGrammarCountLimit
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "DirectoryGrammarCountLimit");
				}
			}

			public IFeature UMDataCenterAD
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "UMDataCenterAD");
				}
			}

			public IFeature AddressListGrammars
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "AddressListGrammars");
				}
			}

			public IFeature GetServerDialPlans
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "GetServerDialPlans");
				}
			}

			public IFeature UMDataCenterLanguages
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "UMDataCenterLanguages");
				}
			}

			public IFeature UMDataCenterCallRouting
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "UMDataCenterCallRouting");
				}
			}

			public IFeature HuntGroupCreationForSipDialplans
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "HuntGroupCreationForSipDialplans");
				}
			}

			public IFeature DTMFMapGenerator
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "DTMFMapGenerator");
				}
			}

			public IFeature AlwaysLogTraces
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "AlwaysLogTraces");
				}
			}

			public IFeature AnonymizeLogging
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "AnonymizeLogging");
				}
			}

			public IFeature ServerDialPlanLink
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "ServerDialPlanLink");
				}
			}

			public IFeature SipInfoNotifications
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("UM.settings.ini", "SipInfoNotifications");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct VariantConfigSettingsIni
		{
			internal VariantConfigSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("VariantConfig.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("VariantConfig.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("VariantConfig.settings.ini", id1, ids);
			}

			public IOrganizationNameSettings Microsoft
			{
				get
				{
					return this.snapshot.GetObject<IOrganizationNameSettings>("VariantConfig.settings.ini", "Microsoft");
				}
			}

			public IFeature InternalAccess
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("VariantConfig.settings.ini", "InternalAccess");
				}
			}

			public IOverrideSyncSettings SettingOverrideSync
			{
				get
				{
					return this.snapshot.GetObject<IOverrideSyncSettings>("VariantConfig.settings.ini", "SettingOverrideSync");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct WorkingSetSettingsIni
		{
			internal WorkingSetSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("WorkingSet.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("WorkingSet.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("WorkingSet.settings.ini", id1, ids);
			}

			public IFeature WorkingSetAgent
			{
				get
				{
					return this.snapshot.GetObject<IFeature>("WorkingSet.settings.ini", "WorkingSetAgent");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}

		public struct WorkloadManagementSettingsIni
		{
			internal WorkloadManagementSettingsIni(VariantConfigurationSnapshot snapshot)
			{
				this.snapshot = snapshot;
			}

			public IDictionary<string, T> GetObjectsOfType<T>() where T : class, ISettings
			{
				return this.snapshot.GetObjectsOfType<T>("WorkloadManagement.settings.ini");
			}

			public T GetObject<T>(string id) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("WorkloadManagement.settings.ini", id);
			}

			public T GetObject<T>(object id1, params object[] ids) where T : class, ISettings
			{
				return this.snapshot.GetObject<T>("WorkloadManagement.settings.ini", id1, ids);
			}

			public IWorkloadSettings PowerShell
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShell");
				}
			}

			public IWorkloadSettings PowerShellForwardSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShellForwardSync");
				}
			}

			public IWorkloadSettings Ews
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Ews");
				}
			}

			public IResourceSettings Processor
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "Processor");
				}
			}

			public IWorkloadSettings StoreUrgentMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "StoreUrgentMaintenanceAssistant");
				}
			}

			public IWorkloadSettings SharePointSignalStoreAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "SharePointSignalStoreAssistant");
				}
			}

			public IWorkloadSettings StoreOnlineIntegrityCheckAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "StoreOnlineIntegrityCheckAssistant");
				}
			}

			public IWorkloadSettings PowerShellLowPriorityWorkFlow
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShellLowPriorityWorkFlow");
				}
			}

			public IWorkloadSettings E4eRecipient
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "E4eRecipient");
				}
			}

			public IWorkloadSettings Eas
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Eas");
				}
			}

			public IWorkloadSettings Transport
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Transport");
				}
			}

			public IWorkloadSettings InferenceDataCollectionAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "InferenceDataCollectionAssistant");
				}
			}

			public IWorkloadSettings Owa
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Owa");
				}
			}

			public IWorkloadSettings PeopleRelevanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PeopleRelevanceAssistant");
				}
			}

			public IWorkloadSettings PublicFolderAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PublicFolderAssistant");
				}
			}

			public IWorkloadSettings TransportSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "TransportSync");
				}
			}

			public IWorkloadSettings OrgContactsSyncAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "OrgContactsSyncAssistant");
				}
			}

			public IWorkloadSettings InferenceTrainingAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "InferenceTrainingAssistant");
				}
			}

			public IWorkloadSettings ProbeTimeBasedAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "ProbeTimeBasedAssistant");
				}
			}

			public IWorkloadSettings DarRuntime
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "DarRuntime");
				}
			}

			public IBlackoutSettings Blackout
			{
				get
				{
					return this.snapshot.GetObject<IBlackoutSettings>("WorkloadManagement.settings.ini", "Blackout");
				}
			}

			public IWorkloadSettings MailboxReplicationServiceHighPriority
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxReplicationServiceHighPriority");
				}
			}

			public IWorkloadSettings MailboxReplicationServiceInteractive
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxReplicationServiceInteractive");
				}
			}

			public IWorkloadSettings StoreMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "StoreMaintenanceAssistant");
				}
			}

			public IWorkloadSettings CalendarSyncAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "CalendarSyncAssistant");
				}
			}

			public IWorkloadSettings DarTaskStoreTimeBasedAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "DarTaskStoreTimeBasedAssistant");
				}
			}

			public IWorkloadSettings ELCAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "ELCAssistant");
				}
			}

			public ISystemWorkloadManagerSettings SystemWorkloadManager
			{
				get
				{
					return this.snapshot.GetObject<ISystemWorkloadManagerSettings>("WorkloadManagement.settings.ini", "SystemWorkloadManager");
				}
			}

			public IResourceSettings DiskLatency
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "DiskLatency");
				}
			}

			public IWorkloadSettings O365SuiteService
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "O365SuiteService");
				}
			}

			public IDiskLatencyMonitorSettings DiskLatencySettings
			{
				get
				{
					return this.snapshot.GetObject<IDiskLatencyMonitorSettings>("WorkloadManagement.settings.ini", "DiskLatencySettings");
				}
			}

			public IWorkloadSettings PublicFolderMailboxSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PublicFolderMailboxSync");
				}
			}

			public IWorkloadSettings OABGeneratorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "OABGeneratorAssistant");
				}
			}

			public IWorkloadSettings TeamMailboxSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "TeamMailboxSync");
				}
			}

			public IWorkloadSettings StoreScheduledIntegrityCheckAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "StoreScheduledIntegrityCheckAssistant");
				}
			}

			public IWorkloadSettings Domt
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Domt");
				}
			}

			public IWorkloadSettings MailboxReplicationServiceInternalMaintenance
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxReplicationServiceInternalMaintenance");
				}
			}

			public IWorkloadSettings PowerShellGalSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShellGalSync");
				}
			}

			public IWorkloadSettings Momt
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Momt");
				}
			}

			public IWorkloadSettings MailboxProcessorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxProcessorAssistant");
				}
			}

			public IWorkloadSettings SearchIndexRepairTimebasedAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "SearchIndexRepairTimebasedAssistant");
				}
			}

			public IWorkloadSettings PeopleCentricTriageAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PeopleCentricTriageAssistant");
				}
			}

			public IWorkloadSettings TopNAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "TopNAssistant");
				}
			}

			public IWorkloadSettings OwaVoice
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "OwaVoice");
				}
			}

			public IWorkloadSettings E4eSender
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "E4eSender");
				}
			}

			public IWorkloadSettings Imap
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Imap");
				}
			}

			public IWorkloadSettings SiteMailboxAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "SiteMailboxAssistant");
				}
			}

			public IWorkloadSettings UMReportingAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "UMReportingAssistant");
				}
			}

			public IResourceSettings ActiveDirectoryReplicationLatency
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "ActiveDirectoryReplicationLatency");
				}
			}

			public IWorkloadSettings OutlookService
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "OutlookService");
				}
			}

			public IWorkloadSettings GroupMailboxAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "GroupMailboxAssistant");
				}
			}

			public IResourceSettings MdbAvailability
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "MdbAvailability");
				}
			}

			public IResourceSettings MdbLatency
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "MdbLatency");
				}
			}

			public IWorkloadSettings JunkEmailOptionsCommitterAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "JunkEmailOptionsCommitterAssistant");
				}
			}

			public IWorkloadSettings DirectoryProcessorAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "DirectoryProcessorAssistant");
				}
			}

			public IWorkloadSettings CalendarRepairAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "CalendarRepairAssistant");
				}
			}

			public IWorkloadSettings MailboxAssociationReplicationAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxAssociationReplicationAssistant");
				}
			}

			public IWorkloadSettings StoreDSMaintenanceAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "StoreDSMaintenanceAssistant");
				}
			}

			public IResourceSettings CiAgeOfLastNotification
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "CiAgeOfLastNotification");
				}
			}

			public IWorkloadSettings SharingPolicyAssistant
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "SharingPolicyAssistant");
				}
			}

			public IResourceSettings MdbReplication
			{
				get
				{
					return this.snapshot.GetObject<IResourceSettings>("WorkloadManagement.settings.ini", "MdbReplication");
				}
			}

			public IWorkloadSettings PowerShellBackSync
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShellBackSync");
				}
			}

			public IWorkloadSettings Pop
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "Pop");
				}
			}

			public IWorkloadSettings MailboxReplicationService
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "MailboxReplicationService");
				}
			}

			public IWorkloadSettings PushNotificationService
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PushNotificationService");
				}
			}

			public IWorkloadSettings PowerShellDiscretionaryWorkFlow
			{
				get
				{
					return this.snapshot.GetObject<IWorkloadSettings>("WorkloadManagement.settings.ini", "PowerShellDiscretionaryWorkFlow");
				}
			}

			private VariantConfigurationSnapshot snapshot;
		}
	}
}
