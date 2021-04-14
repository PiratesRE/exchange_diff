using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxRelocationRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxRelocationRequestStatistics : GetRequestStatistics<MailboxRelocationRequestIdParameter, MailboxRelocationRequestStatistics>
	{
		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryLocalUserNotNull(index);
		}
	}
}
