using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "PublicFolderMigrationRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderMigrationRequestStatistics : GetRequestStatistics<PublicFolderMigrationRequestIdParameter, PublicFolderMigrationRequestStatistics>
	{
	}
}
