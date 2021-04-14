using System;

namespace Microsoft.Exchange.Monitoring
{
	public enum ActiveDirectorySearchError
	{
		None,
		OverThreshold,
		ADTransientException,
		OtherException = 10
	}
}
