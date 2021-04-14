using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Logging;
using Microsoft.Exchange.MailboxLoadBalance.Logging.MailboxStatistics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class DirectoryMailbox : DirectoryObject
	{
		public DirectoryMailbox(IDirectoryProvider directory, DirectoryIdentity identity, IEnumerable<IPhysicalMailbox> physicalMailboxes, DirectoryMailboxType mailboxType = DirectoryMailboxType.Organization) : base(directory, identity)
		{
			this.physicalMailboxes = physicalMailboxes.ToList<IPhysicalMailbox>();
			this.MailboxType = mailboxType;
		}

		public virtual bool IsArchiveOnly
		{
			get
			{
				if (this.physicalMailboxes.Count > 0)
				{
					return this.physicalMailboxes.All((IPhysicalMailbox pm) => pm.IsArchive);
				}
				return false;
			}
		}

		[DataMember]
		public bool IsBeingLoadBalanced { get; set; }

		public virtual long ItemCount
		{
			get
			{
				return this.physicalMailboxes.Sum((IPhysicalMailbox mbx) => (long)mbx.ItemCount);
			}
		}

		public int ItemsPendingUpgrade
		{
			get
			{
				return this.physicalMailboxes.Aggregate(0, (int current, IPhysicalMailbox mailbox) => mailbox.ItemsPendingUpgrade + current);
			}
		}

		public virtual ByteQuantifiedSize LogicalSize
		{
			get
			{
				return this.physicalMailboxes.Aggregate(ByteQuantifiedSize.Zero, (ByteQuantifiedSize current, IPhysicalMailbox mailbox) => current + mailbox.TotalLogicalSize);
			}
		}

		public IMailboxProvisioningConstraints MailboxProvisioningConstraints { get; set; }

		[DataMember]
		public DirectoryMailboxType MailboxType { get; set; }

		public Guid OrganizationId
		{
			get
			{
				return base.Identity.OrganizationId;
			}
		}

		public IEnumerable<IPhysicalMailbox> PhysicalMailboxes
		{
			get
			{
				return this.physicalMailboxes;
			}
		}

		public virtual ByteQuantifiedSize PhysicalSize
		{
			get
			{
				return this.physicalMailboxes.Aggregate(ByteQuantifiedSize.Zero, (ByteQuantifiedSize current, IPhysicalMailbox mailbox) => current + mailbox.TotalPhysicalSize);
			}
		}

		public override bool SupportsMoving
		{
			get
			{
				return true;
			}
		}

		public virtual long TotalCpu
		{
			get
			{
				return 0L;
			}
		}

		public TimeSpan MinimumAgeInDatabase
		{
			get
			{
				if (this.physicalMailboxes.Count == 0)
				{
					return TimeSpan.Zero;
				}
				return this.physicalMailboxes.Min((IPhysicalMailbox pm) => TimeProvider.UtcNow - pm.CreationTimestamp);
			}
		}

		public override IRequest CreateRequestToMove(DirectoryIdentity target, string batchName, ILogger logger)
		{
			if (target != null && target.ObjectType != DirectoryObjectType.Database)
			{
				throw new NotSupportedException("Mailboxes can only be moved into databases.");
			}
			return base.Directory.CreateRequestToMove(this, target, batchName, logger);
		}

		public void EmitLogEntry(ObjectLogCollector logCollector)
		{
			MailboxStatisticsLogEntry mailboxStatisticsLogEntry = new MailboxStatisticsLogEntry();
			mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.RecipientGuid] = base.Guid;
			mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.ExternalDirectoryOrganizationId] = this.OrganizationId;
			mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.MailboxType] = LoadBalanceMailboxType.OrgIdMailbox;
			mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.MailboxState] = MailboxState.AdOnly;
			if (this.MailboxProvisioningConstraints != null)
			{
				mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.MailboxProvisioningConstraint] = string.Format("{0}", this.MailboxProvisioningConstraints.HardConstraint);
				mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.MailboxProvisioningPreferences] = string.Join(";", from sc in this.MailboxProvisioningConstraints.SoftConstraints
				select sc.Value);
			}
			if (base.Parent != null)
			{
				mailboxStatisticsLogEntry[MailboxStatisticsLogEntrySchema.DatabaseName] = base.Parent.Name;
			}
			bool flag = false;
			foreach (IPhysicalMailbox physicalMailbox in this.PhysicalMailboxes.OfType<PhysicalMailbox>())
			{
				MailboxStatisticsLogEntry mailboxStatisticsLogEntry2 = new MailboxStatisticsLogEntry();
				mailboxStatisticsLogEntry2.CopyChangesFrom(mailboxStatisticsLogEntry);
				physicalMailbox.PopulateLogEntry(mailboxStatisticsLogEntry2);
				logCollector.LogObject<MailboxStatisticsLogEntry>(mailboxStatisticsLogEntry2);
				flag = true;
			}
			if (!flag)
			{
				logCollector.LogObject<MailboxStatisticsLogEntry>(mailboxStatisticsLogEntry);
			}
		}

		public DirectoryDatabase GetDatabaseForMailbox()
		{
			return base.Directory.GetDatabaseForMailbox(base.Identity);
		}

		[DataMember]
		private readonly List<IPhysicalMailbox> physicalMailboxes;
	}
}
