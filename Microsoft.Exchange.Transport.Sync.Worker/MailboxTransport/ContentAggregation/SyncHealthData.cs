using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncHealthData
	{
		public SyncHealthData()
		{
			this.ThrottlingStatistics = new ThrottlingStatistics();
			this.Exceptions = new List<Exception>();
			this.CloudStatistics = new CloudStatistics();
		}

		public int TotalItemAddsEnumeratedFromRemoteServer { get; set; }

		public int TotalItemAddsAppliedToLocalServer { get; set; }

		public int TotalItemChangesEnumeratedFromRemoteServer { get; set; }

		public int TotalItemChangesAppliedToLocalServer { get; set; }

		public int TotalItemDeletesEnumeratedFromRemoteServer { get; set; }

		public int TotalItemDeletesAppliedToLocalServer { get; set; }

		public int TotalFolderAddsEnumeratedFromRemoteServer { get; set; }

		public int TotalFolderAddsAppliedToLocalServer { get; set; }

		public int TotalFolderChangesEnumeratedFromRemoteServer { get; set; }

		public int TotalFolderChangesAppliedToLocalServer { get; set; }

		public int TotalFolderDeletesEnumeratedFromRemoteServer { get; set; }

		public int TotalFolderDeletesAppliedToLocalServer { get; set; }

		public int TotalItemAddsPermanentExceptions { get; set; }

		public int TotalItemAddsTransientExceptions { get; set; }

		public int TotalItemDeletesPermanentExceptions { get; set; }

		public int TotalItemDeletesTransientExceptions { get; set; }

		public int TotalItemSoftDeletesPermanentExceptions { get; set; }

		public int TotalItemSoftDeletesTransientExceptions { get; set; }

		public int TotalItemChangesPermanentExceptions { get; set; }

		public int TotalItemChangesTransientExceptions { get; set; }

		public int TotalFolderAddsPermanentExceptions { get; set; }

		public int TotalFolderAddsTransientExceptions { get; set; }

		public int TotalFolderDeletesPermanentExceptions { get; set; }

		public int TotalFolderDeletesTransientExceptions { get; set; }

		public int TotalFolderSoftDeletesPermanentExceptions { get; set; }

		public int TotalFolderSoftDeletesTransientExceptions { get; set; }

		public int TotalFolderChangesPermanentExceptions { get; set; }

		public int TotalFolderChangesTransientExceptions { get; set; }

		public TimeSpan SyncDuration { get; set; }

		public bool RecoverySync { get; set; }

		public bool IsPermanentSyncError { get; set; }

		public bool IsTransientSyncError { get; set; }

		public int PoisonItemErrorsCount { get; set; }

		public int OverSizeItemErrorsCount { get; set; }

		public int UnresolveableFolderNameErrorsCount { get; set; }

		public int ObjectNotFoundErrorsCount { get; set; }

		public int OtherItemErrorsCount { get; set; }

		public int PermanentItemErrorsCount { get; set; }

		public int TransientItemErrorsCount { get; set; }

		public int PermanentFolderErrorsCount { get; set; }

		public int TransientFolderErrorsCount { get; set; }

		public ByteQuantifiedSize TotalBytesEnumeratedFromRemoteServer { get; set; }

		public int TotalSuccessfulRemoteRoundtrips { get; set; }

		public TimeSpan AverageSuccessfulRemoteRoundtripTime { get; set; }

		public int TotalUnsuccessfulRemoteRoundtrips { get; set; }

		public TimeSpan AverageUnsuccessfulRemoteRoundtripTime { get; set; }

		public int TotalSuccessfulEngineRoundtrips { get; set; }

		public TimeSpan AverageSuccessfulEngineRoundtripTime { get; set; }

		public int TotalUnsuccessfulEngineRoundtrips { get; set; }

		public TimeSpan AverageUnsuccessfulEngineRoundtripTime { get; set; }

		public TimeSpan AverageEngineBackoffTime { get; set; }

		public int TotalSuccessfulNativeRoundtrips { get; set; }

		public TimeSpan AverageSuccessfulNativeRoundtripTime { get; set; }

		public int TotalUnsuccessfulNativeRoundtrips { get; set; }

		public TimeSpan AverageUnsuccessfulNativeRoundtripTime { get; set; }

		public TimeSpan AverageNativeBackoffTime { get; set; }

		public List<Exception> Exceptions { get; set; }

		public ThrottlingStatistics ThrottlingStatistics { get; set; }

		public CloudStatistics CloudStatistics { get; set; }

		public Exception SyncEngineException { get; set; }

		public int TotalItemsSubmittedToTransport { get; set; }
	}
}
