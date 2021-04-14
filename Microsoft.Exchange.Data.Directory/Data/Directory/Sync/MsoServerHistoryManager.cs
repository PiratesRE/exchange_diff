using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class MsoServerHistoryManager
	{
		internal MsoServerHistoryManager(string serviceInstanceName, int maxServerHistoryEntries, bool createRootContainer)
		{
			this.serverHistorySession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 63, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\MsoServerHistoryManager.cs");
			this.serverHistorySession.UseConfigNC = false;
			this.MaxServerHistoryEntries = maxServerHistoryEntries;
			this.ServiceInstanceName = serviceInstanceName;
			ADObjectId serviceInstanceObjectId = SyncServiceInstance.GetServiceInstanceObjectId(this.ServiceInstanceName);
			Container container = this.serverHistorySession.Read<Container>(serviceInstanceObjectId.GetChildId("ServerHistory"));
			if (container == null && createRootContainer)
			{
				container = new Container();
				container.SetId(serviceInstanceObjectId.GetChildId("ServerHistory"));
				try
				{
					this.serverHistorySession.Save(container);
				}
				catch (ADObjectAlreadyExistsException)
				{
				}
			}
			if (container != null)
			{
				ServerHistoryEntry[] array = this.serverHistorySession.Find<ServerHistoryEntry>(container.Id, QueryScope.OneLevel, null, null, 0);
				Array.Sort<ServerHistoryEntry>(array, (ServerHistoryEntry x, ServerHistoryEntry y) => DateTime.Compare(DateTime.Parse(x.Name.Substring(0, x.Name.IndexOf("-"))), DateTime.Parse(y.Name.Substring(0, y.Name.IndexOf("-")))));
				this.serverHistoryEntriesList = array.ToList<ServerHistoryEntry>();
				this.serverHistoryRootContainerID = container.Id;
			}
		}

		public int MaxServerHistoryEntries { get; set; }

		public string ServiceInstanceName { get; set; }

		internal void UpdateOrCreateServerHistoryEntry(DateTime activeTimestamp)
		{
			ServerHistoryEntryData serverHistoryEntryData = this.ReadLastSyncServerHistory();
			string machineName = Environment.MachineName;
			if (serverHistoryEntryData != null && !string.Equals(serverHistoryEntryData.ServerName, machineName, StringComparison.InvariantCultureIgnoreCase) && serverHistoryEntryData.ActiveTimestamp.Equals(serverHistoryEntryData.PassiveTimestamp))
			{
				serverHistoryEntryData.PassiveReason = "Server became passive for unknown reason. Passive timestamp is not accurate.";
				this.WriteSyncServerHistory(serverHistoryEntryData, false);
			}
			if (serverHistoryEntryData == null || !string.Equals(serverHistoryEntryData.ServerName, machineName, StringComparison.InvariantCultureIgnoreCase) || !serverHistoryEntryData.ActiveTimestamp.Equals(serverHistoryEntryData.PassiveTimestamp))
			{
				this.WriteSyncServerHistory(new ServerHistoryEntryData(machineName, activeTimestamp, activeTimestamp, string.Empty), true);
			}
		}

		internal void UpdateLastServerHistoryEntry(DateTime passiveTimestamp, string passiveReason)
		{
			if (this.serverHistoryRootContainerID != null)
			{
				ServerHistoryEntryData serverHistoryEntryData = this.ReadLastSyncServerHistory();
				if (serverHistoryEntryData != null && string.Equals(serverHistoryEntryData.ServerName, Environment.MachineName))
				{
					serverHistoryEntryData.PassiveTimestamp = passiveTimestamp;
					serverHistoryEntryData.PassiveReason = passiveReason;
					this.WriteSyncServerHistory(serverHistoryEntryData, false);
				}
			}
		}

		private void WriteSyncServerHistory(ServerHistoryEntryData serverHistoryData, bool writeNewEntry)
		{
			if (serverHistoryData == null)
			{
				throw new ArgumentNullException("serverHistoryEntryData");
			}
			if (this.serverHistoryEntriesList.Count == 0 && !writeNewEntry)
			{
				throw new Exception("last server history entry is null");
			}
			ServerHistoryEntry serverHistoryEntry = writeNewEntry ? null : this.serverHistoryEntriesList.Last<ServerHistoryEntry>();
			DateTime dateTime = (DateTime)ExDateTime.UtcNow;
			if (writeNewEntry)
			{
				if (this.serverHistoryEntriesList.Count > this.MaxServerHistoryEntries - 1)
				{
					this.CleanupOldServerHistory();
				}
				serverHistoryEntry = new ServerHistoryEntry();
				string unescapedCommonName = string.Format("{0}-{1}", dateTime, Environment.MachineName);
				serverHistoryEntry.SetId(this.serverHistoryRootContainerID.GetChildId(unescapedCommonName));
				serverHistoryEntry.Name = serverHistoryEntry.Id.Name;
				serverHistoryEntry.m_Session = this.serverHistorySession;
				serverHistoryEntry.Version = 1;
				this.serverHistoryEntriesList.Add(serverHistoryEntry);
			}
			serverHistoryEntry.Timestamp = dateTime;
			serverHistoryEntry.Data = serverHistoryData.ToBinary();
			this.serverHistorySession.Save(serverHistoryEntry);
		}

		private ServerHistoryEntryData ReadLastSyncServerHistory()
		{
			if (this.serverHistoryEntriesList.Count > 0)
			{
				return new ServerHistoryEntryData(this.serverHistoryEntriesList.Last<ServerHistoryEntry>());
			}
			return null;
		}

		private void CleanupOldServerHistory()
		{
			while (this.serverHistoryEntriesList.Count > this.MaxServerHistoryEntries - 1)
			{
				try
				{
					this.serverHistorySession.Delete(this.serverHistoryEntriesList.First<ServerHistoryEntry>());
					this.serverHistoryEntriesList.RemoveAt(0);
				}
				catch (Exception ex)
				{
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_FailedToCleanupCookies, this.ServiceInstanceName, new object[]
					{
						ex
					});
					break;
				}
			}
		}

		private const int ServerHistoryVersion = 1;

		public const string ServerHistoryContainerName = "ServerHistory";

		private readonly ITopologyConfigurationSession serverHistorySession;

		private readonly List<ServerHistoryEntry> serverHistoryEntriesList;

		private readonly ADObjectId serverHistoryRootContainerID;
	}
}
