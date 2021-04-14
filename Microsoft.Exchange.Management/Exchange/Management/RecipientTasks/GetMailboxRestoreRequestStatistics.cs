using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxRestoreRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxRestoreRequestStatistics : GetRequestStatistics<MailboxRestoreRequestIdParameter, MailboxRestoreRequestStatistics>
	{
		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryLocalUserNotNull(index);
		}
	}
}
