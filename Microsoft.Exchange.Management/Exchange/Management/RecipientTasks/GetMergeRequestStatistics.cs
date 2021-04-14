using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MergeRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetMergeRequestStatistics : GetRequestStatistics<MergeRequestIdParameter, MergeRequestStatistics>
	{
		internal override void CheckIndexEntry(IRequestIndexEntry index)
		{
			base.CheckIndexEntry(index);
			base.CheckIndexEntryLocalUserNotNull(index);
		}
	}
}
