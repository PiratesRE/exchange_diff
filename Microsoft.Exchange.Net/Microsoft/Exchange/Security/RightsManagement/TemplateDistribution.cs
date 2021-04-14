using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum TemplateDistribution : uint
	{
		NonSilent = 1U,
		ObtainAll = 2U,
		Cancel = 4U
	}
}
