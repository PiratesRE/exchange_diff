using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBestCopySelection : IBestCopySelector
	{
		public AmBestCopySelection(Guid dbGuid, IADDatabase database, AmDbActionCode actionCode, AmMultiNodeCopyStatusFetcher statusFetcher, AmServerName sourceServerName, AmConfig amConfig, DatabaseMountDialOverride mountDialOverride, AmBcsSkipFlags skipValidationChecks, AmDbAction.PrepareSubactionArgsDelegate prepareSubaction) : this(dbGuid, database, actionCode, statusFetcher, sourceServerName, amConfig, mountDialOverride, skipValidationChecks, prepareSubaction, null)
		{
		}

		public AmBestCopySelection(Guid dbGuid, IADDatabase database, AmDbActionCode actionCode, AmMultiNodeCopyStatusFetcher statusFetcher, AmServerName sourceServerName, AmConfig amConfig, DatabaseMountDialOverride mountDialOverride, AmBcsSkipFlags skipValidationChecks, AmDbAction.PrepareSubactionArgsDelegate prepareSubaction, string componentName)
		{
			this.m_amConfig = amConfig;
			this.m_statusFetcher = statusFetcher;
			this.m_mountDialOverride = mountDialOverride;
			IAmBcsErrorLogger errorLogger = new AmBcsServerFailureLogger(dbGuid, database.Name, true);
			this.m_bcsContext = new AmBcsContext(dbGuid, sourceServerName, errorLogger);
			this.m_bcsContext.Database = database;
			this.m_bcsContext.ActionCode = actionCode;
			this.m_bcsContext.SkipValidationChecks = skipValidationChecks;
			this.m_bcsContext.InitiatingComponent = componentName;
			this.m_bcsContext.PrepareSubaction = prepareSubaction;
			this.m_perfTracker = new BcsPerformanceTracker(prepareSubaction);
		}

		public AmBcsType BestCopySelectionType
		{
			get
			{
				return AmBcsType.BestCopySelection;
			}
		}

		public IAmBcsErrorLogger ErrorLogger
		{
			get
			{
				return this.m_bcsContext.ErrorLogger;
			}
		}

		public Exception LastException
		{
			get
			{
				return this.m_bestCopySelector.LastException;
			}
		}

		public AmServerName FindNextBestCopy()
		{
			if (this.m_bestCopySelector == null)
			{
				this.InitializeBestCopySelector();
			}
			return this.m_bestCopySelector.FindNextBestCopy();
		}

		private void InitializeBestCopySelector()
		{
			this.m_bcsWatch.Start();
			try
			{
				bool flag = this.m_statusFetcher == null;
				if (this.m_bcsContext.ShouldLogSubactionEvent)
				{
					ReplayCrimsonEvents.BcsInitiated.LogGeneric(this.m_bcsContext.PrepareSubaction(new object[]
					{
						this.BestCopySelectionType,
						flag
					}));
				}
				bool fDbNeverMounted = false;
				this.m_perfTracker.RunTimedOperation(BcsOperation.HasDatabaseBeenMounted, delegate
				{
					fDbNeverMounted = !AmBestCopySelectionHelper.HasDatabaseBeenMounted(this.m_bcsContext.DatabaseGuid, this.m_amConfig);
					this.m_bcsContext.DatabaseNeverMounted = fDbNeverMounted;
				});
				Dictionary<AmServerName, RpcHealthStateInfo[]> stateInfoMap = null;
				this.m_bcsContext.StatusTable = this.ConstructBcsStatusTable(fDbNeverMounted, flag, out stateInfoMap);
				this.m_bcsContext.ComponentStateWrapper = new ComponentStateWrapper(this.m_bcsContext.Database.Name, this.m_bcsContext.InitiatingComponent, this.m_bcsContext.SourceServerName, this.m_bcsContext.ActionCode, stateInfoMap);
				this.m_bcsContext.SortCopiesByActivationPreference = this.ShouldDatabaseCopiesBeSortedByPreference();
				this.m_bestCopySelector = new AmBestCopySelector(this.m_bcsContext);
			}
			finally
			{
				this.m_bcsWatch.Stop();
				this.m_perfTracker.RecordDuration(BcsOperation.BcsOverall, this.m_bcsWatch.Elapsed);
				this.m_perfTracker.LogEvent();
			}
		}

		private Dictionary<AmServerName, RpcDatabaseCopyStatus2> ConstructBcsStatusTable(bool fDbNeverMounted, bool fConstructStatusFetcher, out Dictionary<AmServerName, RpcHealthStateInfo[]> healthTable)
		{
			Dictionary<AmServerName, RpcDatabaseCopyStatus2> statusTable = null;
			List<AmServerName> serversToContact = null;
			healthTable = null;
			Dictionary<AmServerName, RpcHealthStateInfo[]> tmpHealthTable = null;
			this.m_bcsContext.IsSourceServerAllowedForMount = false;
			if (!fDbNeverMounted)
			{
				if (fConstructStatusFetcher)
				{
					this.m_perfTracker.RunTimedOperation(BcsOperation.DetermineServersToContact, delegate
					{
						serversToContact = this.DetermineServersToContact();
					});
					this.m_statusFetcher = new AmMultiNodeCopyStatusFetcher(serversToContact, new Guid[]
					{
						this.m_bcsContext.DatabaseGuid
					}, null, RpcGetDatabaseCopyStatusFlags2.ReadThrough, null, true);
				}
				this.m_perfTracker.RunTimedOperation(BcsOperation.GetCopyStatusRpc, delegate
				{
					statusTable = this.GetStatusTable(out tmpHealthTable);
				});
				healthTable = tmpHealthTable;
			}
			else
			{
				this.m_perfTracker.RunTimedOperation(BcsOperation.DetermineServersToContact, delegate
				{
					serversToContact = this.DetermineServersToContact();
				});
				statusTable = new Dictionary<AmServerName, RpcDatabaseCopyStatus2>(serversToContact.Count);
				foreach (AmServerName key in serversToContact)
				{
					statusTable[key] = null;
				}
			}
			if (serversToContact != null && serversToContact.Contains(this.m_bcsContext.SourceServerName) && this.m_bcsContext.ActionCode.IsMountOrRemountOperation)
			{
				this.m_bcsContext.IsSourceServerAllowedForMount = true;
			}
			return statusTable;
		}

		private bool ShouldDatabaseCopiesBeSortedByPreference()
		{
			if (this.m_mountDialOverride == DatabaseMountDialOverride.Lossless)
			{
				return true;
			}
			this.PopulateMiniServersForDatabaseCopies();
			return this.m_miniServers.Any((IADServer server) => server.AutoDatabaseMountDial == AutoDatabaseMountDial.Lossless);
		}

		private void PopulateDatabaseCopiesIfNecessary()
		{
			if (this.m_dbCopies != null)
			{
				return;
			}
			IADDatabaseCopy[] dbCopies = null;
			Guid dbGuid = this.m_bcsContext.DatabaseGuid;
			IADDatabase database = this.m_bcsContext.Database;
			this.m_perfTracker.RunTimedOperation(BcsOperation.GetDatabaseCopies, delegate
			{
				dbCopies = AmBestCopySelectionHelper.GetDatabaseCopies(dbGuid, ref database);
			});
			if (database != null)
			{
				this.m_bcsContext.Database = database;
			}
			this.m_dbCopies = dbCopies;
		}

		private void PopulateMiniServersForDatabaseCopies()
		{
			if (this.m_miniServers != null)
			{
				return;
			}
			this.PopulateDatabaseCopiesIfNecessary();
			List<IADServer> list = new List<IADServer>(this.m_dbCopies.Length);
			Exception ex = null;
			foreach (IADDatabaseCopy iaddatabaseCopy in this.m_dbCopies)
			{
				AmServerName serverName = new AmServerName(iaddatabaseCopy.HostServerName);
				IADServer miniServer = AmBestCopySelectionHelper.GetMiniServer(serverName, out ex);
				if (miniServer == null)
				{
					throw ex;
				}
				list.Add(miniServer);
			}
			this.m_miniServers = list;
		}

		private List<AmServerName> DetermineServersToContact()
		{
			AmServerName sourceServerName = this.m_bcsContext.SourceServerName;
			IADDatabase database = this.m_bcsContext.Database;
			AmConfig amConfig = this.m_amConfig;
			this.PopulateDatabaseCopiesIfNecessary();
			List<AmServerName> list = new List<AmServerName>(this.m_dbCopies.Length);
			string name = database.Name;
			LocalizedString empty = LocalizedString.Empty;
			AmBcsServerChecks serverValidationChecks = AmBcsServerValidation.GetServerValidationChecks(this.m_bcsContext.ActionCode, false);
			foreach (IADDatabaseCopy iaddatabaseCopy in this.m_dbCopies)
			{
				AmServerName amServerName = new AmServerName(iaddatabaseCopy.HostServerName);
				AmBcsServerValidation amBcsServerValidation = new AmBcsServerValidation(amServerName, sourceServerName, database, amConfig, this.m_bcsContext.ErrorLogger, null);
				if (amBcsServerValidation.RunChecks(serverValidationChecks, ref empty))
				{
					list.Add(amServerName);
				}
			}
			return list;
		}

		private Dictionary<AmServerName, RpcDatabaseCopyStatus2> GetStatusTable(out Dictionary<AmServerName, RpcHealthStateInfo[]> healthTable)
		{
			Dictionary<AmServerName, CopyStatusClientCachedEntry> cachedEntryTable = null;
			List<AmServerName> serversToContact = this.m_statusFetcher.ServersToContact;
			healthTable = null;
			Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> status = this.m_statusFetcher.GetStatus(out healthTable);
			Dictionary<AmServerName, RpcDatabaseCopyStatus2> dictionary;
			if (status.TryGetValue(this.m_bcsContext.DatabaseGuid, out cachedEntryTable))
			{
				Dictionary<AmServerName, Exception> rpcErrorTable = null;
				rpcErrorTable = (from server in serversToContact
				let possibleEx = this.m_statusFetcher.GetPossibleExceptionForServer(server)
				where possibleEx != null
				select new KeyValuePair<AmServerName, Exception>(server, possibleEx)).ToDictionary((KeyValuePair<AmServerName, Exception> kvp) => kvp.Key, (KeyValuePair<AmServerName, Exception> kvp) => kvp.Value);
				IEnumerable<KeyValuePair<AmServerName, Exception>> second = from kvp in cachedEntryTable
				where !rpcErrorTable.ContainsKey(kvp.Key) && kvp.Value != null && kvp.Value.CopyStatus == null
				select new KeyValuePair<AmServerName, Exception>(kvp.Key, kvp.Value.LastException);
				IEnumerable<KeyValuePair<AmServerName, Exception>> rpcErrors = rpcErrorTable.Concat(second);
				AmBestCopySelection.ReportRpcErrors(rpcErrors, this.m_bcsContext);
				IEnumerable<CopyStatusClientCachedEntry> source = from server in serversToContact
				let possibleEx = this.m_statusFetcher.GetPossibleExceptionForServer(server)
				where possibleEx == null && cachedEntryTable.ContainsKey(server) && cachedEntryTable[server].CopyStatus != null
				select cachedEntryTable[server];
				dictionary = source.ToDictionary((CopyStatusClientCachedEntry entry) => entry.ServerContacted, (CopyStatusClientCachedEntry entry) => entry.CopyStatus);
			}
			else
			{
				dictionary = new Dictionary<AmServerName, RpcDatabaseCopyStatus2>();
			}
			if (dictionary.Count == 0)
			{
				ReplayCrimsonEvents.BcsDbNodeCopyStatusRpcFailed.Log<string, Guid>(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_bcsContext.DatabaseGuid);
			}
			return dictionary;
		}

		internal static void ReportRpcErrors(IEnumerable<KeyValuePair<AmServerName, Exception>> rpcErrors, AmBcsContext bcsContext)
		{
			foreach (KeyValuePair<AmServerName, Exception> keyValuePair in rpcErrors)
			{
				AmServerName key = keyValuePair.Key;
				Exception value = keyValuePair.Value;
				if (value != null)
				{
					bcsContext.ErrorLogger.ReportServerFailure(key, "CopyStatusRpcCheck", value.Message, ReplayCrimsonEvents.BcsDbSpecificNodeCopyStatusRpcFailed, new object[]
					{
						bcsContext.GetDatabaseNameOrGuid(),
						bcsContext.DatabaseGuid,
						key,
						value.Message
					});
				}
			}
		}

		private AmBcsContext m_bcsContext;

		private AmMultiNodeCopyStatusFetcher m_statusFetcher;

		private AmBestCopySelector m_bestCopySelector;

		private AmConfig m_amConfig;

		private DatabaseMountDialOverride m_mountDialOverride;

		private IADDatabaseCopy[] m_dbCopies;

		private List<IADServer> m_miniServers;

		private Stopwatch m_bcsWatch = new Stopwatch();

		private BcsPerformanceTracker m_perfTracker;
	}
}
