using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class ADRecipientObjectSession : ADDataSession, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		public ADRecipientObjectSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		public ADRecipientObjectSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(false, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
			if (searchRoot != null && (searchRoot.IsDescendantOf(ADSession.GetConfigurationNamingContext(sessionSettings.GetAccountOrResourceForestFqdn())) || searchRoot.IsDescendantOf(ADSession.GetConfigurationUnitsRoot(sessionSettings.GetAccountOrResourceForestFqdn()))))
			{
				this.addressListMembershipFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, searchRoot),
					new ExistsFilter(ADRecipientSchema.DisplayName)
				});
			}
			else
			{
				this.SetSearchRoot(searchRoot);
			}
			base.Lcid = lcid;
			base.UseGlobalCatalog = base.ReadOnly;
			base.EnforceContainerizedScoping = true;
		}

		public new ADObjectId SearchRoot
		{
			get
			{
				return base.SearchRoot;
			}
		}

		protected void SetSearchRoot(ADObjectId searchRoot)
		{
			base.SearchRoot = searchRoot;
		}

		public bool IsReducedRecipientSession()
		{
			return this.isReducedRecipientSession;
		}

		protected void CheckConfigScopeParameter(ConfigScopes configScope)
		{
			if (ConfigScopes.TenantSubTree != configScope && ConfigScopes.TenantLocal != configScope)
			{
				throw new NotSupportedException("Only ConfigScopes.TenantSubTree or ConfigScopes.TenantLocal");
			}
		}

		protected override ADObject CreateAndInitializeObject<TResult>(ADPropertyBag propertyBag, ADRawEntry dummyObject)
		{
			if (!this.isReducedRecipientSession)
			{
				return ADObjectFactory.CreateAndInitializeRecipientObject<TResult>(propertyBag, dummyObject, this);
			}
			return ADObjectFactory.CreateAndInitializeObject<TResult>(propertyBag, this);
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		public ADRecipient[] Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.Find<ADRecipient>(rootId, scope, filter, sortBy, maxResults, null, false);
		}

		public ADUser[] FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			return base.Find<ADUser>(rootId, scope, filter, sortBy, maxResults, null, false);
		}

		public ADUser FindADUserByObjectId(ADObjectId adObjectId)
		{
			ADUser[] array = this.FindADUser(adObjectId, QueryScope.Base, null, null, 1);
			if (array == null || array.Length != 1)
			{
				return null;
			}
			return array[0];
		}

		public ADUser FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			QueryFilter filter = ADRecipientObjectSession.QueryFilterFromExternalDirectoryObjectId(externalDirectoryObjectId);
			ADUser[] array = base.Find<ADUser>(null, QueryScope.SubTree, filter, null, 1, null, false);
			if (array == null || array.Length != 1)
			{
				return null;
			}
			return array[0];
		}

		public Result<ADRawEntry>[] FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties)
		{
			return this.FindByExternalDirectoryObjectIds(externalDirectoryObjectIds, false, properties);
		}

		public Result<ADRawEntry>[] FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties)
		{
			if (externalDirectoryObjectIds == null)
			{
				throw new ArgumentNullException("externalDirectoryObjectIds");
			}
			if (externalDirectoryObjectIds.Length == 0)
			{
				return new Result<ADRawEntry>[0];
			}
			if (properties == null)
			{
				properties = new PropertyDefinition[2];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 2);
			}
			properties[properties.Length - 2] = ADRecipientSchema.ExternalDirectoryObjectId;
			properties[properties.Length - 1] = ADObjectSchema.WhenCreatedUTC;
			return this.ReadMultipleRecipientsWithDeletedObjects<string>(externalDirectoryObjectIds, new Converter<string, QueryFilter>(ADRecipientObjectSession.QueryFilterFromExternalDirectoryObjectId), SyncRecipient.SyncRecipientObjectTypeFilter, new ADDataSession.HashInserter<ADRawEntry>(ADRecipientObjectSession.FindByExternalDirectoryObjectIdsHashInserter<ADRawEntry>), new ADDataSession.HashLookup<string, ADRawEntry>(ADRecipientObjectSession.FindByExternalDirectoryObjectIdsHashLookup<ADRawEntry>), properties, includeDeletedObjects);
		}

		private static QueryFilter QueryFilterFromExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ExternalDirectoryObjectId, externalDirectoryObjectId);
		}

		private static void FindByExternalDirectoryObjectIdsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			string key = ((string)entry.propertyBag[ADRecipientSchema.ExternalDirectoryObjectId]).ToLowerInvariant();
			if (hash.ContainsKey(key))
			{
				if ((DateTime)entry.propertyBag[ADObjectSchema.WhenCreatedUTC] > (DateTime)((Result<TResult>)hash[key]).Data.propertyBag[ADObjectSchema.WhenCreatedUTC])
				{
					hash[key] = result;
					return;
				}
			}
			else
			{
				hash.Add(key, result);
			}
		}

		private static Result<TResult> FindByExternalDirectoryObjectIdsHashLookup<TResult>(Hashtable hash, string key) where TResult : ADRawEntry
		{
			string key2 = key.ToLowerInvariant();
			if (hash.ContainsKey(key2))
			{
				return (Result<TResult>)hash[key2];
			}
			return new Result<TResult>(default(TResult), ProviderError.NotFound);
		}

		public ADRawEntry[] FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.Find<ADRawEntry>(rootId, scope, filter, sortBy, maxResults, properties, false);
		}

		public MiniRecipient[] FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			return base.Find<MiniRecipient>(rootId, scope, filter, sortBy, maxResults, properties, false);
		}

		public ADPagedReader<ADRecipient> FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			return base.FindPaged<ADRecipient>(rootId, scope, filter, sortBy, pageSize, null);
		}

		public ADPagedReader<TEntry> FindPagedMiniRecipient<TEntry>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new()
		{
			return base.FindPaged<TEntry>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		public ADRecipient Read(ADObjectId entryId)
		{
			return base.InternalRead<ADRecipient>(entryId, null);
		}

		internal void EnableReducedRecipientSession()
		{
			this.isReducedRecipientSession = true;
		}

		public MiniRecipient ReadMiniRecipient(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			return this.ReadMiniRecipient<MiniRecipient>(entryId, properties);
		}

		public TMiniRecipient ReadMiniRecipient<TMiniRecipient>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties) where TMiniRecipient : ADObject, new()
		{
			if (!typeof(MiniRecipient).IsAssignableFrom(typeof(TMiniRecipient)))
			{
				throw new InvalidOperationException("Object should be minirecipient");
			}
			return base.InternalRead<TMiniRecipient>(entryId, properties);
		}

		public ADRawEntry FindUserBySid(SecurityIdentifier sId, IList<PropertyDefinition> properties)
		{
			if (properties != null)
			{
				return this.FindADRawEntryBySid(sId, properties);
			}
			return this.FindBySid(sId);
		}

		public void Save(ADRecipient instanceToSave)
		{
			base.Save(instanceToSave, ADRecipientProperties.Instance.AllProperties);
		}

		public void Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			base.Save(instanceToSave, ADRecipientProperties.Instance.AllProperties, bypassValidation);
		}

		public void Delete(ADRecipient instanceToDelete)
		{
			base.Delete(instanceToDelete, instanceToDelete is ADUser);
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			return base.FindPaged<T>((ADObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, sortBy, pageSize, null);
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			if (this.isReducedRecipientSession)
			{
				return base.InternalRead<ReducedRecipient>((ADObjectId)identity, null);
			}
			if (!typeof(ADRecipient).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(DirectoryStrings.ErrorWrongTypeParameter);
			}
			ADRecipient adrecipient = this.Read((ADObjectId)identity);
			if (!(adrecipient is T))
			{
				return null;
			}
			return adrecipient;
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			if (this.isReducedRecipientSession)
			{
				return (IConfigurable[])this.Find((ADObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, filter, sortBy, 0);
			}
			if (!typeof(ADRecipient).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(DirectoryStrings.ErrorWrongTypeParameter);
			}
			QueryFilter queryFilter = filter;
			if (typeof(ADRecipient) != typeof(T))
			{
				ADObject adobject = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)) as ADObject;
				queryFilter = ((queryFilter == null) ? adobject.ImplicitFilter : new AndFilter(new QueryFilter[]
				{
					adobject.ImplicitFilter,
					queryFilter
				}));
			}
			return (IConfigurable[])this.Find((ADObjectId)rootId, deepSearch ? QueryScope.SubTree : QueryScope.OneLevel, queryFilter, sortBy, 0);
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			if (this.isReducedRecipientSession)
			{
				throw new NotSupportedException("The Reduced RecipientSession should never be used to save an object");
			}
			this.Save((ADRecipient)instance);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			if (this.isReducedRecipientSession)
			{
				throw new NotSupportedException("The Reduced RecipientSession should never be used to delete an object");
			}
			this.Delete((ADRecipient)instance);
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return base.LastUsedDc;
			}
		}

		public ADRecipient FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return this.FindByProxyAddress<ADRecipient>(proxyAddress, null);
		}

		public ADRawEntry FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return this.FindByProxyAddress<ADRawEntry>(proxyAddress, properties);
		}

		public TEntry FindByProxyAddress<TEntry>(ProxyAddress proxyAddress) where TEntry : ADObject, new()
		{
			return this.FindByProxyAddress<TEntry>(proxyAddress, null);
		}

		public ADRecipient FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			if (legacyExchangeDN == null)
			{
				throw new ArgumentNullException("legacyExchangeDN");
			}
			if (string.IsNullOrEmpty(legacyExchangeDN))
			{
				throw new ArgumentException(DirectoryStrings.ExEmptyStringArgumentException("legacyExchangeDN"), "legacyExchangeDN");
			}
			QueryFilter filter = ADRecipientObjectSession.FindByLegacyExchangeDNsFilterBuilder(legacyExchangeDN);
			ADRecipient[] array = this.Find(null, QueryScope.SubTree, filter, null, 2);
			switch (array.Length)
			{
			case 0:
				if (base.SessionSettings.ConfigReadScope == null || base.SessionSettings.ConfigReadScope.Root == null || this.SearchRoot != null)
				{
					return null;
				}
				array = this.Find(base.SessionSettings.ConfigReadScope.Root, QueryScope.SubTree, filter, null, 2);
				switch (array.Length)
				{
				case 0:
					return null;
				case 1:
					return array[0];
				default:
					throw new NonUniqueRecipientException(legacyExchangeDN, new NonUniqueLegacyExchangeDNError(DirectoryStrings.ErrorNonUniqueLegacyDN(legacyExchangeDN), array[0].Id, string.Empty));
				}
				break;
			case 1:
				return array[0];
			default:
				throw new NonUniqueRecipientException(legacyExchangeDN, new NonUniqueLegacyExchangeDNError(DirectoryStrings.ErrorNonUniqueLegacyDN(legacyExchangeDN), array[0].Id, string.Empty));
			}
		}

		public ADRecipient[] FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			if (string.IsNullOrEmpty(anrMatch))
			{
				throw new ADFilterException(DirectoryStrings.InvalidAnrFilter);
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new AmbiguousNameResolutionFilter(anrMatch),
				new ExistsFilter(ADRecipientSchema.AddressListMembership),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false)
			});
			return this.Find(null, QueryScope.SubTree, filter, sortBy, maxResults);
		}

		public ADRawEntry[] FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			if (string.IsNullOrEmpty(anrMatch))
			{
				throw new ADFilterException(DirectoryStrings.InvalidAnrFilter);
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new AmbiguousNameResolutionFilter(anrMatch),
				new ExistsFilter(ADRecipientSchema.AddressListMembership),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false)
			});
			return base.Find(null, QueryScope.SubTree, filter, sortBy, maxResults, properties);
		}

		public MiniRecipient[] FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			if (string.IsNullOrEmpty(anrMatch))
			{
				throw new ADFilterException(DirectoryStrings.InvalidAnrFilter);
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new AmbiguousNameResolutionFilter(anrMatch),
				new ExistsFilter(ADRecipientSchema.AddressListMembership)
			});
			return base.Find<MiniRecipient>(null, QueryScope.SubTree, filter, sortBy, maxResults, properties, false);
		}

		public ADRawEntry[] FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			if (sizeLimit > ADDataSession.RangedValueDefaultPageSize)
			{
				throw new ArgumentOutOfRangeException("sizeLimit");
			}
			if (endUsn < startUsn)
			{
				throw new ArgumentOutOfRangeException("endUsn");
			}
			List<QueryFilter> list = new List<QueryFilter>(3);
			list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.UsnChanged, startUsn));
			if (endUsn != 9223372036854775807L)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ADRecipientSchema.UsnChanged, endUsn));
			}
			if (additionalFilter != null)
			{
				list.Add(additionalFilter);
			}
			return base.Find<ADRawEntry>(root, scope, (list.Count == 1) ? list[0] : new AndFilter(list.ToArray()), ADDataSession.SortByUsn, sizeLimit, properties, false);
		}

		public ADRawEntry[] FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			if (sizeLimit > ADDataSession.RangedValueDefaultPageSize)
			{
				throw new ArgumentOutOfRangeException("sizeLimit");
			}
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.UsnChanged, startUsn),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, lastKnownParentId)
			});
			if (additionalFilter != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					additionalFilter
				});
			}
			ADObjectId deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(lastKnownParentId.DomainId);
			return base.Find<ADRawEntry>(deletedObjectsContainer, QueryScope.OneLevel, queryFilter, ADDataSession.SortByUsn, sizeLimit, properties, true);
		}

		public Result<ADRecipient>[] ReadMultiple(ADObjectId[] entryIds)
		{
			return base.ReadMultiple<ADObjectId, ADRecipient>(entryIds, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), new ADDataSession.HashInserter<ADRecipient>(ADRecipientObjectSession.ADObjectIdHashInserter<ADRecipient>), new ADDataSession.HashLookup<ADObjectId, ADRecipient>(ADRecipientObjectSession.ADObjectIdHashLookup<ADRecipient>), null);
		}

		public Result<ADUser>[] ReadMultipleADUsers(ADObjectId[] userIds)
		{
			return base.ReadMultiple<ADObjectId, ADUser>(userIds, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), new ADDataSession.HashInserter<ADUser>(ADRecipientObjectSession.ADObjectIdHashInserter<ADUser>), new ADDataSession.HashLookup<ADObjectId, ADUser>(ADRecipientObjectSession.ADObjectIdHashLookup<ADUser>), null);
		}

		public Result<ADRawEntry>[] ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return base.ReadMultiple<ADObjectId, ADRawEntry>(entryIds, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), new ADDataSession.HashInserter<ADRawEntry>(ADRecipientObjectSession.ADObjectIdHashInserter<ADRawEntry>), new ADDataSession.HashLookup<ADObjectId, ADRawEntry>(ADRecipientObjectSession.ADObjectIdHashLookup<ADRawEntry>), properties);
		}

		public Result<ADRawEntry>[] ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return this.ReadMultipleRecipientsWithDeletedObjects<ADObjectId>(entryIds, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), null, new ADDataSession.HashInserter<ADRawEntry>(ADRecipientObjectSession.ADObjectIdHashInserter<ADRawEntry>), new ADDataSession.HashLookup<ADObjectId, ADRawEntry>(ADRecipientObjectSession.ADObjectIdHashLookup<ADRawEntry>), properties, true);
		}

		protected Result<ADRawEntry>[] ReadMultipleRecipientsWithDeletedObjects<TKey>(TKey[] keys, Converter<TKey, QueryFilter> filterBuilder, QueryFilter commonFilter, ADDataSession.HashInserter<ADRawEntry> hashInserter, ADDataSession.HashLookup<TKey, ADRawEntry> hashLookup, PropertyDefinition[] properties, bool includeDeletedObjects)
		{
			Result<ADRawEntry>[] array = base.ReadMultiple<TKey, ADRawEntry>(keys, null, filterBuilder, commonFilter, hashInserter, hashLookup, properties, includeDeletedObjects, false);
			if (includeDeletedObjects && base.SessionSettings.ConfigScopes != ConfigScopes.RootOrg && (from x in array
			where x.Error == ProviderError.NotFound
			select x).Any<Result<ADRawEntry>>())
			{
				Result<ADRawEntry>[] array2 = base.ReadMultiple<TKey, ADRawEntry>(keys, ADSession.GetDeletedObjectsContainer(base.GetRootDomainNamingContext()), filterBuilder, commonFilter, hashInserter, hashLookup, properties, true, false);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Data == null && array2[i].Data != null)
					{
						array[i] = array2[i];
					}
				}
			}
			return array;
		}

		public Result<ADGroup>[] ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			return base.ReadMultiple<ADObjectId, ADGroup>(entryIds, new Converter<ADObjectId, QueryFilter>(ADRecipientObjectSession.ADObjectIdFilterBuilder), new ADDataSession.HashInserter<ADGroup>(ADRecipientObjectSession.ADObjectIdHashInserter<ADGroup>), new ADDataSession.HashLookup<ADObjectId, ADGroup>(ADRecipientObjectSession.ADObjectIdHashLookup<ADGroup>), null);
		}

		public ITableView Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			ADObjectId[] addressListIds = null;
			if (addressListId != null)
			{
				addressListIds = new ADObjectId[]
				{
					addressListId
				};
			}
			return new ADVirtualListView(this, this.SearchRoot, addressListIds, new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending), rowCountSuggestion, properties);
		}

		public bool IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			QueryFilter restrictingFilter = new ComparisonFilter(ComparisonOperator.Equal, IADDistributionListSchema.Members, id);
			PropertyDefinition[] props = new PropertyDefinition[]
			{
				ADObjectSchema.Guid
			};
			ADRawEntry adrawEntry = base.ResolveWellKnownGuid<ADRawEntry>(wellKnownGuid, containerDN, restrictingFilter, props);
			return adrawEntry != null;
		}

		public bool IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			QueryFilter filter = this.QueryFilterFromProxyAddress(proxyAddress);
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.MasterAccountSid
			};
			ADRawEntry[] array = base.Find<ADRawEntry>(null, QueryScope.SubTree, filter, null, 2, properties, false);
			switch (array.Length)
			{
			case 0:
				return false;
			case 1:
			{
				RecipientType recipientType = (RecipientType)array[0][ADRecipientSchema.RecipientType];
				SecurityIdentifier left = array[0][ADRecipientSchema.MasterAccountSid] as SecurityIdentifier;
				return (recipientType != RecipientType.MailContact && recipientType != RecipientType.Contact && recipientType != RecipientType.MailUser) || !(left == null);
			}
			default:
				throw new NonUniqueRecipientException(proxyAddress, new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), array[0].Id, string.Empty));
			}
		}

		public Result<ADRawEntry>[] FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			if (proxyAddresses.Length == 0)
			{
				return new Result<ADRawEntry>[0];
			}
			if (properties == null)
			{
				properties = new PropertyDefinition[2];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 2);
			}
			properties[properties.Length - 1] = ADRecipientSchema.LegacyExchangeDN;
			properties[properties.Length - 2] = ADRecipientSchema.EmailAddresses;
			return this.FindByProxyAddresses<ADRawEntry>(proxyAddresses, properties);
		}

		public Result<ADRecipient>[] FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			if (proxyAddresses.Length == 0)
			{
				return new Result<ADRecipient>[0];
			}
			return this.FindByProxyAddresses<ADRecipient>(proxyAddresses, null);
		}

		public Result<TEntry>[] FindByProxyAddresses<TEntry>(ProxyAddress[] proxyAddresses) where TEntry : ADObject, new()
		{
			if (proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			if (proxyAddresses.Length == 0)
			{
				return new Result<TEntry>[0];
			}
			return this.FindByProxyAddresses<TEntry>(proxyAddresses, null);
		}

		public Result<ADRecipient>[] FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (keys.Length == 0)
			{
				return new Result<ADRecipient>[0];
			}
			return this.FindByExchangeGuidsIncludingArchive<ADRecipient>(keys, null);
		}

		private static QueryFilter QueryFilterFromExchangeGuid(Guid exchangeGuid, bool includeAlternative, bool includeArchive)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ExchangeGuid, exchangeGuid);
			QueryFilter queryFilter2 = null;
			if (includeAlternative)
			{
				queryFilter2 = new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxGuidsRaw, exchangeGuid.ToString()),
					new TextFilter(ADUserSchema.AggregatedMailboxGuids, exchangeGuid.ToString(), MatchOptions.Prefix, MatchFlags.Loose)
				});
			}
			QueryFilter queryFilter3 = null;
			if (includeArchive)
			{
				queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveGuid, exchangeGuid);
			}
			QueryFilter result = queryFilter;
			if (queryFilter2 != null && queryFilter3 != null)
			{
				result = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2,
					queryFilter3
				});
			}
			else if (queryFilter2 != null)
			{
				result = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}
			else if (queryFilter3 != null)
			{
				result = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter3
				});
			}
			return result;
		}

		private static QueryFilter ConstructRecipientSidFilter(SecurityIdentifier sId)
		{
			return new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Sid, sId),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, sId),
					new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.SidHistory, sId)
				}),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.ObjectClass, "foreignSecurityPrincipal")
			});
		}

		private static void FindByProxyAddressesHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			string text = (string)entry.propertyBag[ADRecipientSchema.LegacyExchangeDN];
			if (hash.ContainsKey(text))
			{
				hash[text] = new Result<TResult>(default(TResult), new NonUniqueLegacyExchangeDNError(DirectoryStrings.ErrorNonUniqueLegacyDN(text), entry.Id, string.Empty));
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT, text, new object[]
				{
					text
				});
			}
			else
			{
				hash.Add(text, result);
			}
			foreach (ProxyAddress proxyAddress in ((ProxyAddressCollection)entry.propertyBag[ADRecipientSchema.EmailAddresses]))
			{
				string text2 = proxyAddress.ToString();
				if (hash.ContainsKey(text2))
				{
					hash[text2] = new Result<TResult>(default(TResult), new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), entry.Id, string.Empty));
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT, text2, new object[]
					{
						text2
					});
				}
				else
				{
					hash.Add(text2, result);
				}
			}
		}

		private static Result<TResult> FindByProxyAddressesHashLookup<TResult>(Hashtable hash, ProxyAddress key) where TResult : ADRawEntry
		{
			if (key.Prefix == ProxyAddressPrefix.LegacyDN)
			{
				string valueString = key.ValueString;
				string key2 = "x500:" + valueString;
				object obj = hash[valueString];
				object obj2 = hash[key2];
				object obj3;
				if ((obj3 = (obj ?? obj2)) == null)
				{
					obj3 = new Result<TResult>(default(TResult), ProviderError.NotFound);
				}
				return (Result<TResult>)obj3;
			}
			string key3 = key.ToString();
			if (hash.ContainsKey(key3))
			{
				return (Result<TResult>)hash[key3];
			}
			return new Result<TResult>(default(TResult), ProviderError.NotFound);
		}

		private static QueryFilter FindByLegacyExchangeDNsFilterBuilder(string legDN)
		{
			return new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legDN),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "x500:" + legDN)
			});
		}

		private static void FindByLegacyExchangeDNsHashInserter<T>(Hashtable hash, T entry) where T : ADRawEntry
		{
			Result<T> result = new Result<T>(entry, null);
			string text = ((string)entry.propertyBag[ADRecipientSchema.LegacyExchangeDN]).ToLowerInvariant();
			if (hash.ContainsKey(text))
			{
				hash[text] = new Result<T>(default(T), new NonUniqueLegacyExchangeDNError(DirectoryStrings.ErrorNonUniqueLegacyDN(text), entry.Id, string.Empty));
			}
			else
			{
				hash.Add(text, result);
			}
			foreach (ProxyAddress proxyAddress in ((ProxyAddressCollection)entry.propertyBag[ADRecipientSchema.EmailAddresses]))
			{
				string text2 = proxyAddress.ToString().ToLowerInvariant();
				if (proxyAddress.Prefix == ProxyAddressPrefix.X500)
				{
					if (hash.ContainsKey(text2))
					{
						hash[text2] = new Result<T>(default(T), new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), entry.Id, string.Empty));
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT, text2, new object[]
						{
							text2
						});
					}
					else
					{
						hash.Add(text2, result);
					}
				}
			}
		}

		private static Result<T> FindByLegacyExchangeDNsHashLookup<T>(Hashtable hash, string key) where T : ADRawEntry
		{
			string text = key.ToLowerInvariant();
			string key2 = "x500:" + text;
			object obj = hash[text];
			object obj2 = hash[key2];
			object obj3;
			if ((obj3 = (obj ?? obj2)) == null)
			{
				obj3 = new Result<T>(default(T), ProviderError.NotFound);
			}
			return (Result<T>)obj3;
		}

		public bool IsLegacyDNInUse(string legacyDN)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, legacyDN);
			string proxyAddressString = "x500:" + legacyDN;
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, ProxyAddress.Parse(proxyAddressString));
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADRecipientSchema.LegacyExchangeDN
			};
			ADRawEntry[] array = base.Find<ADRawEntry>(null, QueryScope.SubTree, filter, null, 1, properties, true);
			return array.Length != 0;
		}

		internal static QueryFilter ADObjectIdFilterBuilder(ADObjectId id)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id);
		}

		internal static void ADObjectIdHashInserter<T>(Hashtable hash, T entry) where T : ADRawEntry
		{
			hash.Add(entry.Id.DistinguishedName, new Result<T>(entry, null));
			hash.Add(entry.Id.ObjectGuid.ToString(), new Result<T>(entry, null));
		}

		internal static Result<T> ADObjectIdHashLookup<T>(Hashtable hash, ADObjectId key) where T : ADRawEntry
		{
			object obj;
			if (string.IsNullOrEmpty(key.DistinguishedName))
			{
				obj = hash[key.ObjectGuid.ToString()];
			}
			else
			{
				obj = hash[key.DistinguishedName];
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				obj2 = new Result<T>(default(T), ProviderError.NotFound);
			}
			return (Result<T>)obj2;
		}

		private TEntry InternalFindByExchangeGuid<TEntry>(Guid exchangeGuid, bool includeAlternative, bool includeArchive, IEnumerable<PropertyDefinition> properties) where TEntry : ADRawEntry, new()
		{
			QueryFilter filter = ADRecipientObjectSession.QueryFilterFromExchangeGuid(exchangeGuid, includeAlternative, includeArchive);
			IEnumerable<PropertyDefinition> enumerable = properties;
			if (includeArchive && enumerable != null)
			{
				enumerable = properties.Concat(new List<PropertyDefinition>
				{
					ADMailboxRecipientSchema.ExchangeGuid,
					SharedPropertyDefinitions.ProvisioningFlags
				});
			}
			TEntry[] array = base.Find<TEntry>(null, QueryScope.SubTree, filter, null, 2, enumerable, false);
			switch (array.Length)
			{
			case 0:
				return default(TEntry);
			case 1:
				return array[0];
			default:
				if (includeArchive)
				{
					foreach (TEntry result in array)
					{
						if (result[ADMailboxRecipientSchema.ExchangeGuid] != null && result[ADRecipientSchema.ProvisioningFlags] != null && ((Guid)result[ADMailboxRecipientSchema.ExchangeGuid]).Equals(exchangeGuid) && ((int)result[ADRecipientSchema.ProvisioningFlags] & 131072) == 131072)
						{
							return result;
						}
					}
				}
				throw new NonUniqueRecipientException(exchangeGuid, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueExchangeGuid(exchangeGuid.ToString()), array[0].Id, string.Empty));
			}
		}

		protected QueryFilter QueryFilterFromProxyAddress(ProxyAddress proxyAddress)
		{
			if (proxyAddress.Prefix == ProxyAddressPrefix.LegacyDN)
			{
				string addressString = proxyAddress.AddressString;
				string propertyValue = "x500:" + addressString;
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, propertyValue),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.LegacyExchangeDN, addressString)
				});
			}
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, proxyAddress.ToString());
		}

		private TResult FindByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties) where TResult : ADRawEntry, new()
		{
			QueryFilter filter = this.QueryFilterFromProxyAddress(proxyAddress);
			TResult[] array = base.Find<TResult>(null, QueryScope.SubTree, filter, null, 2, properties, false);
			switch (array.Length)
			{
			case 0:
				if (base.SessionSettings.ConfigReadScope == null || base.SessionSettings.ConfigReadScope.Root == null || this.SearchRoot != null)
				{
					return default(TResult);
				}
				array = base.Find<TResult>(base.SessionSettings.ConfigReadScope.Root, QueryScope.SubTree, filter, null, 2, properties, false);
				switch (array.Length)
				{
				case 0:
					return default(TResult);
				case 1:
					return array[0];
				default:
					throw new NonUniqueRecipientException(proxyAddress, new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), array[0].Id, string.Empty));
				}
				break;
			case 1:
				return array[0];
			default:
				throw new NonUniqueRecipientException(proxyAddress, new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), array[0].Id, string.Empty));
			}
		}

		private Result<TData>[] FindByExchangeGuidsIncludingArchive<TData>(Guid[] keys, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			Converter<Guid, QueryFilter> filterBuilder = (Guid g) => ADRecipientObjectSession.QueryFilterFromExchangeGuid(g, false, true);
			return base.ReadMultiple<Guid, TData>(keys, filterBuilder, null, null, properties);
		}

		private Result<TData>[] FindByProxyAddresses<TData>(ProxyAddress[] keys, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			Converter<ProxyAddress, QueryFilter> filterBuilder = new Converter<ProxyAddress, QueryFilter>(this.QueryFilterFromProxyAddress);
			ADDataSession.HashInserter<TData> hashInserter = new ADDataSession.HashInserter<TData>(ADRecipientObjectSession.FindByProxyAddressesHashInserter<TData>);
			ADDataSession.HashLookup<ProxyAddress, TData> hashLookup = new ADDataSession.HashLookup<ProxyAddress, TData>(ADRecipientObjectSession.FindByProxyAddressesHashLookup<TData>);
			Result<TData>[] array = base.ReadMultiple<ProxyAddress, TData>(keys, filterBuilder, hashInserter, hashLookup, properties);
			if (base.SessionSettings.ConfigReadScope != null && base.SessionSettings.ConfigReadScope.Root != null && this.SearchRoot == null)
			{
				List<ProxyAddress> list = new List<ProxyAddress>();
				List<int> list2 = new List<int>();
				for (int i = 0; i < array.Length; i++)
				{
					Result<TData> result = array[i];
					if (result.Error == ProviderError.NotFound)
					{
						list.Add(keys[i]);
						list2.Add(i);
					}
				}
				if (list.Count > 0)
				{
					Result<TData>[] array2 = base.ReadMultiple<ProxyAddress, TData>(list.ToArray(), base.SessionSettings.ConfigReadScope.Root, filterBuilder, null, hashInserter, hashLookup, properties);
					for (int j = 0; j < array2.Length; j++)
					{
						Result<TData> result2 = array2[j];
						if (result2.Error == null)
						{
							array[list2[j]] = result2;
						}
					}
				}
			}
			return array;
		}

		public Result<ADRawEntry>[] FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			if (legacyExchangeDNs == null)
			{
				throw new ArgumentNullException("legacyExchangeDNs");
			}
			if (legacyExchangeDNs.Length == 0)
			{
				return new Result<ADRawEntry>[0];
			}
			if (properties == null)
			{
				properties = new PropertyDefinition[2];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 2);
			}
			properties[properties.Length - 1] = ADRecipientSchema.LegacyExchangeDN;
			properties[properties.Length - 2] = ADRecipientSchema.EmailAddresses;
			return base.ReadMultiple<string, ADRawEntry>(legacyExchangeDNs, new Converter<string, QueryFilter>(ADRecipientObjectSession.FindByLegacyExchangeDNsFilterBuilder), new ADDataSession.HashInserter<ADRawEntry>(ADRecipientObjectSession.FindByLegacyExchangeDNsHashInserter<ADRawEntry>), new ADDataSession.HashLookup<string, ADRawEntry>(ADRecipientObjectSession.FindByLegacyExchangeDNsHashLookup<ADRawEntry>), properties);
		}

		public Result<ADRecipient>[] FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			if (legacyExchangeDNs == null)
			{
				throw new ArgumentNullException("legacyExchangeDNs");
			}
			if (legacyExchangeDNs.Length == 0)
			{
				return new Result<ADRecipient>[0];
			}
			return base.ReadMultiple<string, ADRecipient>(legacyExchangeDNs, new Converter<string, QueryFilter>(ADRecipientObjectSession.FindByLegacyExchangeDNsFilterBuilder), new ADDataSession.HashInserter<ADRecipient>(ADRecipientObjectSession.FindByLegacyExchangeDNsHashInserter<ADRecipient>), new ADDataSession.HashLookup<string, ADRecipient>(ADRecipientObjectSession.FindByLegacyExchangeDNsHashLookup<ADRecipient>), null);
		}

		public ADRecipient[] FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			return this.FindNames<ADRecipient>(dictionary, limit, null);
		}

		public object[][] FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			ADRawEntry[] recipients = this.FindNames<ADRawEntry>(dictionary, limit, properties);
			return ADSession.ConvertToView(recipients, properties);
		}

		private TResult[] FindNames<TResult>(IDictionary<PropertyDefinition, object> dictionary, int limit, IEnumerable<PropertyDefinition> properties) where TResult : ADRawEntry, new()
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			int count = dictionary.Keys.Count;
			if (count == 0)
			{
				return new TResult[0];
			}
			int num = 0;
			QueryFilter[] array = new QueryFilter[count];
			foreach (PropertyDefinition propertyDefinition in dictionary.Keys)
			{
				if (dictionary[propertyDefinition] is string)
				{
					array[num] = new TextFilter(propertyDefinition, (string)dictionary[propertyDefinition], MatchOptions.Prefix, MatchFlags.Loose);
				}
				else
				{
					array[num] = new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, dictionary[propertyDefinition]);
				}
				num++;
			}
			QueryFilter filter = new AndFilter(array);
			SortBy sortBy = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);
			return base.Find<TResult>(null, QueryScope.SubTree, filter, sortBy, limit, properties, false);
		}

		public ADObject FindByAccountName<T>(string domainName, string accountName) where T : IConfigurable, new()
		{
			IEnumerable<T> enumerable = this.FindByAccountName<T>(domainName, accountName, null, null);
			IEnumerator<T> enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return null;
			}
			ADObject adobject = (ADObject)((object)enumerator.Current);
			if (enumerator.MoveNext())
			{
				throw new NonUniqueRecipientException(domainName + "\\" + accountName, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueDomainAccount(domainName, accountName), adobject.Id, string.Empty));
			}
			return adobject;
		}

		public IEnumerable<T> FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter) where T : IConfigurable, new()
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (account.Length == 0)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionInvalidAccountName(account));
			}
			bool isNetbiosCompatible = account.Contains("\\");
			if (isNetbiosCompatible)
			{
				string[] array = account.Split(new char[]
				{
					'\\'
				});
				if (array.Length > 2)
				{
					isNetbiosCompatible = false;
				}
				else if (!string.IsNullOrEmpty(domain) && !domain.Equals(array[0], StringComparison.OrdinalIgnoreCase))
				{
					isNetbiosCompatible = false;
				}
				else
				{
					domain = array[0];
					account = array[1];
				}
			}
			else
			{
				isNetbiosCompatible = !string.IsNullOrEmpty(domain);
			}
			ComparisonFilter accountFilter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.SamAccountName, account);
			searchFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				searchFilter,
				accountFilter
			});
			IEnumerable<T> results = ((IConfigDataProvider)this).FindPaged<T>(searchFilter, rootId, true, null, 0);
			IEnumerator<T> enumRes = results.GetEnumerator();
			if (!string.IsNullOrEmpty(domain))
			{
				bool found = false;
				bool isFqdn = domain.Contains(".");
				ADObjectId domainId = null;
				AdName domainRdn = null;
				if (isFqdn)
				{
					domainId = new ADObjectId(NativeHelpers.DistinguishedNameFromCanonicalName(domain));
				}
				else
				{
					domainRdn = new AdName("DC", domain);
				}
				while (enumRes.MoveNext())
				{
					!0 ! = enumRes.Current;
					ADObject recipient = (ADObject)((object)!);
					bool match = isFqdn ? ADObjectId.Equals(recipient.Id.DomainId, domainId) : (recipient.Id.DomainId.Rdn == domainRdn);
					if (match)
					{
						found = true;
						yield return enumRes.Current;
					}
				}
				if (!found)
				{
					ExTraceGlobals.ADReadDetailsTracer.TraceDebug<string>((long)this.GetHashCode(), "ADRecipientObjectSession::FindByAccountName - None of recipients matched specified domain. Trying to resolve domain name {0} using AD. This has performance impact. If you have domain with Netbios name that does not match left part of DNS name consider renaming it to improve performance.", domain);
					ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, base.ReadOnly, base.ConsistencyMode, base.NetworkCredential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(base.SessionSettings.PartitionId), 2145, "FindByAccountName", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipientObjectSession.cs");
					ADCrossRef[] crossRefs = configSession.FindADCrossRefByNetBiosName(domain);
					if (crossRefs.Length > 0)
					{
						results = ((IConfigDataProvider)this).FindPaged<T>(searchFilter, rootId, true, null, 0);
						enumRes = results.GetEnumerator();
						ADCrossRef crossRef = crossRefs[0];
						while (enumRes.MoveNext())
						{
							!0 !2 = enumRes.Current;
							ADObject recipient2 = (ADObject)((object)!2);
							if (ADObjectId.Equals(recipient2.Id.DomainId, crossRef.NCName))
							{
								yield return enumRes.Current;
							}
						}
					}
				}
			}
			else
			{
				foreach (T item in results)
				{
					yield return item;
				}
			}
			yield break;
		}

		public IEnumerable<ADGroup> FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sId)
		{
			if (sId == null)
			{
				throw new ArgumentNullException("sId");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ForeignGroupSid, sId);
			return base.FindPaged<ADGroup>(root, QueryScope.SubTree, filter, null, 0, null);
		}

		public ADRawEntry FindADRawEntryBySid(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties)
		{
			QueryFilter filter = ADRecipientObjectSession.ConstructRecipientSidFilter(sId);
			ADPagedReader<ADRawEntry> adpagedReader = this.FindPagedADRawEntryWithDefaultFilters<ADRecipient>(null, QueryScope.SubTree, filter, null, 2, properties);
			ADRawEntry adrawEntry = null;
			foreach (ADRawEntry adrawEntry2 in adpagedReader)
			{
				if (adrawEntry != null)
				{
					throw new NonUniqueRecipientException(sId, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueSid(sId.ToString()), adrawEntry.Id, string.Empty));
				}
				adrawEntry = adrawEntry2;
			}
			return adrawEntry;
		}

		public TResult FindMiniRecipientBySid<TResult>(SecurityIdentifier sId, IEnumerable<PropertyDefinition> properties) where TResult : MiniRecipient, new()
		{
			QueryFilter filter = ADRecipientObjectSession.ConstructRecipientSidFilter(sId);
			ADPagedReader<TResult> adpagedReader = base.FindPaged<TResult>(null, QueryScope.SubTree, filter, null, 2, properties);
			TResult tresult = default(TResult);
			foreach (TResult tresult2 in adpagedReader)
			{
				if (tresult != null)
				{
					throw new NonUniqueRecipientException(sId, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueSid(sId.ToString()), tresult.Id, string.Empty));
				}
				tresult = tresult2;
			}
			return tresult;
		}

		public TResult FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties) where TResult : MiniRecipient, new()
		{
			return this.FindByProxyAddress<TResult>(proxyAddress, properties);
		}

		public ADRecipient FindBySid(SecurityIdentifier sId)
		{
			QueryFilter filter = ADRecipientObjectSession.ConstructRecipientSidFilter(sId);
			ADRecipient[] array = this.Find(null, QueryScope.SubTree, filter, null, 2);
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return array[0];
			default:
				throw new NonUniqueRecipientException(sId, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueSid(sId.ToString()), array[0].Id, string.Empty));
			}
		}

		public ADRecipient FindByExchangeObjectId(Guid exchangeObjectId)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeObjectId, exchangeObjectId);
			ADRecipient[] array = this.Find(null, QueryScope.SubTree, filter, null, 2);
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return array[0];
			default:
				throw new NonUniqueRecipientException(exchangeObjectId, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueExchangeObjectId(exchangeObjectId.ToString()), array[0].Id, string.Empty));
			}
		}

		public ADRecipient FindByExchangeGuid(Guid exchangeGuid)
		{
			return this.InternalFindByExchangeGuid<ADRecipient>(exchangeGuid, false, false, null);
		}

		public Result<OWAMiniRecipient>[] FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			if (userPrincipalNames == null)
			{
				throw new ArgumentNullException("userPrincipalNames");
			}
			if (userPrincipalNames.Length == 0)
			{
				return new Result<OWAMiniRecipient>[0];
			}
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				ADRecipientSchema.EmailAddresses
			};
			return base.ReadMultiple<string, OWAMiniRecipient>(userPrincipalNames, new Converter<string, QueryFilter>(this.QueryFilterFromUserPrincipalName), null, null, properties);
		}

		public ADRecipient FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			return this.InternalFindByExchangeGuid<ADRecipient>(exchangeGuid, true, true, null);
		}

		public ADRawEntry FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalFindByExchangeGuid<ADRawEntry>(exchangeGuid, true, true, properties);
		}

		public TObject FindByExchangeGuidIncludingAlternate<TObject>(Guid exchangeGuid) where TObject : ADObject, new()
		{
			return this.InternalFindByExchangeGuid<TObject>(exchangeGuid, true, true, null);
		}

		public ADRecipient FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			return this.InternalFindByExchangeGuid<ADRecipient>(exchangeGuid, false, true, null);
		}

		public TEntry FindByExchangeGuid<TEntry>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties) where TEntry : MiniRecipient, new()
		{
			return this.InternalFindByExchangeGuid<TEntry>(exchangeGuid, false, false, properties);
		}

		public ADRawEntry FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return this.InternalFindByExchangeGuid<ADRawEntry>(exchangeGuid, false, false, properties);
		}

		public bool IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			if (base.ConfigScope == ConfigScopes.TenantSubTree || base.ConfigScope == ConfigScopes.None)
			{
				throw new InvalidOperationException("Default throttling policies can only be obtained when the Session has a ConfigScope of TenantLocal or Global.");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ThrottlingPolicy, throttlingPolicyId);
			ADRawEntry[] array = base.Find<ADRawEntry>(null, QueryScope.SubTree, filter, null, 1, ADRecipientObjectSession.IsThrottlingPolicyInUseProperties, false);
			return array != null && array.Length > 0;
		}

		public ADRawEntry[] FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			QueryFilter certificateMatchFilter = ADUser.GetCertificateMatchFilter(identifier);
			return base.Find<ADRawEntry>(null, QueryScope.SubTree, certificateMatchFilter, null, 0, properties, false);
		}

		public ADRecipient FindByCertificate(X509Identifier identifier)
		{
			QueryFilter certificateMatchFilter = ADUser.GetCertificateMatchFilter(identifier);
			ADRecipient[] array = this.Find(null, QueryScope.SubTree, certificateMatchFilter, null, 2);
			switch (array.Length)
			{
			case 0:
				return null;
			case 1:
				return array[0];
			default:
				throw new NonUniqueRecipientException(identifier, new ObjectValidationError(DirectoryStrings.ErrorNonUniqueSid(identifier.ToString()), array[0].Id, string.Empty));
			}
		}

		public ADRecipient FindByObjectGuid(Guid guid)
		{
			return this.Read(new ADObjectId(null, guid));
		}

		public SecurityIdentifier GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			if (!base.UseGlobalCatalog || base.UseConfigNC)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionWKGuidNeedsGCSession(wkguid));
			}
			ADObjectId containerId = string.IsNullOrEmpty(base.DomainController) ? base.GetConfigurationNamingContext() : ADSession.GetConfigurationNamingContext(base.DomainController, base.NetworkCredential);
			ADGroup adgroup = base.ResolveWellKnownGuid<ADGroup>(wkguid, containerId);
			if (adgroup == null)
			{
				throw new ADExternalException(DirectoryStrings.ExceptionADTopologyCannotFindWellKnownExchangeGroup);
			}
			return adgroup.Sid;
		}

		public ADObjectId[] ResolveSidsToADObjectIds(string[] sids)
		{
			if (sids == null)
			{
				throw new ArgumentNullException("sids");
			}
			if (sids.Length == 0)
			{
				throw new ArgumentException("sids");
			}
			Result<ADRawEntry>[] array = base.ReadMultiple<string, ADRawEntry>(sids, new Converter<string, QueryFilter>(this.ObjectSidQueryFilter), null, null, new ProviderPropertyDefinition[0]);
			List<ADObjectId> list = new List<ADObjectId>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Data != null)
				{
					list.Add(array[i].Data.Id);
				}
			}
			return list.ToArray();
		}

		public MiniRecipientWithTokenGroups ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			bool useGlobalCatalog = base.UseGlobalCatalog;
			MiniRecipientWithTokenGroups[] array;
			try
			{
				base.UseGlobalCatalog = false;
				base.EnforceDefaultScope = false;
				array = base.Find<MiniRecipientWithTokenGroups>(id, QueryScope.Base, null, null, 1);
			}
			finally
			{
				base.UseGlobalCatalog = useGlobalCatalog;
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public List<string> GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (assignmentMethodFlags == AssignmentMethod.None)
			{
				return null;
			}
			ADObjectId userId = null;
			ADObjectId planId = null;
			SecurityIdentifier userSid = null;
			if ((assignmentMethodFlags & AssignmentMethod.SecurityGroup) != AssignmentMethod.None || (assignmentMethodFlags & AssignmentMethod.RoleGroup) != AssignmentMethod.None)
			{
				userId = (ADObjectId)user[ADObjectSchema.Id];
			}
			if ((assignmentMethodFlags & AssignmentMethod.MailboxPlan) != AssignmentMethod.None)
			{
				planId = (ADObjectId)user[ADRecipientSchema.MailboxPlan];
			}
			if ((assignmentMethodFlags & AssignmentMethod.Direct) != AssignmentMethod.None)
			{
				userSid = (SecurityIdentifier)user[IADSecurityPrincipalSchema.Sid];
			}
			return this.GetTokenSids(userId, userSid, planId, assignmentMethodFlags);
		}

		public List<string> GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			if (userId == null)
			{
				throw new ArgumentNullException("userId");
			}
			return this.GetTokenSids(userId, null, null, assignmentMethodFlags);
		}

		protected QueryFilter QueryFilterFromUserPrincipalName(string userPrincipalName)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.UserPrincipalName, userPrincipalName);
		}

		private QueryFilter ObjectSidQueryFilter(string sidString)
		{
			SecurityIdentifier propertyValue = new SecurityIdentifier(sidString);
			return new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Sid, propertyValue);
		}

		private List<string> GetTokenSids(ADObjectId userId, SecurityIdentifier userSid, ADObjectId planId, AssignmentMethod assignmentMethodFlags)
		{
			if (assignmentMethodFlags == AssignmentMethod.None)
			{
				return null;
			}
			int num = 0;
			MultiValuedProperty<SecurityIdentifier> multiValuedProperty = null;
			if ((assignmentMethodFlags & AssignmentMethod.Direct) != AssignmentMethod.None)
			{
				num++;
			}
			if (((assignmentMethodFlags & AssignmentMethod.SecurityGroup) != AssignmentMethod.None || (assignmentMethodFlags & AssignmentMethod.RoleGroup) != AssignmentMethod.None) && userId != null)
			{
				bool flag = OrganizationId.ForestWideOrgId.Equals(base.SessionSettings.CurrentOrganizationId);
				bool flag2 = base.SessionSettings.ConfigScopes == ConfigScopes.AllTenants || (base.SessionSettings.ConfigScopes == ConfigScopes.TenantLocal && !flag);
				MiniRecipientWithTokenGroups miniRecipientWithTokenGroups;
				if (flag2)
				{
					ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(base.DomainController, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 2783, "GetTokenSids", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipientObjectSession.cs");
					CompositeTenantRecipientSession compositeTenantRecipientSession = tenantRecipientSession as CompositeTenantRecipientSession;
					if (compositeTenantRecipientSession != null)
					{
						compositeTenantRecipientSession.CacheSessionForDeletingOnly = false;
					}
					miniRecipientWithTokenGroups = tenantRecipientSession.ReadTokenGroupsGlobalAndUniversal(userId);
				}
				else
				{
					miniRecipientWithTokenGroups = this.ReadTokenGroupsGlobalAndUniversal(userId);
				}
				if (miniRecipientWithTokenGroups != null)
				{
					multiValuedProperty = miniRecipientWithTokenGroups.TokenGroupsGlobalAndUniversal;
				}
				if (multiValuedProperty != null)
				{
					num += multiValuedProperty.Count;
				}
			}
			if (planId != null)
			{
				num++;
			}
			if ((assignmentMethodFlags & AssignmentMethod.ExtraDefaultSids) != AssignmentMethod.None)
			{
				num += 2;
			}
			List<string> list = new List<string>(num);
			if ((assignmentMethodFlags & AssignmentMethod.SecurityGroup) != AssignmentMethod.None || (assignmentMethodFlags & AssignmentMethod.RoleGroup) != AssignmentMethod.None)
			{
				if (userId != null && multiValuedProperty != null)
				{
					using (MultiValuedProperty<SecurityIdentifier>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SecurityIdentifier securityIdentifier = enumerator.Current;
							list.Add(securityIdentifier.ToString());
							ExTraceGlobals.ADFindTracer.TraceDebug<string>(0L, "Adding group SID {0}", securityIdentifier.ToString());
						}
						goto IL_171;
					}
				}
				ExTraceGlobals.ADFindTracer.TraceError(0L, "User SID and/or groupSids are null");
			}
			IL_171:
			if ((assignmentMethodFlags & AssignmentMethod.Direct) != AssignmentMethod.None)
			{
				if (userSid != null)
				{
					list.Add(userSid.ToString());
					ExTraceGlobals.ADFindTracer.TraceDebug<string>(0L, "Adding user SID {0}", userSid.ToString());
				}
				else
				{
					ExTraceGlobals.ADFindTracer.TraceError(0L, "User SID is null!");
				}
			}
			if ((assignmentMethodFlags & AssignmentMethod.MailboxPlan) != AssignmentMethod.None && planId != null)
			{
				ADRawEntry adrawEntry = base.ReadADRawEntry(planId, new ADPropertyDefinition[]
				{
					IADSecurityPrincipalSchema.Sid
				});
				if (adrawEntry != null)
				{
					SecurityIdentifier securityIdentifier2 = (SecurityIdentifier)adrawEntry[IADSecurityPrincipalSchema.Sid];
					if (securityIdentifier2 != null)
					{
						list.Add(securityIdentifier2.ToString());
						ExTraceGlobals.ADFindTracer.TraceDebug<string>(0L, "Adding parent mailbox plan SID {0}", securityIdentifier2.ToString());
					}
					else
					{
						ExTraceGlobals.ADFindTracer.TraceError(0L, "Mailbox plan SID is null!");
					}
				}
			}
			if ((assignmentMethodFlags & AssignmentMethod.ExtraDefaultSids) != AssignmentMethod.None)
			{
				list.Add(ADRecipientObjectSession.EveryoneSid);
				list.Add(ADRecipientObjectSession.AuthenticatedUserSid);
			}
			return list;
		}

		public override QueryFilter ApplyDefaultFilters(QueryFilter clientFilter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			applyImplicitFilter = !Filters.HasWellKnownRecipientTypeFilter(clientFilter);
			if (!applyImplicitFilter)
			{
				ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug(0L, "ADRecipientObjectSession.ApplyDefaultFilters:  Filters.HasWellKnownRecipientTypeFilter found a well-known recipient type filter so default recipient filter will not be added");
			}
			QueryFilter queryFilter = base.ApplyDefaultFilters(clientFilter, scope, dummyObject, applyImplicitFilter);
			if (this.addressListMembershipFilter == null)
			{
				return queryFilter;
			}
			return new AndFilter(new QueryFilter[]
			{
				queryFilter,
				this.addressListMembershipFilter
			});
		}

		private const string ForeignSecurityPrincipalClass = "foreignSecurityPrincipal";

		internal static readonly int ReadMultipleMaxBatchSize = 20;

		private static readonly PropertyDefinition[] IsThrottlingPolicyInUseProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id
		};

		protected QueryFilter addressListMembershipFilter;

		private static readonly string EveryoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null).ToString();

		private static readonly string AuthenticatedUserSid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null).ToString();

		protected bool isReducedRecipientSession;
	}
}
