using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "MailboxExportRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendMailboxExportRequest : SuspendRequest<MailboxExportRequestIdParameter>
	{
	}
}
