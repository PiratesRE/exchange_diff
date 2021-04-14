using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal interface IStoreProvider
	{
		TObject FindByExchangeGuidIncludingAlternate<TObject>(Guid mailboxGuid, TenantPartitionHint tenantPartitionHint) where TObject : ADObject, new();

		MailboxSession OpenStore(OrganizationId organizationId, string displayName, string mailboxFqdn, string mailboxServerDN, Guid mailboxGuid, Guid mdbGuid, MultiValuedProperty<CultureInfo> senderLocales, MultiValuedProperty<Guid> aggregatedMailboxGuids);

		PublicFolderSession OpenStore(OrganizationId organizationId, Guid mailboxGuid);

		MessageItem GetMessageItem(StoreSession storeSession, StoreId storeId, StorePropertyDefinition[] contentConversionProperties);

		Exception CallDoneWithMessageWithRetry(StoreSession session, MessageItem item, int retryCount, MailItemSubmitter context);

		bool TryGetSendAsSubscription(MessageItem item, SendAsManager sendAsManager, out ISendAsSource subscription);
	}
}
