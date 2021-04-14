using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MailboxImportRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveMailboxImportRequest : RemoveRequest<MailboxImportRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new MailboxImportRequest(entry).ToString();
		}
	}
}
