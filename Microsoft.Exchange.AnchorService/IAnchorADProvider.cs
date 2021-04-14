using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorADProvider
	{
		string TenantOrganizationName { get; }

		MicrosoftExchangeRecipient PrimaryExchangeRecipient { get; }

		ADRecipient GetADRecipientByObjectId(ADObjectId objectId);

		void AddCapability(ADObjectId objectId, OrganizationCapability capability);

		void RemoveCapability(ADObjectId objectId, OrganizationCapability capability);

		string GetDatabaseServerFqdn(Guid mdbGuid, bool forceRediscovery);

		string GetMailboxServerFqdn(ADUser user, bool forceRefresh);

		void EnsureLocalMailbox(ADUser user, bool forceRefresh);

		string GetPreferredDomainController();

		ADRecipient GetADRecipientByProxyAddress(string userEmail);

		IEnumerable<ADUser> GetOrganizationMailboxesByCapability(OrganizationCapability capability);

		ADPagedReader<TEntry> FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new();

		ADPagedReader<ADRawEntry> FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties);
	}
}
