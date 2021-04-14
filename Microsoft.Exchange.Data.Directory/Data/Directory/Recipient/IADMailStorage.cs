using System;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADMailStorage
	{
		ADObjectId Database { get; set; }

		DeletedItemRetention DeletedItemFlags { get; set; }

		bool DeliverToMailboxAndForward { get; set; }

		Guid ExchangeGuid { get; set; }

		RawSecurityDescriptor ExchangeSecurityDescriptor { get; set; }

		ExternalOofOptions ExternalOofOptions { get; set; }

		EnhancedTimeSpan RetainDeletedItemsFor { get; set; }

		bool IsMailboxEnabled { get; }

		ADObjectId OfflineAddressBook { get; set; }

		Unlimited<ByteQuantifiedSize> ProhibitSendQuota { get; set; }

		Unlimited<ByteQuantifiedSize> ProhibitSendReceiveQuota { get; set; }

		string ServerLegacyDN { get; set; }

		string ServerName { get; }

		bool? UseDatabaseQuotaDefaults { get; set; }

		Unlimited<ByteQuantifiedSize> IssueWarningQuota { get; set; }

		ByteQuantifiedSize RulesQuota { get; set; }
	}
}
