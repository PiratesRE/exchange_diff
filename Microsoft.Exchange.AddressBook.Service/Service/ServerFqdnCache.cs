using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class ServerFqdnCache
	{
		public static void InitializeCache()
		{
			if (ServerFqdnCache.cacheTimer == null)
			{
				lock (ServerFqdnCache.cacheTimerLock)
				{
					if (ServerFqdnCache.cacheTimer == null)
					{
						ServerFqdnCache.cacheTimer = new Timer(new TimerCallback(ServerFqdnCache.ReloadCache), null, ServerFqdnCache.StartImmediately, ServerFqdnCache.ReloadInterval);
					}
				}
			}
		}

		public static void TerminateCache()
		{
			if (ServerFqdnCache.cacheTimer != null)
			{
				lock (ServerFqdnCache.cacheTimerLock)
				{
					if (ServerFqdnCache.cacheTimer != null)
					{
						ServerFqdnCache.cacheTimer.Dispose();
						ServerFqdnCache.cacheTimer = null;
					}
				}
			}
		}

		public static RfriStatus LookupFQDNByLegacyDN(string legacyDN, out string serverFQDN)
		{
			Dictionary<string, string> dictionary = ServerFqdnCache.serverFqdnDictionary;
			if (dictionary == null)
			{
				serverFQDN = null;
				return RfriStatus.GeneralFailure;
			}
			if (!dictionary.TryGetValue(legacyDN ?? "//default//", out serverFQDN))
			{
				serverFQDN = null;
				return RfriStatus.NoSuchObject;
			}
			return RfriStatus.Success;
		}

		private static void ReloadCache(object state)
		{
			ServerFqdnCache.ReferralTracer.TraceDebug(0L, "Reloading RFR server FQDN cache");
			if (Interlocked.CompareExchange(ref ServerFqdnCache.reloadCacheInProgress, 1, 0) == 1)
			{
				ServerFqdnCache.ReferralTracer.TraceDebug(0L, "Reload already running on another thread, skipping.");
				return;
			}
			ComparisonFilter comparisonFilter = null;
			try
			{
				Dictionary<string, string> dictionary = null;
				try
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 186, "ReloadCache", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\ServerFqdnCache.cs");
					topologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
					if (DateTime.UtcNow - ServerFqdnCache.lastFullSync < ServerFqdnCache.FullSyncInterval && ServerFqdnCache.lastDeltaSync != DateTime.MinValue)
					{
						comparisonFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.WhenChanged, ServerFqdnCache.lastDeltaSync - ServerFqdnCache.ReplicationDelayTolerance);
					}
					else
					{
						ServerFqdnCache.lastFullSync = DateTime.UtcNow;
						ServerFqdnCache.lastDeltaSync = DateTime.MinValue;
					}
					dictionary = ((ServerFqdnCache.serverFqdnDictionary == null || comparisonFilter == null) ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(ServerFqdnCache.serverFqdnDictionary, StringComparer.OrdinalIgnoreCase));
					IEnumerable<Server> enumerable = topologyConfigurationSession.FindPaged<Server>(null, QueryScope.SubTree, comparisonFilter, null, 0);
					DateTime dateTime = DateTime.MinValue;
					foreach (Server server in enumerable)
					{
						if (server.WhenChanged != null && server.WhenChanged > dateTime)
						{
							dateTime = server.WhenChanged.Value;
						}
						if (!string.IsNullOrEmpty(server.ExchangeLegacyDN) && !string.IsNullOrEmpty(server.Fqdn))
						{
							dictionary[server.ExchangeLegacyDN] = server.Fqdn;
							ServerFqdnCache.ReferralTracer.TraceDebug<string, string>(0L, "Server {0}: {1}", server.ExchangeLegacyDN, server.Fqdn);
						}
					}
					if (dateTime != DateTime.MinValue)
					{
						ServerFqdnCache.lastDeltaSync = dateTime;
					}
					ADObjectId adobjectId = null;
					Server server2 = topologyConfigurationSession.ReadLocalServer();
					if (server2 != null && !string.IsNullOrEmpty(server2.Fqdn))
					{
						ServerFqdnCache.ReferralTracer.TraceDebug<string>(0L, "Default server: {0}", server2.Fqdn);
						dictionary["//default//"] = server2.Fqdn;
						adobjectId = server2.ServerSite;
					}
					IEnumerable<ClientAccessArray> enumerable2 = topologyConfigurationSession.FindPaged<ClientAccessArray>(null, QueryScope.SubTree, ClientAccessArray.PriorTo15ExchangeObjectVersionFilter, null, 0);
					foreach (ClientAccessArray clientAccessArray in enumerable2)
					{
						if (!string.IsNullOrEmpty(clientAccessArray.Fqdn) && !string.IsNullOrEmpty(clientAccessArray.ExchangeLegacyDN))
						{
							dictionary[clientAccessArray.ExchangeLegacyDN] = clientAccessArray.Fqdn;
							ServerFqdnCache.ReferralTracer.TraceDebug<string, string>(0L, "Array {0}: {1}", clientAccessArray.ExchangeLegacyDN, clientAccessArray.Fqdn);
						}
						if (adobjectId != null && adobjectId.Equals(clientAccessArray.Site))
						{
							dictionary["//default//"] = clientAccessArray.Fqdn;
							ServerFqdnCache.ReferralTracer.TraceDebug<string>(0L, "Default server: {0} (array)", clientAccessArray.Fqdn);
						}
					}
				}
				catch (ADTransientException arg)
				{
					ServerFqdnCache.ReferralTracer.TraceError<ADTransientException>(0L, "Exception reloading RFR server FQDN cache: {0}", arg);
					ServerFqdnCache.RestartTimer(ServerFqdnCache.ErrorRetryInterval, ServerFqdnCache.ReloadInterval);
					return;
				}
				catch (DataSourceOperationException arg2)
				{
					ServerFqdnCache.ReferralTracer.TraceError<DataSourceOperationException>(0L, "Exception reloading RFR server FQDN cache: {0}", arg2);
					ServerFqdnCache.RestartTimer(ServerFqdnCache.ErrorRetryInterval, ServerFqdnCache.ReloadInterval);
					return;
				}
				ServerFqdnCache.serverFqdnDictionary = dictionary;
				ServerFqdnCache.RestartTimer(ServerFqdnCache.ReloadInterval, ServerFqdnCache.ReloadInterval);
				ServerFqdnCache.ReferralTracer.TraceDebug(0L, "Reload complete");
			}
			finally
			{
				Interlocked.Exchange(ref ServerFqdnCache.reloadCacheInProgress, 0);
			}
		}

		private static void RestartTimer(TimeSpan timerDelayStart, TimeSpan timerInterval)
		{
			lock (ServerFqdnCache.cacheTimerLock)
			{
				if (ServerFqdnCache.cacheTimer != null)
				{
					ServerFqdnCache.cacheTimer.Change(timerDelayStart, timerInterval);
				}
			}
		}

		private const string DefaultLegacyDN = "//default//";

		private static readonly Trace ReferralTracer = ExTraceGlobals.ReferralTracer;

		private static readonly TimeSpan ReloadInterval = TimeSpan.FromMinutes(15.0);

		private static readonly TimeSpan ErrorRetryInterval = TimeSpan.FromSeconds(30.0);

		private static readonly TimeSpan StartImmediately = TimeSpan.FromMinutes(0.0);

		private static Dictionary<string, string> serverFqdnDictionary;

		private static Timer cacheTimer;

		private static object cacheTimerLock = new object();

		private static int reloadCacheInProgress = 0;

		private static DateTime lastDeltaSync;

		private static DateTime lastFullSync;

		private static readonly TimeSpan FullSyncInterval = TimeSpan.FromDays(1.0);

		private static readonly TimeSpan ReplicationDelayTolerance = TimeSpan.FromDays(1.0);
	}
}
