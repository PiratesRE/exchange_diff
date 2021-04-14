using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IDiagnosticsConfigProvider
	{
		TimeSpan SmtpRecvLogAsyncInterval { get; }

		int SmtpRecvLogBufferSize { get; }

		TimeSpan SmtpRecvLogFlushInterval { get; }
	}
}
