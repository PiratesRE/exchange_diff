using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmSingleCopySelection : IBestCopySelector
	{
		public AmSingleCopySelection(Guid dbGuid, IADDatabase database, AmDbActionCode actionCode, AmServerName sourceServerName, AmServerName targetServerName, AmConfig amConfig, AmBcsSkipFlags skipValidationChecks, AmDbAction.PrepareSubactionArgsDelegate prepareSubaction)
		{
			this.m_amConfig = amConfig;
			this.m_targetServerName = targetServerName;
			IAmBcsErrorLogger errorLogger = new AmBcsServerFailureLogger(dbGuid, database.Name, true);
			this.m_bcsContext = new AmBcsContext(dbGuid, sourceServerName, errorLogger);
			this.m_bcsContext.Database = database;
			this.m_bcsContext.ActionCode = actionCode;
			this.m_bcsContext.SkipValidationChecks = skipValidationChecks;
			this.m_bcsContext.PrepareSubaction = prepareSubaction;
			this.m_perfTracker = new BcsPerformanceTracker(prepareSubaction);
		}

		public AmBcsType BestCopySelectionType
		{
			get
			{
				return AmBcsType.SingleCopySelection;
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
				return this.m_lastException;
			}
		}

		public AmServerName FindNextBestCopy()
		{
			AmServerName amServerName = null;
			LocalizedString error = LocalizedString.Empty;
			bool flag = false;
			AmBcsChecks amBcsChecks = AmBcsChecks.None;
			if (!this.m_fInitialized)
			{
				this.m_bcsWatch.Start();
				try
				{
					if (this.m_bcsContext.ShouldLogSubactionEvent)
					{
						ReplayCrimsonEvents.BcsInitiated.LogGeneric(this.m_bcsContext.PrepareSubaction(new object[]
						{
							this.BestCopySelectionType,
							false
						}));
					}
					bool fDbNeverMounted = false;
					this.m_perfTracker.RunTimedOperation(BcsOperation.HasDatabaseBeenMounted, delegate
					{
						fDbNeverMounted = !AmBestCopySelectionHelper.HasDatabaseBeenMounted(this.m_bcsContext.DatabaseGuid, this.m_amConfig);
						this.m_bcsContext.DatabaseNeverMounted = fDbNeverMounted;
					});
					AmBcsServerValidation serverValidator = new AmBcsServerValidation(this.m_targetServerName, this.m_bcsContext.SourceServerName, this.m_bcsContext.Database, this.m_amConfig, this.m_bcsContext.ErrorLogger, null);
					AmBcsServerChecks serverChecks = AmBcsServerValidation.GetServerValidationChecks(this.m_bcsContext.ActionCode, true);
					bool serverChecksPassed = false;
					this.m_perfTracker.RunTimedOperation(BcsOperation.DetermineServersToContact, delegate
					{
						serverChecksPassed = serverValidator.RunChecks(serverChecks, ref error);
					});
					if (!serverChecksPassed)
					{
						goto IL_39C;
					}
					if (!fDbNeverMounted && !this.m_bcsContext.ActionCode.IsMountOrRemountOperation)
					{
						List<AmServerName> serversToContact = new List<AmServerName>(2);
						serversToContact.Add(this.m_targetServerName);
						if (!AmServerName.IsEqual(this.m_bcsContext.SourceServerName, this.m_targetServerName) && (this.m_bcsContext.SkipValidationChecks & AmBcsSkipFlags.SkipActiveCopyChecks) == AmBcsSkipFlags.None)
						{
							serversToContact.Add(this.m_bcsContext.SourceServerName);
						}
						this.m_perfTracker.RunTimedOperation(BcsOperation.GetCopyStatusRpc, delegate
						{
							this.ConstructBcsStatusTable(serversToContact);
						});
						AmBcsServerFailureLogger amBcsServerFailureLogger = this.m_bcsContext.ErrorLogger as AmBcsServerFailureLogger;
						string concatenatedErrorString = amBcsServerFailureLogger.GetConcatenatedErrorString();
						if (concatenatedErrorString != null)
						{
							error = new LocalizedString(concatenatedErrorString);
							goto IL_39C;
						}
					}
					this.m_fInitialized = true;
				}
				finally
				{
					this.m_bcsWatch.Stop();
					this.m_perfTracker.RecordDuration(BcsOperation.BcsOverall, this.m_bcsWatch.Elapsed);
					this.m_perfTracker.LogEvent();
				}
			}
			if (this.TargetHasBeenTried(ref error))
			{
				this.m_bcsContext.ErrorLogger.ReportServerFailure(this.m_targetServerName, "CopyHasBeenTriedCheck", error, false);
			}
			else
			{
				if (this.m_bcsContext.ActionCode.IsMountOrRemountOperation)
				{
					amBcsChecks = AmBcsChecks.None;
					AmTrace.Debug("BCS: FindNextBestCopy: Skipping validation checks for Database '{0}' on server '{1}'.", new object[]
					{
						this.m_bcsContext.GetDatabaseNameOrGuid(),
						this.m_targetServerName
					});
				}
				else if (this.m_bcsContext.DatabaseNeverMounted)
				{
					amBcsChecks = AmBcsChecks.IsPassiveCopy;
					AmTrace.Debug("BCS: FindNextBestCopy: Database '{0}' has never been mounted. Running non-status related checks.", new object[]
					{
						this.m_bcsContext.GetDatabaseNameOrGuid()
					});
				}
				else
				{
					if (!this.CheckActiveForMove(ref error))
					{
						goto IL_39C;
					}
					amBcsChecks = (AmBcsChecks.IsHealthyOrDisconnected | AmBcsChecks.IsCatalogStatusHealthy | AmBcsChecks.CopyQueueLength | AmBcsChecks.ReplayQueueLength | AmBcsChecks.IsPassiveCopy | AmBcsChecks.IsSeedingSource | AmBcsChecks.TotalQueueLengthMaxAllowed | AmBcsChecks.MaxActivesUnderPreferredLimit);
				}
				RpcDatabaseCopyStatus2 copyStatus = null;
				this.m_bcsContext.StatusTable.TryGetValue(this.m_targetServerName, out copyStatus);
				AmBcsCopyValidation amBcsCopyValidation = new AmBcsCopyValidation(this.m_bcsContext.DatabaseGuid, this.m_bcsContext.GetDatabaseNameOrGuid(), amBcsChecks, this.m_bcsContext.SourceServerName, this.m_targetServerName, copyStatus, this.m_bcsContext.ErrorLogger, this.m_bcsContext.SkipValidationChecks, this.m_bcsContext.ComponentStateWrapper);
				flag = amBcsCopyValidation.RunChecks(ref error);
				amBcsChecks = amBcsCopyValidation.CompletedChecks;
			}
			IL_39C:
			if (flag)
			{
				AmTrace.Info("BCS: FindNextBestCopy: DatabaseCopy: '{0}\\{1}' passed validation checks.", new object[]
				{
					this.m_bcsContext.GetDatabaseNameOrGuid(),
					this.m_targetServerName.NetbiosName
				});
				amServerName = this.m_targetServerName;
				this.m_serverAlreadyTried = this.m_targetServerName;
				ReplayCrimsonEvents.BcsDbMoveChecksPassed.Log<string, Guid, AmServerName, AmBcsChecks>(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_bcsContext.DatabaseGuid, amServerName, amBcsChecks);
			}
			else
			{
				AmTrace.Error("BCS: FindNextBestCopy: DatabaseCopy: '{0}\\{1}'. Checks returned error: {2}", new object[]
				{
					this.m_bcsContext.GetDatabaseNameOrGuid(),
					this.m_targetServerName.NetbiosName,
					error
				});
				AmBcsServerFailureLogger amBcsServerFailureLogger2 = this.m_bcsContext.ErrorLogger as AmBcsServerFailureLogger;
				string concatenatedErrorString2 = amBcsServerFailureLogger2.GetConcatenatedErrorString();
				this.m_lastException = new AmBcsSingleCopyValidationException(concatenatedErrorString2);
				ReplayCrimsonEvents.BcsDbMoveChecksFailed.Log<string, Guid, AmServerName, AmBcsChecks, LocalizedString>(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_bcsContext.DatabaseGuid, this.m_targetServerName, amBcsChecks, error);
			}
			return amServerName;
		}

		private bool TargetHasBeenTried(ref LocalizedString error)
		{
			if (!AmServerName.IsNullOrEmpty(this.m_serverAlreadyTried))
			{
				AmTrace.Debug("BCS: Target server '{0}' has already been tried for database '{1}'.", new object[]
				{
					this.m_serverAlreadyTried.NetbiosName,
					this.m_bcsContext.GetDatabaseNameOrGuid()
				});
				error = ReplayStrings.AmBcsDatabaseCopyAlreadyTried(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_serverAlreadyTried.ToString());
				return true;
			}
			return false;
		}

		private bool CheckActiveForMove(ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus;
			if (this.m_bcsContext.ActionCode.IsAdminMoveOperation && this.m_bcsContext.SourceServerName != this.m_targetServerName && (this.m_bcsContext.SkipValidationChecks & AmBcsSkipFlags.SkipActiveCopyChecks) == AmBcsSkipFlags.None && this.m_bcsContext.StatusTable.TryGetValue(this.m_bcsContext.SourceServerName, out rpcDatabaseCopyStatus) && rpcDatabaseCopyStatus.SeedingSourceForDB)
			{
				error = ReplayStrings.AmBcsActiveCopyIsSeedingSource(this.m_bcsContext.GetDatabaseNameOrGuid(), this.m_bcsContext.SourceServerName.ToString());
				this.m_bcsContext.ErrorLogger.ReportServerFailure(this.m_bcsContext.SourceServerName, "ActiveCopyIsSeedingSource", error);
				return false;
			}
			return true;
		}

		private void ConstructBcsStatusTable(List<AmServerName> serversToContact)
		{
			Dictionary<AmServerName, RpcHealthStateInfo[]> stateInfoMap = null;
			Dictionary<AmServerName, CopyStatusClientCachedEntry> cachedEntryTable = null;
			this.m_statusFetcher = new AmMultiNodeCopyStatusFetcher(serversToContact, new Guid[]
			{
				this.m_bcsContext.DatabaseGuid
			}, null, RpcGetDatabaseCopyStatusFlags2.ReadThrough, null, true);
			Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> status = this.m_statusFetcher.GetStatus(out stateInfoMap);
			Dictionary<AmServerName, RpcDatabaseCopyStatus2> statusTable;
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
				statusTable = source.ToDictionary((CopyStatusClientCachedEntry entry) => entry.ServerContacted, (CopyStatusClientCachedEntry entry) => entry.CopyStatus);
			}
			else
			{
				statusTable = new Dictionary<AmServerName, RpcDatabaseCopyStatus2>();
			}
			this.m_bcsContext.StatusTable = statusTable;
			this.m_bcsContext.ComponentStateWrapper = new ComponentStateWrapper(this.m_bcsContext.Database.Name, this.m_bcsContext.InitiatingComponent, this.m_bcsContext.SourceServerName, this.m_bcsContext.ActionCode, stateInfoMap);
		}

		private bool m_fInitialized;

		private AmBcsContext m_bcsContext;

		private AmServerName m_targetServerName;

		private AmConfig m_amConfig;

		private AmMultiNodeCopyStatusFetcher m_statusFetcher;

		private AmServerName m_serverAlreadyTried;

		private Exception m_lastException;

		private Stopwatch m_bcsWatch = new Stopwatch();

		private BcsPerformanceTracker m_perfTracker;
	}
}
