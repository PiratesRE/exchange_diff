using System;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	internal enum MigrationServiceRpcResultCode
	{
		Success = 4097,
		VersionMismatchError = 8193,
		ArgumentMismatchError,
		ServerShutdown = 40963,
		UnknownMethodError = 8196,
		ResultParseError,
		IncorrectMethodInvokedError,
		InvalidSubscriptionAction,
		PropertyBagMissingError,
		ServerNotInitialized = 40969,
		InvalidSubscriptionMessageId = 16385,
		SubscriptionCreationFailed,
		MailboxNotFound = 16643,
		SubscriptionNotFound,
		SubscriptionUpdateFailed = 16389,
		MigrationJobNotFound,
		MigrationTransientError = 49159,
		StorageTransientError,
		MigrationPermanentError = 16393,
		SubscriptionAlreadyFinalized,
		StoragePermanentError = 16400,
		RpcException = 8209,
		RpcTransientException = 40978
	}
}
