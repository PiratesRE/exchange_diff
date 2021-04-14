using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	internal class PhysicalMailbox : IPhysicalMailbox, IExtensibleDataObject
	{
		public PhysicalMailbox(DirectoryIdentity identity, ByteQuantifiedSize totalLogicalSize, ByteQuantifiedSize totalPhysicalSize, bool isQuarantined, MailboxMiscFlags mailboxFlags, StoreMailboxType mailboxType, ulong itemCount, DateTime? lastLogonTimestamp) : this(identity, totalLogicalSize, totalPhysicalSize, isQuarantined, mailboxType, itemCount, lastLogonTimestamp, null, false, mailboxFlags.HasFlag(MailboxMiscFlags.SoftDeletedMailbox) || mailboxFlags.HasFlag(MailboxMiscFlags.MRSSoftDeletedMailbox), mailboxFlags.HasFlag(MailboxMiscFlags.ArchiveMailbox), mailboxFlags.HasFlag(MailboxMiscFlags.DisabledMailbox), mailboxFlags.HasFlag(MailboxMiscFlags.CreatedByMove))
		{
		}

		public PhysicalMailbox(DirectoryIdentity identity, ByteQuantifiedSize totalLogicalSize, ByteQuantifiedSize totalPhysicalSize, bool isQuarantined, StoreMailboxType mailboxType, ulong itemCount, DateTime? lastLogonTimestamp, DateTime? disconnectDate, bool isConsumer, bool isSoftDeleted, bool isArchive, bool isDisabled, bool isMoveDestination)
		{
			this.Identity = identity;
			this.TotalLogicalSize = totalLogicalSize;
			this.TotalPhysicalSize = totalPhysicalSize;
			this.IsQuarantined = isQuarantined;
			this.IsArchive = isArchive;
			this.IsSoftDeleted = isSoftDeleted;
			this.IsMoveDestination = isMoveDestination;
			this.IsDisabled = isDisabled;
			this.MailboxType = mailboxType;
			this.ItemCount = itemCount;
			this.LastLogonAge = ((lastLogonTimestamp == null) ? TimeSpan.MaxValue : (DateTime.UtcNow - lastLogonTimestamp.Value));
			this.LastLogonTimestamp = lastLogonTimestamp;
			this.DisconnectDate = disconnectDate;
			this.IsConsumer = isConsumer;
		}

		[DataMember]
		public ByteQuantifiedSize AttachmentTableTotalSize { get; set; }

		[DataMember]
		public string DatabaseName { get; set; }

		[DataMember]
		public ulong DeletedItemCount { get; set; }

		[DataMember]
		public DateTime? DisconnectDate { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		public Guid Guid
		{
			get
			{
				return this.Identity.Guid;
			}
		}

		[DataMember]
		public DirectoryIdentity Identity { get; private set; }

		[DataMember]
		public bool IsArchive { get; private set; }

		[DataMember]
		public bool IsConsumer { get; private set; }

		[DataMember]
		public bool IsDisabled { get; private set; }

		[DataMember]
		public bool IsMoveDestination { get; private set; }

		[DataMember]
		public bool IsQuarantined { get; private set; }

		[DataMember]
		public bool IsSoftDeleted { get; private set; }

		[DataMember]
		public ulong ItemCount { get; private set; }

		[DataMember]
		public TimeSpan LastLogonAge { get; private set; }

		[DataMember]
		public DateTime? LastLogonTimestamp { get; private set; }

		[DataMember]
		public StoreMailboxType MailboxType { get; private set; }

		[DataMember]
		public ByteQuantifiedSize MessageTableTotalSize { get; set; }

		public string Name
		{
			get
			{
				return this.Identity.Name;
			}
		}

		[DataMember]
		public Guid OrganizationId { get; set; }

		[DataMember]
		public ByteQuantifiedSize OtherTablesTotalSize { get; set; }

		[DataMember]
		public ByteQuantifiedSize TotalDeletedItemSize { get; set; }

		[DataMember]
		public ByteQuantifiedSize TotalItemSize { get; set; }

		public ByteQuantifiedSize TotalLogicalSize
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(this.totalLogicalSizeBytes);
			}
			private set
			{
				this.totalLogicalSizeBytes = value.ToBytes();
			}
		}

		public ByteQuantifiedSize TotalPhysicalSize
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(this.totalPhysicalSizeBytes);
			}
			private set
			{
				this.totalPhysicalSizeBytes = value.ToBytes();
			}
		}

		[DataMember]
		public DateTime CreationTimestamp { get; set; }

		[DataMember]
		public int ItemsPendingUpgrade { get; set; }

		private MailboxState MailboxState
		{
			get
			{
				if (this.IsSoftDeleted)
				{
					return MailboxState.SoftDeleted;
				}
				if (this.IsDisabled)
				{
					return MailboxState.Disabled;
				}
				return MailboxState.Connected;
			}
		}

		public void PopulateLogEntry(MailboxStatisticsLogEntry logEntry)
		{
			logEntry[MailboxStatisticsLogEntrySchema.AttachmentTableTotalSizeInBytes] = this.AttachmentTableTotalSize.ToBytes();
			logEntry[MailboxStatisticsLogEntrySchema.DatabaseName] = this.DatabaseName;
			logEntry[MailboxStatisticsLogEntrySchema.DeletedItemCount] = this.DeletedItemCount;
			logEntry[MailboxStatisticsLogEntrySchema.DisconnectDate] = this.DisconnectDate;
			logEntry[MailboxStatisticsLogEntrySchema.MailboxState] = this.MailboxState;
			logEntry[MailboxStatisticsLogEntrySchema.ExternalDirectoryOrganizationId] = this.OrganizationId;
			logEntry[MailboxStatisticsLogEntrySchema.IsArchiveMailbox] = this.IsArchive;
			logEntry[MailboxStatisticsLogEntrySchema.IsMoveDestination] = this.IsMoveDestination;
			logEntry[MailboxStatisticsLogEntrySchema.IsQuarantined] = this.IsQuarantined;
			logEntry[MailboxStatisticsLogEntrySchema.ItemCount] = this.ItemCount;
			logEntry[MailboxStatisticsLogEntrySchema.LastLogonTime] = this.LastLogonTimestamp;
			logEntry[MailboxStatisticsLogEntrySchema.LogicalSizeInM] = this.TotalLogicalSize.ToMB();
			logEntry[MailboxStatisticsLogEntrySchema.MailboxGuid] = this.Guid;
			logEntry[MailboxStatisticsLogEntrySchema.MailboxType] = this.ComputeMailboxType((LoadBalanceMailboxType)logEntry[MailboxStatisticsLogEntrySchema.MailboxType]);
			logEntry[MailboxStatisticsLogEntrySchema.MessageTableTotalSizeInBytes] = this.MessageTableTotalSize.ToBytes();
			logEntry[MailboxStatisticsLogEntrySchema.OtherTablesTotalSizeInBytes] = this.OtherTablesTotalSize.ToBytes();
			logEntry[MailboxStatisticsLogEntrySchema.PhysicalSizeInM] = this.TotalPhysicalSize.ToMB();
			logEntry[MailboxStatisticsLogEntrySchema.TotalDeletedItemSizeInBytes] = this.TotalDeletedItemSize.ToBytes();
			logEntry[MailboxStatisticsLogEntrySchema.TotalItemSizeInBytes] = this.TotalItemSize.ToBytes();
			logEntry[MailboxStatisticsLogEntrySchema.CreationTimestamp] = this.CreationTimestamp;
			logEntry[MailboxStatisticsLogEntrySchema.ItemsPendingUpgrade] = this.ItemsPendingUpgrade;
		}

		private LoadBalanceMailboxType ComputeMailboxType(LoadBalanceMailboxType directoryMailboxType)
		{
			switch (this.MailboxType)
			{
			case StoreMailboxType.PublicFolderPrimary:
				return LoadBalanceMailboxType.PublicFolderPrimary;
			case StoreMailboxType.PublicFolderSecondary:
				return LoadBalanceMailboxType.PublicFolderSecondary;
			default:
				if (this.IsConsumer)
				{
					return LoadBalanceMailboxType.Consumer;
				}
				return directoryMailboxType;
			}
		}

		[DataMember]
		private ulong totalLogicalSizeBytes;

		[DataMember]
		private ulong totalPhysicalSizeBytes;
	}
}
