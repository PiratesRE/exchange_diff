using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachedPhysicalDatabase : CachedClient, IPhysicalDatabase, IDisposeTrackable, IDisposable
	{
		public CachedPhysicalDatabase(IPhysicalDatabase database) : base(null)
		{
			this.database = database;
		}

		Guid IPhysicalDatabase.DatabaseGuid
		{
			get
			{
				return this.database.DatabaseGuid;
			}
		}

		public void LoadMailboxes()
		{
			this.database.LoadMailboxes();
		}

		IEnumerable<IPhysicalMailbox> IPhysicalDatabase.GetConsumerMailboxes()
		{
			return this.database.GetConsumerMailboxes();
		}

		DatabaseSizeInfo IPhysicalDatabase.GetDatabaseSpaceData()
		{
			return this.database.GetDatabaseSpaceData();
		}

		IPhysicalMailbox IPhysicalDatabase.GetMailbox(Guid mailboxGuid)
		{
			return this.database.GetMailbox(mailboxGuid);
		}

		IEnumerable<IPhysicalMailbox> IPhysicalDatabase.GetNonConnectedMailboxes()
		{
			return this.database.GetNonConnectedMailboxes();
		}

		internal override void Cleanup()
		{
			this.database.Dispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CachedPhysicalDatabase>(this);
		}

		private readonly IPhysicalDatabase database;
	}
}
