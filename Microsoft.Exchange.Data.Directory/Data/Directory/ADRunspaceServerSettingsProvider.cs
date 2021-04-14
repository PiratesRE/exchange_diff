using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADRunspaceServerSettingsProvider
	{
		internal ADRunspaceServerSettingsProvider()
		{
		}

		public void ReportServerDown(string partitionFqdn, string serverFqdn, ADServerRole role)
		{
			SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider> syncWrapper;
			if (this.internalProviders.TryGetValue(partitionFqdn, out syncWrapper) && syncWrapper.Value != null)
			{
				syncWrapper.Value.ReportServerDown(serverFqdn, role);
			}
		}

		public ADServerInfo GetConfigDc(string forestFqdn)
		{
			PartitionBasedADRunspaceServerSettingsProvider providerForPartition = this.GetProviderForPartition(forestFqdn);
			if (providerForPartition != null)
			{
				return providerForPartition.ConfigDC;
			}
			return null;
		}

		internal bool IsServerKnown(Fqdn serverFqdn)
		{
			foreach (SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider> syncWrapper in this.internalProviders.Values)
			{
				if (syncWrapper.Value != null && syncWrapper.Value.IsServerKnown(serverFqdn))
				{
					return true;
				}
			}
			return false;
		}

		internal ADServerInfo GetGcFromToken(string partitionFqdn, string token, out bool isInLocalSite, bool forestWideAffinityRequested = false)
		{
			PartitionBasedADRunspaceServerSettingsProvider providerForPartition = this.GetProviderForPartition(partitionFqdn);
			if (providerForPartition != null)
			{
				ADServerInfo gcFromToken = providerForPartition.GetGcFromToken(token, out isInLocalSite, forestWideAffinityRequested);
				if (gcFromToken != null)
				{
					return gcFromToken;
				}
			}
			if (Globals.IsDatacenter)
			{
				throw new ADTransientException(DirectoryStrings.ExceptionADTopologyNoServersForPartition(partitionFqdn));
			}
			throw new ADTransientException(DirectoryStrings.ExceptionADTopologyHasNoAvailableServersInForest(partitionFqdn));
		}

		internal ADServerInfo[] GetGcFromToken(string partitionFqdn, string token, int serverCountRequested, out bool isInLocalSite, bool forestWideAffinityRequested = false)
		{
			PartitionBasedADRunspaceServerSettingsProvider providerForPartition = this.GetProviderForPartition(partitionFqdn);
			if (providerForPartition != null)
			{
				ADServerInfo[] gcFromToken = providerForPartition.GetGcFromToken(token, serverCountRequested, out isInLocalSite, forestWideAffinityRequested);
				if (gcFromToken != null)
				{
					return gcFromToken;
				}
			}
			if (Globals.IsDatacenter)
			{
				throw new ADTransientException(DirectoryStrings.ExceptionADTopologyNoServersForPartition(partitionFqdn));
			}
			throw new ADTransientException(DirectoryStrings.ExceptionADTopologyHasNoAvailableServersInForest(partitionFqdn));
		}

		internal static ADRunspaceServerSettingsProvider GetInstance()
		{
			if (ADRunspaceServerSettingsProvider.staticInstance == null)
			{
				ADRunspaceServerSettingsProvider value = new ADRunspaceServerSettingsProvider();
				Interlocked.CompareExchange<ADRunspaceServerSettingsProvider>(ref ADRunspaceServerSettingsProvider.staticInstance, value, null);
			}
			return ADRunspaceServerSettingsProvider.staticInstance;
		}

		private PartitionBasedADRunspaceServerSettingsProvider GetProviderForPartition(string partitionFqdn)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			TopologyProvider instance = TopologyProvider.GetInstance();
			SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider> orAdd = this.internalProviders.GetOrAdd(partitionFqdn, (string key) => new SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider>());
			PartitionBasedADRunspaceServerSettingsProvider value = orAdd.Value;
			if (value != null && Globals.GetTickDifference(value.LastTopoRecheck, Environment.TickCount) < (ulong)((long)instance.TopoRecheckIntervalMsec))
			{
				ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "ADRunspaceServerSettingsProvider {0} ignoring topology version recheck for partition.", partitionFqdn);
				return value;
			}
			lock (orAdd)
			{
				value = orAdd.Value;
				if (value == null || Globals.GetTickDifference(value.LastTopoRecheck, Environment.TickCount) > (ulong)((long)TopologyProvider.GetInstance().TopoRecheckIntervalMsec))
				{
					int topologyVersion = instance.GetTopologyVersion(partitionFqdn);
					if (value == null || value.TopologyVersion != topologyVersion)
					{
						ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ADRunspaceServerSettingsProvider {0}. Creating new provider version {1}", partitionFqdn, topologyVersion);
						PartitionBasedADRunspaceServerSettingsProvider value2;
						if (PartitionBasedADRunspaceServerSettingsProvider.TryCreateNew(partitionFqdn, instance, value, out value2))
						{
							orAdd.Value = value2;
							orAdd.Value.TopologyVersion = topologyVersion;
							orAdd.Value.LastTopoRecheck = Environment.TickCount;
						}
					}
				}
			}
			return orAdd.Value;
		}

		[Conditional("DEBUG")]
		private void DbgCheckConstructorCaller()
		{
			string text = Environment.StackTrace.ToString();
			text.Contains("ADRunspaceServerSettingsProvider.GetInstance()");
		}

		private static ADRunspaceServerSettingsProvider staticInstance;

		private ConcurrentDictionary<string, SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider>> internalProviders = new ConcurrentDictionary<string, SyncWrapper<PartitionBasedADRunspaceServerSettingsProvider>>(StringComparer.OrdinalIgnoreCase);

		[Serializable]
		internal class ADServerInfoState : IComparable
		{
			public ADServerInfoState(ADServerInfo serverInfo) : this(serverInfo, false)
			{
			}

			public ADServerInfoState(ADServerInfo serverInfo, bool isLazySite)
			{
				this.serverInfo = serverInfo;
				this.isDown = false;
				this.isLazySite = isLazySite;
				if (isLazySite)
				{
					this.isInLocalSite = false;
					return;
				}
				this.isInLocalSite = ADRunspaceServerSettingsProvider.ADServerInfoState.IsDcInLocalSite(serverInfo);
			}

			public ADServerInfo ServerInfo
			{
				get
				{
					return this.serverInfo;
				}
			}

			public bool IsDown
			{
				get
				{
					return this.isDown;
				}
				set
				{
					this.isDown = value;
				}
			}

			public bool IsInLocalSite
			{
				get
				{
					if (this.isLazySite)
					{
						this.isInLocalSite = ADRunspaceServerSettingsProvider.ADServerInfoState.IsDcInLocalSite(this.serverInfo);
						this.isLazySite = false;
					}
					return this.isInLocalSite;
				}
			}

			private static bool IsDcInLocalSite(ADServerInfo serverInfo)
			{
				bool flag = false;
				if (!string.IsNullOrEmpty(serverInfo.PartitionFqdn) && !PartitionId.IsLocalForestPartition(serverInfo.PartitionFqdn))
				{
					ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string, string>((long)serverInfo.GetHashCode(), "Server {0} is in forest {1} which is not local forest, return IsDcInLocalSite == false", serverInfo.Fqdn, serverInfo.PartitionFqdn);
					return false;
				}
				if (ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName == null)
				{
					try
					{
						ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName = NativeHelpers.GetSiteName(false);
						ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string>((long)serverInfo.GetHashCode(), "Setting the local site name to {0}", ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName);
					}
					catch (CannotGetSiteInfoException)
					{
						ExTraceGlobals.ServerSettingsProviderTracer.TraceError((long)serverInfo.GetHashCode(), "GetSiteName has thrown a CannotGetSiteInfoException");
					}
				}
				if (ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName != null)
				{
					if (Globals.IsMicrosoftHostedOnly && !string.IsNullOrEmpty(serverInfo.SiteName))
					{
						flag = string.Equals(serverInfo.SiteName, ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName, StringComparison.OrdinalIgnoreCase);
						ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug((long)serverInfo.GetHashCode(), "local site is: {0}, server {1} is in site {2}, return IsDcInLocalSite == {3}", new object[]
						{
							ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName,
							serverInfo.Fqdn,
							serverInfo.SiteName,
							flag
						});
						return flag;
					}
					try
					{
						StringCollection dcSiteCoverage = NativeHelpers.GetDcSiteCoverage(serverInfo.Fqdn);
						flag = dcSiteCoverage.Contains(ADRunspaceServerSettingsProvider.ADServerInfoState.localSiteName);
						ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string, string>((long)serverInfo.GetHashCode(), "Server {0} {1} in the local site", serverInfo.Fqdn, flag ? "is" : "is not");
					}
					catch (CannotGetSiteInfoException)
					{
						ExTraceGlobals.ServerSettingsProviderTracer.TraceError((long)serverInfo.GetHashCode(), "GetDcSiteCoverage has thrown a CannotGetSiteInfoException");
					}
				}
				return flag;
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				ADRunspaceServerSettingsProvider.ADServerInfoState adserverInfoState = obj as ADRunspaceServerSettingsProvider.ADServerInfoState;
				if (adserverInfoState == null)
				{
					throw new ArgumentException("obj is not of the required type");
				}
				return string.Compare(this.ServerInfo.Fqdn, adserverInfoState.ServerInfo.Fqdn, StringComparison.OrdinalIgnoreCase);
			}

			private static string localSiteName;

			private bool isLazySite;

			private ADServerInfo serverInfo;

			private bool isDown;

			private bool isInLocalSite;
		}
	}
}
