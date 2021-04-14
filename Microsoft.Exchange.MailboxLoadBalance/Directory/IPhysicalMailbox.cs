using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	internal interface IPhysicalMailbox
	{
		ByteQuantifiedSize AttachmentTableTotalSize { get; }

		string DatabaseName { get; set; }

		ulong DeletedItemCount { get; }

		DateTime? DisconnectDate { get; }

		Guid Guid { get; }

		DirectoryIdentity Identity { get; }

		bool IsArchive { get; }

		bool IsDisabled { get; }

		bool IsMoveDestination { get; }

		bool IsQuarantined { get; }

		bool IsSoftDeleted { get; }

		bool IsConsumer { get; }

		ulong ItemCount { get; }

		TimeSpan LastLogonAge { get; }

		DateTime? LastLogonTimestamp { get; }

		StoreMailboxType MailboxType { get; }

		ByteQuantifiedSize MessageTableTotalSize { get; }

		string Name { get; }

		Guid OrganizationId { get; }

		ByteQuantifiedSize OtherTablesTotalSize { get; }

		ByteQuantifiedSize TotalDeletedItemSize { get; }

		ByteQuantifiedSize TotalItemSize { get; }

		ByteQuantifiedSize TotalLogicalSize { get; }

		ByteQuantifiedSize TotalPhysicalSize { get; }

		DateTime CreationTimestamp { get; }

		int ItemsPendingUpgrade { get; }

		void PopulateLogEntry(MailboxStatisticsLogEntry logEntry);
	}
}
