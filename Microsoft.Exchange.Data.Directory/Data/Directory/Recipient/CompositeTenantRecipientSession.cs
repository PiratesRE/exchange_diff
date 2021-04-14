using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class CompositeTenantRecipientSession : CompositeDirectorySession<ITenantRecipientSession>, ITenantRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider, IAggregateSession
	{
		protected override string Implementor
		{
			get
			{
				return "CompositeTenantRecipientSession";
			}
		}

		internal CompositeTenantRecipientSession(ITenantRecipientSession cacheSession, ITenantRecipientSession directorySession, bool cacheSessionForDeletingOnly = false) : base(cacheSession, directorySession, cacheSessionForDeletingOnly)
		{
			this.compositeRecipientSession = new CompositeRecipientSession(cacheSession, directorySession, cacheSessionForDeletingOnly);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IRecipientSession GetCompositeRecipientSession()
		{
			return this.compositeRecipientSession;
		}

		ADObjectId IRecipientSession.SearchRoot
		{
			get
			{
				return this.GetCompositeRecipientSession().SearchRoot;
			}
		}

		ITableView IRecipientSession.Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ITableView>(() => this.GetCompositeRecipientSession().Browse(addressListId, rowCountSuggestion, properties), "Browse");
		}

		void IRecipientSession.Delete(ADRecipient instanceToDelete)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetCompositeRecipientSession().Delete(instanceToDelete);
				return true;
			}, "Delete");
		}

		ADRecipient[] IRecipientSession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetCompositeRecipientSession().Find(rootId, scope, filter, sortBy, maxResults), "Find");
		}

		ADRawEntry IRecipientSession.FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(() => this.GetCompositeRecipientSession().FindADRawEntryBySid(sId, properties), "FindADRawEntryBySid");
		}

		ADRawEntry[] IRecipientSession.FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetCompositeRecipientSession().FindADRawEntryByUsnRange(root, startUsn, endUsn, sizeLimit, properties, scope, additionalFilter), "FindADRawEntryByUsnRange");
		}

		Result<ADRecipient>[] IRecipientSession.FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetCompositeRecipientSession().FindADRecipientsByLegacyExchangeDNs(legacyExchangeDNs), "FindADRecipientsByLegacyExchangeDNs");
		}

		ADUser[] IRecipientSession.FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.InvokeWithAPILogging<ADUser[]>(() => this.GetCompositeRecipientSession().FindADUser(rootId, scope, filter, sortBy, maxResults), "FindADUser");
		}

		ADUser IRecipientSession.FindADUserByObjectId(ADObjectId adObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADUser>(() => this.GetCompositeRecipientSession().FindADUserByObjectId(adObjectId), "FindADUserByObjectId");
		}

		ADUser IRecipientSession.FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADUser>(() => this.GetCompositeRecipientSession().FindADUserByExternalDirectoryObjectId(externalDirectoryObjectId), "FindADUserByExternalDirectoryObjectId");
		}

		ADObject IRecipientSession.FindByAccountName<T>(string domainName, string accountName)
		{
			return base.InvokeGetObjectWithAPILogging<ADObject>(() => this.GetCompositeRecipientSession().FindByAccountName<T>(domainName, accountName), "FindByAccountName");
		}

		IEnumerable<T> IRecipientSession.FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter)
		{
			return base.InvokeWithAPILogging<IEnumerable<T>>(() => this.GetCompositeRecipientSession().FindByAccountName<T>(domain, account, rootId, searchFilter), "FindByAccountName");
		}

		ADRecipient[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetCompositeRecipientSession().FindByANR(anrMatch, maxResults, sortBy), "FindByANR");
		}

		ADRawEntry[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetCompositeRecipientSession().FindByANR(anrMatch, maxResults, sortBy, properties), "FindByANR");
		}

		ADRecipient IRecipientSession.FindByCertificate(X509Identifier identifier)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByCertificate(identifier), "FindByCertificate");
		}

		ADRawEntry[] IRecipientSession.FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetCompositeRecipientSession().FindByCertificate(identifier, properties), "FindByCertificate");
		}

		ADRawEntry IRecipientSession.FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(() => this.GetCompositeRecipientSession().FindByExchangeGuid(exchangeGuid, properties), "FindByExchangeGuid");
		}

		TEntry IRecipientSession.FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TEntry>(() => this.GetCompositeRecipientSession().FindByExchangeGuid<TEntry>(exchangeGuid, properties), "FindByExchangeGuid");
		}

		ADRecipient IRecipientSession.FindByExchangeObjectId(Guid exchangeObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByExchangeObjectId(exchangeObjectId), "FindByExchangeObjectId");
		}

		ADRecipient IRecipientSession.FindByExchangeGuid(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByExchangeGuid(exchangeGuid), "FindByExchangeGuid");
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByExchangeGuidIncludingAlternate(exchangeGuid), "FindByExchangeGuidIncludingAlternate");
		}

		ADRawEntry IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(() => this.GetCompositeRecipientSession().FindByExchangeGuidIncludingAlternate(exchangeGuid, properties), "FindByExchangeGuidIncludingAlternate");
		}

		TObject IRecipientSession.FindByExchangeGuidIncludingAlternate<TObject>(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<TObject>(() => this.GetCompositeRecipientSession().FindByExchangeGuidIncludingAlternate<TObject>(exchangeGuid), "FindByExchangeGuidIncludingAlternate");
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByExchangeGuidIncludingArchive(exchangeGuid), "FindByExchangeGuidIncludingArchive");
		}

		Result<ADRecipient>[] IRecipientSession.FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetCompositeRecipientSession().FindByExchangeGuidsIncludingArchive(keys), "FindByExchangeGuidsIncludingArchive");
		}

		ADRecipient IRecipientSession.FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByLegacyExchangeDN(legacyExchangeDN), "FindByLegacyExchangeDN");
		}

		Result<ADRawEntry>[] IRecipientSession.FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetCompositeRecipientSession().FindByLegacyExchangeDNs(legacyExchangeDNs, properties), "FindByLegacyExchangeDNs");
		}

		ADRecipient IRecipientSession.FindByObjectGuid(Guid guid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByObjectGuid(guid), "FindByObjectGuid");
		}

		ADRecipient IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindByProxyAddress(proxyAddress), "FindByProxyAddress");
		}

		ADRawEntry IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(() => this.GetCompositeRecipientSession().FindByProxyAddress(proxyAddress, properties), "FindByProxyAddress");
		}

		TEntry IRecipientSession.FindByProxyAddress<TEntry>(ProxyAddress proxyAddress)
		{
			return base.InvokeGetObjectWithAPILogging<TEntry>(() => this.GetCompositeRecipientSession().FindByProxyAddress<TEntry>(proxyAddress), "FindByProxyAddress");
		}

		Result<ADRawEntry>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetCompositeRecipientSession().FindByProxyAddresses(proxyAddresses, properties), "FindByProxyAddresses");
		}

		Result<TEntry>[] IRecipientSession.FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses)
		{
			return base.InvokeWithAPILogging<Result<TEntry>[]>(() => this.GetCompositeRecipientSession().FindByProxyAddresses<TEntry>(proxyAddresses), "FindByProxyAddresses");
		}

		Result<ADRecipient>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetCompositeRecipientSession().FindByProxyAddresses(proxyAddresses), "FindByProxyAddresses");
		}

		ADRecipient IRecipientSession.FindBySid(SecurityIdentifier sId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().FindBySid(sId), "FindBySid");
		}

		ADRawEntry[] IRecipientSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetCompositeRecipientSession().FindDeletedADRawEntryByUsnRange(lastKnownParentId, startUsn, sizeLimit, properties, additionalFilter), "FindDeletedADRawEntryByUsnRange");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<MiniRecipient[]>(() => this.GetCompositeRecipientSession().FindMiniRecipient(rootId, scope, filter, sortBy, maxResults, properties), "FindMiniRecipient");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<MiniRecipient[]>(() => this.GetCompositeRecipientSession().FindMiniRecipientByANR(anrMatch, maxResults, sortBy, properties), "FindMiniRecipientByANR");
		}

		TResult IRecipientSession.FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.GetCompositeRecipientSession().FindMiniRecipientByProxyAddress<TResult>(proxyAddress, properties), "FindMiniRecipientByProxyAddress");
		}

		TResult IRecipientSession.FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.GetCompositeRecipientSession().FindMiniRecipientBySid<TResult>(sId, properties), "FindMiniRecipientBySid");
		}

		ADRecipient[] IRecipientSession.FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetCompositeRecipientSession().FindNames(dictionary, limit), "FindNames");
		}

		object[][] IRecipientSession.FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<object[][]>(() => this.GetCompositeRecipientSession().FindNamesView(dictionary, limit, properties), "FindNamesView");
		}

		Result<OWAMiniRecipient>[] IRecipientSession.FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			return base.InvokeWithAPILogging<Result<OWAMiniRecipient>[]>(() => this.GetCompositeRecipientSession().FindOWAMiniRecipientByUserPrincipalName(userPrincipalNames), "FindOWAMiniRecipientByUserPrincipalName");
		}

		ADPagedReader<ADRecipient> IRecipientSession.FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			return base.InvokeWithAPILogging<ADPagedReader<ADRecipient>>(() => this.GetCompositeRecipientSession().FindPaged(rootId, scope, filter, sortBy, pageSize), "FindPaged");
		}

		ADPagedReader<TEntry> IRecipientSession.FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADPagedReader<TEntry>>(() => this.GetCompositeRecipientSession().FindPagedMiniRecipient<TEntry>(rootId, scope, filter, sortBy, pageSize, properties), "FindPagedMiniRecipient");
		}

		ADRawEntry[] IRecipientSession.FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetCompositeRecipientSession().FindRecipient(rootId, scope, filter, sortBy, maxResults, properties), "FindRecipient");
		}

		IEnumerable<ADGroup> IRecipientSession.FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId)
		{
			return base.InvokeWithAPILogging<IEnumerable<ADGroup>>(() => this.GetCompositeRecipientSession().FindRoleGroupsByForeignGroupSid(root, sId), "FindRoleGroupsByForeignGroupSid");
		}

		List<string> IRecipientSession.GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			return base.InvokeWithAPILogging<List<string>>(() => this.GetCompositeRecipientSession().GetTokenSids(user, assignmentMethodFlags), "GetTokenSids");
		}

		List<string> IRecipientSession.GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			return base.InvokeWithAPILogging<List<string>>(() => this.GetCompositeRecipientSession().GetTokenSids(userId, assignmentMethodFlags), "GetTokenSids");
		}

		SecurityIdentifier IRecipientSession.GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			return base.InvokeWithAPILogging<SecurityIdentifier>(() => this.GetCompositeRecipientSession().GetWellKnownExchangeGroupSid(wkguid), "GetWellKnownExchangeGroupSid");
		}

		bool IRecipientSession.IsLegacyDNInUse(string legacyDN)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetCompositeRecipientSession().IsLegacyDNInUse(legacyDN), "IsLegacyDNInUse");
		}

		bool IRecipientSession.IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetCompositeRecipientSession().IsMemberOfGroupByWellKnownGuid(wellKnownGuid, containerDN, id), "IsMemberOfGroupByWellKnownGuid");
		}

		bool IRecipientSession.IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetCompositeRecipientSession().IsRecipientInOrg(proxyAddress), "IsRecipientInOrg");
		}

		bool IRecipientSession.IsReducedRecipientSession()
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetCompositeRecipientSession().IsReducedRecipientSession(), "IsReducedRecipientSession");
		}

		bool IRecipientSession.IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetCompositeRecipientSession().IsThrottlingPolicyInUse(throttlingPolicyId), "IsThrottlingPolicyInUse");
		}

		ADRecipient IRecipientSession.Read(ADObjectId entryId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetCompositeRecipientSession().Read(entryId), "Read");
		}

		MiniRecipient IRecipientSession.ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<MiniRecipient>(() => this.GetCompositeRecipientSession().ReadMiniRecipient(entryId, properties), "ReadMiniRecipient");
		}

		TMiniRecipient IRecipientSession.ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TMiniRecipient>(() => this.GetCompositeRecipientSession().ReadMiniRecipient<TMiniRecipient>(entryId, properties), "ReadMiniRecipient");
		}

		ADRawEntry IRecipientSession.FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(() => this.GetCompositeRecipientSession().FindUserBySid(sId, properties), "FindUserBySid");
		}

		Result<ADRecipient>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetCompositeRecipientSession().ReadMultiple(entryIds), "ReadMultiple");
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetCompositeRecipientSession().ReadMultiple(entryIds, properties), "ReadMultiple");
		}

		Result<ADGroup>[] IRecipientSession.ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			return base.InvokeWithAPILogging<Result<ADGroup>[]>(() => this.GetCompositeRecipientSession().ReadMultipleADGroups(entryIds), "ReadMultipleADGroups");
		}

		Result<ADUser>[] IRecipientSession.ReadMultipleADUsers(ADObjectId[] userIds)
		{
			return base.InvokeWithAPILogging<Result<ADUser>[]>(() => this.GetCompositeRecipientSession().ReadMultipleADUsers(userIds), "ReadMultipleADUsers");
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetCompositeRecipientSession().ReadMultipleWithDeletedObjects(entryIds, properties), "ReadMultipleWithDeletedObjects");
		}

		MiniRecipientWithTokenGroups IRecipientSession.ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			return this.GetCompositeRecipientSession().ReadTokenGroupsGlobalAndUniversal(id);
		}

		ADObjectId[] IRecipientSession.ResolveSidsToADObjectIds(string[] sids)
		{
			return base.InvokeWithAPILogging<ADObjectId[]>(() => this.GetCompositeRecipientSession().ResolveSidsToADObjectIds(sids), "ResolveSidsToADObjectIds");
		}

		void IRecipientSession.Save(ADRecipient instanceToSave)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetCompositeRecipientSession().Save(instanceToSave);
				return true;
			}, "Save");
		}

		void IRecipientSession.Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetCompositeRecipientSession().Save(instanceToSave, bypassValidation);
				return true;
			}, "Save");
		}

		void IRecipientSession.SetPassword(ADObject obj, SecureString password)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetCompositeRecipientSession().SetPassword(obj, password);
				return true;
			}, "SetPassword");
		}

		void IRecipientSession.SetPassword(ADObjectId id, SecureString password)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetCompositeRecipientSession().SetPassword(id, password);
				return true;
			}, "SetPassword");
		}

		ADRawEntry ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADRawEntry[] entries)
		{
			return base.InvokeWithAPILogging<ADRawEntry>(() => this.GetSession().ChooseBetweenAmbiguousUsers(entries), "ChooseBetweenAmbiguousUsers");
		}

		ADObjectId ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id)
		{
			return base.InvokeWithAPILogging<ADObjectId>(() => this.GetSession().ChooseBetweenAmbiguousUsers(user1Id, user2Id), "ChooseBetweenAmbiguousUsers");
		}

		DirectoryBackendType ITenantRecipientSession.DirectoryBackendType
		{
			get
			{
				return base.GetSession().DirectoryBackendType;
			}
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().FindByExternalDirectoryObjectIds(externalDirectoryObjectIds, properties), "FindByExternalDirectoryObjectIds");
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().FindByExternalDirectoryObjectIds(externalDirectoryObjectIds, includeDeletedObjects, properties), "FindByExternalDirectoryObjectIds");
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindByNetID(netID, organizationContext, properties), "FindByNetID");
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindByNetID(netID, properties), "FindByNetID");
		}

		MiniRecipient ITenantRecipientSession.FindRecipientByNetID(NetID netId)
		{
			return base.InvokeGetObjectWithAPILogging<MiniRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<MiniRecipient>((ITenantRecipientSession session) => session.FindRecipientByNetID(netId), null, null), "FindRecipientByNetID");
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] propertiesToRead = new List<PropertyDefinition>(properties)
				{
					ADUserSchema.NetID
				}.ToArray();
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((ITenantRecipientSession session) => session.FindUniqueEntryByNetID(netID, organizationContext, propertiesToRead), null, propertiesToRead);
			}, "FindUniqueEntryByNetID");
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] propertiesToRead = new List<PropertyDefinition>(properties)
				{
					ADUserSchema.NetID
				}.ToArray();
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((ITenantRecipientSession session) => session.FindUniqueEntryByNetID(netID, propertiesToRead), null, propertiesToRead);
			}, "FindUniqueEntryByNetID");
		}

		public ADRawEntry FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry>(() => this.GetSession().FindByLiveIdMemberName(liveIdMemberName, properties), "FindByLiveIdMemberName");
		}

		Result<ADRawEntry>[] ITenantRecipientSession.ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().ReadMultipleByLinkedPartnerId(entryIds, properties), "ReadMultipleByLinkedPartnerId");
		}

		MbxReadMode IAggregateSession.MbxReadMode
		{
			get
			{
				IAggregateSession aggregateSession = base.GetSession() as IAggregateSession;
				if (aggregateSession != null)
				{
					return aggregateSession.MbxReadMode;
				}
				LocalizedString message = DirectoryStrings.ApiNotSupportedInBusinessSessionError(base.GetSession().GetType().FullName, "MbxReadMode");
				throw new ApiNotSupportedException(message);
			}
			set
			{
				IAggregateSession aggregateSession = base.GetSession() as IAggregateSession;
				if (aggregateSession != null)
				{
					aggregateSession.MbxReadMode = value;
					return;
				}
				LocalizedString message = DirectoryStrings.ApiNotSupportedInBusinessSessionError(base.GetSession().GetType().FullName, "MbxReadMode");
				throw new ApiNotSupportedException(message);
			}
		}

		BackendWriteMode IAggregateSession.BackendWriteMode
		{
			get
			{
				IAggregateSession aggregateSession = base.GetSession() as IAggregateSession;
				if (aggregateSession != null)
				{
					return aggregateSession.BackendWriteMode;
				}
				LocalizedString message = DirectoryStrings.ApiNotSupportedInBusinessSessionError(base.GetSession().GetType().FullName, "BackendWriteMode");
				throw new ApiNotSupportedException(message);
			}
			set
			{
				IAggregateSession aggregateSession = base.GetSession() as IAggregateSession;
				if (aggregateSession != null)
				{
					aggregateSession.BackendWriteMode = value;
					return;
				}
				LocalizedString message = DirectoryStrings.ApiNotSupportedInBusinessSessionError(base.GetSession().GetType().FullName, "BackendWriteMode");
				throw new ApiNotSupportedException(message);
			}
		}

		private CompositeRecipientSession compositeRecipientSession;
	}
}
