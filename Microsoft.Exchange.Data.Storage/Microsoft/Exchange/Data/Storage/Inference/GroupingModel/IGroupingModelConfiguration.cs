using System;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	public interface IGroupingModelConfiguration
	{
		int CurrentVersion { get; }

		int MinimumSupportedVersion { get; }
	}
}
