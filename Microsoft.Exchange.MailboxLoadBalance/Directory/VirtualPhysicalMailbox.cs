using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class VirtualPhysicalMailbox : IPhysicalMailbox
	{
		public VirtualPhysicalMailbox(IClientFactory clientFactory, DirectoryDatabase database, Guid mailboxGuid, ILogger logger, bool isArchive)
		{
			this.clientFactory = clientFactory;
			this.database = database;
			this.logger = logger;
			this.physicalMailbox = new Lazy<IPhysicalMailbox>(new Func<IPhysicalMailbox>(this.LoadMailboxData));
			this.Guid = mailboxGuid;
			this.IsArchive = isArchive;
			this.IsConsumer = false;
		}

		public ByteQuantifiedSize AttachmentTableTotalSize
		{
			get
			{
				return this.PhysicalMailbox.AttachmentTableTotalSize;
			}
		}

		public string DatabaseName { get; set; }

		public ulong DeletedItemCount
		{
			get
			{
				return this.PhysicalMailbox.DeletedItemCount;
			}
		}

		public DateTime? DisconnectDate
		{
			get
			{
				return this.PhysicalMailbox.DisconnectDate;
			}
		}

		[DataMember]
		public Guid Guid { get; private set; }

		public DirectoryIdentity Identity
		{
			get
			{
				return this.PhysicalMailbox.Identity;
			}
		}

		public bool IsArchive { get; private set; }

		public bool IsDisabled
		{
			get
			{
				return this.PhysicalMailbox.IsDisabled;
			}
		}

		public bool IsMoveDestination
		{
			get
			{
				return this.PhysicalMailbox.IsMoveDestination;
			}
		}

		public bool IsQuarantined
		{
			get
			{
				return this.PhysicalMailbox.IsQuarantined;
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return this.PhysicalMailbox.IsSoftDeleted;
			}
		}

		[DataMember]
		public bool IsConsumer { get; private set; }

		public ulong ItemCount
		{
			get
			{
				return this.PhysicalMailbox.ItemCount;
			}
		}

		public TimeSpan LastLogonAge
		{
			get
			{
				return this.PhysicalMailbox.LastLogonAge;
			}
		}

		public DateTime? LastLogonTimestamp
		{
			get
			{
				return this.PhysicalMailbox.LastLogonTimestamp;
			}
		}

		public StoreMailboxType MailboxType
		{
			get
			{
				return this.PhysicalMailbox.MailboxType;
			}
		}

		public ByteQuantifiedSize MessageTableTotalSize
		{
			get
			{
				return this.PhysicalMailbox.MessageTableTotalSize;
			}
		}

		public string Name
		{
			get
			{
				return this.PhysicalMailbox.Name;
			}
		}

		public Guid OrganizationId
		{
			get
			{
				return this.PhysicalMailbox.OrganizationId;
			}
		}

		public ByteQuantifiedSize OtherTablesTotalSize
		{
			get
			{
				return this.PhysicalMailbox.OtherTablesTotalSize;
			}
		}

		public ByteQuantifiedSize TotalDeletedItemSize
		{
			get
			{
				return this.PhysicalMailbox.TotalDeletedItemSize;
			}
		}

		public ByteQuantifiedSize TotalItemSize
		{
			get
			{
				return this.PhysicalMailbox.TotalItemSize;
			}
		}

		public ByteQuantifiedSize TotalLogicalSize
		{
			get
			{
				return this.PhysicalMailbox.TotalLogicalSize;
			}
		}

		public ByteQuantifiedSize TotalPhysicalSize
		{
			get
			{
				return this.PhysicalMailbox.TotalPhysicalSize;
			}
		}

		public DateTime CreationTimestamp
		{
			get
			{
				return this.PhysicalMailbox.CreationTimestamp;
			}
		}

		public int ItemsPendingUpgrade
		{
			get
			{
				return this.PhysicalMailbox.ItemsPendingUpgrade;
			}
		}

		private IPhysicalMailbox PhysicalMailbox
		{
			get
			{
				return this.physicalMailbox.Value;
			}
		}

		public void PopulateLogEntry(MailboxStatisticsLogEntry logEntry)
		{
			throw new InvalidOperationException("Virtual mailboxes should not be logged.");
		}

		private IPhysicalMailbox LoadMailboxData()
		{
			IPhysicalMailbox result;
			using (OperationTracker.Create(this.logger, "Retrieving single mailbox {0} data from database {1}", new object[]
			{
				this.Guid,
				this.database.Identity
			}))
			{
				using (IPhysicalDatabase physicalDatabaseConnection = this.clientFactory.GetPhysicalDatabaseConnection(this.database))
				{
					result = (physicalDatabaseConnection.GetMailbox(this.Guid) ?? EmptyPhysicalMailbox.Instance);
				}
			}
			return result;
		}

		private readonly IClientFactory clientFactory;

		[DataMember]
		private readonly DirectoryDatabase database;

		private readonly ILogger logger;

		private readonly Lazy<IPhysicalMailbox> physicalMailbox;
	}
}
