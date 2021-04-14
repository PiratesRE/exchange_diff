using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxImportRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxImportRequestStatistics : GetRequestStatistics<MailboxImportRequestIdParameter, MailboxImportRequestStatistics>
	{
		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryLocalUserNotNull(index);
		}
	}
}
