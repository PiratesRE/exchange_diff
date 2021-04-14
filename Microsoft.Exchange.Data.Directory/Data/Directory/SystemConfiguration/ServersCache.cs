using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ServersCache
	{
		private static ADPagedReader<MiniServer> ReadLocalSiteMailboxServers()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 102, "ReadLocalSiteMailboxServers", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\ServersCache.cs");
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, ServersCache.LocalSiteId);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				ServersCache.MailboxServerRoleFilter,
				ServersCache.ServerOnlineFilter
			});
			SortBy sortBy = new SortBy(ServerSchema.VersionNumber, SortOrder.Ascending);
			return topologyConfigurationSession.FindPaged<MiniServer>(null, QueryScope.SubTree, filter, sortBy, 0, null);
		}

		private static bool IsLocalSite(ADObjectId adSiteId)
		{
			return adSiteId != null && adSiteId.Equals(ServersCache.LocalSiteId);
		}

		private static MiniServer GetOneMailboxServerForASite(ADObjectId adSiteId, int versionNumber, bool needsExactVersionMatch)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 145, "GetOneMailboxServerForASite", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\ServersCache.cs");
			QueryFilter queryFilter;
			if (needsExactVersionMatch)
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.VersionNumber, versionNumber);
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ServerSchema.VersionNumber, versionNumber);
			}
			QueryFilter filter;
			if (adSiteId != null)
			{
				QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, adSiteId);
				filter = new AndFilter(new QueryFilter[]
				{
					queryFilter2,
					ServersCache.MailboxServerRoleFilter,
					ServersCache.ServerOnlineFilter,
					queryFilter
				});
			}
			else
			{
				filter = new AndFilter(new QueryFilter[]
				{
					ServersCache.MailboxServerRoleFilter,
					ServersCache.ServerOnlineFilter,
					queryFilter
				});
			}
			MiniServer[] array = topologyConfigurationSession.Find<MiniServer>(null, QueryScope.SubTree, filter, null, 1, null);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			throw new ServerHasNotBeenFoundException(versionNumber, string.Empty, needsExactVersionMatch, (adSiteId != null) ? adSiteId.Name : string.Empty);
		}

		private static void UpdateServerInfoInList(List<ServersCache.ServerInfo> list, ServersCache.ServerInfo serverInfo)
		{
			if (list != null || list.Count > 0)
			{
				int num = list.FindIndex((ServersCache.ServerInfo x) => string.Compare(x.MiniServer.Fqdn, serverInfo.MiniServer.Fqdn, true) == 0);
				if (num != -1)
				{
					list.RemoveAt(num);
				}
				list.Insert(0, serverInfo);
			}
		}

		private static void UpdateMiniServerIntoCache(MiniServer miniServer)
		{
			ServersCache.ServerInfo serverInfo = null;
			if (miniServer != null)
			{
				try
				{
					ServersCache.CacheLockForServersDictionary.EnterWriteLock();
					serverInfo = new ServersCache.ServerInfo(DateTime.UtcNow, miniServer);
					ServersCache.ServersDictionary[miniServer.Fqdn] = serverInfo;
				}
				finally
				{
					ServersCache.CacheLockForServersDictionary.ExitWriteLock();
				}
				ADObjectId serverSite = miniServer.ServerSite;
				if (!ServersCache.IsLocalSite(serverSite))
				{
					try
					{
						ServersCache.CacheLockForSiteToServersDictionary.EnterWriteLock();
						if (ServersCache.SiteToServersDictionary.ContainsKey(serverSite))
						{
							ServersCache.UpdateServerInfoInList(ServersCache.SiteToServersDictionary[serverSite], serverInfo);
						}
						else
						{
							List<ServersCache.ServerInfo> list = new List<ServersCache.ServerInfo>();
							list.Add(serverInfo);
							ServersCache.SiteToServersDictionary[serverSite] = list;
						}
					}
					finally
					{
						ServersCache.CacheLockForSiteToServersDictionary.ExitWriteLock();
					}
				}
			}
		}

		private static bool TryCalculateStartAndEndIndex(List<ServersCache.ServerInfo> list, int versionNumber, bool needsExactVersionMatch, out int startIndex, out int endIndex)
		{
			startIndex = -1;
			endIndex = -1;
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].MiniServer.VersionNumber == versionNumber && needsExactVersionMatch)
				{
					if (endIndex != -1 && list[i].MiniServer.VersionNumber > list[endIndex].MiniServer.VersionNumber)
					{
						break;
					}
					if (startIndex == -1)
					{
						startIndex = i;
						endIndex = i;
					}
					else
					{
						endIndex = i;
					}
				}
				else if (list[i].MiniServer.VersionNumber >= versionNumber && !needsExactVersionMatch && startIndex == -1)
				{
					startIndex = i;
					break;
				}
			}
			if (startIndex == -1)
			{
				return false;
			}
			if (!needsExactVersionMatch)
			{
				endIndex = list.Count - 1;
			}
			return true;
		}

		private static int GenerateIndexOfServer(int startIndex, int endIndex, string identifier = null)
		{
			if (string.IsNullOrWhiteSpace(identifier))
			{
				Random random = new Random();
				return random.Next(startIndex, endIndex + 1);
			}
			int num = Math.Abs(identifier.GetHashCode());
			int num2 = num % (endIndex - startIndex + 1);
			return num2 + startIndex;
		}

		private static MiniServer FindMiniServerInListWithoutAffinity(List<ServersCache.ServerInfo> list, int versionNumber, bool needsExactVersionMatch)
		{
			MiniServer result = null;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					TimeSpan t = DateTime.UtcNow - list[i].LastRefreshTime;
					if (needsExactVersionMatch)
					{
						if (list[i].MiniServer.VersionNumber == versionNumber && t <= ServersCache.RefreshInterval)
						{
							result = list[i].MiniServer;
						}
					}
					else if (list[i].MiniServer.VersionNumber >= versionNumber && t <= ServersCache.RefreshInterval)
					{
						result = list[i].MiniServer;
					}
				}
			}
			return result;
		}

		private static MiniServer FindAndReturnMiniServerFromCacheForASite(ADObjectId siteId, int versionNumber, string identifier, bool needsExactVersionMatch)
		{
			int startIndex = -1;
			int endIndex = -1;
			MiniServer miniServer = null;
			if (ServersCache.SiteToServersDictionary.ContainsKey(siteId))
			{
				try
				{
					ServersCache.CacheLockForSiteToServersDictionary.EnterReadLock();
					List<ServersCache.ServerInfo> list = ServersCache.SiteToServersDictionary[siteId];
					if (ServersCache.IsLocalSite(siteId))
					{
						if (DateTime.UtcNow - ServersCache.LastRefreshTimeForLocalSiteCache > ServersCache.RefreshInterval)
						{
							miniServer = null;
						}
						else
						{
							bool flag = ServersCache.TryCalculateStartAndEndIndex(list, versionNumber, needsExactVersionMatch, out startIndex, out endIndex);
							if (flag)
							{
								int index = ServersCache.GenerateIndexOfServer(startIndex, endIndex, identifier);
								miniServer = list[index].MiniServer;
							}
						}
					}
					else
					{
						miniServer = ServersCache.FindMiniServerInListWithoutAffinity(list, versionNumber, needsExactVersionMatch);
					}
					if (miniServer == null)
					{
						ServersCache.Tracer.TraceError<int>(0L, "ServersCache: No server with the version number {0} in the cache.", versionNumber);
					}
					return miniServer;
				}
				finally
				{
					ServersCache.CacheLockForSiteToServersDictionary.ExitReadLock();
				}
			}
			return null;
		}

		private static MiniServer GetDeterministicBackEndServerForASite(int versionNumber, string identifier, bool needsExactVersionMatch = false, ADObjectId adSiteId = null)
		{
			ADObjectId adobjectId = (adSiteId != null) ? adSiteId : ServersCache.LocalSiteId;
			bool flag = ServersCache.IsLocalSite(adobjectId);
			MiniServer miniServer = ServersCache.FindAndReturnMiniServerFromCacheForASite(adobjectId, versionNumber, identifier, needsExactVersionMatch);
			if (miniServer != null)
			{
				return miniServer;
			}
			if (flag)
			{
				if (ServersCache.SiteToServersDictionary.ContainsKey(adobjectId) && DateTime.UtcNow - ServersCache.LastRefreshTimeForLocalSiteCache <= ServersCache.RefreshInterval)
				{
					throw new ServerHasNotBeenFoundException(versionNumber, identifier, needsExactVersionMatch, adobjectId.Name);
				}
				lock (ServersCache.LockForLocalSiteDiscovery)
				{
					if (ServersCache.SiteToServersDictionary.ContainsKey(adobjectId) && DateTime.UtcNow - ServersCache.LastRefreshTimeForLocalSiteCache <= ServersCache.RefreshInterval)
					{
						MiniServer miniServer2 = ServersCache.FindAndReturnMiniServerFromCacheForASite(adobjectId, versionNumber, identifier, needsExactVersionMatch);
						if (miniServer2 == null)
						{
							ServersCache.Tracer.TraceError(0L, "ServersCache: No server with the version number {0}, identifier '{1}', needsExactVersionMatch {2} and siteId.Name {3} in the cache for local site.", new object[]
							{
								versionNumber,
								identifier,
								needsExactVersionMatch,
								adobjectId.Name
							});
						}
						return miniServer2;
					}
					ADPagedReader<MiniServer> adpagedReader = ServersCache.ReadLocalSiteMailboxServers();
					List<ServersCache.ServerInfo> list = null;
					if (adpagedReader == null)
					{
						throw new ServerHasNotBeenFoundException(versionNumber, identifier, needsExactVersionMatch, adobjectId.Name);
					}
					list = new List<ServersCache.ServerInfo>();
					foreach (MiniServer miniServer3 in adpagedReader)
					{
						ServersCache.ServerInfo item = new ServersCache.ServerInfo(DateTime.UtcNow, miniServer3);
						list.Add(item);
					}
					try
					{
						ServersCache.CacheLockForServersDictionary.EnterWriteLock();
						foreach (ServersCache.ServerInfo serverInfo in list)
						{
							ServersCache.ServersDictionary[serverInfo.MiniServer.Fqdn] = serverInfo;
						}
					}
					finally
					{
						ServersCache.CacheLockForServersDictionary.ExitWriteLock();
					}
					if (list.Count == 0)
					{
						throw new ServerHasNotBeenFoundException(versionNumber, identifier, needsExactVersionMatch, adobjectId.Name);
					}
					ServersCache.LastRefreshTimeForLocalSiteCache = DateTime.UtcNow;
					try
					{
						ServersCache.CacheLockForSiteToServersDictionary.EnterWriteLock();
						ServersCache.SiteToServersDictionary[adobjectId] = list;
					}
					finally
					{
						ServersCache.CacheLockForSiteToServersDictionary.ExitWriteLock();
					}
					MiniServer miniServer4 = ServersCache.FindAndReturnMiniServerFromCacheForASite(adobjectId, versionNumber, identifier, needsExactVersionMatch);
					if (miniServer4 == null)
					{
						throw new ServerHasNotBeenFoundException(versionNumber, identifier, needsExactVersionMatch, adobjectId.Name);
					}
					return miniServer4;
				}
			}
			MiniServer oneMailboxServerForASite = ServersCache.GetOneMailboxServerForASite(adobjectId, versionNumber, needsExactVersionMatch);
			ServersCache.UpdateMiniServerIntoCache(oneMailboxServerForASite);
			return oneMailboxServerForASite;
		}

		private static MiniServer MakeADQueryToGetServer(string serverFQDN)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 622, "MakeADQueryToGetServer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\ServersCache.cs");
			MiniServer miniServer = topologyConfigurationSession.FindMiniServerByFqdn(serverFQDN);
			ServersCache.UpdateMiniServerIntoCache(miniServer);
			if (miniServer == null)
			{
				throw new LocalServerNotFoundException(serverFQDN);
			}
			return miniServer;
		}

		internal static MiniServer GetDeterministicBackEndServerFromLocalSite(int versionNumber, string identifier, bool needsExactVersionMatch = false)
		{
			return ServersCache.GetDeterministicBackEndServerForASite(versionNumber, identifier, needsExactVersionMatch, null);
		}

		internal static MiniServer GetAnyBackEndServerFromLocalSite(int versionNumber, bool needsExactVersionMatch = false)
		{
			return ServersCache.GetDeterministicBackEndServerForASite(versionNumber, null, needsExactVersionMatch, null);
		}

		internal static MiniServer GetAnyBackEndServerFromASite(ADObjectId adSiteId, int versionNumber, bool needsExactVersionMatch = false)
		{
			return ServersCache.GetDeterministicBackEndServerForASite(versionNumber, null, needsExactVersionMatch, adSiteId);
		}

		internal static MiniServer GetServerByFQDN(string serverFQDN, out bool isFromCache)
		{
			if (string.IsNullOrWhiteSpace(serverFQDN))
			{
				throw new ArgumentNullException("serverName should not be empty or null.");
			}
			isFromCache = true;
			try
			{
				ServersCache.CacheLockForServersDictionary.EnterReadLock();
				if (ServersCache.ServersDictionary.ContainsKey(serverFQDN) && DateTime.UtcNow - ServersCache.ServersDictionary[serverFQDN].LastRefreshTime <= ServersCache.RefreshInterval)
				{
					return ServersCache.ServersDictionary[serverFQDN].MiniServer;
				}
			}
			finally
			{
				ServersCache.CacheLockForServersDictionary.ExitReadLock();
			}
			MiniServer result = ServersCache.MakeADQueryToGetServer(serverFQDN);
			isFromCache = false;
			return result;
		}

		internal static MiniServer GetDeterministicBackEndServerFromSameSite(string sourceServerFQDN, int versionNumber, string identifier, bool needsExactVersionMatch = false)
		{
			if (string.IsNullOrWhiteSpace(sourceServerFQDN))
			{
				throw new ArgumentNullException("sourceServerName should not be null");
			}
			bool flag = true;
			MiniServer serverByFQDN = ServersCache.GetServerByFQDN(sourceServerFQDN, out flag);
			if (!flag)
			{
				ServersCache.UpdateMiniServerIntoCache(serverByFQDN);
			}
			ADObjectId serverSite = serverByFQDN.ServerSite;
			return ServersCache.GetDeterministicBackEndServerForASite(versionNumber, identifier, needsExactVersionMatch, serverSite);
		}

		internal static MiniServer GetAnyBackEndServerWithExactVersion(int versionNumber)
		{
			return ServersCache.GetAnyBackEndServer(versionNumber, true);
		}

		internal static MiniServer GetAnyBackEndServerWithMinVersion(int miniversionNumber)
		{
			return ServersCache.GetAnyBackEndServer(miniversionNumber, false);
		}

		private static MiniServer GetAnyBackEndServer(int versionNumber, bool needsExactVersionMatch)
		{
			MiniServer miniServer = null;
			try
			{
				miniServer = ServersCache.GetDeterministicBackEndServerForASite(versionNumber, null, needsExactVersionMatch, null);
			}
			catch (ServerHasNotBeenFoundException)
			{
			}
			if (miniServer != null)
			{
				return miniServer;
			}
			try
			{
				ServersCache.CacheLockForServersDictionary.EnterReadLock();
				miniServer = ServersCache.FindMiniServerInListWithoutAffinity(ServersCache.ServersDictionary.Values.ToList<ServersCache.ServerInfo>(), versionNumber, needsExactVersionMatch);
			}
			finally
			{
				ServersCache.CacheLockForServersDictionary.ExitReadLock();
			}
			if (miniServer != null)
			{
				return miniServer;
			}
			MiniServer oneMailboxServerForASite = ServersCache.GetOneMailboxServerForASite(null, versionNumber, needsExactVersionMatch);
			ServersCache.UpdateMiniServerIntoCache(oneMailboxServerForASite);
			return oneMailboxServerForASite;
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1.0);

		private static DateTime LastRefreshTimeForLocalSiteCache = DateTime.MinValue;

		private static Dictionary<ADObjectId, List<ServersCache.ServerInfo>> SiteToServersDictionary = new Dictionary<ADObjectId, List<ServersCache.ServerInfo>>();

		private static Dictionary<string, ServersCache.ServerInfo> ServersDictionary = new Dictionary<string, ServersCache.ServerInfo>();

		private static ReaderWriterLockSlim CacheLockForServersDictionary = new ReaderWriterLockSlim();

		private static ReaderWriterLockSlim CacheLockForSiteToServersDictionary = new ReaderWriterLockSlim();

		private static readonly object LockForLocalSiteDiscovery = new object();

		private static QueryFilter MailboxServerRoleFilter = new BitMaskAndFilter(ServerSchema.CurrentServerRole, 2UL);

		private static AndFilter ServerOnlineFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ActiveDirectoryServerSchema.AreServerStatesOnline, true),
			new ComparisonFilter(ComparisonOperator.Equal, ActiveDirectoryServerSchema.IsOutOfService, false)
		});

		private static ADObjectId LocalSiteId = LocalSiteCache.LocalSite.Id;

		internal class ServerInfo
		{
			public DateTime LastRefreshTime { get; private set; }

			public MiniServer MiniServer { get; private set; }

			public ServerInfo(DateTime lastRefreshTime, MiniServer miniServer)
			{
				this.LastRefreshTime = lastRefreshTime;
				this.MiniServer = miniServer;
			}
		}
	}
}
