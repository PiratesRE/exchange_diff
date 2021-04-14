using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantRelocationStateCache
	{
		private TenantRelocationStateCache()
		{
			this.syncObject = new object();
			this.tenantRelocationStates = new Dictionary<string, ExpiringTenantRelocationStateValue>(StringComparer.OrdinalIgnoreCase);
		}

		private static TenantRelocationStateCache GetInstance()
		{
			if (TenantRelocationStateCache.instance == null)
			{
				TenantRelocationStateCache.instance = new TenantRelocationStateCache();
			}
			return TenantRelocationStateCache.instance;
		}

		internal static void Reset()
		{
			TenantRelocationStateCache.instance = null;
		}

		private static bool IsSourceTenantStateInRange(ADObjectId identity, params TenantRelocationStatus[] states)
		{
			TenantRelocationState tenantRelocationState;
			bool flag;
			if (TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(identity, out tenantRelocationState, out flag) && flag)
			{
				foreach (TenantRelocationStatus tenantRelocationStatus in states)
				{
					if (tenantRelocationState.SourceForestState == tenantRelocationStatus)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsTargetTenantStateInRange(ADObjectId identity, params RelocationStatusDetailsDestination[] states)
		{
			TenantRelocationState tenantRelocationState;
			bool flag;
			if (TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(identity, out tenantRelocationState, out flag) && !flag)
			{
				foreach (RelocationStatusDetailsDestination relocationStatusDetailsDestination in states)
				{
					if (tenantRelocationState.TargetForestState == relocationStatusDetailsDestination)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static ExchangeConfigurationUnit GetExchangeCUAndThrowIfNotFound(string tenantName, ITenantConfigurationSession session)
		{
			session.SessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			ExchangeConfigurationUnit exchangeConfigurationUnitByName = session.GetExchangeConfigurationUnitByName(tenantName);
			if (exchangeConfigurationUnitByName == null)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>(0L, "TenantRelocationStateCache::GetExchangeCUAndThrowIfNotFound(): Couldn't find tenant {0}.", tenantName);
				throw new CannotResolveTenantNameException(DirectoryStrings.ErrorCannotFindTenant(tenantName, session.SessionSettings.PartitionId.ForestFQDN));
			}
			return exchangeConfigurationUnitByName;
		}

		internal static bool TryLoadTenantRelocationStateSourceReplica(string tenantName, PartitionId partitionId, out RelocationStatusDetailsSource status)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 164, "TryLoadTenantRelocationStateSourceReplica", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\TenantRelocationStateCache.cs");
			tenantConfigurationSession.SessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			ExchangeConfigurationUnit exchangeConfigurationUnitByName = tenantConfigurationSession.GetExchangeConfigurationUnitByName(tenantName);
			if (exchangeConfigurationUnitByName != null)
			{
				status = exchangeConfigurationUnitByName.RelocationStatusDetailsSource;
				return true;
			}
			status = RelocationStatusDetailsSource.NotStarted;
			return false;
		}

		private static TenantRelocationState LoadTenantRelocationState(string tenantName, PartitionId partitionId)
		{
			ITenantConfigurationSession session = DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 185, "LoadTenantRelocationState", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\TenantRelocationStateCache.cs");
			ExchangeConfigurationUnit exchangeCUAndThrowIfNotFound = TenantRelocationStateCache.GetExchangeCUAndThrowIfNotFound(tenantName, session);
			if (exchangeCUAndThrowIfNotFound.IsCached)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>(0L, "TenantRelocationStateCache::LoadTenantRelocationState(): Tenant {0} found in cache.", tenantName);
				if ((DateTime.UtcNow - exchangeCUAndThrowIfNotFound.WhenReadUTC).Value.TotalSeconds >= 30.0)
				{
					ITenantConfigurationSession session2 = DirectorySessionFactory.NonCacheSessionFactory.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 203, "LoadTenantRelocationState", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\TenantRelocationStateCache.cs");
					exchangeCUAndThrowIfNotFound = TenantRelocationStateCache.GetExchangeCUAndThrowIfNotFound(tenantName, session2);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.CacheSessionFactory.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 206, "LoadTenantRelocationState", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\TenantRelocationStateCache.cs");
					tenantConfigurationSession.Delete(exchangeCUAndThrowIfNotFound);
					tenantConfigurationSession.Save(exchangeCUAndThrowIfNotFound);
				}
			}
			if ((string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.TargetForest) && string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.RelocationSourceForestRaw)) || (!string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.RelocationSourceForestRaw) && exchangeCUAndThrowIfNotFound.RelocationStateRequested == RelocationStateRequested.Cleanup))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string, bool>(0L, "TenantRelocationStateCache::LoadTenantRelocationState(): Found tenant {0} on DC {1} Cached {2} - not being relocated.", tenantName, exchangeCUAndThrowIfNotFound.OriginatingServer, exchangeCUAndThrowIfNotFound.IsCached);
				return new TenantRelocationState(partitionId.ForestFQDN, TenantRelocationStatus.NotStarted);
			}
			if (string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.ExternalDirectoryOrganizationId))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string, bool>(0L, "TenantRelocationStateCache::LoadTenantRelocationState(): Found tenant {0} on DC {1} Cached {2} - prepared for deletion.", tenantName, exchangeCUAndThrowIfNotFound.OriginatingServer, exchangeCUAndThrowIfNotFound.IsCached);
				TenantRelocationStatus sourceForestState;
				if (!string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.TargetForest))
				{
					sourceForestState = exchangeCUAndThrowIfNotFound.RelocationStatus;
				}
				else
				{
					sourceForestState = TenantRelocationStatus.NotStarted;
				}
				return new TenantRelocationState(partitionId.ForestFQDN, sourceForestState);
			}
			if (!string.IsNullOrEmpty(exchangeCUAndThrowIfNotFound.TargetForest))
			{
				TenantRelocationRequest tenantRelocationRequest;
				Exception ex;
				TenantRelocationRequest.LoadOtherForestObjectInternal(null, exchangeCUAndThrowIfNotFound.TargetForest, exchangeCUAndThrowIfNotFound.DistinguishedName, exchangeCUAndThrowIfNotFound.ExternalDirectoryOrganizationId, true, out tenantRelocationRequest, out ex);
				if (ex != null && !(ex is CannotFindTargetTenantException))
				{
					throw ex;
				}
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>(0L, "TenantRelocationStateCache::LoadTenantRelocationState(): Found tenant {0} on DC {1} - this is relocation source.", tenantName, (tenantRelocationRequest != null) ? tenantRelocationRequest.OriginatingServer : "<unknown>");
				return new TenantRelocationState(partitionId.ForestFQDN, exchangeCUAndThrowIfNotFound.RelocationStatus, exchangeCUAndThrowIfNotFound.TargetForest, (RelocationStatusDetailsDestination)((tenantRelocationRequest != null) ? tenantRelocationRequest.RelocationStatusDetailsRaw : RelocationStatusDetails.NotStarted), exchangeCUAndThrowIfNotFound.OrganizationId, (tenantRelocationRequest != null) ? tenantRelocationRequest.OrganizationId : null);
			}
			else
			{
				Exception ex;
				TenantRelocationRequest tenantRelocationRequest2;
				TenantRelocationRequest.LoadOtherForestObjectInternal(null, exchangeCUAndThrowIfNotFound.RelocationSourceForestRaw, exchangeCUAndThrowIfNotFound.DistinguishedName, exchangeCUAndThrowIfNotFound.ExternalDirectoryOrganizationId, false, out tenantRelocationRequest2, out ex);
				if (ex == null)
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug(0L, string.Format("TenantRelocationStateCache::LoadTenantRelocationState(): Found tenant {0} on DC {1} - this is relocation target.", tenantName, (tenantRelocationRequest2 != null) ? tenantRelocationRequest2.OriginatingServer : "<unknown>"));
					return new TenantRelocationState(exchangeCUAndThrowIfNotFound.RelocationSourceForestRaw, (tenantRelocationRequest2 != null) ? tenantRelocationRequest2.RelocationStatus : TenantRelocationStatus.NotStarted, partitionId.ForestFQDN, (RelocationStatusDetailsDestination)exchangeCUAndThrowIfNotFound.RelocationStatusDetailsSource, (tenantRelocationRequest2 != null) ? tenantRelocationRequest2.OrganizationId : null, exchangeCUAndThrowIfNotFound.OrganizationId);
				}
				if (ex is CannotFindTargetTenantException)
				{
					return new TenantRelocationState(partitionId.ForestFQDN, TenantRelocationStatus.NotStarted);
				}
				throw ex;
			}
		}

		internal static bool IgnoreRelocationTimeConstraints()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > TenantRelocationStateCache.nextRegistryCheckTimestamp)
			{
				TenantRelocationStateCache.ignoreRelocationTimeConstraints = TenantRelocationStateCache.ReadIgnoreRelocationTimeConstraintsValue();
				TenantRelocationStateCache.nextRegistryCheckTimestamp = utcNow.AddMinutes(5.0);
			}
			return TenantRelocationStateCache.ignoreRelocationTimeConstraints;
		}

		private static bool ReadIgnoreRelocationTimeConstraintsValue()
		{
			int intValueFromRegistry = Globals.GetIntValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeLabs", "TenantRelocationIgnoreTimeConstraints", 0, TenantRelocationStateCache.GetInstance().GetHashCode());
			return intValueFromRegistry > 0;
		}

		private void AddOrUpdate(string tenantName, ExpiringTenantRelocationStateValue state)
		{
			lock (this.syncObject)
			{
				if (this.tenantRelocationStates.Count >= 25000)
				{
					int num = this.tenantRelocationStates.Count - 22500;
					List<string> list = new List<string>(num);
					foreach (KeyValuePair<string, ExpiringTenantRelocationStateValue> keyValuePair in this.tenantRelocationStates)
					{
						if (keyValuePair.Value.Expired)
						{
							list.Add(keyValuePair.Key);
						}
					}
					int num2 = num - list.Count;
					if (num2 > 0)
					{
						foreach (KeyValuePair<string, ExpiringTenantRelocationStateValue> keyValuePair2 in this.tenantRelocationStates)
						{
							if (!keyValuePair2.Value.Expired)
							{
								list.Add(keyValuePair2.Key);
								if (--num2 <= 0)
								{
									break;
								}
							}
						}
					}
					foreach (string key in list)
					{
						this.tenantRelocationStates.Remove(key);
					}
				}
				this.tenantRelocationStates[tenantName] = state;
			}
		}

		public static bool IsTenantRetired(ADObjectId identity)
		{
			return TenantRelocationStateCache.IsSourceTenantStateInRange(identity, new TenantRelocationStatus[]
			{
				TenantRelocationStatus.Retired
			});
		}

		public static bool IsTenantLockedDown(ADObjectId identity)
		{
			return TenantRelocationStateCache.IsSourceTenantStateInRange(identity, new TenantRelocationStatus[]
			{
				TenantRelocationStatus.Lockdown
			});
		}

		public static bool IsTenantVolatile(ADObjectId identity)
		{
			return TenantRelocationStateCache.IsSourceTenantStateInRange(identity, new TenantRelocationStatus[]
			{
				TenantRelocationStatus.Synchronization,
				TenantRelocationStatus.Lockdown,
				TenantRelocationStatus.Retired
			});
		}

		public static bool IsTenantArriving(ADObjectId identity)
		{
			TenantRelocationState tenantRelocationState;
			bool flag;
			return TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(identity, out tenantRelocationState, out flag) && !flag && tenantRelocationState.TargetForestState <= RelocationStatusDetailsDestination.Arriving && tenantRelocationState.SourceForestState != TenantRelocationStatus.Retired;
		}

		public static TimeSpan GetTenantCacheExpirationWindow(string tenantName, PartitionId partitionId)
		{
			TenantRelocationState tenantRelocationState = null;
			try
			{
				bool flag;
				tenantRelocationState = TenantRelocationStateCache.GetTenantRelocationState(tenantName, partitionId, out flag, false);
			}
			catch (CannotResolveTenantNameException)
			{
			}
			if (tenantRelocationState != null)
			{
				return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.GetExpirationWindow(tenantRelocationState);
			}
			return ExpiringTenantRelocationStateValue.TenantRelocationStateExpirationWindowProvider.DefaultExpirationWindow;
		}

		public static TenantRelocationState GetTenantRelocationState(string tenantName, PartitionId partitionId, out bool isSourceTenant, bool readThrough = false)
		{
			if (string.IsNullOrEmpty(tenantName))
			{
				throw new ArgumentNullException("tenantName");
			}
			if (!ForestTenantRelocationsCache.IsTenantRelocationAllowed(partitionId.ForestFQDN))
			{
				isSourceTenant = true;
				return new TenantRelocationState(partitionId.ForestFQDN, TenantRelocationStatus.NotStarted);
			}
			TenantRelocationStateCache tenantRelocationStateCache = TenantRelocationStateCache.GetInstance();
			ExpiringTenantRelocationStateValue expiringTenantRelocationStateValue;
			if (!tenantRelocationStateCache.tenantRelocationStates.TryGetValue(tenantName, out expiringTenantRelocationStateValue) || expiringTenantRelocationStateValue.Expired || readThrough || TenantRelocationStateCache.IgnoreRelocationTimeConstraints())
			{
				TenantRelocationState tenantRelocationState = TenantRelocationStateCache.LoadTenantRelocationState(tenantName, partitionId);
				ExTraceGlobals.TenantRelocationTracer.TraceDebug(0L, "TenantRelocationStateCache::GetTenantRelocationState(): Inserting new TenantRelocationState value for tenant {0} to the cache: SourceForestFQDN={1}, SourceForestState={2}, TargetForestFQDN={3}, TargetForestState={4}, TargetOrganizationId={5}", new object[]
				{
					tenantName,
					tenantRelocationState.SourceForestFQDN,
					tenantRelocationState.SourceForestState,
					tenantRelocationState.TargetForestFQDN,
					tenantRelocationState.TargetForestState,
					(tenantRelocationState.TargetOrganizationId != null) ? tenantRelocationState.TargetOrganizationId.ToString() : "<null>"
				});
				expiringTenantRelocationStateValue = new ExpiringTenantRelocationStateValue(tenantRelocationState);
				tenantRelocationStateCache.AddOrUpdate(tenantName, expiringTenantRelocationStateValue);
			}
			if (expiringTenantRelocationStateValue.Value.TargetOrganizationId == null || expiringTenantRelocationStateValue.Value.TargetOrganizationId.OrganizationalUnit == null)
			{
				isSourceTenant = true;
			}
			else if (expiringTenantRelocationStateValue.Value.TargetOrganizationId.PartitionId != partitionId)
			{
				isSourceTenant = true;
			}
			else if (expiringTenantRelocationStateValue.Value.SourceForestFQDN == expiringTenantRelocationStateValue.Value.TargetForestFQDN)
			{
				isSourceTenant = (tenantName != expiringTenantRelocationStateValue.Value.TargetOrganizationId.OrganizationalUnit.Name);
			}
			else
			{
				isSourceTenant = false;
			}
			return expiringTenantRelocationStateValue.Value;
		}

		public static bool TryGetTenantRelocationStateByObjectId(ADObjectId identity, out TenantRelocationState state, out bool isSourceTenant)
		{
			ArgumentValidator.ThrowIfNull("identity", identity);
			state = null;
			isSourceTenant = true;
			PartitionId partitionId = identity.GetPartitionId();
			if (!ForestTenantRelocationsCache.IsTenantRelocationAllowed(partitionId.ForestFQDN))
			{
				return false;
			}
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 595, "TryGetTenantRelocationStateByObjectId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\RelocationCache\\TenantRelocationStateCache.cs");
			ADObjectId configurationUnitsRoot = tenantConfigurationSession.GetConfigurationUnitsRoot();
			if (configurationUnitsRoot.Equals(identity))
			{
				return false;
			}
			ADObjectId adobjectId = null;
			if (identity.IsDescendantOf(configurationUnitsRoot))
			{
				adobjectId = identity.GetFirstGenerationDecendantOf(configurationUnitsRoot);
			}
			else
			{
				ADObjectId hostedOrganizationsRoot = tenantConfigurationSession.GetHostedOrganizationsRoot();
				if (hostedOrganizationsRoot.Equals(identity))
				{
					return false;
				}
				if (identity.IsDescendantOf(hostedOrganizationsRoot))
				{
					adobjectId = identity.GetFirstGenerationDecendantOf(hostedOrganizationsRoot);
				}
			}
			if (adobjectId == null)
			{
				return false;
			}
			try
			{
				state = TenantRelocationStateCache.GetTenantRelocationState(adobjectId.Name, partitionId, out isSourceTenant, false);
			}
			catch (CannotResolveTenantNameException)
			{
				return false;
			}
			return true;
		}

		internal const int Capacity = 25000;

		internal const int SoftCapacityThreshold = 22500;

		private static TenantRelocationStateCache instance;

		private static bool ignoreRelocationTimeConstraints;

		private static DateTime nextRegistryCheckTimestamp = DateTime.MinValue;

		private object syncObject;

		private Dictionary<string, ExpiringTenantRelocationStateValue> tenantRelocationStates;
	}
}
