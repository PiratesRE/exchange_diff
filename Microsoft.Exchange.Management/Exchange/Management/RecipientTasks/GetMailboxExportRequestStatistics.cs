using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxExportRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxExportRequestStatistics : GetRequestStatistics<MailboxExportRequestIdParameter, MailboxExportRequestStatistics>
	{
		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryLocalUserNotNull(index);
		}
	}
}
