using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public enum ErrorBehavior
	{
		Stop,
		SilentlyContinue,
		Continue,
		ErrorAsWarning
	}
}
