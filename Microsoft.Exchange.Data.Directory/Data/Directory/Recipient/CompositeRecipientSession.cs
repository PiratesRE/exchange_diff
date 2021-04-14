using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class CompositeRecipientSession : CompositeDirectorySession<IRecipientSession>, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		protected override string Implementor
		{
			get
			{
				return "CompositeRecipientSession";
			}
		}

		internal CompositeRecipientSession(IRecipientSession cacheSession, IRecipientSession directorySession, bool cacheSessionForDeletingOnly = false) : base(cacheSession, directorySession, cacheSessionForDeletingOnly)
		{
		}

		ADObjectId IRecipientSession.SearchRoot
		{
			get
			{
				return base.GetSession().SearchRoot;
			}
		}

		ITableView IRecipientSession.Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ITableView>(() => this.GetSession().Browse(addressListId, rowCountSuggestion, properties), "Browse");
		}

		void IRecipientSession.Delete(ADRecipient instanceToDelete)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.InternalDelete(instanceToDelete);
				return true;
			}, "Delete");
		}

		ADRecipient[] IRecipientSession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetSession().Find(rootId, scope, filter, sortBy, maxResults), "Find");
		}

		ADRawEntry IRecipientSession.FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] second = new PropertyDefinition[]
				{
					IADSecurityPrincipalSchema.Sid
				};
				IEnumerable<PropertyDefinition> propertiesToRead = properties.Concat(second);
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((IRecipientSession session) => session.FindADRawEntryBySid(sId, propertiesToRead), null, propertiesToRead);
			}, "FindADRawEntryBySid");
		}

		ADRawEntry[] IRecipientSession.FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindADRawEntryByUsnRange(root, startUsn, endUsn, sizeLimit, properties, scope, additionalFilter), "FindADRawEntryByUsnRange");
		}

		Result<ADRecipient>[] IRecipientSession.FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetSession().FindADRecipientsByLegacyExchangeDNs(legacyExchangeDNs), "FindADRecipientsByLegacyExchangeDNs");
		}

		ADUser[] IRecipientSession.FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.InvokeWithAPILogging<ADUser[]>(() => this.GetSession().FindADUser(rootId, scope, filter, sortBy, maxResults), "FindADUser");
		}

		ADUser IRecipientSession.FindADUserByObjectId(ADObjectId adObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADUser>(() => this.ExecuteSingleObjectQueryWithFallback<ADUser>((IRecipientSession session) => session.FindADUserByObjectId(adObjectId), null, null), "FindADUserByObjectId");
		}

		ADUser IRecipientSession.FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADUser>(() => this.GetSession().FindADUserByExternalDirectoryObjectId(externalDirectoryObjectId), "FindADUserByExternalDirectoryObjectId");
		}

		ADObject IRecipientSession.FindByAccountName<T>(string domainName, string accountName)
		{
			return base.InvokeGetObjectWithAPILogging<ADObject>(() => this.GetSession().FindByAccountName<T>(domainName, accountName), "FindByAccountName");
		}

		IEnumerable<T> IRecipientSession.FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter)
		{
			return base.InvokeWithAPILogging<IEnumerable<T>>(() => this.GetSession().FindByAccountName<T>(domain, account, rootId, searchFilter), "FindByAccountName");
		}

		ADRecipient[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetSession().FindByANR(anrMatch, maxResults, sortBy), "FindByANR");
		}

		ADRawEntry[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindByANR(anrMatch, maxResults, sortBy, properties), "FindByANR");
		}

		ADRecipient IRecipientSession.FindByCertificate(X509Identifier identifier)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetSession().FindByCertificate(identifier), "FindByCertificate");
		}

		ADRawEntry[] IRecipientSession.FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindByCertificate(identifier, properties), "FindByCertificate");
		}

		ADRawEntry IRecipientSession.FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] second = new PropertyDefinition[]
				{
					ADMailboxRecipientSchema.ExchangeGuid
				};
				IEnumerable<PropertyDefinition> propertiesToRead = properties.Concat(second);
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((IRecipientSession session) => session.FindByExchangeGuid(exchangeGuid, propertiesToRead), null, propertiesToRead);
			}, "FindByExchangeGuid");
		}

		TEntry IRecipientSession.FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TEntry>(() => this.GetSession().FindByExchangeGuid<TEntry>(exchangeGuid, properties), "FindByExchangeGuid");
		}

		ADRecipient IRecipientSession.FindByExchangeObjectId(Guid exchangeObjectId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.GetSession().FindByExchangeObjectId(exchangeObjectId), "FindByExchangeObjectId");
		}

		ADRecipient IRecipientSession.FindByExchangeGuid(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByExchangeGuid(exchangeGuid), null, null), "FindByExchangeGuid");
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByExchangeGuidIncludingAlternate(exchangeGuid), null, null), "FindByExchangeGuidIncludingAlternate");
		}

		ADRawEntry IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] second = new PropertyDefinition[]
				{
					ADMailboxRecipientSchema.ExchangeGuid
				};
				IEnumerable<PropertyDefinition> propertiesToRead = properties.Concat(second);
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((IRecipientSession session) => session.FindByExchangeGuidIncludingAlternate(exchangeGuid, propertiesToRead), null, propertiesToRead);
			}, "FindByExchangeGuidIncludingAlternate");
		}

		TObject IRecipientSession.FindByExchangeGuidIncludingAlternate<TObject>(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<TObject>(() => this.ExecuteSingleObjectQueryWithFallback<TObject>((IRecipientSession session) => session.FindByExchangeGuidIncludingAlternate<TObject>(exchangeGuid), null, null), "FindByExchangeGuidIncludingAlternate");
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByExchangeGuidIncludingArchive(exchangeGuid), null, null), "FindByExchangeGuidIncludingArchive");
		}

		Result<ADRecipient>[] IRecipientSession.FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetSession().FindByExchangeGuidsIncludingArchive(keys), "FindByExchangeGuidsIncludingArchive");
		}

		ADRecipient IRecipientSession.FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByLegacyExchangeDN(legacyExchangeDN), null, null), "FindByLegacyExchangeDN");
		}

		Result<ADRawEntry>[] IRecipientSession.FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().FindByLegacyExchangeDNs(legacyExchangeDNs, properties), "FindByLegacyExchangeDNs");
		}

		ADRecipient IRecipientSession.FindByObjectGuid(Guid guid)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByObjectGuid(guid), null, null), "FindByObjectGuid");
		}

		ADRecipient IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindByProxyAddress(proxyAddress), null, null), "FindByProxyAddress");
		}

		ADRawEntry IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties == null)
				{
					return null;
				}
				PropertyDefinition[] second = new PropertyDefinition[]
				{
					ADRecipientSchema.EmailAddresses
				};
				IEnumerable<PropertyDefinition> propertiesToRead = properties.Concat(second);
				return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((IRecipientSession session) => session.FindByProxyAddress(proxyAddress, propertiesToRead), null, propertiesToRead);
			}, "FindByProxyAddress");
		}

		TEntry IRecipientSession.FindByProxyAddress<TEntry>(ProxyAddress proxyAddress)
		{
			return base.InvokeGetObjectWithAPILogging<TEntry>(() => this.GetSession().FindByProxyAddress<TEntry>(proxyAddress), "FindByProxyAddress");
		}

		Result<ADRawEntry>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().FindByProxyAddresses(proxyAddresses, properties), "FindByProxyAddresses");
		}

		Result<TEntry>[] IRecipientSession.FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses)
		{
			return base.InvokeWithAPILogging<Result<TEntry>[]>(() => this.GetSession().FindByProxyAddresses<TEntry>(proxyAddresses), "FindByProxyAddresses");
		}

		Result<ADRecipient>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetSession().FindByProxyAddresses(proxyAddresses), "FindByProxyAddresses");
		}

		ADRecipient IRecipientSession.FindBySid(SecurityIdentifier sId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.FindBySid(sId), null, null), "FindBySid");
		}

		ADRawEntry[] IRecipientSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindDeletedADRawEntryByUsnRange(lastKnownParentId, startUsn, sizeLimit, properties, additionalFilter), "FindDeletedADRawEntryByUsnRange");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<MiniRecipient[]>(() => this.GetSession().FindMiniRecipient(rootId, scope, filter, sortBy, maxResults, properties), "FindMiniRecipient");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<MiniRecipient[]>(() => this.GetSession().FindMiniRecipientByANR(anrMatch, maxResults, sortBy, properties), "FindMiniRecipientByANR");
		}

		TResult IRecipientSession.FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.ExecuteSingleObjectQueryWithFallback<TResult>((IRecipientSession session) => session.FindMiniRecipientByProxyAddress<TResult>(proxyAddress, properties), null, properties), "FindMiniRecipientByProxyAddress");
		}

		TResult IRecipientSession.FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TResult>(() => this.ExecuteSingleObjectQueryWithFallback<TResult>((IRecipientSession session) => session.FindMiniRecipientBySid<TResult>(sId, properties), null, properties), "FindMiniRecipientBySid");
		}

		ADRecipient[] IRecipientSession.FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			return base.InvokeWithAPILogging<ADRecipient[]>(() => this.GetSession().FindNames(dictionary, limit), "FindNames");
		}

		object[][] IRecipientSession.FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<object[][]>(() => this.GetSession().FindNamesView(dictionary, limit, properties), "FindNamesView");
		}

		Result<OWAMiniRecipient>[] IRecipientSession.FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			return base.InvokeWithAPILogging<Result<OWAMiniRecipient>[]>(() => this.GetSession().FindOWAMiniRecipientByUserPrincipalName(userPrincipalNames), "FindOWAMiniRecipientByUserPrincipalName");
		}

		ADPagedReader<ADRecipient> IRecipientSession.FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			return base.InvokeWithAPILogging<ADPagedReader<ADRecipient>>(() => this.GetSession().FindPaged(rootId, scope, filter, sortBy, pageSize), "FindPaged");
		}

		ADPagedReader<TEntry> IRecipientSession.FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADPagedReader<TEntry>>(() => this.GetSession().FindPagedMiniRecipient<TEntry>(rootId, scope, filter, sortBy, pageSize, properties), "FindPagedMiniRecipient");
		}

		ADRawEntry[] IRecipientSession.FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeWithAPILogging<ADRawEntry[]>(() => this.GetSession().FindRecipient(rootId, scope, filter, sortBy, maxResults, properties), "FindRecipient");
		}

		IEnumerable<ADGroup> IRecipientSession.FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId)
		{
			return base.InvokeWithAPILogging<IEnumerable<ADGroup>>(() => this.GetSession().FindRoleGroupsByForeignGroupSid(root, sId), "FindRoleGroupsByForeignGroupSid");
		}

		List<string> IRecipientSession.GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			return base.InvokeWithAPILogging<List<string>>(() => this.GetSession().GetTokenSids(user, assignmentMethodFlags), "GetTokenSids");
		}

		List<string> IRecipientSession.GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			return base.InvokeWithAPILogging<List<string>>(() => this.GetSession().GetTokenSids(userId, assignmentMethodFlags), "GetTokenSids");
		}

		SecurityIdentifier IRecipientSession.GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			return base.InvokeWithAPILogging<SecurityIdentifier>(() => this.GetSession().GetWellKnownExchangeGroupSid(wkguid), "GetWellKnownExchangeGroupSid");
		}

		bool IRecipientSession.IsLegacyDNInUse(string legacyDN)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetSession().IsLegacyDNInUse(legacyDN), "IsLegacyDNInUse");
		}

		bool IRecipientSession.IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetSession().IsMemberOfGroupByWellKnownGuid(wellKnownGuid, containerDN, id), "IsMemberOfGroupByWellKnownGuid");
		}

		bool IRecipientSession.IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetSession().IsRecipientInOrg(proxyAddress), "IsRecipientInOrg");
		}

		bool IRecipientSession.IsReducedRecipientSession()
		{
			return base.InvokeWithAPILogging<bool>(() => base.GetSession().IsReducedRecipientSession(), "IsReducedRecipientSession");
		}

		bool IRecipientSession.IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			return base.InvokeWithAPILogging<bool>(() => this.GetSession().IsThrottlingPolicyInUse(throttlingPolicyId), "IsThrottlingPolicyInUse");
		}

		ADRecipient IRecipientSession.Read(ADObjectId entryId)
		{
			return base.InvokeGetObjectWithAPILogging<ADRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<ADRecipient>((IRecipientSession session) => session.Read(entryId), null, null), "Read");
		}

		MiniRecipient IRecipientSession.ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<MiniRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<MiniRecipient>((IRecipientSession session) => session.ReadMiniRecipient(entryId, properties), null, properties), "ReadMiniRecipient");
		}

		TMiniRecipient IRecipientSession.ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<TMiniRecipient>(() => this.ExecuteSingleObjectQueryWithFallback<TMiniRecipient>((IRecipientSession session) => session.ReadMiniRecipient<TMiniRecipient>(entryId, properties), null, properties), "ReadMiniRecipient");
		}

		ADRawEntry IRecipientSession.FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties)
		{
			return base.InvokeGetObjectWithAPILogging<ADRawEntry>(delegate
			{
				if (properties != null)
				{
					List<PropertyDefinition> propertiesToRead = new List<PropertyDefinition>();
					propertiesToRead.Add(IADSecurityPrincipalSchema.Sid);
					propertiesToRead.AddRange(properties);
					return this.ExecuteSingleObjectQueryWithFallback<ADRawEntry>((IRecipientSession session) => session.FindUserBySid(sId, propertiesToRead), null, propertiesToRead);
				}
				return ((IRecipientSession)this).FindBySid(sId);
			}, "FindUserBySid");
		}

		Result<ADRecipient>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds)
		{
			return base.InvokeWithAPILogging<Result<ADRecipient>[]>(() => this.GetSession().ReadMultiple(entryIds), "ReadMultiple");
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().ReadMultiple(entryIds, properties), "ReadMultiple");
		}

		Result<ADGroup>[] IRecipientSession.ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			return base.InvokeWithAPILogging<Result<ADGroup>[]>(() => this.GetSession().ReadMultipleADGroups(entryIds), "ReadMultipleADGroups");
		}

		Result<ADUser>[] IRecipientSession.ReadMultipleADUsers(ADObjectId[] userIds)
		{
			return base.InvokeWithAPILogging<Result<ADUser>[]>(() => this.GetSession().ReadMultipleADUsers(userIds), "ReadMultipleADUsers");
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return base.InvokeWithAPILogging<Result<ADRawEntry>[]>(() => this.GetSession().ReadMultipleWithDeletedObjects(entryIds, properties), "ReadMultipleWithDeletedObjects");
		}

		MiniRecipientWithTokenGroups IRecipientSession.ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			return base.ExecuteSingleObjectQueryWithFallback<MiniRecipientWithTokenGroups>((IRecipientSession session) => session.ReadTokenGroupsGlobalAndUniversal(id), null, null);
		}

		ADObjectId[] IRecipientSession.ResolveSidsToADObjectIds(string[] sids)
		{
			return base.InvokeWithAPILogging<ADObjectId[]>(() => this.GetSession().ResolveSidsToADObjectIds(sids), "ResolveSidsToADObjectIds");
		}

		void IRecipientSession.Save(ADRecipient instanceToSave)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.InternalSave(instanceToSave);
				return true;
			}, "Save");
		}

		void IRecipientSession.Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				ObjectState objectState = instanceToSave.ObjectState;
				this.GetSession().Save(instanceToSave, bypassValidation);
				this.CacheUpdateFromSavedObject(instanceToSave, objectState);
				return true;
			}, "Save");
		}

		void IRecipientSession.SetPassword(ADObject obj, SecureString password)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetSession().SetPassword(obj, password);
				return true;
			}, "SetPassword");
		}

		void IRecipientSession.SetPassword(ADObjectId id, SecureString password)
		{
			base.InvokeWithAPILogging<bool>(delegate
			{
				this.GetSession().SetPassword(id, password);
				return true;
			}, "SetPassword");
		}
	}
}
