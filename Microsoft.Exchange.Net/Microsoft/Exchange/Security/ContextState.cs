using System;

namespace Microsoft.Exchange.Security
{
	internal enum ContextState
	{
		Uninitialized,
		Initialized,
		Negotiating,
		NegotiationComplete
	}
}
