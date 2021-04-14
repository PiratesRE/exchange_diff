using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class PrimaryServerInfoMap : IPrimaryServerInfoMap
	{
		public PrimaryServerInfoMap(ShadowRedundancyEventLogger shadowRedundancyEventLogger, DataTable serverInfoTable, bool shouldCommit = true)
		{
			this.shadowRedundancyEventLogger = shadowRedundancyEventLogger;
			this.serverInfoTable = serverInfoTable;
			this.shouldCommit = shouldCommit;
		}

		public event Action<PrimaryServerInfo> NotifyPrimaryServerStateChanged;

		public int Count
		{
			get
			{
				return this.primaryServerInfos.Count;
			}
		}

		public TimeSpan MaxDumpsterTime
		{
			get
			{
				return Components.Configuration.TransportSettings.TransportSettings.MaxDumpsterTime;
			}
		}

		public void Add(PrimaryServerInfo primaryServerInfo)
		{
			lock (this)
			{
				this.primaryServerInfos.Add(primaryServerInfo);
			}
		}

		public IEnumerable<PrimaryServerInfo> GetAll()
		{
			return this.primaryServerInfos.ToArray();
		}

		public PrimaryServerInfo GetActive(string serverFqdn)
		{
			return (from primaryServerInfo in this.primaryServerInfos
			where serverFqdn.Equals(primaryServerInfo.ServerFqdn, StringComparison.InvariantCultureIgnoreCase) && primaryServerInfo.IsActive
			select primaryServerInfo).FirstOrDefault<PrimaryServerInfo>();
		}

		public PrimaryServerInfo UpdateServerState(string serverFqdn, string state, ShadowRedundancyCompatibilityVersion version)
		{
			PrimaryServerInfoMap.StateChangeType stateChangeType = PrimaryServerInfoMap.StateChangeType.Add;
			PrimaryServerInfo primaryServerInfo;
			lock (this)
			{
				DateTime utcNow = DateTime.UtcNow;
				primaryServerInfo = this.GetActive(serverFqdn);
				stateChangeType = PrimaryServerInfoMap.GetStateChangeType(serverFqdn, state, this.shadowRedundancyEventLogger, primaryServerInfo);
				switch (stateChangeType)
				{
				case PrimaryServerInfoMap.StateChangeType.Add:
					primaryServerInfo = new PrimaryServerInfo(this.serverInfoTable)
					{
						ServerFqdn = serverFqdn,
						DatabaseState = state,
						StartTime = utcNow,
						Version = version
					};
					this.Commit(primaryServerInfo);
					this.Add(primaryServerInfo);
					break;
				case PrimaryServerInfoMap.StateChangeType.Change:
				{
					PrimaryServerInfo primaryServerInfo2 = new PrimaryServerInfo(primaryServerInfo, this.serverInfoTable)
					{
						DatabaseState = state,
						StartTime = utcNow
					};
					this.Commit(primaryServerInfo2);
					this.Add(primaryServerInfo2);
					primaryServerInfo.EndTime = utcNow;
					this.Commit(primaryServerInfo);
					break;
				}
				case PrimaryServerInfoMap.StateChangeType.Delete:
					primaryServerInfo.EndTime = utcNow;
					this.Commit(primaryServerInfo);
					break;
				}
			}
			if (stateChangeType != PrimaryServerInfoMap.StateChangeType.None && this.NotifyPrimaryServerStateChanged != null)
			{
				this.NotifyPrimaryServerStateChanged(primaryServerInfo);
			}
			return primaryServerInfo;
		}

		public bool Remove(PrimaryServerInfo primaryServerInfo)
		{
			bool result;
			lock (this)
			{
				result = this.primaryServerInfos.Remove(primaryServerInfo);
			}
			return result;
		}

		public IEnumerable<PrimaryServerInfo> RemoveExpiredServers(DateTime now)
		{
			Lazy<List<PrimaryServerInfo>> lazy = new Lazy<List<PrimaryServerInfo>>();
			Lazy<List<PrimaryServerInfo>> lazy2 = new Lazy<List<PrimaryServerInfo>>();
			lock (this)
			{
				foreach (PrimaryServerInfo primaryServerInfo in this.primaryServerInfos)
				{
					if (primaryServerInfo.IsActive || primaryServerInfo.EndTime + this.MaxDumpsterTime > now)
					{
						lazy.Value.Add(primaryServerInfo);
					}
					else
					{
						lazy2.Value.Add(primaryServerInfo);
					}
				}
				if (lazy.IsValueCreated)
				{
					this.primaryServerInfos = lazy.Value;
				}
			}
			if (!lazy2.IsValueCreated)
			{
				return null;
			}
			return lazy2.Value;
		}

		private static PrimaryServerInfoMap.StateChangeType GetStateChangeType(string serverFqdn, string newState, ShadowRedundancyEventLogger shadowRedundancyEventLogger, PrimaryServerInfo primaryServerInfo)
		{
			if (primaryServerInfo != null)
			{
				string databaseState = primaryServerInfo.DatabaseState;
				PrimaryServerInfoMap.StateChangeType stateChangeType;
				if (string.Equals(newState, databaseState, StringComparison.OrdinalIgnoreCase))
				{
					stateChangeType = PrimaryServerInfoMap.StateChangeType.None;
				}
				else if (newState == "0de1e7ed-0de1-0de1-0de1-de1e7edele7e")
				{
					stateChangeType = PrimaryServerInfoMap.StateChangeType.Delete;
				}
				else
				{
					stateChangeType = PrimaryServerInfoMap.StateChangeType.Change;
					if (shadowRedundancyEventLogger != null)
					{
						shadowRedundancyEventLogger.LogPrimaryServerDatabaseStateChanged(serverFqdn, databaseState, newState);
					}
				}
				ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "NotifyPrimaryServerState(): Server '{0}' Change Type {1} Old State {2} New State {3}", new object[]
				{
					serverFqdn,
					stateChangeType,
					databaseState,
					newState
				});
				return stateChangeType;
			}
			if (newState == "0de1e7ed-0de1-0de1-0de1-de1e7edele7e")
			{
				throw new InvalidOperationException(string.Format("Server {0} cannot be deleted as it is not present in PrimaryServerInfomap", serverFqdn));
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug(0L, "NotifyPrimaryServerState(): Server '{0}' Change Type {1} Old State {2} New State {3}", new object[]
			{
				serverFqdn,
				PrimaryServerInfoMap.StateChangeType.Add,
				"-NA-",
				newState
			});
			return PrimaryServerInfoMap.StateChangeType.Add;
		}

		private void Commit(PrimaryServerInfo primaryServerInfo)
		{
			if (this.shouldCommit)
			{
				primaryServerInfo.Commit(TransactionCommitMode.MediumLatencyLazy);
			}
		}

		private readonly bool shouldCommit;

		private List<PrimaryServerInfo> primaryServerInfos = new List<PrimaryServerInfo>();

		private ShadowRedundancyEventLogger shadowRedundancyEventLogger;

		private DataTable serverInfoTable;

		private enum StateChangeType
		{
			None,
			Add,
			Change,
			Delete
		}
	}
}
