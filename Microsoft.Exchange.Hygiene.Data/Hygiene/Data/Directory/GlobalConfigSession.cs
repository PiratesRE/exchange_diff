using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Cache.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class GlobalConfigSession
	{
		private IConfigDataProvider CompositeProvider
		{
			get
			{
				if (this.compositeProvider == null)
				{
					lock (this)
					{
						this.compositeProvider = (this.compositeProvider ?? ConfigDataProviderFactory.CacheFallbackDefault.Create(DatabaseType.Directory));
					}
				}
				return this.compositeProvider;
			}
		}

		private IConfigDataProvider WebStoreDataProvider
		{
			get
			{
				if (this.webStoreDataProvider == null)
				{
					lock (this)
					{
						this.webStoreDataProvider = (this.webStoreDataProvider ?? ConfigDataProviderFactory.Default.Create(DatabaseType.Directory));
					}
				}
				return this.webStoreDataProvider;
			}
		}

		private IBloomFilterDataProvider BloomFilterProvider
		{
			get
			{
				return GlobalConfigSession.bloomFilterProvider.Value;
			}
		}

		public void Save(OnDemandQueryRequest reportRequest)
		{
			if (reportRequest == null)
			{
				throw new ArgumentNullException("reportRequest");
			}
			reportRequest[ADObjectSchema.OrganizationalUnitRoot] = GlobalConfigSession.onDemandReportsFixedTenantId;
			this.WebStoreDataProvider.Save(reportRequest);
		}

		public bool IsRegionEmailOptout()
		{
			IEnumerable<RegionEmailFilter> regionEmailOptout = this.GetRegionEmailOptout();
			return regionEmailOptout.Any<RegionEmailFilter>() && regionEmailOptout.First<RegionEmailFilter>().Enabled;
		}

		public IEnumerable<RegionEmailFilter> GetRegionEmailOptout()
		{
			return this.WebStoreDataProvider.Find<RegionEmailFilter>(null, null, false, null).Cast<RegionEmailFilter>();
		}

		public IEnumerable<OnDemandQueryRequest> FindReportRequestsByTenant(Guid tenantId, Guid? requestId = null, DateTime? submissionDateTimeStart = null)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, GlobalConfigSession.onDemandReportsFixedTenantId);
			if (requestId != null)
			{
				return (from OnDemandQueryRequest r in this.WebStoreDataProvider.Find<OnDemandQueryRequest>(QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, OnDemandQueryRequestSchema.RequestId, requestId.Value)
				}), null, false, null)
				where submissionDateTimeStart == null || r.SubmissionTime >= submissionDateTimeStart.Value.Subtract(TimeSpan.FromSeconds(1.0))
				select r).Cache<OnDemandQueryRequest>();
			}
			return (from r in new ConfigDataProviderPagedReader<OnDemandQueryRequest>(this.WebStoreDataProvider, null, queryFilter, null, 500)
			where r.TenantId == tenantId && (requestId == null || r.RequestId == requestId) && (submissionDateTimeStart == null || r.SubmissionTime >= submissionDateTimeStart.Value.Subtract(TimeSpan.FromSeconds(1.0)))
			select r).Cache<OnDemandQueryRequest>();
		}

		public IEnumerable<OnDemandQueryRequest> FindPagedReportRequests(IEnumerable<OnDemandQueryType> queryTypes, IEnumerable<OnDemandQueryRequestStatus> requestStatuses, ref string pageCookie, out bool complete, int pageSize = 100)
		{
			QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, GlobalConfigSession.onDemandReportsFixedTenantId), pageCookie);
			IEnumerable<OnDemandQueryRequest> result = (from OnDemandQueryRequest r in this.WebStoreDataProvider.FindPaged<OnDemandQueryRequest>(pagingQueryFilter, null, false, null, pageSize)
			where queryTypes.Any((OnDemandQueryType t) => t == r.QueryType) && requestStatuses.Any((OnDemandQueryRequestStatus s) => s == r.RequestStatus)
			select r).Cache<OnDemandQueryRequest>();
			pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out complete);
			return result;
		}

		internal ADMiniDomain GetDomainByDomainNames(IEnumerable<string> domainNames)
		{
			if (domainNames != null && domainNames.Any<string>())
			{
				if (domainNames.All((string item) => !string.IsNullOrEmpty(item)))
				{
					return null;
				}
			}
			throw new ArgumentNullException("domainNames");
		}

		internal void FindEnabledInboundConnectorsByIPAddressOrCertificate(string ipAddress, IEnumerable<string> certificateFqdns, out IEnumerable<TenantInboundConnector> certConnectors, out IEnumerable<TenantInboundConnector> ipConnectors)
		{
			if (string.IsNullOrEmpty(ipAddress) && (certificateFqdns == null || !certificateFqdns.Any<string>()))
			{
				throw new ArgumentException("Both ipAddress and certificateFqdns cannot be empty");
			}
			certConnectors = GlobalConfigSession.FindInboundConnectorsByCertificate(this.CompositeProvider, certificateFqdns, true);
			ipConnectors = GlobalConfigSession.FindInboundConnectorsByOutboundIp(this.CompositeProvider, ipAddress, true);
		}

		internal bool IsRecipientValid(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			return this.BloomFilterProvider.Check<UserExtendedPropertiesEmailAddress>(new ComparisonFilter(ComparisonOperator.Equal, UserExtendedPropertiesEmailAddress.UserEmailAddressProp, emailAddress.ToLower())) || this.BloomFilterProvider.Check<GroupExtendedPropertiesEmailAddress>(new ComparisonFilter(ComparisonOperator.Equal, GroupExtendedPropertiesEmailAddress.GroupEmailAddressProp, emailAddress.ToLower())) || this.BloomFilterProvider.Check<ContactExtendedPropertiesEmailAddress>(new ComparisonFilter(ComparisonOperator.Equal, ContactExtendedPropertiesEmailAddress.ContactEmailAddressProp, emailAddress.ToLower()));
		}

		public bool TryFindMatchingDomain(SmtpDomain inputDomain, out SmtpDomain bestMatch, out bool isExactMatch)
		{
			if (inputDomain == null)
			{
				throw new ArgumentNullException("inputDomain");
			}
			return GlobalConfigSession.TryFindMatchingDomainInternal(inputDomain, this.BloomFilterProvider, out bestMatch, out isExactMatch);
		}

		internal ADMiniUser GetUserByCertificate(string certificateSubjectName, string certificateIssuerName)
		{
			if (string.IsNullOrEmpty(certificateSubjectName))
			{
				throw new ArgumentNullException("certificateSubjectName");
			}
			if (string.IsNullOrEmpty(certificateIssuerName))
			{
				throw new ArgumentNullException("certificateIssuerName");
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, UserCertificate.CertificateSubjectNameProp, certificateSubjectName),
				new ComparisonFilter(ComparisonOperator.Equal, UserCertificate.CertificateIssuerNameProp, certificateIssuerName)
			});
			return GlobalConfigSession.GetMiniUser(this.WebStoreDataProvider, filter);
		}

		internal IEnumerable<TenantIPInfo> FindTenantIPs(TenantIPCookie tenantIPCookie)
		{
			IEnumerable<TenantIPInfo> result;
			try
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.StartTimeProp, tenantIPCookie.UpdateWatermark());
				IConfigurable[] source = this.WebStoreDataProvider.Find<TenantIPInfo>(filter, null, false, null);
				tenantIPCookie.CommitNewWatermark();
				result = source.Cast<TenantIPInfo>();
			}
			catch
			{
				tenantIPCookie.RevertToOldWatermark();
				throw;
			}
			return result;
		}

		internal IEnumerable<TenantIPInfo> FindPagedTenantIPs(ref string pageCookie, int pageSize)
		{
			List<TenantIPInfo> list = new List<TenantIPInfo>();
			string text = pageCookie ?? string.Empty;
			foreach (object propertyValue in ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions())
			{
				QueryFilter baseQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, propertyValue);
				QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(baseQueryFilter, text);
				list.AddRange(this.WebStoreDataProvider.FindPaged<TenantIPInfo>(pagingQueryFilter, null, false, null, pageSize));
				bool flag = true;
				text = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
			}
			pageCookie = text;
			return list;
		}

		internal FfoTenant GetTenantByName(string tenantName)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, tenantName);
			return (FfoTenant)this.WebStoreDataProvider.Find<FfoTenant>(filter, null, false, null).FirstOrDefault<IConfigurable>();
		}

		internal IEnumerable<ProbeOrganizationInfo> GetProbeOrganizations(string featureTag)
		{
			if (string.IsNullOrEmpty(featureTag))
			{
				throw new ArgumentNullException("featureTag");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.RawName, featureTag);
			return this.WebStoreDataProvider.Find<ProbeOrganizationInfo>(filter, null, false, null).Cast<ProbeOrganizationInfo>();
		}

		internal IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			if (typeof(T) != typeof(ProbeOrganizationInfo) && typeof(T) != typeof(TenantConfigurationCacheEntry))
			{
				throw new ArgumentException(string.Format("The type {0} is not supported for global session Find<T>; please use a scoped DAL session.", typeof(T).Name));
			}
			return this.WebStoreDataProvider.Find<T>(filter, rootId, deepSearch, sortBy).ToArray<IConfigurable>();
		}

		internal void Save(IConfigurable configurable)
		{
			if (!(configurable is ProbeOrganizationInfo) && !(configurable is TenantConfigurationCacheEntry))
			{
				throw new ArgumentException(string.Format("The type {0} is not supported for the global session; please use a scoped DAL session instead.", configurable.GetType().ToString()));
			}
			this.WebStoreDataProvider.Save(configurable);
		}

		internal void Delete(IConfigurable configurable)
		{
			if (!(configurable is ProbeOrganizationInfo) && !(configurable is TenantConfigurationCacheEntry))
			{
				throw new ArgumentException(string.Format("The type {0} is not supported for the global session; please use a scoped DAL session instead.", configurable.GetType().ToString()));
			}
			this.WebStoreDataProvider.Delete(configurable);
		}

		internal IEnumerable<TenantConfigurationCacheEntry> FindPinnedTenantConfigurationCacheEntries()
		{
			return this.Find<TenantConfigurationCacheEntry>(null, null, false, null).Cast<TenantConfigurationCacheEntry>();
		}

		internal IPagedReader<TenantConfigurationCacheEntry> FindPagedTenantConfigurationCacheEntries(int pageSize = 100)
		{
			List<IPagedReader<TenantConfigurationCacheEntry>> list = new List<IPagedReader<TenantConfigurationCacheEntry>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<TenantConfigurationCacheEntry>(this.webStoreDataProvider, null, new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, propertyValue), null, pageSize));
			}
			return new CompositePagedReader<TenantConfigurationCacheEntry>(list.ToArray());
		}

		internal IEnumerable<TenantInboundConnector> FindTenantInboundConnectorsForTenantIds(IEnumerable<Guid> tenantIds)
		{
			if (tenantIds == null)
			{
				throw new ArgumentNullException("tenantIds");
			}
			IEnumerable<Guid> enumerable = from tenantId in tenantIds.Distinct<Guid>()
			where tenantId != Guid.Empty
			select tenantId;
			if (enumerable.Count<Guid>() == 0)
			{
				throw new ArgumentException(HygieneDataStrings.ErrorEmptyList, "tenantIds");
			}
			Dictionary<object, List<Guid>> dictionary = DalHelper.SplitByPhysicalInstance<Guid>((IHashBucket)this.WebStoreDataProvider, enumerable, (Guid i) => i.ToString());
			List<TenantInboundConnector> list = new List<TenantInboundConnector>();
			foreach (object obj in dictionary.Keys)
			{
				list.AddRange(this.webStoreDataProvider.Find<TenantInboundConnector>(QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.TenantIds, new MultiValuedProperty<Guid>(dictionary[obj])),
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, obj)
				}), null, false, null).Cast<TenantInboundConnector>());
			}
			return list;
		}

		private static IEnumerable<TenantInboundConnector> GetInboundConnectors(IConfigDataProvider dataProvider, ADPropertyDefinition propertyDefinition, IEnumerable<string> propertyValues, bool enabledOnly)
		{
			IEnumerable<ComparisonFilter> propertyNameFilters = from propertyValue in propertyValues
			select new ComparisonFilter(ComparisonOperator.Equal, propertyDefinition, propertyValue);
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				QueryFilter.OrTogether(propertyNameFilters.ToArray<QueryFilter>()),
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.CacheFailoverModeProp, CacheFailoverMode.BloomFilter)
			});
			IEnumerable<IConfigurable> inboundConnectors = dataProvider.Find<TenantInboundConnector>(filter, null, false, null);
			foreach (IConfigurable configurable in inboundConnectors)
			{
				TenantInboundConnector inboundConnector = (TenantInboundConnector)configurable;
				if (!enabledOnly || inboundConnector.Enabled)
				{
					FfoDirectorySession.FixDistinguishedName(inboundConnector, DalHelper.GetTenantDistinguishedName(inboundConnector.OrganizationalUnitRoot.ObjectGuid.ToString()), inboundConnector.OrganizationalUnitRoot.ObjectGuid, inboundConnector.Id.ObjectGuid, null);
					yield return inboundConnector;
				}
			}
			yield break;
		}

		private static ADMiniUser GetMiniUser(IConfigDataProvider dataProvider, QueryFilter filter)
		{
			IConfigurable[] array = dataProvider.Find<ADMiniUser>(filter, null, false, null);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length > 1)
			{
				throw new AmbiguousMatchException(string.Format("Found multiple entries for given query filter. QueryFilter: {0}", filter.ToString()));
			}
			return (ADMiniUser)array[0];
		}

		internal static IEnumerable<TenantInboundConnector> FindInboundConnectorsByOutboundIp(IConfigDataProvider dataProvider, string ipAddress, bool enabledOnly)
		{
			if (string.IsNullOrEmpty(ipAddress))
			{
				return GlobalConfigSession.emptyInboundConnectorArray;
			}
			return GlobalConfigSession.GetInboundConnectors(dataProvider, TenantInboundConnectorSchema.RemoteIPRanges, new string[]
			{
				ipAddress
			}, enabledOnly).ToArray<TenantInboundConnector>();
		}

		internal static IEnumerable<TenantInboundConnector> FindInboundConnectorsByCertificate(IConfigDataProvider dataProvider, IEnumerable<string> certificateFqdns, bool enabledOnly)
		{
			if (certificateFqdns == null || !certificateFqdns.Any<string>())
			{
				return GlobalConfigSession.emptyInboundConnectorArray;
			}
			HashSet<string> hashSet = null;
			try
			{
				hashSet = GlobalConfigSession.GetSearchableCertificates(certificateFqdns);
			}
			catch (Exception ex)
			{
				if (RetryHelper.IsSystemFatal(ex))
				{
					throw;
				}
			}
			return GlobalConfigSession.GetInboundConnectors(dataProvider, TenantInboundConnectorSchema.TlsSenderCertificateName, (hashSet != null && hashSet.Any<string>()) ? hashSet : certificateFqdns, enabledOnly).ToArray<TenantInboundConnector>();
		}

		internal static bool TryFindMatchingDomainInternal(SmtpDomain inputDomain, IBloomFilterDataProvider dataProvider, out SmtpDomain bestMatch, out bool isExactMatch)
		{
			string text = inputDomain.Domain.ToLower();
			if (dataProvider.Check<AcceptedDomain>(new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, text)))
			{
				bestMatch = inputDomain;
				isExactMatch = true;
				return true;
			}
			foreach (string text2 in GlobalConfigSession.ExpandSubdomains(text))
			{
				if (dataProvider.Check<AcceptedDomain>(new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, "*." + text2)))
				{
					bestMatch = new SmtpDomain(text2);
					isExactMatch = false;
					return true;
				}
			}
			bestMatch = null;
			isExactMatch = false;
			return false;
		}

		private static HashSet<string> GetSearchableCertificates(IEnumerable<string> certificateFqdns)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (certificateFqdns != null && certificateFqdns.Any<string>())
			{
				foreach (string text in certificateFqdns)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						if (!hashSet.Contains(text))
						{
							hashSet.Add(text);
						}
						string text2 = string.Empty;
						string text3 = string.Empty;
						SmtpX509Identifier smtpX509Identifier;
						SmtpDomain smtpDomain;
						if (SmtpX509Identifier.TryParse(text, out smtpX509Identifier))
						{
							if (smtpX509Identifier != null && smtpX509Identifier.SubjectCommonName != null && smtpX509Identifier.SubjectCommonName.SmtpDomain != null)
							{
								text2 = smtpX509Identifier.SubjectCommonName.SmtpDomain.Domain;
							}
						}
						else if (SmtpDomain.TryParse(text, out smtpDomain) && smtpDomain != null)
						{
							text2 = smtpDomain.Domain;
						}
						if (!string.IsNullOrWhiteSpace(text2))
						{
							int num = -1;
							do
							{
								num = text2.IndexOf('.', num + 1);
								if (num != -1)
								{
									if (!string.IsNullOrWhiteSpace(text3))
									{
										string item = "*." + text3;
										if (!hashSet.Contains(item))
										{
											hashSet.Add(item);
										}
									}
									text3 = text2.Substring(num + 1);
								}
							}
							while (num != -1);
						}
					}
				}
			}
			return hashSet;
		}

		internal IEnumerable<UserExtendedPropertiesEmailAddress> FindPagedUserExtendedPropertiesEmailAddress(ref string[] cookie, int pageSize, out bool isComplete)
		{
			return this.FindPagedGenericData<UserExtendedPropertiesEmailAddress>(ref cookie, pageSize, out isComplete);
		}

		internal IEnumerable<GroupExtendedPropertiesEmailAddress> FindPagedGroupExtendedPropertiesEmailAddress(ref string[] cookie, int pageSize, out bool isComplete)
		{
			return this.FindPagedGenericData<GroupExtendedPropertiesEmailAddress>(ref cookie, pageSize, out isComplete);
		}

		internal IEnumerable<ContactExtendedPropertiesEmailAddress> FindPagedContactExtendedPropertiesEmailAddress(ref string[] cookie, int pageSize, out bool isComplete)
		{
			return this.FindPagedGenericData<ContactExtendedPropertiesEmailAddress>(ref cookie, pageSize, out isComplete);
		}

		internal IEnumerable<AcceptedDomain> FindPagedAcceptedDomains(ref string[] cookie, int pageSize, out bool isComplete)
		{
			return this.FindPagedGenericData<AcceptedDomain>(ref cookie, pageSize, out isComplete);
		}

		internal static IEnumerable<string> ExpandSubdomains(string fqdn)
		{
			int dotIndex = fqdn.IndexOf('.');
			do
			{
				int nextIndex = fqdn.IndexOf('.', dotIndex + 1);
				if (nextIndex != -1)
				{
					yield return fqdn.Substring(dotIndex + 1);
				}
				dotIndex = nextIndex;
			}
			while (dotIndex != -1);
			yield break;
		}

		private IEnumerable<T> FindPagedGenericData<T>(ref string[] cookie, int pageSize, out bool isComplete) where T : IConfigurable, new()
		{
			string text = "[end]";
			List<T> list = new List<T>();
			int[] array = ((IPartitionedDataProvider)this.WebStoreDataProvider).GetAllPhysicalPartitions().Cast<int>().ToArray<int>();
			string[] array2;
			if (cookie == null)
			{
				array2 = new string[array.Length];
			}
			else
			{
				array2 = cookie;
			}
			isComplete = true;
			foreach (int num in array)
			{
				bool flag = true;
				QueryFilter baseQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, num);
				string text2 = array2[num] ?? string.Empty;
				if (text2 != text)
				{
					QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(baseQueryFilter, text2);
					list.AddRange(this.WebStoreDataProvider.FindPaged<T>(pagingQueryFilter, null, false, null, pageSize));
					text2 = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
				}
				if (flag)
				{
					array2[num] = text;
				}
				else
				{
					array2[num] = text2;
					isComplete = false;
				}
			}
			cookie = array2;
			return list;
		}

		public static readonly QueryFilter TenantInboundConnectorEnabledFilter = new ComparisonFilter(ComparisonOperator.Equal, TenantInboundConnectorSchema.Enabled, true);

		private static Guid onDemandReportsFixedTenantId = new Guid("00D89CFE-7C72-4D91-B1FE-A8BBDF4DEE62");

		private static TenantInboundConnector[] emptyInboundConnectorArray = new TenantInboundConnector[0];

		private static Lazy<IBloomFilterDataProvider> bloomFilterProvider = new Lazy<IBloomFilterDataProvider>(() => BloomFilterProviderFactory.Default.Create(new Type[]
		{
			typeof(UserExtendedPropertiesEmailAddress),
			typeof(GroupExtendedPropertiesEmailAddress),
			typeof(ContactExtendedPropertiesEmailAddress),
			typeof(AcceptedDomain)
		}, CacheConfiguration.Instance.BloomFilterUpdateFrequency, CacheConfiguration.Instance.BloomFilterTracerTokensEnabled));

		private IConfigDataProvider compositeProvider;

		private IConfigDataProvider webStoreDataProvider;
	}
}
