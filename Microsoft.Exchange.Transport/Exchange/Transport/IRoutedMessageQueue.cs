using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal interface IRoutedMessageQueue
	{
		NextHopSolutionKey Key { get; }

		SmtpResponse LastError { get; }

		int ActiveQueueLength { get; }
	}
}
