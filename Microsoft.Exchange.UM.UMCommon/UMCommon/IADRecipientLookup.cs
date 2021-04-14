using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IADRecipientLookup
	{
		IRecipientSession ScopedRecipientSession { get; }

		ADUser GetUMDataStorageMailbox();

		ADRecipient LookupByExchangeGuid(Guid exchangeGuid);

		ADRecipient LookupByObjectId(ADObjectId objectId);

		ADRecipient LookupByExtensionAndDialPlan(string extension, UMDialPlan dialPlan);

		ADRecipient LookupByExtensionAndEquivalentDialPlan(string extension, UMDialPlan dialPlan);

		ADRecipient LookupByExchangePrincipal(IExchangePrincipal exchangePrincipal);

		ADRecipient LookupByLegacyExchangeDN(string legacyExchangeDN);

		ADRecipient LookupBySmtpAddress(string emailAddress);

		ADRecipient[] LookupBySmtpAddresses(List<string> smtpAddresses);

		ADRecipient LookupByUmAddress(string proxyAddressStr);

		ADRecipient LookupByParticipant(Participant p);

		ADRecipient LookupBySipExtension(string extension);

		ADRecipient LookupByEumSipResourceIdentifierPrefix(string sipUri);

		ADRecipient[] LookupByDtmfMap(string mode, string dtmf, bool removeHiddenUsers, bool anonymousCaller, UMDialPlan targetDialPlan, int numberOfResults);

		ADRecipient[] LookupByQueryFilter(QueryFilter filter);

		void ProcessRecipients(QueryFilter recipientFilter, PropertyDefinition[] properties, ADConfigurationProcessor<ADRawEntry> configurationProcessor, int retryCount);

		bool TenantSizeExceedsThreshold(QueryFilter filter, int threshold);
	}
}
