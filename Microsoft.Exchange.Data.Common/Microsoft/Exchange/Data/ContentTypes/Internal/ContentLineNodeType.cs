using System;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	[Flags]
	internal enum ContentLineNodeType
	{
		DocumentStart = 0,
		ComponentStart = 1,
		ComponentEnd = 2,
		Parameter = 4,
		Property = 8,
		BeforeComponentStart = 16,
		BeforeComponentEnd = 32,
		DocumentEnd = 64
	}
}
