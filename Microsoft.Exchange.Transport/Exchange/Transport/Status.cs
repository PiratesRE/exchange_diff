using System;

namespace Microsoft.Exchange.Transport
{
	internal enum Status
	{
		Ready,
		Retry,
		Handled,
		Complete,
		Locked
	}
}
