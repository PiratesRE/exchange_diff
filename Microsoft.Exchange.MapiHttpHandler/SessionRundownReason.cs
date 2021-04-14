using System;

namespace Microsoft.Exchange.MapiHttp
{
	internal enum SessionRundownReason
	{
		ProtocolFault,
		ClientRundown,
		ClientRecreate,
		ContextHandleCleared,
		Expired
	}
}
