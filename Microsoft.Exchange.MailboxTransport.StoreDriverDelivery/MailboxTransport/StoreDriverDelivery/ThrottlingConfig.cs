using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class ThrottlingConfig : IThrottlingConfig
	{
		public int RecipientThreadLimit
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.RecipientThreadLimit;
			}
		}

		public int MaxConcurrentMailboxDeliveries
		{
			get
			{
				return Components.Configuration.LocalServer.MaxConcurrentMailboxDeliveries;
			}
		}

		public int MailboxServerThreadLimit
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxServerThreadLimit;
			}
		}

		public int MaxMailboxDeliveryPerMdbConnections
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryPerMdbConnections;
			}
		}

		public bool MailboxDeliveryThrottlingEnabled
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxDeliveryThrottlingEnabled;
			}
		}

		public int MdbHealthMediumToHighThreshold
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MdbHealthMediumToHighThreshold;
			}
		}

		public int MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent;
			}
		}

		public int MdbHealthLowToMediumThreshold
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MdbHealthLowToMediumThreshold;
			}
		}

		public int MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent;
			}
		}

		public int MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent;
			}
		}

		public int MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent;
			}
		}

		public bool DynamicMailboxDatabaseThrottlingEnabled
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.DynamicMailboxDatabaseThrottlingEnabled;
			}
		}

		public TimeSpan AcquireConnectionTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds(Math.Round(Components.TransportAppConfig.ConnectionCacheConfig.ConnectionInactivityTimeout.TotalMilliseconds * 0.8));
			}
		}

		public ulong MaxConcurrentMessageSizeLimit
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MaxMailboxDeliveryConcurrentMessageSizeLimit.ToBytes();
			}
		}

		public bool ThrottlingLogEnabled
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.MailboxDeliveryThrottlingLogEnabled;
			}
		}

		public EnhancedTimeSpan ThrottlingLogMaxAge
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.MailboxDeliveryThrottlingLogMaxAge;
			}
		}

		public Unlimited<ByteQuantifiedSize> ThrottlingLogMaxDirectorySize
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.MailboxDeliveryThrottlingLogMaxDirectorySize;
			}
		}

		public Unlimited<ByteQuantifiedSize> ThrottlingLogMaxFileSize
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.MailboxDeliveryThrottlingLogMaxFileSize;
			}
		}

		public LocalLongFullPath ThrottlingLogPath
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.MailboxDeliveryThrottlingLogPath;
			}
		}

		public int ThrottlingLogBufferSize
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxDeliveryThrottlingLogBufferSize;
			}
		}

		public TimeSpan ThrottlingLogFlushInterval
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxDeliveryThrottlingLogFlushInterval;
			}
		}

		public TimeSpan AsyncLogInterval
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxDeliveryThrottlingLogAsyncLogInterval;
			}
		}

		public TimeSpan ThrottlingSummaryLoggingInterval
		{
			get
			{
				return Components.TransportAppConfig.RemoteDelivery.MailboxDeliveryThrottlingLogSummaryLoggingInterval;
			}
		}
	}
}
