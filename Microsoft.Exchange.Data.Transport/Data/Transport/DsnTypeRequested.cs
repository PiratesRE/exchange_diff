using System;

namespace Microsoft.Exchange.Data.Transport
{
	public enum DsnTypeRequested
	{
		NotSpecified,
		Success,
		Failure,
		Delay = 4,
		Never = 8
	}
}
