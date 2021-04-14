using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MailboxRelocationRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveMailboxRelocationRequest : RemoveRequest<MailboxRelocationRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new MailboxRelocationRequest(entry).ToString();
		}
	}
}
