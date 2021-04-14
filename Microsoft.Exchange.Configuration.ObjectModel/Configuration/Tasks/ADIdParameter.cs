using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class ADIdParameter : IIdentityParameter
	{
		protected ADIdParameter()
		{
		}

		protected ADIdParameter(ADObjectId adObjectId)
		{
			this.Initialize(adObjectId);
		}

		protected ADIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.displayName = this.TryResolveRedactedPii(namedIdentity.DisplayName);
		}

		protected ADIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			identity = this.TryResolveRedactedPii(identity);
			ADObjectId adobjectId;
			if (ADObjectId.TryParseDnOrGuid(identity, out adobjectId) && !adobjectId.IsRelativeDn)
			{
				this.Initialize(adobjectId);
			}
			this.rawIdentity = identity;
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		internal ADObjectId InternalADObjectId
		{
			get
			{
				return this.adObjectId;
			}
		}

		internal string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		internal bool HasRedactedPiiData
		{
			get
			{
				return this.hasRedactedPiiData;
			}
			private set
			{
				this.hasRedactedPiiData = value;
			}
		}

		internal bool IsRedactedPiiResolved
		{
			get
			{
				return this.isRedactedPiiResolved;
			}
			private set
			{
				this.isRedactedPiiResolved = value;
			}
		}

		internal string OriginalRedactedPiiData
		{
			get
			{
				return this.originalRedactedPiiData;
			}
			private set
			{
				this.originalRedactedPiiData = value;
			}
		}

		protected virtual QueryFilter AdditionalQueryFilter
		{
			get
			{
				return null;
			}
		}

		internal virtual ADPropertyDefinition[] AdditionalMatchingProperties
		{
			get
			{
				return null;
			}
		}

		protected virtual SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		private static string LocalForestDomainNamingContext
		{
			get
			{
				if (ADIdParameter.localForestDomainNamingContext == null)
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 243, "LocalForestDomainNamingContext", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
					ADIdParameter.localForestDomainNamingContext = topologyConfigurationSession.GetRootDomainNamingContextFromCurrentReadConnection();
				}
				return ADIdParameter.localForestDomainNamingContext;
			}
		}

		public static explicit operator string(ADIdParameter adIdParameter)
		{
			if (adIdParameter != null)
			{
				return adIdParameter.ToString();
			}
			return null;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.OriginalRedactedPiiData))
			{
				return this.OriginalRedactedPiiData;
			}
			if (!string.IsNullOrEmpty(this.displayName))
			{
				return this.displayName;
			}
			if (this.InternalADObjectId != null && !string.IsNullOrEmpty(this.InternalADObjectId.DistinguishedName))
			{
				return this.InternalADObjectId.ToString();
			}
			return this.RawIdentity ?? string.Empty;
		}

		protected OrganizationId GetOrganizationId(OrganizationId currentOrgId, string orgName)
		{
			if (OrganizationId.ForestWideOrgId.Equals(currentOrgId) && !string.IsNullOrEmpty(orgName))
			{
				if (TemplateTenantConfiguration.IsTemplateTenantName(orgName) && TemplateTenantConfiguration.GetLocalTemplateTenant() != null)
				{
					return TemplateTenantConfiguration.GetLocalTemplateTenant().OrganizationId;
				}
				ExchangeConfigurationUnit configurationUnit = this.GetConfigurationUnit(orgName);
				if (configurationUnit != null)
				{
					if (this.MustScopeToSharedConfiguration(configurationUnit))
					{
						SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(configurationUnit.OrganizationId);
						if (sharedConfiguration != null)
						{
							return sharedConfiguration.SharedConfigurationCU.OrganizationId;
						}
					}
					return configurationUnit.OrganizationId;
				}
			}
			return currentOrgId;
		}

		protected OrganizationId GetOrganizationId(OrganizationId currentOrgId, ADObjectId id)
		{
			if (OrganizationId.ForestWideOrgId.Equals(currentOrgId) && id != null)
			{
				if (id.Parent != null && TemplateTenantConfiguration.IsTemplateTenantName(id.Parent.Name) && TemplateTenantConfiguration.GetLocalTemplateTenant() != null)
				{
					return TemplateTenantConfiguration.GetLocalTemplateTenant().OrganizationId;
				}
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(id), 350, "GetOrganizationId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
				ADRawEntry adrawEntry = tenantOrTopologyConfigurationSession.ReadADRawEntry(id, new ADPropertyDefinition[]
				{
					ADObjectSchema.OrganizationId
				});
				if (adrawEntry != null)
				{
					return (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(id), 365, "GetOrganizationId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
				adrawEntry = tenantOrRootOrgRecipientSession.ReadADRawEntry(id, new ADPropertyDefinition[]
				{
					ADObjectSchema.OrganizationId
				});
				if (adrawEntry != null)
				{
					return (OrganizationId)adrawEntry[ADObjectSchema.OrganizationId];
				}
			}
			return currentOrgId;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (!(session is IDirectorySession))
			{
				throw new ArgumentException("Session should be an IDirectorySession", "session");
			}
			if (rootId != null && !(rootId is ADObjectId))
			{
				throw new ArgumentException("RootId must be an ADObjectId", "rootId");
			}
			IDirectorySession directorySession = (IDirectorySession)session;
			IDirectorySession directorySession2 = null;
			if (!(this is OrganizationIdParameter) && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && rootId == null && ADSessionSettings.GetProcessServerSettings() == null && directorySession is IConfigurationSession)
			{
				IConfigurationSession configurationSession = directorySession as IConfigurationSession;
				ADObjectId configurationUnitsRoot = directorySession.GetConfigurationUnitsRoot();
				bool flag = !string.IsNullOrEmpty(this.rawIdentity) && this.rawIdentity.IndexOf("\\") != -1;
				if (this.InternalADObjectId != null)
				{
					flag = !string.IsNullOrEmpty(this.InternalADObjectId.DistinguishedName);
				}
				if (!flag && configurationSession.UseConfigNC && !configurationUnitsRoot.IsDescendantOf(directorySession.GetConfigurationNamingContext()) && typeof(ADConfigurationObject).IsAssignableFrom(typeof(T)) && !typeof(ADNonExchangeObject).IsAssignableFrom(typeof(T)))
				{
					T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
					ADObject adobject = t as ADObject;
					ObjectScopeAttribute objectScopeAttribute;
					bool flag2 = adobject.IsApplicableToTenant(out objectScopeAttribute);
					if (directorySession.SessionSettings.ExecutingUserOrganizationId.Equals(OrganizationId.ForestWideOrgId) && directorySession.SessionSettings.CurrentOrganizationId.Equals(directorySession.SessionSettings.ExecutingUserOrganizationId) && flag2)
					{
						directorySession2 = directorySession;
					}
				}
			}
			if (directorySession2 == null)
			{
				directorySession2 = ADSession.RescopeSessionToTenantSubTree(directorySession);
			}
			return this.GetObjects<T>((ADObjectId)rootId, directorySession, directorySession2, optionalData, out notFoundReason);
		}

		internal virtual void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			ADObjectId adobjectId = objectId as ADObjectId;
			if (adobjectId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(ADObjectId).Name), "objectId");
			}
			if (this.InternalADObjectId != null)
			{
				throw new InvalidOperationException(Strings.ErrorChangeImmutableType);
			}
			if (string.IsNullOrEmpty(adobjectId.DistinguishedName) && adobjectId.ObjectGuid == Guid.Empty)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("objectId"), "objectId");
			}
			if (adobjectId.IsRelativeDn)
			{
				throw new ArgumentException(Strings.ErrorRelativeDn(adobjectId.ToDNString()), "objectId");
			}
			if (PiiMapManager.ContainsRedactedPiiValue(adobjectId.DistinguishedName))
			{
				string distinguishedName = this.TryResolveRedactedPii(adobjectId.DistinguishedName);
				if (this.IsRedactedPiiResolved)
				{
					adobjectId = new ADObjectId(distinguishedName, adobjectId.ObjectGuid, adobjectId.PartitionGuid);
				}
			}
			this.adObjectId = adobjectId;
			this.rawIdentity = adobjectId.ToDNString();
		}

		internal virtual IEnumerableFilter<T> GetEnumerableFilter<T>() where T : IConfigurable, new()
		{
			return null;
		}

		internal virtual IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (subTreeSession == null)
			{
				throw new ArgumentNullException("subTreeSession");
			}
			notFoundReason = null;
			EnumerableWrapper<T> enumerableWrapper = null;
			if (string.IsNullOrEmpty(this.RawIdentity))
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			enumerableWrapper = this.GetEnumerableWrapper<T>(enumerableWrapper, this.GetExactMatchObjects<T>(rootId, subTreeSession, optionalData));
			if (!enumerableWrapper.HasElements() && VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				enumerableWrapper = this.GetEnumerableWrapper<T>(enumerableWrapper, this.GetMultitenancyObjects<T>(this.RawIdentity, rootId, session, optionalData, out notFoundReason));
			}
			if (!enumerableWrapper.HasElements())
			{
				enumerableWrapper = this.GetEnumerableWrapper<T>(enumerableWrapper, this.GetObjectsInOrganization<T>(this.RawIdentity, rootId, session, optionalData));
			}
			if (!enumerableWrapper.HasElements() && !string.IsNullOrEmpty(this.displayName) && this.displayName != this.RawIdentity)
			{
				enumerableWrapper = this.GetEnumerableWrapper<T>(enumerableWrapper, this.GetObjectsInOrganization<T>(this.displayName, rootId, session, optionalData));
			}
			return enumerableWrapper;
		}

		internal EnumerableWrapper<T> GetEnumerableWrapper<T>(EnumerableWrapper<T> noElementsValue, IEnumerable<T> collection) where T : IConfigurable, new()
		{
			EnumerableWrapper<T> result = noElementsValue ?? EnumerableWrapper<T>.Empty;
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(collection, this.GetEnumerableFilter<T>());
			if (wrapper.HasUnfilteredElements())
			{
				result = wrapper;
			}
			return result;
		}

		internal IEnumerable<T> GetExactMatchObjects<T>(ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData) where T : IConfigurable, new()
		{
			EnumerableWrapper<T> enumerableWrapper = EnumerableWrapper<T>.Empty;
			ADObjectId identity;
			if (this.InternalADObjectId != null)
			{
				enumerableWrapper = EnumerableWrapper<T>.GetWrapper(this.GetADObjectIdObjects<T>(this.InternalADObjectId, rootId, session, optionalData));
			}
			else if (ADIdParameter.TryResolveCanonicalName(this.RawIdentity, out identity))
			{
				enumerableWrapper = EnumerableWrapper<T>.GetWrapper(this.GetADObjectIdObjects<T>(identity, rootId, session, optionalData));
				if (enumerableWrapper.HasElements())
				{
					this.UpdateInternalADObjectId(identity);
				}
			}
			return enumerableWrapper;
		}

		internal IEnumerable<T> GetADObjectIdObjects<T>(ADObjectId identity, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData) where T : IConfigurable, new()
		{
			if (identity != null)
			{
				OrganizationId organizationId;
				if (this.InternalADObjectId != null && this.InternalADObjectId.Equals(identity) && this.orgIdResolved)
				{
					organizationId = this.resolvedOrganizationId;
				}
				else
				{
					organizationId = this.GetOrganizationId(session.SessionSettings.CurrentOrganizationId, identity);
				}
				IDirectorySession directorySession = session;
				if (organizationId != null)
				{
					directorySession = TaskHelper.UnderscopeSessionToOrganization(session, organizationId, true);
				}
				if (session.ConfigScope == ConfigScopes.TenantSubTree)
				{
					directorySession = ADSession.RescopeSessionToTenantSubTree(directorySession);
				}
				if (directorySession.IsRootIdWithinScope<T>(rootId))
				{
					if (ADObjectId.Equals(identity, identity.DomainId) && !typeof(OrganizationalUnitIdParameterBase).IsAssignableFrom(base.GetType()))
					{
						if (!typeof(ADRawEntryIdParameter).IsAssignableFrom(base.GetType()))
						{
							goto IL_15F;
						}
					}
					try
					{
						ADObjectId rootId2 = rootId;
						bool enforceContainerizedScoping = directorySession.EnforceContainerizedScoping;
						bool flag = directorySession is IRecipientSession;
						if (rootId == null && !string.IsNullOrEmpty(identity.DistinguishedName))
						{
							if (!ADObjectId.Equals(identity, identity.DomainId) && directorySession.IsRootIdWithinScope<T>(identity.Parent))
							{
								rootId2 = identity.Parent;
							}
							else if (directorySession.IsRootIdWithinScope<T>(identity))
							{
								rootId2 = identity;
								if (flag)
								{
									directorySession.EnforceContainerizedScoping = false;
								}
							}
						}
						try
						{
							EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(this.PerformPrimarySearch<T>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, identity), rootId2, directorySession, true, optionalData));
							if (wrapper.HasElements())
							{
								return wrapper;
							}
						}
						finally
						{
							if (flag)
							{
								directorySession.EnforceContainerizedScoping = enforceContainerizedScoping;
							}
						}
					}
					catch (LocalizedException exception)
					{
						if (!TaskHelper.IsTaskKnownException(exception))
						{
							throw;
						}
					}
					IL_15F:
					if (identity.ObjectGuid != Guid.Empty)
					{
						return this.PerformPrimarySearch<T>(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, identity.ObjectGuid), rootId, directorySession, true, optionalData);
					}
				}
			}
			return EnumerableWrapper<T>.Empty;
		}

		internal ExchangeConfigurationUnit GetConfigurationUnit(string orgName)
		{
			if (string.IsNullOrEmpty(orgName))
			{
				throw new ArgumentException("OrgName must contain a non-empty value", "orgName");
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = null;
			try
			{
				ADSessionSettings adsessionSettings = ADSessionSettings.FromTenantCUName(orgName);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 866, "GetConfigurationUnit", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
				adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
				exchangeConfigurationUnit = tenantConfigurationSession.GetExchangeConfigurationUnitByName(orgName);
			}
			catch (CannotResolveTenantNameException)
			{
			}
			SmtpDomain smtpDomain = null;
			if (exchangeConfigurationUnit == null && SmtpDomain.TryParse(orgName, out smtpDomain))
			{
				try
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(orgName);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 890, "GetConfigurationUnit", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
					exchangeConfigurationUnit = tenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(orgName);
				}
				catch (CannotResolveTenantNameException)
				{
				}
			}
			Guid externalDirectoryOrganizationId;
			if (exchangeConfigurationUnit == null && GuidHelper.TryParseGuid(orgName, out externalDirectoryOrganizationId))
			{
				try
				{
					PartitionId partitionIdByExternalDirectoryOrganizationId = ADAccountPartitionLocator.GetPartitionIdByExternalDirectoryOrganizationId(externalDirectoryOrganizationId);
					ADSessionSettings sessionSettings2 = ADSessionSettings.FromAllTenantsPartitionId(partitionIdByExternalDirectoryOrganizationId);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings2, 911, "GetConfigurationUnit", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, externalDirectoryOrganizationId.ToString());
					ExchangeConfigurationUnit[] array = tenantConfigurationSession.Find<ExchangeConfigurationUnit>(ADSession.GetConfigurationUnitsRoot(partitionIdByExternalDirectoryOrganizationId.ForestFQDN), QueryScope.SubTree, filter, null, 0);
					if (array.Length == 1)
					{
						exchangeConfigurationUnit = array[0];
					}
				}
				catch (CannotResolveExternalDirectoryOrganizationIdException)
				{
				}
			}
			return exchangeConfigurationUnit;
		}

		internal virtual IEnumerable<T> GetObjectsInOrganization<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData) where T : IConfigurable, new()
		{
			if (string.IsNullOrEmpty(identityString))
			{
				throw new ArgumentException("IdentityString must contain a non-empty value", "identityString");
			}
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(this.PerformPrimarySearch<T>(this.GetNameMatchingFilter(identityString, false), rootId, session, true, optionalData));
			if (!wrapper.HasElements() && this.IsWildcardDefined(identityString))
			{
				wrapper = EnumerableWrapper<T>.GetWrapper(this.PerformPrimarySearch<T>(this.GetNameMatchingFilter(identityString, true), rootId, session, true, optionalData));
			}
			return wrapper;
		}

		internal QueryFilter CreateWildcardOrEqualFilter(ADPropertyDefinition schemaProperty, string name)
		{
			if (this.IsWildcardDefined(name))
			{
				return this.CreateWildcardFilter(schemaProperty, name);
			}
			return new ComparisonFilter(ComparisonOperator.Equal, schemaProperty, name);
		}

		internal QueryFilter CreateWildcardFilter(ADPropertyDefinition schemaProperty, string identityString)
		{
			string text = identityString;
			MatchOptions matchOptions = MatchOptions.FullString;
			if (text.StartsWith("*") && text.EndsWith("*"))
			{
				if (text.Length <= 2)
				{
					return null;
				}
				text = text.Substring(1, text.Length - 2);
				matchOptions = MatchOptions.SubString;
			}
			else if (text.EndsWith("*"))
			{
				text = text.Substring(0, text.Length - 1);
				matchOptions = MatchOptions.Prefix;
			}
			else if (text.StartsWith("*"))
			{
				text = text.Substring(1);
				matchOptions = MatchOptions.Suffix;
			}
			return new TextFilter(schemaProperty, text, matchOptions, MatchFlags.IgnoreCase);
		}

		internal bool IsNullScopedSession(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (session.SessionSettings == null)
			{
				throw new ArgumentException("session.SessionSettings should not be null", "session");
			}
			return session.SessionSettings.IsGlobal || session.SessionSettings.RecipientReadScope == null;
		}

		internal IEnumerable<T> PerformPrimarySearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch, OptionalIdentityData optionalData) where T : IConfigurable, new()
		{
			if (rootId != null && rootId.IsRelativeDn)
			{
				throw new ArgumentException("RootId cannot be a relative DN", "rootId");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (optionalData == null)
			{
				optionalData = new OptionalIdentityData();
			}
			if (session.ConfigScope == ConfigScopes.TenantLocal && rootId == null && !this.IsNullScopedSession(session))
			{
				if (optionalData.ConfigurationContainerRdn != null)
				{
					rootId = this.CreateContainerRootId(session, optionalData.ConfigurationContainerRdn);
				}
				else if (optionalData.RootOrgDomainContainerId != null && this.IsForestWideScopedSession(session))
				{
					rootId = optionalData.RootOrgDomainContainerId;
				}
			}
			QueryFilter filter2 = QueryFilter.AndTogether(new QueryFilter[]
			{
				filter,
				this.AdditionalQueryFilter,
				optionalData.AdditionalFilter
			});
			IEnumerable<T> enumerable = this.PerformSearch<T>(filter2, rootId, session, deepSearch);
			return EnumerableWrapper<T>.GetWrapper(enumerable, this.GetEnumerableFilter<T>());
		}

		internal virtual IEnumerable<T> PerformSearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch) where T : IConfigurable, new()
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return ((IConfigDataProvider)session).FindPaged<T>(filter, rootId, deepSearch, null, 0);
		}

		internal virtual OrganizationId ResolveOrganizationIdBasedOnIdentity(OrganizationId executingUserOrgId)
		{
			if (this.orgIdResolved)
			{
				return this.resolvedOrganizationId;
			}
			if (!this.IsMultitenancyEnabled())
			{
				return OrganizationId.ForestWideOrgId;
			}
			if (executingUserOrgId != null && !executingUserOrgId.Equals(OrganizationId.ForestWideOrgId))
			{
				return executingUserOrgId;
			}
			ADObjectId id = null;
			string text;
			string text2;
			if (this.adObjectId != null)
			{
				this.resolvedOrganizationId = this.GetOrganizationId(executingUserOrgId, this.adObjectId);
			}
			else if (ADIdParameter.TryResolveCanonicalName(this.RawIdentity, out id))
			{
				this.resolvedOrganizationId = this.GetOrganizationId(executingUserOrgId, id);
			}
			else if (this.TryParseOrganizationName(out text, out text2))
			{
				if (this.IsWildcardDefined(text))
				{
					this.resolvedOrganizationId = OrganizationId.ForestWideOrgId;
				}
				else
				{
					this.resolvedOrganizationId = this.GetOrganizationId(executingUserOrgId, text);
				}
			}
			else
			{
				this.resolvedOrganizationId = null;
			}
			this.orgIdResolved = true;
			return this.resolvedOrganizationId;
		}

		protected static bool TryResolveCanonicalName(string canonicalName, out ADObjectId adObjectId)
		{
			adObjectId = null;
			SmtpDomain smtpDomain;
			if (!string.IsNullOrEmpty(canonicalName) && SmtpDomain.TryParse(canonicalName.Split(new char[]
			{
				'/'
			})[0], out smtpDomain))
			{
				try
				{
					string distinguishedName = NativeHelpers.DistinguishedNameFromCanonicalName(canonicalName);
					adObjectId = new ADObjectId(distinguishedName);
					return true;
				}
				catch (NameConversionException)
				{
				}
				return false;
			}
			return false;
		}

		protected void UpdateInternalADObjectId(ADObjectId identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			ADObjectId adobjectId = this.adObjectId;
			this.adObjectId = identity;
		}

		protected bool IsMultitenancyEnabled()
		{
			return ADSession.GetRootDomainNamingContextForLocalForest().Equals(ADSession.GetDomainNamingContextForLocalForest()) && !ADSession.IsBoundToAdam;
		}

		protected bool MustScopeToSharedConfiguration(ExchangeConfigurationUnit configUnit)
		{
			return this.SharedTenantConfigurationMode == SharedTenantConfigurationMode.Static || (this.SharedTenantConfigurationMode == SharedTenantConfigurationMode.Dehydrateable && configUnit.IsDehydrated);
		}

		protected virtual bool IsWildcardDefined(string name)
		{
			return name != null && (name.StartsWith("*") || name.EndsWith("*"));
		}

		private bool TryParseOrganizationName(out string organizationName, out string friendlyName)
		{
			string text = this.RawIdentity;
			int num = text.IndexOf('\\');
			organizationName = null;
			friendlyName = null;
			if (num >= 0)
			{
				if (num > 0 && num < text.Length - 1)
				{
					organizationName = text.Substring(0, num);
					friendlyName = text.Substring(num + 1);
					return true;
				}
				return false;
			}
			else
			{
				num = text.IndexOf('@');
				if (num > 0 && num < text.Length - 1)
				{
					organizationName = text.Substring(num + 1);
					friendlyName = text;
					return true;
				}
				return false;
			}
		}

		private IEnumerable<T> GetMultitenancyObjects<T>(string identityString, ADObjectId rootId, IDirectorySession session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (identityString == null)
			{
				throw new ArgumentNullException("rawIdentity");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			notFoundReason = null;
			string text;
			string identityString2;
			if (!this.IsMultitenancyEnabled() || !this.TryParseOrganizationName(out text, out identityString2))
			{
				return EnumerableWrapper<T>.Empty;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(session.DomainController, true, session.ConsistencyMode, session.NetworkCredential, session.SessionSettings, 1403, "GetMultitenancyObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\ADIdParameter.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = true;
			tenantOrTopologyConfigurationSession.LinkResolutionServer = session.LinkResolutionServer;
			if (!string.IsNullOrEmpty(session.DomainController) && tenantOrTopologyConfigurationSession.SessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId) && tenantOrTopologyConfigurationSession.SessionSettings.PartitionId.ForestFQDN != TopologyProvider.LocalForestFqdn)
			{
				return EnumerableWrapper<T>.Empty;
			}
			if (this.IsWildcardDefined(text))
			{
				notFoundReason = new LocalizedString?(Strings.ErrorOrganizationWildcard);
				return EnumerableWrapper<T>.Empty;
			}
			OrganizationId organizationId;
			if (this.orgIdResolved)
			{
				organizationId = this.resolvedOrganizationId;
			}
			else
			{
				organizationId = this.GetOrganizationId(session.SessionSettings.CurrentOrganizationId, text);
			}
			IDirectorySession directorySession = session;
			if (organizationId != null && !OrganizationId.ForestWideOrgId.Equals(organizationId) && session.SessionSettings != null && session.SessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				directorySession = TaskHelper.UnderscopeSessionToOrganization(session, organizationId, true);
			}
			if (organizationId != null && !OrganizationId.ForestWideOrgId.Equals(organizationId) && organizationId.Equals(directorySession.SessionSettings.CurrentOrganizationId))
			{
				IEnumerable<T> objectsInOrganization = this.GetObjectsInOrganization<T>(identityString2, rootId, directorySession, optionalData);
				return EnumerableWrapper<T>.GetWrapper(objectsInOrganization);
			}
			return EnumerableWrapper<T>.Empty;
		}

		private ADObjectId CreateContainerRootId(IDirectorySession session, ADObjectId configRdn)
		{
			if (this.IsForestWideScopedSession(session))
			{
				return session.SessionSettings.RootOrgId.GetDescendantId(configRdn);
			}
			return session.SessionSettings.CurrentOrganizationId.ConfigurationUnit.GetDescendantId(configRdn);
		}

		private bool IsForestWideScopedSession(IDirectorySession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (session.SessionSettings == null)
			{
				throw new ArgumentException("session.SessionSettings should not be null", "session");
			}
			return OrganizationId.ForestWideOrgId.Equals(session.SessionSettings.CurrentOrganizationId);
		}

		private QueryFilter GetNameMatchingFilter(string identityString, bool wildcard)
		{
			QueryFilter queryFilter;
			if (wildcard)
			{
				queryFilter = this.CreateWildcardFilter(ADObjectSchema.Name, identityString);
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, identityString);
			}
			if (this.AdditionalMatchingProperties == null || this.AdditionalMatchingProperties.Length == 0)
			{
				return queryFilter;
			}
			QueryFilter[] array = new QueryFilter[this.AdditionalMatchingProperties.Length];
			for (int i = 0; i < this.AdditionalMatchingProperties.Length; i++)
			{
				array[i] = (wildcard ? this.CreateWildcardFilter(this.AdditionalMatchingProperties[i], identityString) : new ComparisonFilter(ComparisonOperator.Equal, this.AdditionalMatchingProperties[i], identityString));
			}
			QueryFilter queryFilter2 = QueryFilter.OrTogether(array);
			return QueryFilter.OrTogether(new QueryFilter[]
			{
				queryFilter2,
				queryFilter
			});
		}

		private string TryResolveRedactedPii(string value)
		{
			if (PiiMapManager.ContainsRedactedPiiValue(value))
			{
				string text = PiiMapManager.Instance.ResolveRedactedValue(value);
				this.HasRedactedPiiData = true;
				if (text != value)
				{
					this.IsRedactedPiiResolved = true;
					this.OriginalRedactedPiiData = value;
				}
				return text;
			}
			return value;
		}

		internal const char ElementSeparatorChar = '\\';

		private static string localForestDomainNamingContext;

		private ADObjectId adObjectId;

		private string rawIdentity;

		private string displayName;

		private OrganizationId resolvedOrganizationId;

		private bool orgIdResolved;

		[NonSerialized]
		private bool hasRedactedPiiData;

		[NonSerialized]
		private bool isRedactedPiiResolved;

		[NonSerialized]
		private string originalRedactedPiiData;
	}
}
