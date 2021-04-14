using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "PublicFolderMailboxMigrationRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemovePublicFolderMailboxMigrationRequest : RemoveRequest<PublicFolderMailboxMigrationRequestIdParameter>
	{
		internal override string GenerateIndexEntryString(IRequestIndexEntry entry)
		{
			return new PublicFolderMailboxMigrationRequest(entry).ToString();
		}

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";
	}
}
