using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ProcessManager;

namespace Microsoft.Exchange.Transport.MessageThrottling
{
	internal sealed class MessageThrottlingManager : IMessageThrottlingManager
	{
		public MessageThrottlingManager() : this(null)
		{
		}

		public MessageThrottlingManager(IMessageThrottlingManagerConfig config)
		{
			this.config = (config ?? MessageThrottlingManager.CreateDefaultMessageThrottlingManagerConfig());
			this.ipAddressRateLimiter = new RateLimiter<IPAddress>();
			this.userRateLimiter = new RateLimiter<Guid>();
		}

		public bool Enabled
		{
			get
			{
				return this.config.Enabled;
			}
		}

		public MessageThrottlingReason ShouldThrottleMessage(Guid mailboxGuid, int messageRateLimit)
		{
			return this.ShouldThrottleMessage(mailboxGuid, messageRateLimit, IPAddress.Any, 0, MessageRateSourceFlags.User);
		}

		public MessageThrottlingReason ShouldThrottleMessage(IPAddress ipAddress, int receiveConnectorLimit, MessageRateSourceFlags messageRateSource)
		{
			return this.ShouldThrottleMessage(Guid.Empty, 0, ipAddress, receiveConnectorLimit, messageRateSource & MessageRateSourceFlags.IPAddress);
		}

		public MessageThrottlingReason ShouldThrottleMessage(Guid mailboxGuid, int userMessageRateLimit, IPAddress ipAddress, int receiveConnectorLimit, MessageRateSourceFlags messageRateSource)
		{
			MessageThrottlingReason result = MessageThrottlingReason.NotThrottled;
			if (this.Enabled && messageRateSource != MessageRateSourceFlags.None)
			{
				if ((messageRateSource & MessageRateSourceFlags.User) == MessageRateSourceFlags.User && (userMessageRateLimit == 0 || !this.userRateLimiter.TryFetchToken(mailboxGuid, userMessageRateLimit)))
				{
					result = MessageThrottlingReason.UserLimitExceeded;
				}
				else if ((messageRateSource & MessageRateSourceFlags.IPAddress) == MessageRateSourceFlags.IPAddress && (receiveConnectorLimit == 0 || !this.ipAddressRateLimiter.TryFetchToken(ipAddress, receiveConnectorLimit)))
				{
					if ((messageRateSource & MessageRateSourceFlags.User) == MessageRateSourceFlags.User)
					{
						this.userRateLimiter.ReleaseUnusedToken(mailboxGuid);
					}
					result = MessageThrottlingReason.IPAddressLimitExceeded;
				}
			}
			return result;
		}

		public void CleanupIdleEntries()
		{
			DateTime utcNow = DateTime.UtcNow;
			this.ipAddressRateLimiter.CleanupIdleEntries(utcNow);
			this.userRateLimiter.CleanupIdleEntries(utcNow);
		}

		private static IMessageThrottlingManagerConfig CreateDefaultMessageThrottlingManagerConfig()
		{
			return new MessageThrottlingManagerConfig();
		}

		private IMessageThrottlingManagerConfig config;

		private IRateLimiter<IPAddress> ipAddressRateLimiter;

		private IRateLimiter<Guid> userRateLimiter;
	}
}
