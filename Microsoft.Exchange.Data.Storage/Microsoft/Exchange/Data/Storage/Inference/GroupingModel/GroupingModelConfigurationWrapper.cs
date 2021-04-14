using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupingModelConfigurationWrapper : IGroupingModelConfiguration
	{
		public GroupingModelConfigurationWrapper(GroupingModelConfiguration modelConfiguration)
		{
			ArgumentValidator.ThrowIfNull("modelConfiguration", modelConfiguration);
			this.modelConfiguration = modelConfiguration;
		}

		public int CurrentVersion
		{
			get
			{
				return this.modelConfiguration.CurrentVersion;
			}
		}

		public int MinimumSupportedVersion
		{
			get
			{
				return this.modelConfiguration.MinimumSupportedVersion;
			}
		}

		private readonly GroupingModelConfiguration modelConfiguration;
	}
}
