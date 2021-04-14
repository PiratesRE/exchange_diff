using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "FolderMoveRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveFolderMoveRequest : RemoveRequest<FolderMoveRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new FolderMoveRequest(entry).ToString();
		}
	}
}
