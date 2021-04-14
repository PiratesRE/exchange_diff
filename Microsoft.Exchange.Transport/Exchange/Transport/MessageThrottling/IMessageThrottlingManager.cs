using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal interface IMessageThrottlingManager
	{
		void CleanupIdleEntries();

		bool Enabled { get; }

		MessageThrottlingReason ShouldThrottleMessage(IPAddress ipAddress, int receiveConnectorLimit, MessageRateSourceFlags messageRateSource);

		MessageThrottlingReason ShouldThrottleMessage(Guid mailboxGuid, int userMessageRateLimit, IPAddress ipAddress, int receiveConnectorLimit, MessageRateSourceFlags messageRateSource);

		MessageThrottlingReason ShouldThrottleMessage(Guid mailboxGuid, int messageRateLimit);
	}
}
