using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IThrottlingConfig
	{
		int RecipientThreadLimit { get; }

		int MaxConcurrentMailboxDeliveries { get; }

		int MailboxServerThreadLimit { get; }

		int MaxMailboxDeliveryPerMdbConnections { get; }

		bool MailboxDeliveryThrottlingEnabled { get; }

		int MdbHealthMediumToHighThreshold { get; }

		int MaxMailboxDeliveryPerMdbConnectionsHighHealthPercent { get; }

		int MdbHealthLowToMediumThreshold { get; }

		int MaxMailboxDeliveryPerMdbConnectionsMediumHealthPercent { get; }

		int MaxMailboxDeliveryPerMdbConnectionsLowHealthPercent { get; }

		int MaxMailboxDeliveryPerMdbConnectionsLowestHealthPercent { get; }

		bool DynamicMailboxDatabaseThrottlingEnabled { get; }

		TimeSpan AcquireConnectionTimeout { get; }

		ulong MaxConcurrentMessageSizeLimit { get; }

		bool ThrottlingLogEnabled { get; }

		EnhancedTimeSpan ThrottlingLogMaxAge { get; }

		Unlimited<ByteQuantifiedSize> ThrottlingLogMaxDirectorySize { get; }

		Unlimited<ByteQuantifiedSize> ThrottlingLogMaxFileSize { get; }

		LocalLongFullPath ThrottlingLogPath { get; }

		int ThrottlingLogBufferSize { get; }

		TimeSpan ThrottlingLogFlushInterval { get; }

		TimeSpan AsyncLogInterval { get; }

		TimeSpan ThrottlingSummaryLoggingInterval { get; }
	}
}
