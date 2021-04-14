using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal abstract class FfoDirectorySession : IDirectorySession
	{
		public IActivityScope ActivityScope { get; set; }

		public string CallerInfo
		{
			get
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return null;
			}
		}

		protected IConfigDataProvider DataProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		protected ADObjectId TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		protected ConfigScopes ConfigScope
		{
			get
			{
				return this.configScope;
			}
			set
			{
				this.configScope = value;
			}
		}

		protected string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		protected FfoDirectorySession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			this.domainController = null;
			this.consistencyMode = consistencyMode;
			this.lcid = CultureInfo.CurrentCulture.LCID;
			this.useGlobalCatalog = false;
			this.enforceDefaultScope = true;
			this.useConfigNC = useConfigNC;
			this.readOnly = readOnly;
			this.networkCredential = networkCredential;
			this.sessionSettings = sessionSettings;
			this.enforceContainerizedScoping = false;
			this.configScope = sessionSettings.ConfigScopes;
			if (sessionSettings.CurrentOrganizationId != null)
			{
				this.tenantId = (sessionSettings.CurrentOrganizationId.OrganizationalUnit ?? sessionSettings.CurrentOrganizationId.ConfigurationUnit);
			}
			if (this.tenantId == null)
			{
				this.tenantId = sessionSettings.RootOrgId;
			}
			this.tenantId = this.ExtractTenantId(this.tenantId);
		}

		protected FfoDirectorySession(ADObjectId tenantId)
		{
			this.tenantId = this.ExtractTenantId(tenantId);
			this.sessionSettings = new FfoSessionSettingsFactory().FromExternalDirectoryOrganizationId(this.tenantId.ObjectGuid);
		}

		void IDirectorySession.AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return filter;
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return filter;
		}

		void IDirectorySession.CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IDirectorySession.UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		ADRawEntry[] IDirectorySession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		TResult[] IDirectorySession.Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			IEnumerable<IConfigurable> source = ((IConfigDataProvider)this).Find<TResult>(filter, rootId, false, sortBy);
			if (maxResults > 0)
			{
				source = source.Take(maxResults);
			}
			return source.Cast<TResult>().ToArray<TResult>();
		}

		ADRawEntry[] IDirectorySession.FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		Result<ADRawEntry>[] IDirectorySession.FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<TData>[] IDirectorySession.FindByADObjectIds<TData>(ADObjectId[] ids)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<TData>[0];
		}

		Result<ADRawEntry>[] IDirectorySession.FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<ADRawEntry>[] IDirectorySession.FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		Result<ADRawEntry>[] IDirectorySession.FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new Result<ADRawEntry>[0];
		}

		ADRawEntry[] IDirectorySession.FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		ADPagedReader<TResult> IDirectorySession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return new FfoPagedReader<TResult>(this, rootId, scope, filter, sortBy, pageSize, properties, false);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return new FfoPagedReader<ADRawEntry>(this, rootId, scope, filter, sortBy, pageSize, properties, false);
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return new FfoPagedReader<ADRawEntry>(this, rootId, scope, filter, sortBy, pageSize, properties, false);
		}

		ADPagedReader<TResult> IDirectorySession.FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId IDirectorySession.GetConfigurationNamingContext()
		{
			return DalHelper.FfoRootDN.Parent;
		}

		ADObjectId IDirectorySession.GetDomainNamingContext()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId IDirectorySession.GetRootDomainNamingContext()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId IDirectorySession.GetSchemaNamingContext()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId IDirectorySession.GetHostedOrganizationsRoot()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADObjectId IDirectorySession.GetConfigurationUnitsRoot()
		{
			ADObjectId configurationNamingContext = ((IDirectorySession)this).GetConfigurationNamingContext();
			return configurationNamingContext.GetChildId("CN", ADObject.ConfigurationUnits);
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return ADScope.Empty;
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			applicableScope = ConfigScopes.AllTenants;
			return new ADScope(rootId, null);
		}

		bool IDirectorySession.GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			dummyObject = null;
			ldapAttributes = null;
			return false;
		}

		bool IDirectorySession.IsReadConnectionAvailable()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return true;
		}

		bool IDirectorySession.IsRootIdWithinScope<TObject>(ADObjectId rootId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return true;
		}

		bool IDirectorySession.IsTenantIdentity(ADObjectId id)
		{
			return id.DistinguishedName.Contains("FFO");
		}

		TResult[] IDirectorySession.ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return (TResult[])new ADRawEntry[0];
		}

		ADRawEntry IDirectorySession.ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			ADRawEntry adrawEntry = new ADRawEntry();
			adrawEntry.SetId(entryId);
			return adrawEntry;
		}

		RawSecurityDescriptor IDirectorySession.ReadSecurityDescriptor(ADObjectId id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		SecurityDescriptor IDirectorySession.ReadSecurityDescriptorBlob(ADObjectId id)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		string[] IDirectorySession.ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new string[0];
		}

		bool IDirectorySession.ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId)
		{
			return ((IDirectorySession)this).ResolveWellKnownGuid<TResult>(wellKnownGuid, containerId.DistinguishedName);
		}

		TenantRelocationSyncObject IDirectorySession.RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN)
		{
			RoleGroupInitInfo roleGroupInitInfo = FfoDirectorySession.SupportedRoleGroups.FirstOrDefault((RoleGroupInitInfo roleGroup) => roleGroup.WellKnownGuid == wellKnownGuid);
			if (roleGroupInitInfo.WellKnownGuid == Guid.Empty)
			{
				FfoDirectorySession.LogNotSupportedInFFO(null);
				return default(TResult);
			}
			string propertyValue = roleGroupInitInfo.Name.Replace(" ", string.Empty).Replace("-", string.Empty);
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.RoleGroup),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, propertyValue)
			});
			IEnumerable<IConfigurable> enumerable = ((IConfigDataProvider)this).Find<TResult>(filter, null, false, null);
			if (enumerable != null)
			{
				return enumerable.Cast<TResult>().FirstOrDefault<TResult>();
			}
			return default(TResult);
		}

		ADOperationResultWithData<TResult>[] IDirectorySession.RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		bool IDirectorySession.TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			exception = null;
			return true;
		}

		void IDirectorySession.UpdateServerSettings(PooledLdapConnection connection)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		void IDirectorySession.VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
		}

		TimeSpan? IDirectorySession.ClientSideSearchTimeout
		{
			get
			{
				return this.clientSideSearchTimeout;
			}
			set
			{
				this.clientSideSearchTimeout = value;
			}
		}

		ConfigScopes IDirectorySession.ConfigScope
		{
			get
			{
				return this.configScope;
			}
		}

		ConsistencyMode IDirectorySession.ConsistencyMode
		{
			get
			{
				return this.consistencyMode;
			}
		}

		string IDirectorySession.DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		bool IDirectorySession.EnforceContainerizedScoping
		{
			get
			{
				return this.enforceContainerizedScoping;
			}
			set
			{
				this.enforceContainerizedScoping = value;
			}
		}

		bool IDirectorySession.EnforceDefaultScope
		{
			get
			{
				return this.enforceDefaultScope;
			}
			set
			{
				this.enforceDefaultScope = value;
			}
		}

		string IDirectorySession.LastUsedDc
		{
			get
			{
				return ((IDirectorySession)this).ServerSettings.LastUsedDc(((IDirectorySession)this).SessionSettings.GetAccountOrResourceForestFqdn());
			}
		}

		int IDirectorySession.Lcid
		{
			get
			{
				return this.lcid;
			}
		}

		string IDirectorySession.LinkResolutionServer
		{
			get
			{
				return this.linkResolutionServer;
			}
			set
			{
				this.linkResolutionServer = value;
			}
		}

		bool IDirectorySession.LogSizeLimitExceededEvent
		{
			get
			{
				return this.logSizeLimitExceededEvent;
			}
			set
			{
				this.logSizeLimitExceededEvent = value;
			}
		}

		NetworkCredential IDirectorySession.NetworkCredential
		{
			get
			{
				return this.networkCredential;
			}
		}

		bool IDirectorySession.ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		ADServerSettings IDirectorySession.ServerSettings
		{
			get
			{
				return this.sessionSettings.ServerSettings;
			}
		}

		TimeSpan? IDirectorySession.ServerTimeout
		{
			get
			{
				return this.serverTimeout;
			}
			set
			{
				this.serverTimeout = value;
			}
		}

		ADSessionSettings IDirectorySession.SessionSettings
		{
			get
			{
				return this.sessionSettings;
			}
		}

		bool IDirectorySession.SkipRangedAttributes
		{
			get
			{
				return this.skipRangedAttributes;
			}
			set
			{
				this.skipRangedAttributes = value;
			}
		}

		string[] IDirectorySession.ExclusiveLdapAttributes
		{
			get
			{
				return this.exclusiveLdapAttributes;
			}
			set
			{
				this.exclusiveLdapAttributes = value;
			}
		}

		bool IDirectorySession.UseConfigNC
		{
			get
			{
				return this.useConfigNC;
			}
			set
			{
				this.useConfigNC = value;
			}
		}

		bool IDirectorySession.UseGlobalCatalog
		{
			get
			{
				return this.useGlobalCatalog;
			}
			set
			{
				this.useGlobalCatalog = value;
			}
		}

		protected static void LogNotSupportedInFFO(Exception ex = null)
		{
			EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_UnsupportedFFOAPICalled, null, new object[]
			{
				(ex != null) ? ex.ToString() : Environment.StackTrace
			});
		}

		public static void FixDistinguishedName(ADObject adObject, string tenantDistinguishedName, Guid tenantGuid, Guid objectGuid, ADObjectId relativeConfigDN = null)
		{
			if (adObject == null || string.IsNullOrEmpty(tenantDistinguishedName))
			{
				return;
			}
			if (tenantGuid == Guid.Empty || objectGuid == Guid.Empty)
			{
				throw new InvalidOperationException(string.Format("Unable to fix distinguished name for ADObject = {0}, TenantGuid = {1}, ObjectGuid = {2}, objectName = {3}.", new object[]
				{
					adObject.GetType().Name,
					tenantGuid,
					objectGuid,
					adObject.Name
				}));
			}
			string unescapedCommonName = string.IsNullOrEmpty(adObject.Name) ? objectGuid.ToString() : adObject.Name;
			ADObjectId adobjectId = new ADObjectId(tenantDistinguishedName, tenantGuid);
			ADObjectId adobjectId2 = new ADObjectId(adobjectId.GetChildId("Configuration").DistinguishedName, tenantGuid);
			if (relativeConfigDN != null)
			{
				adobjectId2 = new ADObjectId(adobjectId2.GetDescendantId(relativeConfigDN).DistinguishedName, tenantGuid);
			}
			ADObjectId id = new ADObjectId(adobjectId2.GetChildId(unescapedCommonName).DistinguishedName, objectGuid);
			ADObjectId adobjectId3 = (ADObjectId)adObject[ADObjectSchema.ConfigurationUnit];
			if (adobjectId3 != null && adobjectId3.Name != null && string.Equals(adobjectId3.Name, adobjectId2.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			adObject[ADObjectSchema.OrganizationalUnitRoot] = adobjectId;
			adObject[ADObjectSchema.ConfigurationUnit] = adobjectId2;
			adObject.SetId(id);
			FfoDirectorySession.FixLegacyExchangeDN(adObject, tenantGuid);
			FfoDirectorySession.FixDistinguishedNameForADObjectIDs(adObject, tenantDistinguishedName);
		}

		public static ADObjectId GetUpdatedADObjectIdWithDN(ADObjectId adObject, string tenantDistinguishedName, Guid tenantGuid, ADObjectId relativeConfigDN = null)
		{
			if (adObject == null || string.IsNullOrEmpty(tenantDistinguishedName))
			{
				return null;
			}
			if (tenantGuid == Guid.Empty || adObject.ObjectGuid == Guid.Empty)
			{
				throw new InvalidOperationException(string.Format("Unable to fix distinguished name for ADObject = {0}, TenantGuid = {1}, ObjectGuid = {2}, objectName = {3}.", new object[]
				{
					adObject.GetType().Name,
					tenantGuid,
					adObject.ObjectGuid,
					adObject.Name
				}));
			}
			string unescapedCommonName = string.IsNullOrEmpty(adObject.Name) ? adObject.ObjectGuid.ToString() : adObject.Name;
			ADObjectId adobjectId = new ADObjectId(tenantDistinguishedName, tenantGuid);
			ADObjectId adobjectId2 = new ADObjectId(adobjectId.GetChildId("Configuration").DistinguishedName, tenantGuid);
			if (relativeConfigDN != null)
			{
				adobjectId2 = new ADObjectId(adobjectId2.GetDescendantId(relativeConfigDN).DistinguishedName, tenantGuid);
			}
			return new ADObjectId(adobjectId2.GetChildId(unescapedCommonName).DistinguishedName, adObject.ObjectGuid);
		}

		public static bool TryGetRoleGroupInfo(RoleGroup.RoleGroupTypeIds typeId, out RoleGroupInitInfo roleGroupInfo)
		{
			roleGroupInfo = FfoDirectorySession.SupportedRoleGroups.FirstOrDefault((RoleGroupInitInfo rg) => rg.Id == (int)typeId);
			return roleGroupInfo.WellKnownGuid != Guid.Empty;
		}

		private static void FixLegacyExchangeDN(ADObject adObject, Guid tenantGuid)
		{
			ADObjectId id = adObject.Id;
			if (adObject is ADRecipient || adObject is MiniRecipient)
			{
				adObject[ADRecipientSchema.LegacyExchangeDN] = id.DistinguishedName;
			}
			if (adObject is Organization || adObject is ADOrganizationalUnit || adObject is ExchangeConfigurationUnit)
			{
				adObject[OrganizationSchema.LegacyExchangeDN] = string.Format("/o={0}/ou={1}/cn={2}", tenantGuid.ToString(), tenantGuid.ToString(), id.ObjectGuid.ToString());
			}
		}

		private static void FixDistinguishedNameForADObjectIDs(ADObject adObject, string tenantDistinguishedName)
		{
			string[] stdIDs = new string[]
			{
				ADObjectSchema.Id.Name,
				ADObjectSchema.OrganizationalUnitRoot.Name,
				ADObjectSchema.ConfigurationUnit.Name
			};
			IEnumerable<PropertyDefinition> enumerable = adObject.Schema.AllProperties;
			enumerable = from propertyDefinition in enumerable
			where propertyDefinition != null
			select propertyDefinition;
			enumerable = from propertyDefinition in enumerable
			where propertyDefinition.Type == typeof(ADObjectId)
			select propertyDefinition;
			enumerable = from propertyDefinition in enumerable
			where propertyDefinition is ProviderPropertyDefinition && !((ProviderPropertyDefinition)propertyDefinition).IsReadOnly
			select propertyDefinition;
			enumerable = from propertyDefinition in enumerable
			where !stdIDs.Contains(propertyDefinition.Name)
			select propertyDefinition;
			foreach (PropertyDefinition propertyDefinition2 in enumerable)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition2;
				object obj;
				if (adObject.TryGetValueWithoutDefault(providerPropertyDefinition, out obj) && obj != null)
				{
					if (providerPropertyDefinition.IsMultivalued)
					{
						MultiValuedProperty<ADObjectId> multiValuedProperty = obj as MultiValuedProperty<ADObjectId>;
						int num = 0;
						while (multiValuedProperty != null)
						{
							if (num >= multiValuedProperty.Count)
							{
								break;
							}
							ADObjectId adObjectId = multiValuedProperty[num];
							multiValuedProperty[num] = new ADObjectId(tenantDistinguishedName, Guid.Empty);
							multiValuedProperty[num] = FfoDirectorySession.ReplaceTenantDistinguishedName(tenantDistinguishedName, adObjectId);
							num++;
						}
					}
					else
					{
						ADObjectId adobjectId = obj as ADObjectId;
						if (adobjectId != null)
						{
							adObject[providerPropertyDefinition] = FfoDirectorySession.ReplaceTenantDistinguishedName(tenantDistinguishedName, adobjectId);
						}
					}
				}
			}
		}

		private static ADObjectId ReplaceTenantDistinguishedName(string tenantDistinguishedName, ADObjectId adObjectId)
		{
			return new ADObjectId(adObjectId.DistinguishedName.Replace(DalHelper.GetTenantDistinguishedName("TenantName"), tenantDistinguishedName), adObjectId.ObjectGuid);
		}

		protected void FixOrganizationalUnitRoot(IConfigurable configurable)
		{
			ADObject adobject = configurable as ADObject;
			if (adobject != null)
			{
				if (adobject.OrganizationalUnitRoot == null)
				{
					adobject[ADObjectSchema.OrganizationalUnitRoot] = this.TenantId;
					return;
				}
			}
			else
			{
				ConfigurablePropertyBag configurablePropertyBag = configurable as ConfigurablePropertyBag;
				if (configurablePropertyBag != null)
				{
					configurablePropertyBag[ADObjectSchema.OrganizationalUnitRoot] = this.TenantId;
					return;
				}
				ConfigurableObject configurableObject = configurable as ConfigurableObject;
				if (configurableObject != null)
				{
					configurableObject[ADObjectSchema.OrganizationalUnitRoot] = this.TenantId;
				}
			}
		}

		protected void GenerateIdForObject(IConfigurable configurable)
		{
			ADObject adobject = configurable as ADObject;
			if (adobject != null)
			{
				if (adobject.Id != null && adobject.Id.ObjectGuid == Guid.Empty && !(adobject is ADConfigurationObject))
				{
					adobject.SetId(new ADObjectId(adobject.Id.DistinguishedName, CombGuidGenerator.NewGuid()));
				}
				if ((adobject is TransportRule || adobject is Container) && adobject.Id != null && string.IsNullOrEmpty(adobject.Name))
				{
					adobject.Name = (adobject.Id.Name ?? adobject.Id.ObjectGuid.ToString());
					return;
				}
			}
			else
			{
				ConfigurablePropertyBag configurablePropertyBag = configurable as ConfigurablePropertyBag;
				if (configurablePropertyBag != null)
				{
					if (configurablePropertyBag is UserCertificate)
					{
						configurablePropertyBag[UserCertificate.UserCertificateIdProp] = CombGuidGenerator.NewGuid();
						configurablePropertyBag[UserCertificate.CertificateIdProp] = CombGuidGenerator.NewGuid();
					}
					if (configurablePropertyBag is PartnerCertificate)
					{
						configurablePropertyBag[PartnerCertificate.PartnerCertificateIdDef] = CombGuidGenerator.NewGuid();
						configurablePropertyBag[PartnerCertificate.PartnerIdDef] = CombGuidGenerator.NewGuid();
						configurablePropertyBag[PartnerCertificate.CertificateIdDef] = CombGuidGenerator.NewGuid();
						return;
					}
				}
				else
				{
					ConfigurableObject configurableObject = configurable as ConfigurableObject;
					if (configurableObject != null)
					{
						return;
					}
					throw new ArgumentException("IConfigurable object must be driven from ADObject, ConfigurableObject, or ConfigurablePropertyBag");
				}
			}
		}

		protected ADObjectId GenerateLocalIdentifier()
		{
			return new ADObjectId(CombGuidGenerator.NewGuid());
		}

		protected IConfigurable[] FindTenantObject<T>(params object[] propNameValues) where T : IConfigurable, new()
		{
			QueryFilter[] array = new QueryFilter[propNameValues.Length / 2];
			for (int i = 0; i < propNameValues.Length; i += 2)
			{
				array[i / 2] = new ComparisonFilter(ComparisonOperator.Equal, (PropertyDefinition)propNameValues[i], propNameValues[i + 1]);
			}
			QueryFilter filter = QueryFilter.AndTogether(array);
			IConfigurable[] array2 = this.FindAndHandleException<T>(filter, null, false, null, int.MaxValue).Cast<IConfigurable>().ToArray<IConfigurable>();
			IConfigurable[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				T t = (T)((object)array3[j]);
				ADObject adobject = t as ADObject;
				if (adobject != null)
				{
					FfoDirectorySession.FixDistinguishedName(adobject, this.TenantId.DistinguishedName, this.TenantId.ObjectGuid, ((ADObjectId)adobject.Identity).ObjectGuid, null);
				}
			}
			return array2;
		}

		protected IEnumerable<T> FindAndHandleException<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize = 2147483647) where T : IConfigurable, new()
		{
			IEnumerable<T> result;
			try
			{
				IEnumerable<T> enumerable;
				if (pageSize == 2147483647)
				{
					enumerable = this.DataProvider.Find<T>(this.AddTenantIdFilter(filter), rootId, deepSearch, sortBy).Cast<T>();
				}
				else
				{
					enumerable = this.DataProvider.FindPaged<T>(this.AddTenantIdFilter(filter), rootId, deepSearch, sortBy, pageSize);
				}
				result = enumerable;
			}
			catch (DataProviderMappingException ex)
			{
				FfoDirectorySession.LogNotSupportedInFFO(ex);
				result = new T[0];
			}
			return result;
		}

		protected T ReadAndHandleException<T>(QueryFilter filter) where T : IConfigurable, new()
		{
			try
			{
				IConfigurable[] source = this.DataProvider.Find<T>(filter, null, false, null);
				return (T)((object)source.FirstOrDefault<IConfigurable>());
			}
			catch (DataProviderMappingException ex)
			{
				FfoDirectorySession.LogNotSupportedInFFO(ex);
			}
			return default(T);
		}

		protected QueryFilter AddTenantIdFilter(QueryFilter filter)
		{
			if (this.TenantId == null)
			{
				return filter;
			}
			ADObjectId extractedId = null;
			bool orgFilterAlreadyPresent = false;
			if (filter != null)
			{
				DalHelper.ForEachProperty(filter, delegate(PropertyDefinition propertyDefinition, object value)
				{
					if (propertyDefinition == ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId && value != null)
					{
						extractedId = new ADObjectId(DalHelper.GetTenantDistinguishedName(value.ToString()), Guid.Parse(value.ToString()));
						return;
					}
					if (propertyDefinition == ADObjectSchema.OrganizationalUnitRoot && value != null)
					{
						orgFilterAlreadyPresent = true;
					}
				});
			}
			if (!orgFilterAlreadyPresent)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, extractedId ?? this.TenantId);
				filter = ((filter == null) ? queryFilter : QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					queryFilter
				}));
			}
			return filter;
		}

		protected T GetDefaultObject<T>()
		{
			Func<object> func;
			if (!FfoDirectorySession.defaultInstanceFactory.TryGetValue(typeof(T), out func))
			{
				return default(T);
			}
			T t = (T)((object)func());
			ADConfigurationObject adconfigurationObject = t as ADConfigurationObject;
			if (adconfigurationObject != null)
			{
				adconfigurationObject.Name = "Default";
				adconfigurationObject.SetId(new ADObjectId("CN=Default", FfoDirectorySession.defaultObjectId));
			}
			return t;
		}

		protected T[] GetDefaultArray<T>()
		{
			object obj = this.GetDefaultObject<T>();
			if (obj == null)
			{
				return new T[0];
			}
			return new T[]
			{
				(T)((object)obj)
			};
		}

		protected void ApplyAuditProperties(IConfigurable configurable)
		{
			if (this.sessionSettings == null || string.IsNullOrEmpty(this.sessionSettings.ExecutingUserIdentityName))
			{
				EventLogger.Logger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_AuditUserIdentityMissing, null, new object[]
				{
					Environment.StackTrace
				});
				return;
			}
			AuditHelper.ApplyAuditProperties(configurable as IPropertyBag, Guid.NewGuid(), this.sessionSettings.ExecutingUserIdentityName);
		}

		protected ADObjectId ExtractTenantId(ADObjectId tenantDescendantId)
		{
			if (tenantDescendantId == null)
			{
				return null;
			}
			int num = DalHelper.FfoRootDN.Depth + 1;
			FfoTenant ffoTenant = null;
			if (string.IsNullOrEmpty(tenantDescendantId.DistinguishedName) && tenantDescendantId.ObjectGuid != Guid.Empty)
			{
				ffoTenant = this.ReadAndHandleException<FfoTenant>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, new ADObjectId(tenantDescendantId.ObjectGuid)));
				if (ffoTenant != null)
				{
					string tenantDistinguishedName = DalHelper.GetTenantDistinguishedName(ffoTenant.Name);
					return new ADObjectId(tenantDistinguishedName, tenantDescendantId.ObjectGuid);
				}
			}
			if (tenantDescendantId.Depth <= num)
			{
				return tenantDescendantId;
			}
			ADObjectId adobjectId = tenantDescendantId.AncestorDN(tenantDescendantId.Depth - num);
			string name = adobjectId.Name;
			Guid guid;
			if (Guid.TryParse(name, out guid))
			{
				ffoTenant = this.ReadAndHandleException<FfoTenant>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, new ADObjectId(guid)));
			}
			if (ffoTenant == null && tenantDescendantId.ObjectGuid == Guid.Empty)
			{
				GlobalConfigSession globalConfigSession = new GlobalConfigSession();
				ffoTenant = globalConfigSession.GetTenantByName(name);
			}
			if (ffoTenant == null)
			{
				return tenantDescendantId;
			}
			return ffoTenant.OrganizationalUnitRoot;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FfoDirectorySession()
		{
			Dictionary<Type, Func<object>> dictionary = new Dictionary<Type, Func<object>>();
			dictionary.Add(typeof(TransportConfigContainer), () => new TransportConfigContainer());
			dictionary.Add(typeof(PerimeterConfig), () => new PerimeterConfig());
			dictionary.Add(typeof(DomainContentConfig), () => new DomainContentConfig
			{
				DomainName = SmtpDomainWithSubdomains.StarDomain,
				IsInternal = false,
				TargetDeliveryDomain = false,
				CharacterSet = "iso-8859-1",
				NonMimeCharacterSet = "iso-8859-1",
				AllowedOOFType = AllowedOOFType.External,
				AutoReplyEnabled = true,
				AutoForwardEnabled = true,
				DeliveryReportEnabled = true,
				NDREnabled = true,
				MeetingForwardNotificationEnabled = false,
				ContentType = ContentType.MimeText,
				DisplaySenderName = true,
				PreferredInternetCodePageForShiftJis = PreferredInternetCodePageForShiftJisEnum.Undefined,
				RequiredCharsetCoverage = null,
				TNEFEnabled = null,
				LineWrapSize = Unlimited<int>.UnlimitedValue,
				TrustedMailOutboundEnabled = false,
				TrustedMailInboundEnabled = false,
				UseSimpleDisplayName = false,
				NDRDiagnosticInfoEnabled = true,
				MessageCountThreshold = int.MaxValue
			});
			dictionary.Add(typeof(AdminAuditLogConfig), () => new AdminAuditLogConfig
			{
				AdminAuditLogEnabled = true,
				AdminAuditLogCmdlets = new MultiValuedProperty<string>(new string[]
				{
					"*"
				}),
				AdminAuditLogParameters = new MultiValuedProperty<string>(new string[]
				{
					"*"
				}),
				CaptureDetailsEnabled = true
			});
			FfoDirectorySession.defaultInstanceFactory = dictionary;
			FfoDirectorySession.SupportedRoleGroups = new RoleGroupInitInfo[]
			{
				RoleGroup.ComplianceManagement_InitInfo,
				RoleGroup.HygieneManagement_InitInfo,
				RoleGroup.HelpDesk_InitInfo,
				RoleGroup.RecipientManagement_InitInfo,
				RoleGroup.RecordsManagement_InitInfo,
				RoleGroup.OrganizationManagement_InitInfo,
				RoleGroup.ViewOnlyOrganizationManagement_InitInfo
			};
			FfoDirectorySession.defaultObjectId = new Guid("08075A1F-B49E-4769-983D-BE2587651F3B");
		}

		private static readonly Dictionary<Type, Func<object>> defaultInstanceFactory;

		private static readonly RoleGroupInitInfo[] SupportedRoleGroups;

		private static readonly Guid defaultObjectId;

		private readonly IConfigDataProvider dataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);

		private readonly ConsistencyMode consistencyMode;

		private readonly bool readOnly;

		private readonly NetworkCredential networkCredential;

		private readonly ADSessionSettings sessionSettings;

		private readonly int lcid;

		private ConfigScopes configScope;

		private string domainController;

		private bool useGlobalCatalog;

		private bool enforceDefaultScope;

		private bool useConfigNC;

		private bool enforceContainerizedScoping;

		private TimeSpan? clientSideSearchTimeout;

		private bool skipRangedAttributes;

		private string[] exclusiveLdapAttributes;

		private TimeSpan? serverTimeout;

		private bool logSizeLimitExceededEvent;

		private string linkResolutionServer;

		private ADObjectId tenantId;
	}
}
