using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "PublicFolderMoveRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumePublicFolderMoveRequest : ResumeRequest<PublicFolderMoveRequestIdParameter>
	{
		protected override void CheckIndexEntry()
		{
		}
	}
}
