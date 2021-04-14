using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IRecipientSession : IDirectorySession, IConfigDataProvider
	{
		ADObjectId SearchRoot { get; }

		ITableView Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties);

		void Delete(ADRecipient instanceToDelete);

		ADRecipient[] Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults);

		ADRawEntry FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties);

		ADRawEntry[] FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter);

		Result<ADRecipient>[] FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs);

		ADUser[] FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults);

		ADUser FindADUserByObjectId(ADObjectId adObjectId);

		ADUser FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId);

		ADObject FindByAccountName<T>(string domainName, string accountName) where T : IConfigurable, new();

		IEnumerable<T> FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter) where T : IConfigurable, new();

		ADRecipient[] FindByANR(string anrMatch, int maxResults, SortBy sortBy);

		ADRawEntry[] FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties);

		ADRecipient FindByCertificate(X509Identifier identifier);

		ADRawEntry[] FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties);

		ADRawEntry FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties);

		TEntry FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new();

		ADRecipient FindByExchangeObjectId(Guid exchangeObjectId);

		ADRecipient FindByExchangeGuid(Guid exchangeGuid);

		ADRecipient FindByExchangeGuidIncludingAlternate(Guid exchangeGuid);

		ADRawEntry FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties);

		TObject FindByExchangeGuidIncludingAlternate<TObject>(Guid exchangeGuid) where TObject : ADObject, new();

		ADRecipient FindByExchangeGuidIncludingArchive(Guid exchangeGuid);

		Result<ADRecipient>[] FindByExchangeGuidsIncludingArchive(Guid[] keys);

		ADRecipient FindByLegacyExchangeDN(string legacyExchangeDN);

		Result<ADRawEntry>[] FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties);

		ADRecipient FindByObjectGuid(Guid guid);

		ADRecipient FindByProxyAddress(ProxyAddress proxyAddress);

		ADRawEntry FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties);

		TEntry FindByProxyAddress<TEntry>(ProxyAddress proxyAddress) where TEntry : ADObject, new();

		Result<ADRawEntry>[] FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties);

		Result<TEntry>[] FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses) where TEntry : ADObject, new();

		Result<ADRecipient>[] FindByProxyAddresses(ProxyAddress[] proxyAddresses);

		ADRecipient FindBySid(SecurityIdentifier sId);

		ADRawEntry[] FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter);

		MiniRecipient[] FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties);

		MiniRecipient[] FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties);

		TResult FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties) where TResult : MiniRecipient, new();

		TResult FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties) where TResult : MiniRecipient, new();

		ADRecipient[] FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit);

		object[][] FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties);

		Result<OWAMiniRecipient>[] FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames);

		ADPagedReader<ADRecipient> FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize);

		ADPagedReader<TEntry> FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new();

		ADRawEntry[] FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties);

		IEnumerable<ADGroup> FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId);

		List<string> GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags);

		List<string> GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags);

		SecurityIdentifier GetWellKnownExchangeGroupSid(Guid wkguid);

		bool IsLegacyDNInUse(string legacyDN);

		bool IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id);

		bool IsRecipientInOrg(ProxyAddress proxyAddress);

		bool IsReducedRecipientSession();

		bool IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId);

		ADRecipient Read(ADObjectId entryId);

		MiniRecipient ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties);

		TMiniRecipient ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties) where TMiniRecipient : ADObject, new();

		ADRawEntry FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties);

		Result<ADRecipient>[] ReadMultiple(ADObjectId[] entryIds);

		Result<ADRawEntry>[] ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties);

		Result<ADGroup>[] ReadMultipleADGroups(ADObjectId[] entryIds);

		Result<ADUser>[] ReadMultipleADUsers(ADObjectId[] userIds);

		Result<ADRawEntry>[] ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties);

		MiniRecipientWithTokenGroups ReadTokenGroupsGlobalAndUniversal(ADObjectId id);

		ADObjectId[] ResolveSidsToADObjectIds(string[] sids);

		void Save(ADRecipient instanceToSave);

		void Save(ADRecipient instanceToSave, bool bypassValidation);

		void SetPassword(ADObject obj, SecureString password);

		void SetPassword(ADObjectId id, SecureString password);
	}
}
