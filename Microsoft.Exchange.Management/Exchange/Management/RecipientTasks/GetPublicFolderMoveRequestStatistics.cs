using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "PublicFolderMoveRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderMoveRequestStatistics : GetRequestStatistics<PublicFolderMoveRequestIdParameter, PublicFolderMoveRequestStatistics>
	{
	}
}
