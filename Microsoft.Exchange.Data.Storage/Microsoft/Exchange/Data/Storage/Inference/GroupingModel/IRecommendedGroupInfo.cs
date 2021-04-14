using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecommendedGroupInfo
	{
		Guid ID { get; }

		List<string> Members { get; }

		List<string> Words { get; }
	}
}
