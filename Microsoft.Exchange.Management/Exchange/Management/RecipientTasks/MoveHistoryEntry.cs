using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public sealed class MoveHistoryEntry
	{
		internal MoveHistoryEntry(MoveHistoryEntryInternal mhEntry, bool includeMoveReport)
		{
			this.Status = (RequestStatus)mhEntry.Status;
			this.Flags = (RequestFlags)mhEntry.Flags;
			this.SourceDatabase = ADObjectIdXML.Deserialize(mhEntry.SourceDatabase);
			this.SourceVersion = new ServerVersion(mhEntry.SourceVersion);
			this.SourceServer = mhEntry.SourceServer;
			this.SourceArchiveDatabase = ADObjectIdXML.Deserialize(mhEntry.SourceArchiveDatabase);
			this.SourceArchiveVersion = new ServerVersion(mhEntry.SourceArchiveVersion);
			this.SourceArchiveServer = mhEntry.SourceArchiveServer;
			this.TargetDatabase = ADObjectIdXML.Deserialize(mhEntry.DestinationDatabase);
			this.TargetVersion = new ServerVersion(mhEntry.DestinationVersion);
			this.TargetServer = mhEntry.DestinationServer;
			this.TargetArchiveDatabase = ADObjectIdXML.Deserialize(mhEntry.DestinationArchiveDatabase);
			this.TargetArchiveVersion = new ServerVersion(mhEntry.DestinationArchiveVersion);
			this.TargetArchiveServer = mhEntry.DestinationArchiveServer;
			this.RemoteHostName = mhEntry.RemoteHostName;
			this.RemoteCredentialUsername = mhEntry.RemoteCredentialUserName;
			this.RemoteDatabaseName = mhEntry.RemoteDatabaseName;
			this.RemoteArchiveDatabaseName = mhEntry.RemoteArchiveDatabaseName;
			this.BadItemLimit = mhEntry.BadItemLimit;
			this.BadItemsEncountered = mhEntry.BadItemsEncountered;
			this.LargeItemLimit = mhEntry.LargeItemLimit;
			this.LargeItemsEncountered = mhEntry.LargeItemsEncountered;
			this.MRSServerName = mhEntry.MRSServerName;
			this.TotalMailboxSize = new ByteQuantifiedSize(mhEntry.TotalMailboxSize);
			this.TotalMailboxItemCount = mhEntry.TotalMailboxItemCount;
			this.TotalArchiveSize = ((mhEntry.TotalArchiveSize != null) ? new ByteQuantifiedSize?(new ByteQuantifiedSize(mhEntry.TotalArchiveSize.Value)) : null);
			this.TotalArchiveItemCount = mhEntry.TotalArchiveItemCount;
			this.TargetDeliveryDomain = mhEntry.TargetDeliveryDomain;
			this.ArchiveDomain = mhEntry.ArchiveDomain;
			this.FailureCode = mhEntry.FailureCode;
			this.FailureType = mhEntry.FailureType;
			this.Message = CommonUtils.ByteDeserialize(mhEntry.MessageData);
			this.timeTracker = mhEntry.TimeTracker;
			if (includeMoveReport)
			{
				this.Report = mhEntry.Report;
			}
		}

		public RequestStatus Status { get; private set; }

		public RequestFlags Flags { get; private set; }

		public RequestStyle RequestStyle
		{
			get
			{
				if ((this.Flags & RequestFlags.CrossOrg) == RequestFlags.None)
				{
					return RequestStyle.IntraOrg;
				}
				return RequestStyle.CrossOrg;
			}
		}

		public RequestDirection Direction
		{
			get
			{
				if ((this.Flags & RequestFlags.Push) == RequestFlags.None)
				{
					return RequestDirection.Pull;
				}
				return RequestDirection.Push;
			}
		}

		public bool IsOffline
		{
			get
			{
				return (this.Flags & RequestFlags.Offline) != RequestFlags.None;
			}
		}

		public ADObjectId SourceDatabase { get; private set; }

		public ServerVersion SourceVersion { get; private set; }

		public string SourceServer { get; private set; }

		public ADObjectId SourceArchiveDatabase { get; private set; }

		public ServerVersion SourceArchiveVersion { get; private set; }

		public string SourceArchiveServer { get; private set; }

		public ADObjectId TargetDatabase { get; private set; }

		public ServerVersion TargetVersion { get; private set; }

		public string TargetServer { get; private set; }

		public ADObjectId TargetArchiveDatabase { get; private set; }

		public ServerVersion TargetArchiveVersion { get; private set; }

		public string TargetArchiveServer { get; private set; }

		public string RemoteHostName { get; private set; }

		public string RemoteCredentialUsername
		{
			get
			{
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					string text;
					string text2;
					return SuppressingPiiData.Redact(this.remoteCredentialUsername, out text, out text2);
				}
				return this.remoteCredentialUsername;
			}
			private set
			{
				this.remoteCredentialUsername = value;
			}
		}

		public string RemoteDatabaseName { get; private set; }

		public string RemoteArchiveDatabaseName { get; private set; }

		public Unlimited<int> BadItemLimit { get; private set; }

		public int BadItemsEncountered { get; private set; }

		public Unlimited<int> LargeItemLimit { get; private set; }

		public int LargeItemsEncountered { get; private set; }

		public DateTime? QueuedTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.Creation);
			}
		}

		public DateTime? StartTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.Start);
			}
		}

		public DateTime? InitialSeedingCompletedTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.InitialSeedingCompleted);
			}
		}

		public DateTime? FinalSyncTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.FinalSync);
			}
		}

		public DateTime? CompletionTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.Completion);
			}
		}

		public EnhancedTimeSpan? OverallDuration
		{
			get
			{
				return this.GetDuration(RequestState.OverallMove);
			}
		}

		public EnhancedTimeSpan? TotalFinalizationDuration
		{
			get
			{
				return this.GetDuration(RequestState.Finalization);
			}
		}

		public EnhancedTimeSpan? TotalSuspendedDuration
		{
			get
			{
				return this.GetDuration(RequestState.Suspended);
			}
		}

		public EnhancedTimeSpan? TotalFailedDuration
		{
			get
			{
				return this.GetDuration(RequestState.Failed);
			}
		}

		public EnhancedTimeSpan? TotalQueuedDuration
		{
			get
			{
				return this.GetDuration(RequestState.Queued);
			}
		}

		public EnhancedTimeSpan? TotalInProgressDuration
		{
			get
			{
				return this.GetDuration(RequestState.InProgress);
			}
		}

		public EnhancedTimeSpan? TotalStalledDueToHADuration
		{
			get
			{
				return this.GetDuration(RequestState.StalledDueToHA);
			}
		}

		public EnhancedTimeSpan? TotalTransientFailureDuration
		{
			get
			{
				return this.GetDuration(RequestState.TransientFailure);
			}
		}

		public string MRSServerName { get; private set; }

		public ByteQuantifiedSize TotalMailboxSize { get; private set; }

		public ulong TotalMailboxItemCount { get; private set; }

		public ByteQuantifiedSize? TotalArchiveSize { get; private set; }

		public ulong? TotalArchiveItemCount { get; private set; }

		public string TargetDeliveryDomain { get; private set; }

		public string ArchiveDomain { get; private set; }

		public int? FailureCode { get; private set; }

		public string FailureType { get; private set; }

		public LocalizedString Message { get; private set; }

		public DateTime? FailureTimestamp
		{
			get
			{
				return this.GetTimestamp(RequestJobTimestamp.Failure);
			}
		}

		public Report Report { get; private set; }

		public override string ToString()
		{
			string dbName;
			if (this.TargetDatabase != null)
			{
				dbName = this.TargetDatabase.ToString();
			}
			else
			{
				dbName = this.RemoteDatabaseName;
			}
			if (this.Status == RequestStatus.Completed)
			{
				return Strings.CompletedMoveHistoryEntry(this.CompletionTimestamp.ToString(), dbName, this.TotalMailboxSize.ToString(), this.OverallDuration.ToString());
			}
			if (this.Status == RequestStatus.Failed)
			{
				return Strings.FailedMoveHistoryEntry(this.FailureTimestamp.ToString(), dbName, this.Message);
			}
			if (this.Status == RequestStatus.CompletedWithWarning)
			{
				return Strings.CompletedWithWarningMoveHistoryEntry(this.CompletionTimestamp.ToString(), dbName, this.TotalMailboxSize.ToString(), this.OverallDuration.ToString(), this.Message);
			}
			return Strings.CanceledMoveHistoryEntry(this.CompletionTimestamp.ToString(), dbName);
		}

		internal static List<MoveHistoryEntry> LoadMoveHistory(Guid mailboxGuid, Guid mdbGuid, bool includeMoveReport, UserMailboxFlags flags)
		{
			List<MoveHistoryEntryInternal> list = MoveHistoryEntryInternal.LoadMoveHistory(mailboxGuid, mdbGuid, flags);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<MoveHistoryEntry> list2 = new List<MoveHistoryEntry>(list.Count);
			foreach (MoveHistoryEntryInternal mhEntry in list)
			{
				list2.Add(new MoveHistoryEntry(mhEntry, includeMoveReport));
			}
			return list2;
		}

		private DateTime? GetTimestamp(RequestJobTimestamp type)
		{
			if (this.timeTracker == null)
			{
				return null;
			}
			return this.timeTracker.GetDisplayTimestamp(type);
		}

		private EnhancedTimeSpan? GetDuration(RequestState type)
		{
			if (this.timeTracker == null)
			{
				return null;
			}
			return new EnhancedTimeSpan?(this.timeTracker.GetDisplayDuration(type).Duration);
		}

		private RequestJobTimeTracker timeTracker;

		private string remoteCredentialUsername;
	}
}
