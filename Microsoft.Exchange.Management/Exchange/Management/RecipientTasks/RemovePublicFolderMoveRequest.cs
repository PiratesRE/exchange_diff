using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "PublicFolderMoveRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemovePublicFolderMoveRequest : RemoveRequest<PublicFolderMoveRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new PublicFolderMoveRequest(entry).ToString();
		}
	}
}
