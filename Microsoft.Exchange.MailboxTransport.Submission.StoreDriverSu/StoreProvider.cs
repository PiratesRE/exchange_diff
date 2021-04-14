using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class StoreProvider
	{
		public static IStoreProvider StoreProviderInstance
		{
			get
			{
				if (StoreProvider.storeProvider == null)
				{
					StoreProvider.storeProvider = ADStoreProvider.Instance;
				}
				return StoreProvider.storeProvider;
			}
			set
			{
				StoreProvider.storeProvider = value;
			}
		}

		public static TObject FindByExchangeGuidIncludingAlternate<TObject>(Guid mailboxGuid, TenantPartitionHint tenantPartitionHint) where TObject : ADObject, new()
		{
			return StoreProvider.StoreProviderInstance.FindByExchangeGuidIncludingAlternate<TObject>(mailboxGuid, tenantPartitionHint);
		}

		public static MailboxSession OpenStore(OrganizationId organizationId, string displayName, string mailboxFqdn, string mailboxServerDN, Guid mailboxGuid, Guid mdbGuid, MultiValuedProperty<CultureInfo> senderLocales, MultiValuedProperty<Guid> aggregatedMailboxGuids)
		{
			return StoreProvider.StoreProviderInstance.OpenStore(organizationId, displayName, mailboxFqdn, mailboxServerDN, mailboxGuid, mdbGuid, senderLocales, aggregatedMailboxGuids);
		}

		public static PublicFolderSession OpenStore(OrganizationId organizationId, Guid mailboxGuid)
		{
			return StoreProvider.StoreProviderInstance.OpenStore(organizationId, mailboxGuid);
		}

		public static MessageItem GetMessageItem(StoreSession storeSession, StoreId storeId, StorePropertyDefinition[] contentConversionProperties)
		{
			return StoreProvider.StoreProviderInstance.GetMessageItem(storeSession, storeId, contentConversionProperties);
		}

		public static Exception CallDoneWithMessageWithRetry(StoreSession session, MessageItem item, int retryCount, MailItemSubmitter context)
		{
			return StoreProvider.StoreProviderInstance.CallDoneWithMessageWithRetry(session, item, retryCount, context);
		}

		public static bool TryGetSendAsSubscription(MessageItem item, SendAsManager sendAsManager, out ISendAsSource subscription)
		{
			return StoreProvider.StoreProviderInstance.TryGetSendAsSubscription(item, sendAsManager, out subscription);
		}

		private static IStoreProvider storeProvider;
	}
}
