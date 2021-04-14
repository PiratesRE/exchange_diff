using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum ThrottlingAction
	{
		TempReject,
		Tarpit,
		Drop,
		PriNone
	}
}
