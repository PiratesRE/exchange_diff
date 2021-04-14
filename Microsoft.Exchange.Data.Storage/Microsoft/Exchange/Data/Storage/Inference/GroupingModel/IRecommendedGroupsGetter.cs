using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecommendedGroupsGetter
	{
		IReadOnlyList<IRecommendedGroupInfo> GetRecommendedGroups(MailboxSession session, Action<string> traceDelegate, Action<Exception> traceErrorDelegate);
	}
}
