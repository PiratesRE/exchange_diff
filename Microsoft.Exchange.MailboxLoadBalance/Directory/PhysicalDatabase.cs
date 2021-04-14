using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhysicalDatabase : DisposeTrackableBase, IPhysicalDatabase, IDisposeTrackable, IDisposable
	{
		public PhysicalDatabase(DirectoryDatabase database, IStorePort storePort, ILogger logger)
		{
			AnchorUtil.ThrowOnNullArgument(database, "database");
			AnchorUtil.ThrowOnNullArgument(storePort, "storePort");
			this.databaseLoadLock = new object();
			this.database = database;
			this.logger = logger;
			this.DatabaseGuid = database.Guid;
			this.physicalMailboxes = new Dictionary<Guid, IPhysicalMailbox>();
			this.nonConnectedMailboxes = new List<IPhysicalMailbox>();
			this.storePort = storePort;
		}

		public Guid DatabaseGuid { get; private set; }

		public IEnumerable<IPhysicalMailbox> GetConsumerMailboxes()
		{
			this.LoadAllMailboxes();
			return from m in this.physicalMailboxes.Values
			where m.IsConsumer
			select m;
		}

		public DatabaseSizeInfo GetDatabaseSpaceData()
		{
			return this.storePort.GetDatabaseSize(this.database);
		}

		public IPhysicalMailbox GetMailbox(Guid mailboxGuid)
		{
			if (!this.mailboxesLoaded)
			{
				try
				{
					MailboxTableEntry mailboxTableEntry = this.storePort.GetMailboxTable(this.database, mailboxGuid, MailboxTablePropertyDefinitions.MailboxTablePropertiesToLoad).FirstOrDefault<MailboxTableEntry>();
					return (mailboxTableEntry == null) ? null : mailboxTableEntry.ToPhysicalMailbox();
				}
				catch (MapiExceptionNotFound mapiExceptionNotFound)
				{
					this.logger.LogVerbose("Could not find the mailbox {0} in database {1}. {2}", new object[]
					{
						mailboxGuid,
						this.database.Identity,
						mapiExceptionNotFound
					});
					return null;
				}
			}
			IPhysicalMailbox result;
			if (this.physicalMailboxes.TryGetValue(mailboxGuid, out result))
			{
				return result;
			}
			return null;
		}

		public IEnumerable<IPhysicalMailbox> GetNonConnectedMailboxes()
		{
			this.LoadAllMailboxes();
			return this.nonConnectedMailboxes;
		}

		public void LoadMailboxes()
		{
			this.LoadAllMailboxes();
		}

		protected override void InternalDispose(bool disposing)
		{
			this.nonConnectedMailboxes.Clear();
			this.physicalMailboxes.Clear();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PhysicalDatabase>(this);
		}

		private void LoadAllMailboxes()
		{
			lock (this.databaseLoadLock)
			{
				if (!this.mailboxesLoaded)
				{
					foreach (IPhysicalMailbox physicalMailbox in from tb in this.storePort.GetMailboxTable(this.database, Guid.Empty, null)
					select tb.ToPhysicalMailbox())
					{
						physicalMailbox.DatabaseName = this.database.Name;
						if (physicalMailbox.IsSoftDeleted || physicalMailbox.IsMoveDestination || physicalMailbox.IsDisabled)
						{
							this.nonConnectedMailboxes.Add(physicalMailbox);
						}
						else
						{
							this.physicalMailboxes[physicalMailbox.Guid] = physicalMailbox;
						}
					}
					this.mailboxesLoaded = true;
				}
			}
		}

		private readonly DirectoryDatabase database;

		private readonly object databaseLoadLock;

		private readonly ILogger logger;

		private readonly List<IPhysicalMailbox> nonConnectedMailboxes;

		private readonly Dictionary<Guid, IPhysicalMailbox> physicalMailboxes;

		private readonly IStorePort storePort;

		private bool mailboxesLoaded;
	}
}
