using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "PublicFolderMailboxMigrationRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendPublicFolderMailboxMigrationRequest : SuspendRequest<PublicFolderMailboxMigrationRequestIdParameter>
	{
		protected override void CheckIndexEntry()
		{
		}

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";
	}
}
