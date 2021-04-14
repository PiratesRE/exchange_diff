using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	internal interface IPhysicalDatabase : IDisposeTrackable, IDisposable
	{
		Guid DatabaseGuid { get; }

		IEnumerable<IPhysicalMailbox> GetConsumerMailboxes();

		DatabaseSizeInfo GetDatabaseSpaceData();

		IPhysicalMailbox GetMailbox(Guid mailboxGuid);

		IEnumerable<IPhysicalMailbox> GetNonConnectedMailboxes();

		void LoadMailboxes();
	}
}
