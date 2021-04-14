using System;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	[Flags]
	internal enum ContentLineWriteState
	{
		Start = 1,
		Property = 2,
		PropertyValue = 4,
		PropertyEnd = 8,
		Parameter = 16,
		ParameterValue = 32,
		ParameterEnd = 64,
		Closed = 128
	}
}
