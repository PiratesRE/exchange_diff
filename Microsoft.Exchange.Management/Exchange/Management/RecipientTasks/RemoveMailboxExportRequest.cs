using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MailboxExportRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveMailboxExportRequest : RemoveRequest<MailboxExportRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new MailboxExportRequest(entry).ToString();
		}
	}
}
