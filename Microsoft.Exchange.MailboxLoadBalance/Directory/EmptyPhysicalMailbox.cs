using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EmptyPhysicalMailbox : IPhysicalMailbox
	{
		private EmptyPhysicalMailbox()
		{
		}

		public ByteQuantifiedSize AttachmentTableTotalSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public string DatabaseName { get; set; }

		public ulong DeletedItemCount
		{
			get
			{
				return 0UL;
			}
		}

		public DateTime? DisconnectDate
		{
			get
			{
				return null;
			}
		}

		public Guid Guid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public DirectoryIdentity Identity
		{
			get
			{
				return DirectoryIdentity.NullIdentity;
			}
		}

		public bool IsArchive
		{
			get
			{
				return false;
			}
		}

		public bool IsDisabled
		{
			get
			{
				return false;
			}
		}

		public bool IsMoveDestination
		{
			get
			{
				return false;
			}
		}

		public bool IsQuarantined
		{
			get
			{
				return false;
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return false;
			}
		}

		public bool IsConsumer
		{
			get
			{
				return false;
			}
		}

		public ulong ItemCount
		{
			get
			{
				return 0UL;
			}
		}

		public TimeSpan LastLogonAge
		{
			get
			{
				return TimeSpan.MaxValue;
			}
		}

		public DateTime? LastLogonTimestamp
		{
			get
			{
				return null;
			}
		}

		public StoreMailboxType MailboxType
		{
			get
			{
				return StoreMailboxType.Private;
			}
		}

		public ByteQuantifiedSize MessageTableTotalSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public string Name
		{
			get
			{
				return string.Empty;
			}
		}

		public Guid OrganizationId
		{
			get
			{
				return Guid.Empty;
			}
		}

		public ByteQuantifiedSize OtherTablesTotalSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public ByteQuantifiedSize TotalDeletedItemSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public ByteQuantifiedSize TotalItemSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public ByteQuantifiedSize TotalLogicalSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public ByteQuantifiedSize TotalPhysicalSize
		{
			get
			{
				return ByteQuantifiedSize.Zero;
			}
		}

		public DateTime CreationTimestamp
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		public int ItemsPendingUpgrade
		{
			get
			{
				return 0;
			}
		}

		public void PopulateLogEntry(MailboxStatisticsLogEntry logEntry)
		{
			throw new InvalidOperationException("Empty mailboxes should not be logged.");
		}

		public static readonly IPhysicalMailbox Instance = new EmptyPhysicalMailbox();
	}
}
