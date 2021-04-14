using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "MailboxRelocationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumeMailboxRelocationRequest : ResumeRequest<MailboxRelocationRequestIdParameter>
	{
	}
}
