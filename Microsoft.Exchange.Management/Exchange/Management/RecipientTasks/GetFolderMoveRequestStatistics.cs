using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "FolderMoveRequestStatistics", DefaultParameterSetName = "Identity")]
	public sealed class GetFolderMoveRequestStatistics : GetRequestStatistics<FolderMoveRequestIdParameter, FolderMoveRequestStatistics>
	{
	}
}
