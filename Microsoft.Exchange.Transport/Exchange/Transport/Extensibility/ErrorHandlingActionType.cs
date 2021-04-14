using System;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal enum ErrorHandlingActionType
	{
		NDR,
		Defer,
		Drop,
		Allow
	}
}
