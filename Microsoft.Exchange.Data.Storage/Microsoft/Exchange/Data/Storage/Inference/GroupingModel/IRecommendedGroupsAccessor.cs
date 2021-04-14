using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecommendedGroupsAccessor : IRecommendedGroupsGetter
	{
		void SetRecommendedGroups(MailboxSession session, RecommendedGroupsInfo groupsInfo, int version, Action<string> traceDelegate, Action<Exception> traceErrorDelegate);
	}
}
