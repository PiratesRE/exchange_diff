using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public class MailboxStatistics : MailboxEntry
	{
		private static string FormatShortTermId(long? id)
		{
			if (id != null)
			{
				ulong value = (ulong)id.Value;
				ushort num = (ushort)(value & 65535UL);
				ulong num2 = ((value & 16711680UL) << 24) + ((value & (ulong)-16777216) << 8) + ((value & 1095216660480UL) >> 8) + ((value & 280375465082880UL) >> 24) + ((value & 71776119061217280UL) >> 40) + ((value & 18374686479671623680UL) >> 56);
				return string.Format("{0:X}-{1:X}", num, num2);
			}
			return null;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxStatistics.schema;
			}
		}

		internal bool IncludeQuarantineDetails
		{
			get
			{
				return this.includeQuarantineDetails;
			}
			set
			{
				this.includeQuarantineDetails = value;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			this.ValidateMailboxGuid(errors);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			this.ValidateMailboxGuid(errors);
		}

		private void ValidateMailboxGuid(List<ValidationError> errors)
		{
			if (Guid.Empty == this.MailboxGuid)
			{
				errors.Add(new PropertyValidationError(Strings.ErrorMailboxStatisticsMailboxGuidEmpty, null, this));
			}
		}

		public string CurrentSchemaVersion
		{
			get
			{
				int? num = (int?)this[MailboxStatisticsSchema.CurrentSchemaVersion];
				if (num != null)
				{
					return string.Format("{0}.{1}", num.Value >> 16, num.Value & 65535);
				}
				return null;
			}
		}

		public uint? AssociatedItemCount
		{
			get
			{
				return (uint?)this[MailboxStatisticsSchema.AssociatedItemCount];
			}
		}

		public uint? DeletedItemCount
		{
			get
			{
				return (uint?)this[MailboxStatisticsSchema.DeletedItemCount];
			}
		}

		public DateTime? DisconnectDate
		{
			get
			{
				return (DateTime?)this[MailboxStatisticsSchema.DisconnectDate];
			}
		}

		public MailboxState? DisconnectReason
		{
			get
			{
				object obj = this[MailboxStatisticsSchema.DisconnectDate];
				object obj2 = this[MailboxStatisticsSchema.MailboxMiscFlags];
				MailboxState? result = new MailboxState?(MailboxState.Unknown);
				if (obj != null && obj2 != null)
				{
					if (((MailboxMiscFlags)obj2 & MailboxMiscFlags.DisabledMailbox) == MailboxMiscFlags.DisabledMailbox)
					{
						result = new MailboxState?(MailboxState.Disabled);
					}
					else if (((MailboxMiscFlags)obj2 & MailboxMiscFlags.SoftDeletedMailbox) == MailboxMiscFlags.SoftDeletedMailbox)
					{
						result = new MailboxState?(MailboxState.SoftDeleted);
					}
					else
					{
						result = new MailboxState?(MailboxState.Unknown);
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[MailboxStatisticsSchema.DisplayName];
			}
		}

		public uint? ItemCount
		{
			get
			{
				return (uint?)this[MailboxStatisticsSchema.ItemCount];
			}
		}

		public string LastLoggedOnUserAccount
		{
			get
			{
				return (string)this[MailboxStatisticsSchema.LastLoggedOnUserAccount];
			}
		}

		public DateTime? LastLogoffTime
		{
			get
			{
				return (DateTime?)this[MailboxStatisticsSchema.LastLogoffTime];
			}
		}

		public DateTime? LastLogonTime
		{
			get
			{
				return (DateTime?)this[MailboxStatisticsSchema.LastLogonTime];
			}
		}

		public string LegacyDN
		{
			get
			{
				return (string)this[MailboxStatisticsSchema.LegacyDN];
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return (Guid)this[MailboxStatisticsSchema.MailboxGuid];
			}
		}

		public StoreMailboxType MailboxType
		{
			get
			{
				object obj = this[MailboxStatisticsSchema.StoreMailboxType];
				if (obj != null)
				{
					return (StoreMailboxType)obj;
				}
				return StoreMailboxType.Private;
			}
		}

		public ObjectClass ObjectClass
		{
			get
			{
				return (ObjectClass)this[MailboxStatisticsSchema.ObjectClass];
			}
		}

		public StorageLimitStatus? StorageLimitStatus
		{
			get
			{
				return (StorageLimitStatus?)this[MailboxStatisticsSchema.StorageLimitStatus];
			}
		}

		public Unlimited<ByteQuantifiedSize> TotalDeletedItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.TotalDeletedItemSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> TotalItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.TotalItemSize];
			}
		}

		public string MailboxTableIdentifier
		{
			get
			{
				return MailboxStatistics.FormatShortTermId((long?)this[MailboxStatisticsSchema.MailboxRootEntryId]);
			}
		}

		public ObjectId Database { get; internal set; }

		public string ServerName { get; internal set; }

		public string DatabaseName { get; internal set; }

		public bool IsDatabaseCopyActive { get; internal set; }

		public bool IsQuarantined
		{
			get
			{
				return (bool)this[MailboxStatisticsSchema.IsQuarantined];
			}
		}

		public string QuarantineDescription
		{
			get
			{
				if (!this.IncludeQuarantineDetails)
				{
					return string.Empty;
				}
				return (string)this[MailboxStatisticsSchema.QuarantineDescription];
			}
		}

		public DateTime? QuarantineLastCrash
		{
			get
			{
				if (!this.IncludeQuarantineDetails)
				{
					return null;
				}
				return (DateTime?)this[MailboxStatisticsSchema.QuarantineLastCrash];
			}
		}

		public DateTime? QuarantineEnd
		{
			get
			{
				return (DateTime?)this[MailboxStatisticsSchema.QuarantineEnd];
			}
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				if (this[MailboxStatisticsSchema.PersistableTenantPartitionHint] == null)
				{
					return Guid.Empty;
				}
				return TenantPartitionHint.FromPersistablePartitionHint((byte[])this[MailboxStatisticsSchema.PersistableTenantPartitionHint]).GetExternalDirectoryOrganizationId();
			}
		}

		public bool? IsArchiveMailbox
		{
			get
			{
				return this.CheckMailboxMiscFlags(MailboxMiscFlags.ArchiveMailbox);
			}
		}

		public bool? IsMoveDestination
		{
			get
			{
				return this.CheckMailboxMiscFlags(MailboxMiscFlags.CreatedByMove);
			}
		}

		public int? MailboxMessagesPerFolderCountWarningQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.MailboxMessagesPerFolderCountWarningQuota];
			}
		}

		public int? MailboxMessagesPerFolderCountReceiveQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.MailboxMessagesPerFolderCountReceiveQuota];
			}
		}

		public int? DumpsterMessagesPerFolderCountWarningQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.DumpsterMessagesPerFolderCountWarningQuota];
			}
		}

		public int? DumpsterMessagesPerFolderCountReceiveQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.DumpsterMessagesPerFolderCountReceiveQuota];
			}
		}

		public int? FolderHierarchyChildrenCountWarningQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FolderHierarchyChildrenCountWarningQuota];
			}
		}

		public int? FolderHierarchyChildrenCountReceiveQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FolderHierarchyChildrenCountReceiveQuota];
			}
		}

		public int? FolderHierarchyDepthWarningQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FolderHierarchyDepthWarningQuota];
			}
		}

		public int? FolderHierarchyDepthReceiveQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FolderHierarchyDepthReceiveQuota];
			}
		}

		public int? FoldersCountWarningQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FoldersCountWarningQuota];
			}
		}

		public int? FoldersCountReceiveQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.FoldersCountReceiveQuota];
			}
		}

		public int? NamedPropertiesCountQuota
		{
			get
			{
				return (int?)this[MailboxStatisticsSchema.NamedPropertiesCountQuota];
			}
		}

		public Unlimited<ByteQuantifiedSize> MessageTableTotalSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.MessageTableTotalSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> MessageTableAvailableSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.MessageTableAvailableSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> AttachmentTableTotalSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.AttachmentTableTotalSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> AttachmentTableAvailableSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.AttachmentTableAvailableSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> OtherTablesTotalSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.OtherTablesTotalSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> OtherTablesAvailableSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxStatisticsSchema.OtherTablesAvailableSize];
			}
		}

		public Unlimited<ByteQuantifiedSize> DatabaseIssueWarningQuota { get; internal set; }

		public Unlimited<ByteQuantifiedSize> DatabaseProhibitSendQuota { get; internal set; }

		public Unlimited<ByteQuantifiedSize> DatabaseProhibitSendReceiveQuota { get; internal set; }

		public MailboxStatistics()
		{
		}

		internal MailboxStatistics(MailboxId mapiObjectId, MapiSession mapiSession) : base(mapiObjectId, mapiSession)
		{
		}

		private bool? CheckMailboxMiscFlags(MailboxMiscFlags flags)
		{
			bool? result = null;
			object obj = this[MailboxStatisticsSchema.MailboxMiscFlags];
			if (obj != null)
			{
				if (((MailboxMiscFlags)obj & flags) == flags)
				{
					result = new bool?(true);
				}
				else
				{
					result = new bool?(false);
				}
			}
			return result;
		}

		private static MapiObjectSchema schema = ObjectSchema.GetInstance<MailboxStatisticsSchema>();

		private bool includeQuarantineDetails;
	}
}
