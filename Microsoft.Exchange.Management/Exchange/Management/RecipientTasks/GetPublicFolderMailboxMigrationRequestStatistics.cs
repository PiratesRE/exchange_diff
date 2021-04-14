using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "PublicFolderMailboxMigrationRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderMailboxMigrationRequestStatistics : GetRequestStatistics<PublicFolderMailboxMigrationRequestIdParameter, PublicFolderMailboxMigrationRequestStatistics>
	{
	}
}
