using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Suspend", "PublicFolderMigrationRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class SuspendPublicFolderMigrationRequest : SuspendRequest<PublicFolderMigrationRequestIdParameter>
	{
		protected override void CheckIndexEntry()
		{
		}
	}
}
