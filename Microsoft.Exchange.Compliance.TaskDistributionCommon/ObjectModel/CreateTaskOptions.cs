using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[Flags]
	internal enum CreateTaskOptions
	{
		None = 0,
		FailedBindingsOnly = 1
	}
}
