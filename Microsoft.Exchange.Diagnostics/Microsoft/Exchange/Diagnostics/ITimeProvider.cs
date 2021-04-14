using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal interface ITimeProvider
	{
		DateTime UtcNow { get; }
	}
}
