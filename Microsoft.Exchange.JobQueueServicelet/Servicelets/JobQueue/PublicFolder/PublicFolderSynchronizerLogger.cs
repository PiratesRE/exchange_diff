using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.JobQueue.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderSynchronizerLogger : PublicFolderMailboxLogger
	{
		public PublicFolderSynchronizerLogger(PublicFolderSession publicFolderSession, FolderOperationCounter folderOperationCount, Guid correlationId) : base(publicFolderSession, "PublicFolderSyncInfo", "PublicFolderLastSyncCylceLog", new Guid?(correlationId))
		{
			ArgumentValidator.ThrowIfNull("folderOperationCount", folderOperationCount);
			this.logComponent = "PublicFolderSyncLog";
			this.logSuffixName = "PublicFolderSyncLog";
			this.folderOperationCount = folderOperationCount;
			using (DisposeGuard disposeGuard = this.Guard())
			{
				base.LogEvent(LogEventType.Entry, "Sync started");
				disposeGuard.Success();
			}
		}

		public static void LogOnServer(Exception exception)
		{
			PublicFolderMailboxLoggerBase.LogOnServer(exception, "PublicFolderSyncLog", "PublicFolderSyncLog");
		}

		public static void LogOnServer(string data, LogEventType eventType, Guid? transactionId = null)
		{
			PublicFolderMailboxLoggerBase.LogOnServer(data, eventType, "PublicFolderSyncLog", "PublicFolderSyncLog", transactionId);
		}

		public void LogFolderDeleted(byte[] entryId)
		{
			if (this.folderOperationCount.Deleted < 10)
			{
				base.LogEvent(LogEventType.Verbose, HexConverter.ByteArrayToHexString(entryId) + " is deleted");
			}
			this.folderOperationCount.Deleted++;
		}

		public void LogFolderUpdated(byte[] entryId)
		{
			if (this.folderOperationCount.Updated < 10)
			{
				base.LogEvent(LogEventType.Verbose, HexConverter.ByteArrayToHexString(entryId) + " is updated");
			}
			this.folderOperationCount.Updated++;
		}

		public void LogFolderCreated(byte[] entryId)
		{
			if (this.folderOperationCount.Added < 10)
			{
				base.LogEvent(LogEventType.Verbose, HexConverter.ByteArrayToHexString(entryId) + " is added");
			}
			this.folderOperationCount.Added++;
		}

		protected override void LogFinalFoldersStats()
		{
			base.LogEvent(LogEventType.Statistics, this.folderOperationCount.Added + " folders have been added");
			base.LogEvent(LogEventType.Statistics, this.folderOperationCount.Updated + " folders have been updated");
			base.LogEvent(LogEventType.Statistics, this.folderOperationCount.Deleted + " folders have been deleted");
		}

		private const string LogComponent = "PublicFolderSyncLog";

		private const string LogSuffix = "PublicFolderSyncLog";

		private readonly FolderOperationCounter folderOperationCount;
	}
}
