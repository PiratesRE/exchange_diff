using System;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	[Flags]
	internal enum WriteState
	{
		Start = 1,
		Component = 2,
		Property = 4,
		Parameter = 8,
		Closed = 16
	}
}
