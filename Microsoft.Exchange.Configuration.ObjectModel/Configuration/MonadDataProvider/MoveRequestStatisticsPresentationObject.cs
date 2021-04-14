using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	public class MoveRequestStatisticsPresentationObject
	{
		public string DisplayName { get; set; }

		public RequestStatus Status { get; set; }

		public int PercentComplete { get; set; }

		public EnhancedTimeSpan? OverallDuration { get; set; }

		public ByteQuantifiedSize TotalMailboxSize { get; set; }

		public int? BadItemsEncountered { get; set; }

		public DateTime? LastUpdateTimestamp { get; set; }

		public bool SuspendWhenReadyToComplete { get; set; }

		public bool IsOffline { get; set; }

		public string RemoteHostName { get; set; }

		public string MRSServerName { get; set; }

		public ServerVersion SourceVersion { get; set; }

		public ADObjectId SourceDatabase { get; set; }

		public ServerVersion TargetVersion { get; set; }

		public ADObjectId TargetDatabase { get; set; }

		public DateTime? QueuedTimestamp { get; set; }

		public EnhancedTimeSpan? TotalQueuedDuration { get; set; }

		public DateTime? StartTimestamp { get; set; }

		public DateTime? CompletionTimestamp { get; set; }

		public DateTime? SuspendedTimestamp { get; set; }

		public LocalizedString Message { get; set; }

		public RequestFlags Flags { get; set; }

		public object Report { get; set; }
	}
}
