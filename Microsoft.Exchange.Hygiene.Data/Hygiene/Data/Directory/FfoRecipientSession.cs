using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class FfoRecipientSession : FfoDirectorySession, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		public FfoRecipientSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		protected FfoRecipientSession(ADObjectId tenantId) : base(tenantId)
		{
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			IConfigurable configurable = this.ReadImpl<T>(identity);
			ConfigurableObject configurableObject = configurable as ConfigurableObject;
			if (configurableObject != null)
			{
				configurableObject.ResetChangeTracking();
			}
			return configurable;
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			return (IConfigurable[])((IConfigDataProvider)this).FindPaged<T>(filter, rootId, deepSearch, sortBy, int.MaxValue).ToArray<T>();
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			FfoRecipientSession.LogUnsupportedQueryFilter(typeof(T), filter);
			foreach (T t in this.FindImpl<T>(filter, rootId, deepSearch, sortBy, pageSize))
			{
				ConfigurableObject configurableObject = t as ConfigurableObject;
				if (configurableObject != null)
				{
					configurableObject.ResetChangeTracking();
				}
				yield return t;
			}
			yield break;
		}

		void IRecipientSession.Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			((IConfigDataProvider)this).Save(instanceToSave);
		}

		void IConfigDataProvider.Save(IConfigurable configurable)
		{
			if (this.useGenericInitialization)
			{
				throw new NotSupportedException("The Reduced RecipientSession should never be used to save an object");
			}
			base.FixOrganizationalUnitRoot(configurable);
			base.GenerateIdForObject(configurable);
			base.ApplyAuditProperties(configurable);
			base.DataProvider.Save(configurable);
		}

		void IConfigDataProvider.Delete(IConfigurable configurable)
		{
			if (this.useGenericInitialization)
			{
				throw new NotSupportedException("The Reduced RecipientSession should never be used to delete an object");
			}
			base.FixOrganizationalUnitRoot(configurable);
			base.GenerateIdForObject(configurable);
			base.ApplyAuditProperties(configurable);
			base.DataProvider.Delete(configurable);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return "FfoRecipientSession";
			}
		}

		ADObjectId IRecipientSession.SearchRoot
		{
			get
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return null;
			}
		}

		ITableView IRecipientSession.Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		void IRecipientSession.Delete(ADRecipient instanceToDelete)
		{
			((IConfigDataProvider)this).Delete(instanceToDelete);
		}

		ADRecipient[] IRecipientSession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return (ADRecipient[])((IConfigDataProvider)this).Find<ADRecipient>(filter, rootId, false, sortBy);
		}

		ADRawEntry IRecipientSession.FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry[] IRecipientSession.FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		Result<ADRecipient>[] IRecipientSession.FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRecipient>[0];
		}

		ADUser[] IRecipientSession.FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return (ADUser[])((IConfigDataProvider)this).Find<ADUser>(filter, rootId, false, sortBy);
		}

		ADUser IRecipientSession.FindADUserByObjectId(ADObjectId adObjectId)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, adObjectId);
			return ((IConfigDataProvider)this).Find<ADUser>(filter, null, false, null).Cast<ADUser>().FirstOrDefault<ADUser>();
		}

		ADUser IRecipientSession.FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObject IRecipientSession.FindByAccountName<T>(string domainName, string accountName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		IEnumerable<T> IRecipientSession.FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new T[0];
		}

		ADRecipient[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRecipient[0];
		}

		ADRawEntry[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		ADRecipient IRecipientSession.FindByCertificate(X509Identifier identifier)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry[] IRecipientSession.FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		public ADRecipient FindByExchangeObjectId(Guid exchangeObjectId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry IRecipientSession.FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRecipient IRecipientSession.FindByExchangeGuid(Guid exchangeGuid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		TEntry IRecipientSession.FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(TEntry);
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		TEntry IRecipientSession.FindByExchangeGuidIncludingAlternate<TEntry>(Guid exchangeGuid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(TEntry);
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		Result<ADRecipient>[] IRecipientSession.FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRecipient>[0];
		}

		ADRecipient IRecipientSession.FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		Result<ADRawEntry>[] IRecipientSession.FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		ADRecipient IRecipientSession.FindByObjectGuid(Guid guid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRecipient IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return ((IRecipientSession)this).FindByProxyAddress<ADRecipient>(proxyAddress);
		}

		ADRawEntry IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return ((IRecipientSession)this).FindByProxyAddress(proxyAddress);
		}

		TEntry IRecipientSession.FindByProxyAddress<TEntry>(ProxyAddress proxyAddress)
		{
			Func<IConfigurable, string> func = null;
			IConfigurable[] array = ((IConfigDataProvider)this).Find<TEntry>(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, proxyAddress.AddressString), null, true, null);
			if (array == null || array.Length == 0)
			{
				return default(TEntry);
			}
			if (array.Length > 1)
			{
				string addressString = proxyAddress.AddressString;
				string separator = ",";
				IEnumerable<IConfigurable> source = array;
				if (func == null)
				{
					func = ((IConfigurable rcpt) => rcpt.Identity.ToString());
				}
				throw new AmbiguousMatchException(HygieneDataStrings.ErrorMultipleMatchForUserProxy(addressString, string.Join(separator, source.Select(func).ToArray<string>())));
			}
			return (TEntry)((object)array[0]);
		}

		Result<ADRawEntry>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			if (proxyAddresses == null)
			{
				return base.GetDefaultArray<Result<ADRawEntry>>();
			}
			return (from proxyAddress in proxyAddresses
			select new Result<ADRawEntry>(((IRecipientSession)this).FindByProxyAddress(proxyAddress), null)).ToArray<Result<ADRawEntry>>();
		}

		Result<TEntry>[] IRecipientSession.FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses)
		{
			if (proxyAddresses == null)
			{
				return base.GetDefaultArray<Result<TEntry>>();
			}
			return (from proxyAddress in proxyAddresses
			select new Result<TEntry>(((IRecipientSession)this).FindByProxyAddress<TEntry>(proxyAddress), null)).ToArray<Result<TEntry>>();
		}

		Result<ADRecipient>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			if (proxyAddresses == null)
			{
				return base.GetDefaultArray<Result<ADRecipient>>();
			}
			return (from proxyAddress in proxyAddresses
			select new Result<ADRecipient>(((IRecipientSession)this).FindByProxyAddress(proxyAddress), null)).ToArray<Result<ADRecipient>>();
		}

		ADRecipient IRecipientSession.FindBySid(SecurityIdentifier sId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return base.GetDefaultObject<ADRecipient>();
		}

		ADRawEntry[] IRecipientSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return (MiniRecipient[])((IConfigDataProvider)this).Find<MiniRecipient>(filter, rootId, false, sortBy);
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new MiniRecipient[0];
		}

		TResult IRecipientSession.FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(TResult);
		}

		TResult IRecipientSession.FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return default(TResult);
		}

		ADRawEntry IRecipientSession.FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRecipient[] IRecipientSession.FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRecipient[0];
		}

		object[][] IRecipientSession.FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new object[0][];
		}

		Result<OWAMiniRecipient>[] IRecipientSession.FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<OWAMiniRecipient>[0];
		}

		ADPagedReader<ADRecipient> IRecipientSession.FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADPagedReader<ADRecipient>();
		}

		ADPagedReader<TEntry> IRecipientSession.FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADRawEntry[] IRecipientSession.FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		IEnumerable<ADGroup> IRecipientSession.FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADGroup[0];
		}

		List<string> IRecipientSession.GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new List<string>();
		}

		List<string> IRecipientSession.GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new List<string>();
		}

		SecurityIdentifier IRecipientSession.GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		bool IRecipientSession.IsLegacyDNInUse(string legacyDN)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		bool IRecipientSession.IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		bool IRecipientSession.IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		public bool IsReducedRecipientSession()
		{
			return this.useGenericInitialization;
		}

		bool IRecipientSession.IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		ADRecipient IRecipientSession.Read(ADObjectId entryId)
		{
			ADRecipient adrecipient = ((IConfigDataProvider)this).Read<ADRecipient>(entryId) as ADRecipient;
			if (adrecipient != null)
			{
				return this.FixRecipientProperties(adrecipient) as ADRecipient;
			}
			return null;
		}

		MiniRecipient IRecipientSession.ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return (MiniRecipient)((IConfigDataProvider)this).Read<MiniRecipient>(entryId);
		}

		TMiniRecipient IRecipientSession.ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return (TMiniRecipient)((object)((IConfigDataProvider)this).Read<MiniRecipient>(entryId));
		}

		Result<ADRecipient>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds)
		{
			return (from entryId in entryIds
			select new Result<ADRecipient>((ADRecipient)((IConfigDataProvider)this).Read<ADRecipient>(entryId), null)).ToArray<Result<ADRecipient>>();
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<ADGroup>[] IRecipientSession.ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADGroup>[0];
		}

		Result<ADUser>[] IRecipientSession.ReadMultipleADUsers(ADObjectId[] userIds)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADUser>[0];
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		MiniRecipientWithTokenGroups IRecipientSession.ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId[] IRecipientSession.ResolveSidsToADObjectIds(string[] sids)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADObjectId[0];
		}

		void IRecipientSession.Save(ADRecipient instanceToSave)
		{
			((IConfigDataProvider)this).Save(instanceToSave);
		}

		void IRecipientSession.SetPassword(ADObject obj, SecureString password)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IRecipientSession.SetPassword(ADObjectId id, SecureString password)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		internal void EnableReducedRecipientSession()
		{
			this.useGenericInitialization = true;
		}

		private static void LogUnsupportedQueryFilter(Type dataType, QueryFilter filter)
		{
			if (dataType == typeof(ADGroup))
			{
				string text = filter.ToString();
				if (text.Contains("RecipientTypeDetailsValue Equal RoleGroup") && !FfoRecipientSession.getRoleGroupQueryFilterRegex.IsMatch(text) && !FfoRecipientSession.updateRoleGroupMemberFilterRegex.IsMatch(text))
				{
					EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_UnsupportedQueryFilter, null, new object[]
					{
						text + " \n" + Environment.StackTrace
					});
				}
			}
		}

		private static QueryFilter ReduceSecurityPrincipalFilter(QueryFilter filter)
		{
			if (filter.ToString() == FfoRecipientSession.userOrRoleGroupForExtendedSecurityPrincipal)
			{
				return null;
			}
			CompositeFilter compositeFilter = filter as CompositeFilter;
			if (compositeFilter == null)
			{
				return filter;
			}
			QueryFilter[] array = (from childFilter in compositeFilter.Filters.Select(new Func<QueryFilter, QueryFilter>(FfoRecipientSession.ReduceSecurityPrincipalFilter))
			where childFilter != null
			select childFilter).ToArray<QueryFilter>();
			if (array.Length == 0)
			{
				return null;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			if (compositeFilter is AndFilter)
			{
				return QueryFilter.AndTogether(array);
			}
			if (compositeFilter is OrFilter)
			{
				return QueryFilter.OrTogether(array);
			}
			throw new NotSupportedException(HygieneDataStrings.ErrorQueryFilterType(filter.ToString()));
		}

		private IConfigurable ReadImpl<T>(ObjectId identity) where T : IConfigurable, new()
		{
			QueryFilter filter = base.AddTenantIdFilter(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, identity));
			if (typeof(T) == typeof(ExtendedSecurityPrincipal))
			{
				ADUser user = base.ReadAndHandleException<ADUser>(filter);
				return (T)((object)this.GetExtendedSecurityPrincipal(user));
			}
			T t = base.ReadAndHandleException<T>(filter);
			ADObject adobject = t as ADObject;
			if (adobject != null)
			{
				FfoDirectorySession.FixDistinguishedName(adobject, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)adobject.Identity).ObjectGuid, null);
				return (T)((object)this.FixRecipientProperties(adobject));
			}
			return t;
		}

		private IEnumerable<T> FindImpl<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (base.TenantId == null)
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
			}
			else
			{
				filter = this.AddFilterOperator(filter);
				if (typeof(T) == typeof(ExtendedSecurityPrincipal))
				{
					filter = FfoRecipientSession.ReduceSecurityPrincipalFilter(filter);
					IEnumerable<ADUser> users = base.FindAndHandleException<ADUser>(filter, rootId, deepSearch, sortBy, pageSize);
					foreach (ADUser user in users)
					{
						yield return (T)((object)this.GetExtendedSecurityPrincipal(user));
					}
				}
				else
				{
					IEnumerable<T> configObjs = base.FindAndHandleException<T>(base.AddTenantIdFilter(filter), rootId, deepSearch, sortBy, pageSize);
					foreach (T configObj in configObjs)
					{
						ADObject adObject = configObj as ADObject;
						if (adObject != null)
						{
							FfoDirectorySession.FixDistinguishedName(adObject, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)adObject.Identity).ObjectGuid, null);
							yield return (T)((object)this.FixRecipientProperties(adObject));
						}
						else
						{
							yield return configObj;
						}
					}
				}
			}
			yield break;
		}

		private QueryFilter AddFilterOperator(QueryFilter filter)
		{
			if (filter == null)
			{
				return null;
			}
			string input = filter.ToString();
			Match match = FfoRecipientSession.getRecipientFilterRegex.Match(input);
			if (match.Success)
			{
				Group group = match.Groups[DalHelper.FilteringOperatorProp.Name];
				if (group != null && group.Value == "&")
				{
					filter = QueryFilter.AndTogether(new QueryFilter[]
					{
						filter,
						new ComparisonFilter(ComparisonOperator.Equal, DalHelper.FilteringOperatorProp, "and")
					});
				}
			}
			return filter;
		}

		private IConfigurable GetExtendedSecurityPrincipal(ADUser user)
		{
			if (user == null)
			{
				return null;
			}
			FfoDirectorySession.FixDistinguishedName(user, base.TenantId.DistinguishedName, base.TenantId.ObjectGuid, ((ADObjectId)user.Identity).ObjectGuid, null);
			ExtendedSecurityPrincipal extendedSecurityPrincipal = new ExtendedSecurityPrincipal();
			DalHelper.SetConfigurableObject(user.DisplayName, ExtendedSecurityPrincipalSchema.DisplayName, extendedSecurityPrincipal);
			DalHelper.SetConfigurableObject(user.DisplayName, ADObjectSchema.RawName, extendedSecurityPrincipal);
			DalHelper.SetConfigurableObject(new SecurityIdentifier(WellKnownSidType.NullSid, null), IADSecurityPrincipalSchema.Sid, extendedSecurityPrincipal);
			DalHelper.SetConfigurableObject(user.Id.GetChildId(ADUser.ObjectCategoryNameInternal), ADObjectSchema.ObjectCategory, extendedSecurityPrincipal);
			DalHelper.SetConfigurableObject(user.ObjectClass, ADObjectSchema.ObjectClass, extendedSecurityPrincipal);
			DalHelper.SetConfigurableObject(RecipientTypeDetails.MailUser, ExtendedSecurityPrincipalSchema.RecipientTypeDetails, extendedSecurityPrincipal);
			extendedSecurityPrincipal.SetId(user.Id);
			return extendedSecurityPrincipal;
		}

		private ADObject FixRecipientProperties(ADObject adObject)
		{
			adObject = this.FixObjectType(adObject);
			adObject.SetIsReadOnly(false);
			adObject[IADMailStorageSchema.ProhibitSendQuota] = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			this.FixGroupProperties(adObject as ADGroup);
			ADRecipient adrecipient = adObject as ADRecipient;
			if (adrecipient != null && (adrecipient is ADUser || adrecipient is ADContact))
			{
				if (!(adrecipient.ExternalEmailAddress == null))
				{
					if (adrecipient.EmailAddresses.Any((ProxyAddress proxy) => proxy.Prefix == ProxyAddressPrefix.Smtp))
					{
						goto IL_8A;
					}
				}
				adrecipient.Alias = null;
			}
			IL_8A:
			adObject.SetIsReadOnly(((IDirectorySession)this).ReadOnly);
			if (adObject is ADRecipient || adObject is MiniRecipient)
			{
				adObject.m_Session = (adObject.m_Session ?? this);
			}
			return adObject;
		}

		private ADObject FixObjectType(ADObject adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("The recipient should not be post processed if null.", "recipient");
			}
			Type type = adObject.GetType();
			string key = adObject.ObjectClass.First<string>();
			Type type2;
			if (FfoRecipientSession.objectClassTypes.TryGetValue(key, out type2) && type.IsAssignableFrom(type2))
			{
				adObject = (ADObject)Activator.CreateInstance(type2, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[]
				{
					this,
					adObject.propertyBag
				}, null);
			}
			return adObject;
		}

		private void FixGroupProperties(ADGroup group)
		{
			if (group == null)
			{
				return;
			}
			RoleGroup.RoleGroupTypeIds roleGroupTypeIds;
			if (group.RawCapabilities.Contains(Capability.Partner_Managed))
			{
				if (FfoRecipientSession.partnerManagedRoleGroups.TryGetValue(group.Name, out roleGroupTypeIds))
				{
					group[ADGroupSchema.RoleGroupTypeId] = (int)roleGroupTypeIds;
				}
			}
			else if (Enum.TryParse<RoleGroup.RoleGroupTypeIds>(group.Name, out roleGroupTypeIds))
			{
				group[ADGroupSchema.RoleGroupTypeId] = (int)roleGroupTypeIds;
			}
			ADObjectId organizationalUnitRoot = group.OrganizationalUnitRoot;
			MultiValuedProperty<ADObjectId> multiValuedProperty = group[ADGroupSchema.RoleAssignments] as MultiValuedProperty<ADObjectId>;
			if (multiValuedProperty != null && organizationalUnitRoot != null)
			{
				for (int i = 0; i < multiValuedProperty.Count; i++)
				{
					ADObjectId adobjectId = multiValuedProperty[i];
					adobjectId = new ADObjectId(organizationalUnitRoot.GetChildId(adobjectId.Name).DistinguishedName, adobjectId.ObjectGuid);
					multiValuedProperty[i] = adobjectId;
				}
			}
		}

		private static readonly Dictionary<string, RoleGroup.RoleGroupTypeIds> partnerManagedRoleGroups = new Dictionary<string, RoleGroup.RoleGroupTypeIds>
		{
			{
				"HelpdeskAdmins",
				RoleGroup.RoleGroupTypeIds.MsoManagedTenantHelpdesk
			},
			{
				"TenantAdmins",
				RoleGroup.RoleGroupTypeIds.MsoManagedTenantAdmin
			}
		};

		private static readonly Regex getRoleGroupQueryFilterRegex = new Regex("\\(\\&\\(\\(RoleGroupType NotEqual PartnerLinked\\)\\(\\&\\(\\(\\&\\(\\(RecipientTypeDetails Equal RoleGroup\\)\\(OrganizationalUnitRoot Equal .+\\)\\)\\)\\(\\&\\(\\(RecipientType Equal Group\\)\\(RecipientTypeDetailsValue Equal RoleGroup\\)\\(BitwiseAnd\\(GroupType,2147483656\\)\\)\\)\\)\\)\\)\\)\\)");

		private static readonly Regex updateRoleGroupMemberFilterRegex = new Regex("\\(\\&\\(\\(\\&\\(\\(\\|\\(\\(ExternalDirectoryObjectId Equal .+\\)\\(UserPrincipalName Equal .+\\)\\(LegacyExchangeDN Equal .+\\)\\(EmailAddresses Equal .+\\)\\(Alias Equal .+\\)\\(DisplayName Equal .+\\)\\)\\)\\(RecipientType Equal Group\\)\\)\\)\\(\\&\\(\\(RecipientType Equal Group\\)\\(RecipientTypeDetailsValue Equal RoleGroup\\)\\(BitwiseAnd\\(GroupType,2147483656\\)\\)\\)\\)\\)\\)");

		private static readonly Regex getRecipientFilterRegex = new Regex(string.Format("\\(\\&\\(\\(\\&\\(\\(\\|\\((\\(RecipientTypeDetails Equal [^\\)]+\\))+\\)\\)\\(OrganizationalUnitRoot Equal [^\\)]+\\)(\\((?<{0}>[^\\(]+)\\((\\([^\\)]+\\))+\\)\\))?\\)\\)\\(\\&\\(\\(pageCookie Equal \\)\\(storedProcOutputBag Equal System.Collections.Generic.Dictionary`2\\[Microsoft.Exchange.Data.PropertyDefinition\\,System.Object\\]\\)\\)\\)\\)\\)", DalHelper.FilteringOperatorProp.Name));

		private static readonly string userOrRoleGroupForExtendedSecurityPrincipal = "(|((&((&((ObjectCategory Equal group)(BitwiseOr(GroupType,2147483648))(BitwiseAnd(GroupType,8))))(|((!((BitwiseAnd(GroupType,8))))(&((BitwiseAnd(GroupType,8))(|((Exists(Alias))(&((RecipientType Equal Group)(RecipientTypeDetailsValue Equal RoleGroup)(BitwiseAnd(GroupType,2147483656))))))))))))(&((ObjectCategory Equal person)(ObjectClass Equal user)))))";

		private static readonly Dictionary<string, Type> objectClassTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"group",
				typeof(ADGroup)
			},
			{
				"user",
				typeof(ADUser)
			},
			{
				"contact",
				typeof(ADContact)
			}
		};

		private bool useGenericInitialization;
	}
}
