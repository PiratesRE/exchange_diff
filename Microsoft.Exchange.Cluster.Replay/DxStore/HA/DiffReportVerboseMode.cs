using System;

namespace Microsoft.Exchange.DxStore.HA
{
	[Flags]
	public enum DiffReportVerboseMode
	{
		Disabled = 1,
		ShowKeyNames = 2,
		ShowPropertyNames = 4,
		ShowPropertyValues = 8,
		ShowMatchingKeys = 16,
		ShowMatchingProperties = 32,
		All = 60
	}
}
