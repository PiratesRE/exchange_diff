using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupingModelVersionSelector
	{
		public GroupingModelVersionSelector(IGroupingModelConfiguration configuration)
		{
			this.groupingModelconfiguration = configuration;
		}

		public int GetModelVersionToTrain()
		{
			return this.groupingModelconfiguration.CurrentVersion;
		}

		public int GetModelVersionToAccessRecommendedGroups()
		{
			return this.groupingModelconfiguration.CurrentVersion;
		}

		private readonly IGroupingModelConfiguration groupingModelconfiguration;
	}
}
