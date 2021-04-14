using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.BackSync;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncConfiguration
	{
		protected SyncConfiguration(Guid invocationId, OutputResultDelegate writeResult, ISyncEventLogger eventLogger, IExcludedObjectReporter excludedObjectReporter)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New SyncConfiguration");
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "invocationId {0}", invocationId);
			this.WriteResult = writeResult;
			this.EventLogger = eventLogger;
			this.ExcludedObjectReporter = excludedObjectReporter;
			this.errorSyncObjects = new Dictionary<SyncObject, Exception>();
			this.InvocationId = invocationId;
		}

		public abstract bool MoreData { get; }

		internal static TimeSpan FailoverTimeout { get; private set; } = TimeSpan.FromSeconds((double)SyncConfiguration.GetConfigurationValue<int>("FailoverTimeout", 60));

		internal Guid InvocationId { get; private set; }

		internal static int TraceId
		{
			get
			{
				return Environment.CurrentManagedThreadId;
			}
		}

		private protected OutputResultDelegate WriteResult { protected get; private set; }

		public IExcludedObjectReporter ExcludedObjectReporter { get; private set; }

		private protected ISyncEventLogger EventLogger { protected get; private set; }

		internal ITenantRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
			private set
			{
				this.recipientSession = value;
			}
		}

		internal ITopologyConfigurationSession RootOrgConfigurationSession
		{
			get
			{
				return this.rootOrgConfigurationSession;
			}
			private set
			{
				this.rootOrgConfigurationSession = value;
			}
		}

		internal ITenantConfigurationSession TenantConfigurationSession
		{
			get
			{
				return this.tenantConfigurationSession;
			}
			private set
			{
				this.tenantConfigurationSession = value;
			}
		}

		internal Dictionary<SyncObject, Exception> ErrorSyncObjects
		{
			get
			{
				return this.errorSyncObjects;
			}
		}

		internal static bool SkipSchemaValidation
		{
			get
			{
				return SyncConfiguration.GetConfigurationValue<int>("SkipOutputSchemaValidation", 0) == 1;
			}
		}

		public abstract IEnumerable<ADRawEntry> GetDataPage();

		public abstract byte[] GetResultCookie();

		public bool IsKnownException(Exception exception)
		{
			return exception is InvalidCookieException;
		}

		public abstract Exception HandleException(Exception e);

		public virtual void CheckIfConnectionAllowed()
		{
		}

		internal static bool InlcudeLinks(BackSyncOptions backSyncOptions)
		{
			return (backSyncOptions & BackSyncOptions.IncludeLinks) == BackSyncOptions.IncludeLinks;
		}

		internal static WatermarkMap GetReplicationCursors(ITopologyConfigurationSession configSession)
		{
			return SyncConfiguration.GetReplicationCursors(configSession, false, false);
		}

		internal static WatermarkMap GetReplicationCursors(ITopologyConfigurationSession configSession, bool useConfigNC = false, bool includeRetiredDCs = false)
		{
			WatermarkMap empty = WatermarkMap.Empty;
			bool useConfigNC2 = configSession.UseConfigNC;
			ADObjectId id = useConfigNC ? configSession.GetConfigurationNamingContext() : configSession.GetDomainNamingContext();
			try
			{
				configSession.UseConfigNC = useConfigNC;
				MultiValuedProperty<ReplicationCursor> multiValuedProperty = configSession.ReadReplicationCursors(id);
				foreach (ReplicationCursor replicationCursor in multiValuedProperty)
				{
					if (includeRetiredDCs || replicationCursor.SourceDsa != null)
					{
						empty.Add(replicationCursor.SourceInvocationId, replicationCursor.UpToDatenessUsn);
					}
				}
			}
			finally
			{
				configSession.UseConfigNC = useConfigNC2;
			}
			return empty;
		}

		internal static string FindDomainControllerByInvocationId(Guid dcInvocationId, PartitionId partitionId)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "FindDomainControllerByInvocationIddcInvocationId {0}", dcInvocationId);
			ADTransientException exceptionOnDcNotFound = new ADTransientException(DirectoryStrings.ErrorDCNotFound(string.Format("InvocationId: {0}", dcInvocationId)));
			string text = SyncConfiguration.FindDomainControllerByInvocationId(dcInvocationId, exceptionOnDcNotFound, partitionId);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "FindDomainControllerByInvocationId dcFqdn {0}", text);
			return text;
		}

		internal static string FindDomainControllerByInvocationId(Guid dcInvocationId, Exception exceptionOnDcNotFound, PartitionId partitionId)
		{
			string result = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 467, "FindDomainControllerByInvocationId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\Configuration\\SyncConfiguration.cs");
			ADServer adserver = topologyConfigurationSession.FindDCByInvocationId(dcInvocationId);
			if (adserver != null)
			{
				result = adserver.DnsHostName;
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "adServer.DnsHostName{0}", adserver.DnsHostName);
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceError<Guid>((long)SyncConfiguration.TraceId, "Unable to find a DC for the invocation id {0}", dcInvocationId);
				if (exceptionOnDcNotFound != null)
				{
					throw exceptionOnDcNotFound;
				}
			}
			return result;
		}

		internal static Guid GetPreferredDCWithContainerizedUsnChanged(string serviceInstanceName)
		{
			if (!SyncConfiguration.EnableContainerizedUsnChangedOptimization())
			{
				throw new InvalidOperationException("SyncConfiguration.EnableContainerizedUsnChangedOptimization() == false");
			}
			if (serviceInstanceName == null)
			{
				throw new ArgumentNullException("serviceInstanceName");
			}
			string text = serviceInstanceName.ToLowerInvariant() + "_PreferredContainerizedUsnChangedDC";
			string configurationValue = SyncConfiguration.GetConfigurationValue<string>(text, null);
			if (configurationValue != null)
			{
				List<Guid> list = new List<Guid>();
				foreach (string text2 in configurationValue.Split(new char[]
				{
					','
				}))
				{
					Guid item;
					if (Guid.TryParse(text2, out item))
					{
						list.Add(item);
						ExTraceGlobals.BackSyncTracer.TraceDebug<string, string>((long)SyncConfiguration.TraceId, "GetPreferredDCWithContainerizedUsnChanged Adding a valid DC invocation id: {0} for service instance {1}.", text2, serviceInstanceName);
					}
					else
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<string, string>((long)SyncConfiguration.TraceId, "GetPreferredDCWithContainerizedUsnChanged Invalid DC invocation id: {0} defined for service instance {1}. Skipping it.", text2, serviceInstanceName);
					}
				}
				if (list.Count > 0)
				{
					Random random = new Random();
					return list.ElementAt(random.Next(list.Count));
				}
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetPreferredDCWithContainerizedUsnChanged - Found no valid DC invocation ids for service instance: {0}", serviceInstanceName);
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetPreferredDCWithContainerizedUsnChanged - RegistryValue: {0} is null", text);
			}
			return Guid.Empty;
		}

		internal static T GetConfigurationValue<T>(string registryValueName, T defaultValue)
		{
			T result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackSync"))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Open registry key {0}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackSync");
				if (registryKey != null)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Read value from registry name {0}", registryValueName);
					result = (T)((object)registryKey.GetValue(registryValueName, defaultValue));
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		internal void SetConfiguration(ITopologyConfigurationSession rootOrgConfigurationSession, ITenantConfigurationSession tenantSystemConfigurationSession, ITenantRecipientSession adRecipientSession)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "SyncConfiguration SetConfiguration ...");
			this.RootOrgConfigurationSession = rootOrgConfigurationSession;
			this.TenantConfigurationSession = tenantSystemConfigurationSession;
			this.RecipientSession = adRecipientSession;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "this.RootOrgConfigurationSession.DomainController {0}", this.RootOrgConfigurationSession.DomainController);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "this.TenantConfigurationSession.DomainController {0}", this.TenantConfigurationSession.DomainController);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "this.RecipientSession.DomainController {0}", this.RecipientSession.DomainController);
		}

		public virtual Result<ADRawEntry>[] GetProperties(ADObjectId[] objectIds, PropertyDefinition[] properties)
		{
			if (ExTraceGlobals.BackSyncTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetProperties objectIds {0}", string.Join(";", this.GetDistinguishedNames(objectIds)));
			}
			return this.RecipientSession.ReadMultipleWithDeletedObjects(objectIds, properties);
		}

		public virtual Result<ADRawEntry>[] GetOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties)
		{
			if (ExTraceGlobals.BackSyncTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "GetOrganizationProperties objectIds {0}", string.Join(";", this.GetDistinguishedNames(organizationOUIds)));
			}
			return this.TenantConfigurationSession.ReadMultipleOrganizationProperties(organizationOUIds, properties);
		}

		internal void AddErrorSyncObject(SyncObject errorObject, Exception errorDetail)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "SyncConfiguration.AddErrorSyncObject entering");
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.AddErrorSyncObject errorObject {0}", errorObject.ObjectId);
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.AddErrorSyncObject errorDetail {0}", errorDetail.ToString());
			bool flag = false;
			string objectId = errorObject.ObjectId;
			foreach (SyncObject syncObject in this.ErrorSyncObjects.Keys)
			{
				if (!string.IsNullOrEmpty(syncObject.ObjectId) && syncObject.ObjectId.Equals(objectId, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.AddErrorSyncObject already exists {0} in ErrorSyncObjects", objectId);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.AddErrorSyncObject add {0} to ErrorSyncObjects", objectId);
				this.ErrorSyncObjects.Add(errorObject, errorDetail);
			}
		}

		protected static Guid FindInvocationIdByFqdn(string dcFqdn, PartitionId partitionId)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "FindInvocationIdByFqdn dcFqdn {0}", dcFqdn);
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId), 678, "FindInvocationIdByFqdn", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\BackSync\\Configuration\\SyncConfiguration.cs");
			Guid invocationIdByFqdn = topologyConfigurationSession.GetInvocationIdByFqdn(dcFqdn);
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "FindInvocationIdByFqdn invocationId {0}", invocationIdByFqdn);
			return invocationIdByFqdn;
		}

		protected void UpdateSyncCookieErrorObjectsAndFailureCounts(ISyncCookie syncCookie)
		{
			try
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts this.ErrorSyncObjects.Count = {0}", this.ErrorSyncObjects.Count);
				foreach (KeyValuePair<SyncObject, Exception> keyValuePair in this.ErrorSyncObjects)
				{
					SyncObject key = keyValuePair.Key;
					string objectId = key.ObjectId;
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts objectId {0}", objectId);
					Exception value = keyValuePair.Value;
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts errorDetail {0}", value.ToString());
					int num2;
					if (syncCookie.ErrorObjectsAndFailureCounts.ContainsKey(objectId))
					{
						int num = syncCookie.ErrorObjectsAndFailureCounts[objectId];
						ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts previous error count = {0}", num);
						num2 = num + 1;
						syncCookie.ErrorObjectsAndFailureCounts[objectId] = num2;
						ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts new error count = {0}", num2);
					}
					else
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts add new error object to backsync cookie");
						num2 = 1;
						syncCookie.ErrorObjectsAndFailureCounts.Add(objectId, num2);
					}
					if (num2 >= 3)
					{
						ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts mark exclude-flag on object {0}", objectId);
						ADRecipient adrecipient = this.RecipientSession.Read(key.Id);
						if (adrecipient != null)
						{
							adrecipient[ADRecipientSchema.ExcludedFromBackSync] = true;
							this.RecipientSession.Save(adrecipient);
							if (this.EventLogger != null)
							{
								this.EventLogger.LogSerializationFailedEvent(objectId, num2);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "SyncConfiguration.UpdateSyncCookieErrorObjectsAndFailureCounts exception {0}", ex.ToString());
			}
		}

		protected abstract DateTime GetLastReadFailureStartTime();

		protected abstract DateTime GetSyncSequenceStartTime();

		protected abstract bool IsDCFailoverResilienceEnabled();

		protected bool IsTransientException(Exception e)
		{
			if (e is DataSourceTransientException)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "DataSourceTransientException is transient");
				return true;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Non-transient exception {0}", e.ToString());
			return false;
		}

		protected bool IsSubsequentFailedAttempt()
		{
			bool flag = this.GetLastReadFailureStartTime() != DateTime.MinValue;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "isSubsequentFailedAttempt {0}", flag);
			return flag;
		}

		protected bool IsFailoverTimeoutExceeded(DateTime now)
		{
			if (this.IsDCFailoverResilienceEnabled() && this.GetSyncSequenceStartTime() != DateTime.MinValue)
			{
				return this.ShouldSmartFailover(now);
			}
			bool flag = now.Subtract(this.GetLastReadFailureStartTime()) > SyncConfiguration.FailoverTimeout;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool, bool, DateTime>((long)SyncConfiguration.TraceId, "isFailoverTimeoutExceeded: {0} EnableDCFailoverResilienceForIncrementalSync: {1} SyncSequenceStartTime: {2}", flag, SyncConfiguration.EnableDCFailoverResilienceForIncrementalSync(), this.GetSyncSequenceStartTime());
			return flag;
		}

		private bool ShouldSmartFailover(DateTime now)
		{
			TimeSpan timeSpan = this.GetLastReadFailureStartTime().Subtract(this.GetSyncSequenceStartTime());
			bool flag = now.Subtract(this.GetLastReadFailureStartTime()).TotalSeconds > timeSpan.TotalSeconds * (double)SyncConfiguration.FailoverWaitTimeFactor() / 100.0;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool, DateTime, DateTime>((long)SyncConfiguration.TraceId, "shouldSmartFailover: {0} FailureStartTime: {1} SyncSequenceStartTime: {2}", flag, this.GetLastReadFailureStartTime(), this.GetSyncSequenceStartTime());
			return flag;
		}

		private string[] GetDistinguishedNames(ADObjectId[] objectIds)
		{
			string[] array = new string[objectIds.Length];
			for (int i = 0; i < objectIds.Length; i++)
			{
				array[i] = objectIds[i].DistinguishedName;
			}
			return array;
		}

		public const string BackSyncSettingsOverrideRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackSync";

		internal const int MAX_ERROR_OBJECT_RETRY_COUNT = 3;

		private const string FailoverTimeoutValueName = "FailoverTimeout";

		private const int DefaultFailoverTimeout = 60;

		private ITenantRecipientSession recipientSession;

		private ITopologyConfigurationSession rootOrgConfigurationSession;

		private ITenantConfigurationSession tenantConfigurationSession;

		private Dictionary<SyncObject, Exception> errorSyncObjects;

		internal static readonly Func<bool> EnableIgnoreCookieDCDuringTenantFaultin = () => SyncConfiguration.GetConfigurationValue<int>("EnableIgnoreCookieDCDuringTenantFaultin", 0) == 1;

		internal static readonly Func<long> DirSyncBasedTenantFullSyncThreshold = () => SyncConfiguration.GetConfigurationValue<long>("DirSyncBasedTenantFullSyncThreshold", -1L);

		internal static readonly Func<bool> EnableDCFailoverResilienceForIncrementalSync = () => SyncConfiguration.GetConfigurationValue<int>("EnableDCFailoverResilienceForIncrementalSync", 0) == 1;

		internal static readonly Func<bool> EnableDCFailoverResilienceForTenantFullSync = () => SyncConfiguration.GetConfigurationValue<int>("EnableDCFailoverResilienceForTenantFullSync", 0) == 1;

		internal static readonly Func<int> FailoverWaitTimeFactor = () => SyncConfiguration.GetConfigurationValue<int>("FailoverWaitTimeFactor", 25);

		internal static readonly Func<bool> EnableContainerizedUsnChangedOptimization = () => SyncConfiguration.GetConfigurationValue<int>("EnableContainerizedUsnChangedOptimization", 0) == 1;

		internal static readonly Func<bool> EnableSyncingBackCloudLinks = () => SyncConfiguration.GetConfigurationValue<int>("EnableSyncingBackCloudLinks", 1) == 1;

		internal static readonly Func<bool> EnableCloudPublicDelegatesRecipientFiltering = () => SyncConfiguration.GetConfigurationValue<int>("EnableCloudPublicDelegatesRecipientFiltering", 0) == 1;
	}
}
