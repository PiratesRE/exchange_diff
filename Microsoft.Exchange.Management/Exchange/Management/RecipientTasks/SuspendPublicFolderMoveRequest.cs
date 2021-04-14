using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "PublicFolderMoveRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendPublicFolderMoveRequest : SuspendRequest<PublicFolderMoveRequestIdParameter>
	{
		protected override void CheckIndexEntry()
		{
		}
	}
}
