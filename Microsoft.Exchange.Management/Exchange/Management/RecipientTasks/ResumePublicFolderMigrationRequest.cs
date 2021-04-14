using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "PublicFolderMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumePublicFolderMigrationRequest : ResumeRequest<PublicFolderMigrationRequestIdParameter>
	{
		protected override void CheckIndexEntry()
		{
		}
	}
}
