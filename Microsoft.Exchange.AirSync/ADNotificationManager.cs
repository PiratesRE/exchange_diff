using System;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal static class ADNotificationManager
	{
		public static ADMobileVirtualDirectory ADMobileVirtualDirectory
		{
			get
			{
				if (Command.CurrentCommand != null && Command.CurrentCommand.Request.VDirSettingsHeader != null)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, null, "VDir object returned by ADNotificationManager using command request header:", Command.CurrentCommand.Request.VDirSettingsHeader);
					return ADNotificationManager.vDirSettingsCache.Get(Command.CurrentCommand.Request.VDirSettingsHeader);
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.AlgorithmTracer, null, "Null VDir object returned by ADNotificationManager because no command was being executed");
				return null;
			}
		}

		public static bool Started
		{
			get
			{
				return ADNotificationManager.started;
			}
		}

		public static void Start(IAirSyncContext context)
		{
			lock (ADNotificationManager.startLock)
			{
				if (!ADNotificationManager.started)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "ADNotificationManager is being started ...");
					ADNotificationManager.topoSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 189, "Start", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ADNotificationManager.cs");
					ADNotificationManager.LoadAutoBlockThresholds();
					ADNotificationManager.started = true;
					context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, ADNotificationManager.topoSession.LastUsedDc);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "ADNotificationManager is started.");
					int expireTimeInMinutes = (GlobalSettings.ADCacheExpirationTimeout.TotalMinutes < 1.0) ? 1 : ((int)GlobalSettings.ADCacheExpirationTimeout.TotalMinutes);
					ADNotificationManager.policies = new MruDictionaryCache<string, ADNotificationManager.ADSettingsInfo<PolicyData>>(GlobalSettings.ADCacheMaxOrgCount, GlobalSettings.ADCacheMaxOrgCount, expireTimeInMinutes);
					ADNotificationManager.organizationSettingsCache = new MruDictionaryCache<string, ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData>>(GlobalSettings.ADCacheMaxOrgCount, GlobalSettings.ADCacheMaxOrgCount, expireTimeInMinutes);
					ADNotificationManager.vDirSettingsCache = new ADObjIdToVDirMap();
				}
			}
		}

		public static void Stop()
		{
			if (ADNotificationManager.started)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "ADNotificationManager is being stopped ...");
				ADNotificationManager.started = false;
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "ADNotificationManager is stopped.");
			}
		}

		public static PolicyData GetPolicyData(IAirSyncUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (!ADNotificationManager.started)
			{
				throw new InvalidOperationException("ADNotificationManager should be started first!");
			}
			PolicyData policyData = null;
			if (user.ADUser.ActiveSyncMailboxPolicy != null)
			{
				policyData = ADNotificationManager.GetPolicySetting(user);
			}
			if (policyData == null)
			{
				policyData = ADNotificationManager.GetDefaultPolicySetting(user);
			}
			return policyData;
		}

		public static IOrganizationSettingsData GetOrganizationSettingsData(IAirSyncUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			return ADNotificationManager.GetOrganizationSettingsData(user.OrganizationId, user.Context);
		}

		public static IOrganizationSettingsData GetOrganizationSettingsData(OrganizationId organizationId, IAirSyncContext context)
		{
			if (organizationId == null)
			{
				throw new ArgumentException("OrganizationId is null");
			}
			if (!ADNotificationManager.started)
			{
				throw new InvalidOperationException("ADNotificationManager should be started first!");
			}
			IConfigurationSession scopedSession = null;
			ADObjectId configurationUnit = organizationId.ConfigurationUnit;
			if (configurationUnit == null)
			{
				if (ADNotificationManager.enterpriseConfigurationID == null)
				{
					scopedSession = ADNotificationManager.CreateScopedADSession(organizationId, ConsistencyMode.IgnoreInvalid);
					ADNotificationManager.enterpriseConfigurationID = scopedSession.GetOrgContainerId();
					if (context != null)
					{
						context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, scopedSession.LastUsedDc);
					}
				}
				configurationUnit = ADNotificationManager.enterpriseConfigurationID;
			}
			ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData> adsettingsInfo = null;
			AirSyncDiagnostics.TraceDebug<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "Loaded ConfigurationUnit {0}", configurationUnit);
			Func<IOrganizationSettingsData> loadDataAction = () => ADNotificationManager.LoadOrganizationSettings(scopedSession ?? ADNotificationManager.CreateScopedADSession(organizationId, ConsistencyMode.PartiallyConsistent), context);
			ADNotificationManager.LoadADSettingsData<IOrganizationSettingsData>(ADNotificationManager.organizationSettingsCache, configurationUnit.DistinguishedName, loadDataAction, organizationId.PartitionId, out adsettingsInfo);
			return adsettingsInfo.ADSettingsData;
		}

		public static DeviceAutoBlockThreshold GetAutoBlockThreshold(DeviceAccessStateReason reason)
		{
			AutoblockThresholdType type = (AutoblockThresholdType)(reason - 6);
			return ADNotificationManager.GetAutoBlockThreshold(type);
		}

		public static DeviceAutoBlockThreshold GetAutoBlockThreshold(AutoblockThresholdType type)
		{
			AirSyncDiagnostics.TraceDebug<AutoblockThresholdType>(ExTraceGlobals.RequestsTracer, null, "GetAutoblockThreshold data.AutoblockThresholdType {0}", type);
			if (!Enum.IsDefined(typeof(AutoblockThresholdType), type))
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "InvalidAutoblockReason");
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateCorrupt, null, false)
				{
					ErrorStringForProtocolLogger = "InvalidAutoblockReason"
				};
			}
			if (ADNotificationManager.autoBlockThresholds == null || ADNotificationManager.autoBlockThresholds.DeviceAutoBlockThresholds == null || ADNotificationManager.autoBlockThresholds.WhenCreated.Add(GlobalSettings.ADCacheExpirationTimeout) < ExDateTime.UtcNow)
			{
				lock (ADNotificationManager.startLock)
				{
					if (ADNotificationManager.autoBlockThresholds == null || ADNotificationManager.autoBlockThresholds.DeviceAutoBlockThresholds == null || ADNotificationManager.autoBlockThresholds.WhenCreated.Add(GlobalSettings.ADCacheExpirationTimeout) < ExDateTime.UtcNow)
					{
						AirSyncDiagnostics.TraceDebug<AutoblockThresholdType>(ExTraceGlobals.RequestsTracer, null, "refresh AutoblockThreshold data from AD.AutoblockThresholdType {0}", type);
						ADNotificationManager.LoadAutoBlockThresholds();
					}
				}
			}
			return ADNotificationManager.autoBlockThresholds.DeviceAutoBlockThresholds[(int)type];
		}

		private static void HandleMultipleDefaultPolicies(MobileMailboxPolicy[] defaultPolicies, ADObjectId organizationalUnitRoot, ProtocolLogger protocollogger)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < defaultPolicies.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(defaultPolicies[i].Identity);
			}
			AirSyncDiagnostics.LogPeriodicEvent(AirSyncEventLogConstants.Tuple_MultipleDefaultMobileMailboxPoliciesDetected, "MultipleDefaultMobileMailboxPoliciesDetected", new string[]
			{
				organizationalUnitRoot.Parent.Name,
				stringBuilder.ToString()
			});
			protocollogger.SetValue(ProtocolLoggerData.Error, "MultipleDefaultPoliciesDetected");
		}

		private static IConfigurationSession CreateScopedADSession(IAirSyncUser user)
		{
			return ADNotificationManager.CreateScopedADSession(user.ADUser.OrganizationId, ConsistencyMode.IgnoreInvalid);
		}

		private static IConfigurationSession CreateScopedADSession(OrganizationId scopingOrganizationId, ConsistencyMode mode = ConsistencyMode.IgnoreInvalid)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(mode, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId), 479, "CreateScopedADSession", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ADNotificationManager.cs");
		}

		private static PolicyData GetPolicySetting(IAirSyncUser user)
		{
			ADObjectId policyId = user.ADUser.ActiveSyncMailboxPolicy;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Loading user PolicySettings data.Policy DN {0}", policyId.DistinguishedName);
			Func<PolicyData> loadDataAction = () => ADNotificationManager.LoadPolicySetting(ADNotificationManager.CreateScopedADSession(user), user, policyId, true);
			ADNotificationManager.ADSettingsInfo<PolicyData> adsettingsInfo;
			ADNotificationManager.LoadADSettingsData<PolicyData>(ADNotificationManager.policies, policyId.DistinguishedName, loadDataAction, user.OrganizationId.PartitionId, out adsettingsInfo);
			return adsettingsInfo.ADSettingsData;
		}

		private static PolicyData GetDefaultPolicySetting(IAirSyncUser user)
		{
			ADObjectId configurationUnit = user.ADUser.ConfigurationUnit;
			if (configurationUnit == null)
			{
				if (ADNotificationManager.enterpriseConfigurationID == null)
				{
					IConfigurationSession configurationSession = ADNotificationManager.CreateScopedADSession(user);
					ADNotificationManager.enterpriseConfigurationID = configurationSession.GetOrgContainerId();
					user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, configurationSession.LastUsedDc);
				}
				configurationUnit = ADNotificationManager.enterpriseConfigurationID;
			}
			Func<PolicyData> loadDataAction = () => ADNotificationManager.LoadDefaultPolicySetting(ADNotificationManager.CreateScopedADSession(user), user.Context.ProtocolLogger);
			ADNotificationManager.ADSettingsInfo<PolicyData> adsettingsInfo;
			bool flag = ADNotificationManager.LoadADSettingsData<PolicyData>(ADNotificationManager.policies, configurationUnit.DistinguishedName, loadDataAction, user.OrganizationId.PartitionId, out adsettingsInfo);
			if (flag && adsettingsInfo.ADSettingsData == null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Default PolicySettings not found. Save null in cache.Policy DN {0}", configurationUnit.DistinguishedName);
				ADNotificationManager.policies.Add(configurationUnit.DistinguishedName, new ADNotificationManager.ADSettingsInfo<PolicyData>(user.OrganizationId.PartitionId, null, ExDateTime.UtcNow));
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Returning Default PolicySettings data. DN {0}", configurationUnit.DistinguishedName);
			return adsettingsInfo.ADSettingsData;
		}

		private static string GetDefaultPolicyKey(ADObjectId policyId)
		{
			return policyId.Parent.Parent.DistinguishedName;
		}

		private static PolicyData AddPolicyToCache(MobileMailboxPolicy mobileMaiboxPolicy, PartitionId partitionId)
		{
			PolicyData policyData = null;
			if (mobileMaiboxPolicy != null)
			{
				policyData = new PolicyData(mobileMaiboxPolicy);
				string defaultPolicyKey = ADNotificationManager.GetDefaultPolicyKey(mobileMaiboxPolicy.Id);
				if (policyData.IsDefault)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Adding policy {0} as default.", mobileMaiboxPolicy.Id.DistinguishedName);
					ADNotificationManager.policies.Add(defaultPolicyKey, new ADNotificationManager.ADSettingsInfo<PolicyData>(partitionId, policyData, ExDateTime.UtcNow));
				}
				else if (ADNotificationManager.IsOrgDefaultPolicyEquals(mobileMaiboxPolicy.Id, defaultPolicyKey))
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Removing policy {0} as default.", mobileMaiboxPolicy.Id.DistinguishedName);
					ADNotificationManager.policies.Remove(defaultPolicyKey);
				}
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Adding policy to location {0}.", mobileMaiboxPolicy.Id.DistinguishedName);
				ADNotificationManager.policies.Add(mobileMaiboxPolicy.Id.DistinguishedName, new ADNotificationManager.ADSettingsInfo<PolicyData>(partitionId, policyData, ExDateTime.UtcNow));
			}
			return policyData;
		}

		private static IOrganizationSettingsData AddOrganizationSettingsToCache(ActiveSyncOrganizationSettings organizationSettings, IConfigurationSession scopedSession)
		{
			IOrganizationSettingsData organizationSettingsData = null;
			if (organizationSettings != null)
			{
				organizationSettingsData = new OrganizationSettingsData(organizationSettings, scopedSession);
				if (organizationSettings.OrganizationId != null && organizationSettings.OrganizationId.ConfigurationUnit != null)
				{
					AirSyncDiagnostics.TraceDebug<OrganizationId>(ExTraceGlobals.RequestsTracer, null, "Adding OrganizationSettings for org: {0}.", organizationSettings.OrganizationId);
					ADNotificationManager.organizationSettingsCache.Add(organizationSettings.OrganizationId.ConfigurationUnit.DistinguishedName, new ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData>(organizationSettings.OrganizationId.PartitionId, organizationSettingsData, ExDateTime.UtcNow));
				}
				else
				{
					if (ADNotificationManager.enterpriseConfigurationID == null)
					{
						ADNotificationManager.enterpriseConfigurationID = scopedSession.GetOrgContainerId();
					}
					AirSyncDiagnostics.TraceDebug<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "Adding OrganizationSettings for enterprise: {0}.", ADNotificationManager.enterpriseConfigurationID);
					ADNotificationManager.organizationSettingsCache.Add(ADNotificationManager.enterpriseConfigurationID.DistinguishedName, new ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData>(null, organizationSettingsData, ExDateTime.UtcNow));
				}
			}
			return organizationSettingsData;
		}

		private static PolicyData LoadPolicySetting(IConfigurationSession scopedSession, IAirSyncUser user, ADObjectId policyId, bool forceLoad)
		{
			MobileMailboxPolicy mobileMailboxPolicy = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				mobileMailboxPolicy = scopedSession.Read<MobileMailboxPolicy>(policyId);
			});
			if (!adoperationResult.Succeeded)
			{
				AirSyncDiagnostics.TraceDebug<ADObjectId, string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during LoadPolicySettings {0}. Exception Message - {1}", policyId, adoperationResult.Exception.Message);
				throw adoperationResult.Exception;
			}
			if (user != null)
			{
				user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, scopedSession.LastUsedDc);
			}
			if (mobileMailboxPolicy != null && (forceLoad || mobileMailboxPolicy.IsDefault))
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, null, "LoadPolicySetting from AD");
				return ADNotificationManager.AddPolicyToCache(mobileMailboxPolicy, scopedSession.SessionSettings.CurrentOrganizationId.PartitionId);
			}
			return null;
		}

		private static IOrganizationSettingsData LoadOrganizationSettings(IConfigurationSession scopedSession, IAirSyncContext context)
		{
			ADObjectId organizationId = scopedSession.GetOrgContainerId();
			ActiveSyncOrganizationSettings organizationSettings = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				organizationSettings = scopedSession.Read<ActiveSyncOrganizationSettings>(organizationId.GetDescendantId(new ADObjectId("CN=Mobile Mailbox Settings")));
			});
			if (!adoperationResult.Succeeded)
			{
				AirSyncDiagnostics.TraceDebug<ADObjectId, string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during LoadOrganizationSettings {0}. Exception Message- {1}", organizationId, adoperationResult.Exception.Message);
			}
			ValidationError[] array = organizationSettings.Validate();
			bool flag = false;
			if (array != null)
			{
				foreach (ValidationError validationError in array)
				{
					if (string.Equals(validationError.PropertyName, "ConfigurationXMLRaw", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				organizationSettings.DeviceFiltering = null;
			}
			ADRawEntry adrawEntry = scopedSession.ReadADRawEntry(organizationId, new PropertyDefinition[]
			{
				OrganizationSchema.IntuneManagedStatus
			});
			organizationSettings.IsIntuneManaged = (adrawEntry != null && (bool)adrawEntry[OrganizationSchema.IntuneManagedStatus]);
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, null, "LoadOrganizationSettings from AD.IntuneManagedStatus {0}.", organizationSettings.IsIntuneManaged);
			if (context != null)
			{
				context.ProtocolLogger.SetValue(ProtocolLoggerData.DomainController, scopedSession.LastUsedDc);
			}
			if (organizationSettings != null)
			{
				AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, null, "LoadOrganizationSettings from AD. Found {0} OrganizationSettings.", 1);
				return ADNotificationManager.AddOrganizationSettingsToCache(organizationSettings, scopedSession);
			}
			AirSyncDiagnostics.TraceError<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "Cannot find ActiveSyncOrganizationSettings object in AD for organization {0}", organizationId);
			return null;
		}

		private static void LoadAutoBlockThresholds()
		{
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = ADNotificationManager.startLock, ref flag);
				DeviceAutoBlockThreshold[] newValues = new DeviceAutoBlockThreshold[Enum.GetValues(typeof(AutoblockThresholdType)).Length];
				bool flag2 = false;
				ActiveSyncDeviceAutoblockThreshold[] thresholds = null;
				try
				{
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						thresholds = ADNotificationManager.topoSession.Find<ActiveSyncDeviceAutoblockThreshold>(ADNotificationManager.topoSession.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=Mobile Mailbox Policies")), QueryScope.OneLevel, null, ADNotificationManager.thresholdSortBy, newValues.Length);
					});
					if (!adoperationResult.Succeeded)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during LoadAutoBlockThresholds. Exception Message -{0}", adoperationResult.Exception.Message);
						throw adoperationResult.Exception;
					}
					foreach (ActiveSyncDeviceAutoblockThreshold activeSyncDeviceAutoblockThreshold in thresholds)
					{
						if (activeSyncDeviceAutoblockThreshold.BehaviorType < (AutoblockThresholdType)newValues.Length)
						{
							newValues[(int)activeSyncDeviceAutoblockThreshold.BehaviorType] = new DeviceAutoBlockThreshold(activeSyncDeviceAutoblockThreshold);
						}
					}
					flag2 = true;
				}
				finally
				{
					if (ADNotificationManager.autoBlockThresholds == null || flag2)
					{
						for (int j = 0; j < newValues.Length; j++)
						{
							if (newValues[j] == null)
							{
								newValues[j] = new DeviceAutoBlockThreshold((AutoblockThresholdType)j);
							}
						}
						ADNotificationManager.autoBlockThresholds = new ADNotificationManager.DeviceAutoblockThresholdInfo(newValues, ExDateTime.UtcNow);
					}
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		private static PolicyData LoadDefaultPolicySetting(IConfigurationSession scopedSession, ProtocolLogger protocolLogger)
		{
			MobileMailboxPolicy[] mobileMaiboxPolicies = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				mobileMaiboxPolicies = scopedSession.Find<MobileMailboxPolicy>(scopedSession.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=Mobile Mailbox Policies")), QueryScope.OneLevel, ADNotificationManager.filter, ADNotificationManager.sortBy, 3);
			});
			if (!adoperationResult.Succeeded)
			{
				AirSyncDiagnostics.TraceDebug<ADObjectId, string>(ExTraceGlobals.RequestsTracer, null, "Exception occurred during AD Operation during LoadDefaultPolicySettings for OrgID {0}. Message - {1}", scopedSession.GetOrgContainerId(), adoperationResult.Exception.Message);
				throw adoperationResult.Exception;
			}
			protocolLogger.SetValue(ProtocolLoggerData.DomainController, scopedSession.LastUsedDc);
			if (mobileMaiboxPolicies == null || mobileMaiboxPolicies.Length == 0)
			{
				AirSyncDiagnostics.TraceInfo<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "no default policy setting found for OrgId {0}", scopedSession.GetOrgContainerId());
				return null;
			}
			AirSyncDiagnostics.TraceInfo<ADObjectId>(ExTraceGlobals.RequestsTracer, null, "LoadDefaultPolicySetting from AD.Policy Id {0}", mobileMaiboxPolicies[0].Id);
			if (mobileMaiboxPolicies.Length > 1)
			{
				ADNotificationManager.HandleMultipleDefaultPolicies(mobileMaiboxPolicies, scopedSession.GetOrgContainerId(), protocolLogger);
				protocolLogger.SetValue(ProtocolLoggerData.DomainController, scopedSession.LastUsedDc);
			}
			return ADNotificationManager.AddPolicyToCache(mobileMaiboxPolicies[0], scopedSession.SessionSettings.CurrentOrganizationId.PartitionId);
		}

		private static bool IsPolicyAlreadyLoaded(ADObjectId policyId)
		{
			ADNotificationManager.ADSettingsInfo<PolicyData> adsettingsInfo;
			return ADNotificationManager.policies.TryGetValue(policyId.DistinguishedName, out adsettingsInfo) || ADNotificationManager.IsOrgDefaultPolicyEquals(policyId, ADNotificationManager.GetDefaultPolicyKey(policyId));
		}

		private static bool LoadADSettingsData<T>(MruDictionaryCache<string, ADNotificationManager.ADSettingsInfo<T>> cachedData, string adObjectDN, Func<T> loadDataAction, PartitionId partitionId, out ADNotificationManager.ADSettingsInfo<T> adSettingDataInfo)
		{
			bool result = false;
			if (GlobalSettings.DisableCaching || !cachedData.TryGetValue(adObjectDN, out adSettingDataInfo) || adSettingDataInfo.WhenCreated.Add(GlobalSettings.ADCacheExpirationTimeout) < ExDateTime.UtcNow)
			{
				lock (ADNotificationManager.lockObject)
				{
					if (GlobalSettings.DisableCaching || !cachedData.TryGetValue(adObjectDN, out adSettingDataInfo) || adSettingDataInfo.WhenCreated.Add(GlobalSettings.ADCacheExpirationTimeout) < ExDateTime.UtcNow)
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Load user settings from AD for DN {0}", adObjectDN);
						T adSettings = loadDataAction();
						adSettingDataInfo = new ADNotificationManager.ADSettingsInfo<T>(partitionId, adSettings, ExDateTime.UtcNow);
						result = true;
					}
					else
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Load user adsettings from cache2 for DN {0}", adObjectDN);
					}
					return result;
				}
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, null, "Load user adsettings from cache for DN {0}", adObjectDN);
			return result;
		}

		private static bool IsOrganizationSettingsAlreadyLoaded(ADObjectId organizationConfigurationUnitId)
		{
			ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData> adsettingsInfo = null;
			return ADNotificationManager.organizationSettingsCache.TryGetValue(organizationConfigurationUnitId.DistinguishedName, out adsettingsInfo);
		}

		private static bool IsOrgDefaultPolicyEquals(ADObjectId policyId, string containerDN)
		{
			ADNotificationManager.ADSettingsInfo<PolicyData> adsettingsInfo;
			return ADNotificationManager.policies.TryGetValue(containerDN, out adsettingsInfo) && adsettingsInfo != null && adsettingsInfo.ADSettingsData != null && adsettingsInfo.ADSettingsData.Identity.Equals(policyId);
		}

		private static bool IsOrgDefaultPolicyLoaded(ADObjectId policyId)
		{
			ADNotificationManager.ADSettingsInfo<PolicyData> adsettingsInfo;
			return ADNotificationManager.policies.TryGetValue(ADNotificationManager.GetDefaultPolicyKey(policyId), out adsettingsInfo);
		}

		private static string GetOriginalDNFromLastKnownParent(ADObjectId deletedObjectId, string lastKnownParentDN)
		{
			string[] separator = new string[]
			{
				"\\0ADEL:" + deletedObjectId.ObjectGuid.ToString()
			};
			return "CN=" + deletedObjectId.Rdn.EscapedName.Split(separator, StringSplitOptions.None)[0] + "," + lastKnownParentDN;
		}

		private const string MobileMailBoxPoliciesDN = "CN=Mobile Mailbox Policies";

		private const string MobileMailBoxSettingsDN = "CN=Mobile Mailbox Settings";

		private static readonly QueryFilter filter = new BitMaskAndFilter(MobileMailboxPolicySchema.MobileFlags, 4096UL);

		private static readonly SortBy sortBy = new SortBy(ADObjectSchema.WhenChanged, SortOrder.Descending);

		private static readonly SortBy thresholdSortBy = new SortBy(ActiveSyncDeviceAutoblockThresholdSchema.BehaviorType, SortOrder.Ascending);

		private static readonly TimeSpan delayTolerance = new TimeSpan(0, 0, 5, 0);

		private static object startLock = new object();

		private static ITopologyConfigurationSession topoSession;

		private static MruDictionaryCache<string, ADNotificationManager.ADSettingsInfo<PolicyData>> policies;

		private static MruDictionaryCache<string, ADNotificationManager.ADSettingsInfo<IOrganizationSettingsData>> organizationSettingsCache;

		private static object lockObject = new object();

		private static ADObjIdToVDirMap vDirSettingsCache;

		private static bool started;

		private static DateTime lastPolicyADNotificationTime = DateTime.UtcNow;

		private static DateTime lastOrganizationSettingsADNotificationTime = DateTime.UtcNow;

		private static DateTime lastDeviceAccessRuleADNotificationTime = DateTime.UtcNow;

		private static ADObjectId enterpriseConfigurationID;

		private static ADNotificationManager.DeviceAutoblockThresholdInfo autoBlockThresholds;

		private class DeviceAutoblockThresholdInfo
		{
			public ExDateTime WhenCreated { get; private set; }

			public DeviceAutoblockThresholdInfo(DeviceAutoBlockThreshold[] autoBlockThresholds, ExDateTime whenCreated)
			{
				this.WhenCreated = whenCreated;
				this.DeviceAutoBlockThresholds = autoBlockThresholds;
			}

			public DeviceAutoBlockThreshold[] DeviceAutoBlockThresholds;
		}

		internal class ADSettingsInfo<T>
		{
			public PartitionId PartitionId { get; private set; }

			public T ADSettingsData { get; private set; }

			public ExDateTime WhenCreated { get; private set; }

			public ADSettingsInfo(PartitionId partitionId, T adSettings, ExDateTime whenCreated)
			{
				this.PartitionId = partitionId;
				this.ADSettingsData = adSettings;
				this.WhenCreated = whenCreated;
			}
		}
	}
}
