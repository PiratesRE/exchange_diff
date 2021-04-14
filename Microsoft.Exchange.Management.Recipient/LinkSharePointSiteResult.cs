using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal enum LinkSharePointSiteResult
	{
		Success,
		NotSiteOwner,
		SPServerVersionNotCompatible,
		NotTeamMailboxOwner,
		AlreadyLinkedBySelf,
		CurrentlyNotLinked,
		LinkedByOthers,
		ResultNotSet
	}
}
