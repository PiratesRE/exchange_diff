using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[OutputType(new Type[]
	{
		typeof(DatabaseCopyStatusEntry)
	})]
	[Cmdlet("Get", "MailboxDatabaseCopyStatus", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxDatabaseCopyStatus : GetSystemConfigurationObjectTask<DatabaseCopyIdParameter, DatabaseCopy>
	{
		[Parameter(Mandatory = true, ParameterSetName = "ExplicitServer", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public MailboxServerIdParameter Server
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Active
		{
			get
			{
				return (SwitchParameter)(base.Fields["Active"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Active"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Local
		{
			get
			{
				return (SwitchParameter)(base.Fields["Local"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Local"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ConnectionStatus
		{
			get
			{
				return (SwitchParameter)(base.Fields["ConnectionStatus"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ConnectionStatus"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ExtendedErrorInfo
		{
			get
			{
				return (SwitchParameter)(base.Fields["ExtendedErrorInfo"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ExtendedErrorInfo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseServerCache
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseServerCache"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseServerCache"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			if (e is ClusterException)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<Exception>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: ClusCommonFailException: {0}", e);
				return true;
			}
			if (e is TransientException)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<Exception>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: TransientException: {0}", e);
				return true;
			}
			if (e is ADExternalException)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<Exception>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: ADExternalException: {0}", e);
				return true;
			}
			return base.IsKnownException(e);
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return base.InternalFilter;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.Identity == null && this.Server == null)
			{
				this.Server = MailboxServerIdParameter.Parse(Environment.MachineName);
			}
			MailboxServerIdParameter mailboxServerIdParameter = null;
			if (this.Server != null)
			{
				mailboxServerIdParameter = this.Server;
			}
			else if (this.Local)
			{
				mailboxServerIdParameter = MailboxServerIdParameter.Parse(Environment.MachineName);
			}
			if (mailboxServerIdParameter != null)
			{
				this.m_server = (Server)base.GetDataObject<Server>(mailboxServerIdParameter, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxServerNotFound(mailboxServerIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(mailboxServerIdParameter.ToString())));
				if (base.HasErrors || this.m_server == null)
				{
					base.WriteError(new CouldNotFindServerObject(mailboxServerIdParameter.ToString()), ErrorCategory.InvalidOperation, mailboxServerIdParameter);
					return;
				}
				if (!this.m_server.IsE14OrLater)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorServerNotE14OrLater(mailboxServerIdParameter.ToString())), ErrorCategory.InvalidOperation, mailboxServerIdParameter);
				}
				if (!this.m_server.IsMailboxServer)
				{
					base.WriteError(this.m_server.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, mailboxServerIdParameter);
					return;
				}
				string arg = (this.Identity == null) ? "*" : this.Identity.DatabaseName;
				string text = string.Format("{0}{1}{2}", arg, '\\', this.m_server.Name);
				if (this.Identity != null && !string.IsNullOrEmpty(this.Identity.ServerName) && this.Identity.ServerName != "*")
				{
					this.WriteWarning(Strings.GetDbcsOverridingServer(this.Identity.ServerName, this.m_server.Name, text));
				}
				this.Identity = DatabaseCopyIdParameter.Parse(text);
			}
			this.rootId = new DatabasesContainer().Id;
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new NotFilter(new ExistsFilter(DatabaseCopySchema.ParentObjectClass)),
				new TextFilter(DatabaseCopySchema.ParentObjectClass, MailboxDatabase.MostDerivedClass, MatchOptions.FullString, MatchFlags.IgnoreCase)
			});
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				queryFilter = QueryFilter.OrTogether(new QueryFilter[]
				{
					queryFilter,
					new TextFilter(DatabaseCopySchema.ParentObjectClass, PublicFolderDatabase.MostDerivedClass, MatchOptions.FullString, MatchFlags.IgnoreCase)
				});
			}
			this.Identity.SetAdditionalQueryFilter(queryFilter);
			this.Identity.AllowInvalid = true;
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObjects
			});
			Dictionary<string, Collection<DatabaseCopy>> dictionary = new Dictionary<string, Collection<DatabaseCopy>>();
			foreach (T t in dataObjects)
			{
				IConfigurable configurable = t;
				DatabaseCopy databaseCopy = configurable as DatabaseCopy;
				string key;
				if (this.m_server != null)
				{
					key = this.m_server.Name;
				}
				else
				{
					key = databaseCopy.HostServerName;
				}
				Collection<DatabaseCopy> collection;
				if (!dictionary.TryGetValue(key, out collection))
				{
					collection = new Collection<DatabaseCopy>();
					dictionary.Add(key, collection);
				}
				collection.Add(databaseCopy);
			}
			using (Dictionary<string, Collection<DatabaseCopy>>.KeyCollection.Enumerator enumerator2 = dictionary.Keys.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string dbCopyServerName = enumerator2.Current;
					Collection<DatabaseCopy> collection2 = dictionary[dbCopyServerName];
					Server server2;
					if (this.m_server != null)
					{
						server2 = this.m_server;
					}
					else
					{
						MailboxServerIdParameter mailboxServerIdParameter = MailboxServerIdParameter.Parse(dbCopyServerName);
						IEnumerable<Server> objects = mailboxServerIdParameter.GetObjects<Server>(this.RootId, this.ConfigurationSession);
						server2 = objects.FirstOrDefault((Server server) => server.Name.Equals(dbCopyServerName, StringComparison.OrdinalIgnoreCase));
					}
					if (server2 != null)
					{
						if (ReplayRpcVersionControl.IsGetCopyStatusEx2RpcSupported(server2.AdminDisplayVersion))
						{
							DatabaseCopyStatusEntry[] array = this.PrepareStatusEntryFromRpc(server2, collection2);
							for (int i = 0; i < array.Length; i++)
							{
								this.WriteCopyStatusResultIfAppropriate(array[i]);
							}
						}
						else
						{
							this.WriteError(new InvalidOperationException(Strings.GetDbcsRpcNotSupported(server2.Name, server2.AdminDisplayVersion.ToString(), ReplayRpcVersionControl.GetCopyStatusEx2SupportVersion.ToString())), ErrorCategory.InvalidOperation, server2.Identity, false);
						}
					}
					else
					{
						foreach (DatabaseCopy databaseCopy2 in collection2)
						{
							DatabaseCopyStatusEntry databaseCopyStatusEntry = this.ConstructNewSatusEntry(databaseCopy2);
							this.UpdateMisconfiguredCopyStatusEntry(databaseCopyStatusEntry, databaseCopy2);
							this.WriteCopyStatusResultIfAppropriate(databaseCopyStatusEntry);
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		private void WriteCopyStatusResultIfAppropriate(DatabaseCopyStatusEntry statusEntry)
		{
			if (!this.Active || statusEntry.ActiveCopy)
			{
				this.WriteResult(statusEntry);
			}
		}

		private DatabaseCopyStatusEntry ConstructNewSatusEntry(DatabaseCopy databaseCopy)
		{
			return new DatabaseCopyStatusEntry
			{
				m_copyStatus = CopyStatus.Unknown,
				m_identity = databaseCopy.Id,
				m_name = databaseCopy.Identity.ToString(),
				m_databaseName = databaseCopy.DatabaseName,
				m_mailboxServer = databaseCopy.HostServerName
			};
		}

		private void UpdateMisconfiguredCopyStatusEntry(DatabaseCopyStatusEntry status, DatabaseCopy databaseCopy)
		{
			status.Status = CopyStatus.Misconfigured;
			ValidationError[] array = databaseCopy.ValidateRead();
			if (array.Length > 0)
			{
				Exception ex = new DataValidationException(array[0]);
				status.m_extendedErrorInfo = new ExtendedErrorInfo(ex);
				status.m_errorMessage = ex.Message;
			}
		}

		private void UpdateCopyStatusNoReplicaInstance(DatabaseCopyStatusEntry status)
		{
			status.Status = CopyStatus.Unknown;
			Exception ex = new ReplayServiceRpcUnknownInstanceException();
			status.m_extendedErrorInfo = new ExtendedErrorInfo(ex);
			status.m_errorMessage = ex.Message;
		}

		private DatabaseCopyStatusEntry[] PrepareStatusEntryFromRpc(Server server, Collection<DatabaseCopy> databaseCopies)
		{
			DatabaseCopyStatusEntry[] array = new DatabaseCopyStatusEntry[databaseCopies.Count];
			Guid[] array2 = new Guid[databaseCopies.Count];
			for (int i = 0; i < databaseCopies.Count; i++)
			{
				DatabaseCopy databaseCopy3 = databaseCopies[i];
				Database database = databaseCopy3.GetDatabase<Database>();
				array2[i] = database.Guid;
				array[i] = this.ConstructNewSatusEntry(databaseCopy3);
			}
			RpcGetDatabaseCopyStatusFlags2 rpcGetDatabaseCopyStatusFlags = RpcGetDatabaseCopyStatusFlags2.None;
			if (!this.UseServerCache)
			{
				rpcGetDatabaseCopyStatusFlags |= RpcGetDatabaseCopyStatusFlags2.ReadThrough;
			}
			try
			{
				IEnumerable<string> source = from databaseCopy in databaseCopies
				select databaseCopy.DatabaseName;
				base.WriteVerbose(Strings.GetDbcsRpcQuery(server.Fqdn, string.Join(",", source.ToArray<string>())));
				RpcDatabaseCopyStatus2[] copyStatus = ReplayRpcClientHelper.GetCopyStatus(server.Fqdn, rpcGetDatabaseCopyStatusFlags, array2);
				if (copyStatus != null && copyStatus.Length > 0)
				{
					base.WriteVerbose(Strings.GetDbcsRpcQueryAllDone(copyStatus.Length));
					Dictionary<Guid, RpcDatabaseCopyStatus2> dictionary = new Dictionary<Guid, RpcDatabaseCopyStatus2>(copyStatus.Length);
					for (int j = 0; j < copyStatus.Length; j++)
					{
						dictionary[copyStatus[j].DBGuid] = copyStatus[j];
					}
					for (int k = 0; k < array2.Length; k++)
					{
						RpcDatabaseCopyStatus2 copyStatus2;
						if (dictionary.TryGetValue(array2[k], out copyStatus2))
						{
							GetMailboxDatabaseCopyStatus.GetEntryFromStatus(copyStatus2, array[k]);
						}
						else if (!databaseCopies[k].IsValid)
						{
							ExTraceGlobals.CmdletsTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "GetCopyStatus() didn't find replica instance for database {0} on server {1}. The DatabaseCopy is misconfigured in the Active Directory!", array2[k], server.Fqdn);
							this.UpdateMisconfiguredCopyStatusEntry(array[k], databaseCopies[k]);
						}
						else
						{
							ExTraceGlobals.CmdletsTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "GetCopyStatus() didn't find replica instance for database {0} on server {1}.", array2[k], server.Fqdn);
							this.UpdateCopyStatusNoReplicaInstance(array[k]);
						}
					}
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: ArgumentException: {0}", ex);
				this.WriteError(ex, ErrorCategory.ReadError, null, false);
			}
			catch (TaskServerTransientException ex2)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<TaskServerTransientException>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: ArgumentException: {0}", ex2);
				this.WriteError(ex2, ErrorCategory.ReadError, null, false);
			}
			catch (TaskServerException ex3)
			{
				ExTraceGlobals.CmdletsTracer.TraceError<TaskServerException>((long)this.GetHashCode(), "GetMailboxDatabaseCopyStatus: ReplayServiceRpcException: {0}", ex3);
				if (ex3 is ReplayServiceDownException)
				{
					base.WriteVerbose(Strings.GetDbcsDetectReplayServiceDown(server.Fqdn, ex3.Message));
					for (int l = 0; l < array.Length; l++)
					{
						DatabaseCopyStatusEntry databaseCopyStatusEntry = array[l];
						DatabaseCopy databaseCopy2 = databaseCopies[l];
						if (!databaseCopy2.IsValid)
						{
							this.UpdateMisconfiguredCopyStatusEntry(databaseCopyStatusEntry, databaseCopy2);
						}
						else
						{
							databaseCopyStatusEntry.Status = CopyStatus.ServiceDown;
							databaseCopyStatusEntry.m_errorMessage = ex3.Message;
							databaseCopyStatusEntry.m_extendedErrorInfo = new ExtendedErrorInfo(ex3);
						}
					}
				}
				else if (ex3 is ReplayServiceRpcUnknownInstanceException)
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "GetCopyStatus() was not able to find any replica instances on server {0}.", server.Fqdn);
					for (int m = 0; m < array.Length; m++)
					{
						if (!databaseCopies[m].IsValid)
						{
							this.UpdateMisconfiguredCopyStatusEntry(array[m], databaseCopies[m]);
						}
						else
						{
							this.UpdateCopyStatusNoReplicaInstance(array[m]);
						}
					}
				}
				else
				{
					this.WriteError(ex3, ErrorCategory.ReadError, null, false);
				}
			}
			return array;
		}

		internal static void GetEntryFromStatus(RpcDatabaseCopyStatus2 copyStatus, DatabaseCopyStatusEntry entry)
		{
			CopyStatusEnum copyStatus2 = copyStatus.CopyStatus;
			CopyStatus copyStatus3 = GetMailboxDatabaseCopyStatus.TranslateRpcStatusToTaskStatus(copyStatus2);
			entry.Status = copyStatus3;
			entry.m_statusRetrievedTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.StatusRetrievedTime);
			entry.m_lastStatusTransitionTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastStatusTransitionTime);
			entry.InstanceStartTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.InstanceStartTime);
			entry.m_errorMessage = copyStatus.ErrorMessage;
			entry.m_errorEventId = ((copyStatus.ErrorEventId != 0U) ? new uint?(copyStatus.ErrorEventId) : null);
			entry.m_extendedErrorInfo = copyStatus.ExtendedErrorInfo;
			if (copyStatus3 == CopyStatus.Suspended || copyStatus3 == CopyStatus.FailedAndSuspended || copyStatus3 == CopyStatus.Seeding)
			{
				entry.m_suspendMessage = copyStatus.SuspendComment;
			}
			entry.m_singlePageRestore = copyStatus.SinglePageRestoreNumber;
			entry.m_mailboxServer = copyStatus.MailboxServer;
			entry.m_activeDatabaseCopy = copyStatus.ActiveDatabaseCopy;
			entry.m_actionInitiator = copyStatus.ActionInitiator;
			entry.ActiveCopy = copyStatus.IsActiveCopy();
			if (copyStatus.ActivationPreference > 0)
			{
				entry.ActivationPreference = new int?(copyStatus.ActivationPreference);
			}
			entry.IsLastCopyAvailabilityChecksPassed = new bool?(copyStatus.IsLastCopyAvailabilityChecksPassed);
			entry.IsLastCopyRedundancyChecksPassed = new bool?(copyStatus.IsLastCopyRedundancyChecksPassed);
			entry.LastCopyAvailabilityChecksPassedTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastCopyAvailabilityChecksPassedTime);
			entry.LastCopyRedundancyChecksPassedTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastCopyRedundancyChecksPassedTime);
			entry.DiskFreeSpacePercent = copyStatus.DiskFreeSpacePercent;
			entry.DiskFreeSpace = ByteQuantifiedSize.FromBytes(copyStatus.DiskFreeSpaceBytes);
			entry.DiskTotalSpace = ByteQuantifiedSize.FromBytes(copyStatus.DiskTotalSpaceBytes);
			entry.ExchangeVolumeMountPoint = copyStatus.ExchangeVolumeMountPoint;
			entry.DatabaseVolumeMountPoint = copyStatus.DatabaseVolumeMountPoint;
			entry.DatabaseVolumeName = copyStatus.DatabaseVolumeName;
			entry.DatabasePathIsOnMountedFolder = new bool?(copyStatus.DatabasePathIsOnMountedFolder);
			entry.LogVolumeMountPoint = copyStatus.LogVolumeMountPoint;
			entry.LogVolumeName = copyStatus.LogVolumeName;
			entry.LogPathIsOnMountedFolder = new bool?(copyStatus.LogPathIsOnMountedFolder);
			entry.LastDatabaseVolumeName = copyStatus.LastDatabaseVolumeName;
			entry.LastDatabaseVolumeNameTransitionTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastDatabaseVolumeNameTransitionTime);
			entry.VolumeInfoError = copyStatus.VolumeInfoLastError;
			entry.m_activationSuspended = copyStatus.ActivationSuspended;
			if (copyStatus.ActivationSuspended)
			{
				entry.m_suspendMessage = copyStatus.SuspendComment;
			}
			entry.m_latestAvailableLogTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LatestAvailableLogTime);
			entry.m_latestCopyNotificationTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastCopyNotifiedLogTime);
			entry.m_latestCopyTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastCopiedLogTime);
			entry.m_latestInspectorTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastInspectedLogTime);
			entry.m_latestReplayTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastReplayedLogTime);
			entry.m_latestLogGenerationNumber = copyStatus.LastLogGenerated;
			entry.m_copyNotificationGenerationNumber = copyStatus.LastLogCopyNotified;
			entry.m_copyGenerationNumber = copyStatus.LastLogCopied;
			entry.m_inspectorGenerationNumber = copyStatus.LastLogInspected;
			entry.m_replayGenerationNumber = copyStatus.LastLogReplayed;
			entry.m_contentIndexState = copyStatus.ContentIndexStatus;
			entry.m_contentIndexErrorMessage = copyStatus.ContentIndexErrorMessage;
			entry.m_contentIndexErrorCode = copyStatus.ContentIndexErrorCode;
			entry.m_contentIndexVersion = copyStatus.ContentIndexVersion;
			entry.m_contentIndexBacklog = copyStatus.ContentIndexBacklog;
			entry.m_contentIndexRetryQueueSize = copyStatus.ContentIndexRetryQueueSize;
			entry.m_contentIndexMailboxesToCrawl = copyStatus.ContentIndexMailboxesToCrawl;
			entry.m_contentIndexSeedingPercent = copyStatus.ContentIndexSeedingPercent;
			entry.m_contentIndexSeedingSource = copyStatus.ContentIndexSeedingSource;
			entry.m_logCopyQueueIncreasing = new bool?(copyStatus.CopyQueueNotKeepingUp);
			entry.m_logReplayQueueIncreasing = new bool?(copyStatus.ReplayQueueNotKeepingUp);
			entry.m_replaySuspended = new bool?(copyStatus.ReplaySuspended);
			entry.ResumeBlocked = new bool?(copyStatus.ResumeBlocked);
			entry.ReseedBlocked = new bool?(copyStatus.ReseedBlocked);
			if (copyStatus.WorkerProcessId != 0)
			{
				entry.m_workerProcessId = new int?(copyStatus.WorkerProcessId);
			}
			entry.LastLogInfoIsStale = copyStatus.LastLogInfoIsStale;
			entry.LastLogInfoFromCopierTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastLogInfoFromCopierTime);
			entry.LastLogInfoFromClusterTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LastLogInfoFromClusterTime);
			entry.LastLogInfoFromClusterGen = copyStatus.LastLogInfoFromClusterGen;
			entry.LowestLogPresent = copyStatus.LowestLogPresent;
			entry.m_logsReplayedSinceInstanceStart = new long?(copyStatus.LogsReplayedSinceInstanceStart);
			entry.m_logsCopiedSinceInstanceStart = new long?(copyStatus.LogsCopiedSinceInstanceStart);
			entry.ReplicationIsInBlockMode = copyStatus.ReplicationIsInBlockMode;
			entry.ActivationDisabledAndMoveNow = copyStatus.ActivationDisabledAndMoveNow;
			entry.AutoActivationPolicy = (DatabaseCopyAutoActivationPolicyType)copyStatus.AutoActivationPolicy;
			int num = copyStatus.MinimumSupportedDatabaseSchemaVersion;
			if (num > 0)
			{
				entry.MinimumSupportedDatabaseSchemaVersion = string.Format("{0}.{1}", num >> 16, num & 65535);
			}
			num = copyStatus.MaximumSupportedDatabaseSchemaVersion;
			if (num > 0)
			{
				entry.MaximumSupportedDatabaseSchemaVersion = string.Format("{0}.{1}", num >> 16, num & 65535);
			}
			num = copyStatus.RequestedDatabaseSchemaVersion;
			if (num > 0)
			{
				entry.RequestedDatabaseSchemaVersion = string.Format("{0}.{1}", num >> 16, num & 65535);
			}
			GetMailboxDatabaseCopyStatus.UpdateReplayLagStatus(entry, copyStatus);
			GetMailboxDatabaseCopyStatus.UpdateDatabaseSeedStatus(entry, copyStatus);
			GetMailboxDatabaseCopyStatus.UpdateBackupInfo(entry, copyStatus);
			GetMailboxDatabaseCopyStatus.UpdateDumpsterRequests(entry, copyStatus);
			if (copyStatus.OutgoingConnections != null)
			{
				entry.m_outgoingConnections = (ConnectionStatus[])Serialization.BytesToObject(copyStatus.OutgoingConnections);
			}
			if (copyStatus.IncomingLogCopyingNetwork != null)
			{
				entry.m_incomingLogCopyingNetwork = (ConnectionStatus)Serialization.BytesToObject(copyStatus.IncomingLogCopyingNetwork);
			}
			if (copyStatus.SeedingNetwork != null)
			{
				entry.m_seedingNetwork = (ConnectionStatus)Serialization.BytesToObject(copyStatus.SeedingNetwork);
			}
			entry.MaxLogToReplay = copyStatus.MaxLogToReplay;
		}

		internal static CopyStatus TranslateRpcStatusToTaskStatus(CopyStatusEnum statusEnum)
		{
			CopyStatus result = CopyStatus.Unknown;
			switch (statusEnum)
			{
			case CopyStatusEnum.Unknown:
				result = CopyStatus.Unknown;
				break;
			case CopyStatusEnum.Initializing:
				result = CopyStatus.Initializing;
				break;
			case CopyStatusEnum.Resynchronizing:
				result = CopyStatus.Resynchronizing;
				break;
			case CopyStatusEnum.DisconnectedAndResynchronizing:
				result = CopyStatus.DisconnectedAndResynchronizing;
				break;
			case CopyStatusEnum.DisconnectedAndHealthy:
				result = CopyStatus.DisconnectedAndHealthy;
				break;
			case CopyStatusEnum.Healthy:
				result = CopyStatus.Healthy;
				break;
			case CopyStatusEnum.Failed:
				result = CopyStatus.Failed;
				break;
			case CopyStatusEnum.FailedAndSuspended:
				result = CopyStatus.FailedAndSuspended;
				break;
			case CopyStatusEnum.Suspended:
				result = CopyStatus.Suspended;
				break;
			case CopyStatusEnum.Seeding:
				result = CopyStatus.Seeding;
				break;
			case CopyStatusEnum.Mounting:
				result = CopyStatus.Mounting;
				break;
			case CopyStatusEnum.Mounted:
				result = CopyStatus.Mounted;
				break;
			case CopyStatusEnum.Dismounting:
				result = CopyStatus.Dismounting;
				break;
			case CopyStatusEnum.Dismounted:
				result = CopyStatus.Dismounted;
				break;
			case CopyStatusEnum.NonExchangeReplication:
				result = CopyStatus.NonExchangeReplication;
				break;
			case CopyStatusEnum.SeedingSource:
				result = CopyStatus.SeedingSource;
				break;
			case CopyStatusEnum.Misconfigured:
				result = CopyStatus.Misconfigured;
				break;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for CopyStatusEnum: {0}", new object[]
				{
					statusEnum.ToString()
				});
				break;
			}
			return result;
		}

		private static void UpdateBackupInfo(DatabaseCopyStatusEntry entry, RpcDatabaseCopyStatus2 copyStatus)
		{
			entry.m_latestFullBackupTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LatestFullBackupTime);
			DateTime? dateTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LatestIncrementalBackupTime);
			if (dateTime != null && dateTime.Value >= entry.m_latestFullBackupTime)
			{
				entry.m_latestIncrementalBackupTime = dateTime;
			}
			dateTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LatestDifferentialBackupTime);
			if (dateTime != null && dateTime.Value >= entry.m_latestFullBackupTime)
			{
				entry.m_latestDifferentialBackupTime = dateTime;
			}
			entry.m_latestCopyBackupTime = DumpsterStatisticsEntry.ToNullableLocalDateTime(copyStatus.LatestCopyBackupTime);
			if (entry.m_latestFullBackupTime != null)
			{
				entry.m_snapshotLatestFullBackup = new bool?(copyStatus.SnapshotLatestFullBackup);
				entry.m_snapshotBackup = new bool?(copyStatus.SnapshotLatestFullBackup);
			}
			if (entry.m_latestIncrementalBackupTime != null)
			{
				entry.m_snapshotLatestIncrementalBackup = new bool?(copyStatus.SnapshotLatestIncrementalBackup);
			}
			if (entry.m_latestDifferentialBackupTime != null)
			{
				entry.m_snapshotLatestDifferentialBackup = new bool?(copyStatus.SnapshotLatestDifferentialBackup);
			}
			if (entry.m_latestCopyBackupTime != null)
			{
				entry.m_snapshotLatestCopyBackup = new bool?(copyStatus.SnapshotLatestCopyBackup);
			}
		}

		private static void UpdateDumpsterRequests(DatabaseCopyStatusEntry entry, RpcDatabaseCopyStatus2 copyStatus)
		{
			List<DumpsterRequestEntry> list = new List<DumpsterRequestEntry>();
			if (copyStatus.DumpsterRequired && !string.IsNullOrEmpty(copyStatus.DumpsterServers))
			{
				string[] array = copyStatus.DumpsterServers.Split(new char[]
				{
					','
				});
				if (array != null && array.Length > 0)
				{
					foreach (string server in array)
					{
						list.Add(new DumpsterRequestEntry(server, copyStatus.DumpsterStartTime, copyStatus.DumpsterEndTime));
					}
				}
			}
			entry.m_outstandingDumpsterRequests = new DumpsterRequestEntry[list.Count];
			list.CopyTo(entry.m_outstandingDumpsterRequests);
		}

		private static void UpdateReplayLagStatus(DatabaseCopyStatusEntry entry, RpcDatabaseCopyStatus2 copyStatus)
		{
			if (copyStatus.ReplayLagEnabled != ReplayLagEnabledEnum.Unknown)
			{
				ReplayLagStatus replayLagStatus = new ReplayLagStatus(copyStatus.ReplayLagEnabled == ReplayLagEnabledEnum.Enabled, copyStatus.ConfiguredReplayLagTime, copyStatus.ActualReplayLagTime, copyStatus.ReplayLagPercentage, GetMailboxDatabaseCopyStatus.ConvertLagPlayDownReason(copyStatus.ReplayLagPlayDownReason), copyStatus.ReplayLagDisabledReason);
				entry.m_replayLagStatus = replayLagStatus;
			}
		}

		private static void UpdateDatabaseSeedStatus(DatabaseCopyStatusEntry entry, RpcDatabaseCopyStatus2 copyStatus)
		{
			if (copyStatus.CopyStatus == CopyStatusEnum.Seeding)
			{
				DatabaseSeedStatus dbSeedStatus = new DatabaseSeedStatus(copyStatus.DbSeedingPercent, copyStatus.DbSeedingKBytesRead, copyStatus.DbSeedingKBytesWritten, copyStatus.DbSeedingKBytesReadPerSec, copyStatus.DbSeedingKBytesWrittenPerSec);
				entry.m_dbSeedStatus = dbSeedStatus;
			}
		}

		private static ReplayLagPlayDownReason ConvertLagPlayDownReason(ReplayLagPlayDownReasonEnum reason)
		{
			switch (reason)
			{
			case ReplayLagPlayDownReasonEnum.None:
				return ReplayLagPlayDownReason.None;
			case ReplayLagPlayDownReasonEnum.LagDisabled:
				return ReplayLagPlayDownReason.LagDisabled;
			case ReplayLagPlayDownReasonEnum.NotEnoughFreeSpace:
				return ReplayLagPlayDownReason.NotEnoughFreeSpace;
			case ReplayLagPlayDownReasonEnum.InRequiredRange:
				return ReplayLagPlayDownReason.LogsInRequiredRange;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for enum {0}", new object[]
				{
					reason.ToString()
				});
				return ReplayLagPlayDownReason.None;
			}
		}

		private ADObjectId rootId;

		private Server m_server;
	}
}
