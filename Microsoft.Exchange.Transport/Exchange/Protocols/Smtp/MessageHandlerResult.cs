using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum MessageHandlerResult
	{
		Complete,
		Failure,
		MoreDataRequired
	}
}
