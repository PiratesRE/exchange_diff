using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	public abstract class GetRecipientBase<TIdentity, TDataObject> : GetRecipientObjectTask<TIdentity, TDataObject> where TIdentity : RecipientIdParameter, new() where TDataObject : ADObject, new()
	{
		[ValidateNotNullOrEmpty]
		[Parameter]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, this.FilterableObjectSchema);
				this.inputFilter = monadFilter.InnerFilter;
				base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		[ValidateLength(3, 5120)]
		[Parameter(ParameterSetName = "AnrSet")]
		public string Anr
		{
			get
			{
				return (string)base.Fields["Anr"];
			}
			set
			{
				if (char.IsWhiteSpace(value[0]))
				{
					throw new ArgumentException(Strings.ErrorStringContainsLeadingSpace(value, "Anr"));
				}
				if (char.IsWhiteSpace(value[value.Length - 1]))
				{
					throw new ArgumentException(Strings.ErrorStringContainsTrailingSpace(value, "Anr"));
				}
				base.Fields["Anr"] = value;
				this.inputFilter = new AmbiguousNameResolutionFilter(value);
			}
		}

		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return base.AccountPartition;
			}
			set
			{
				base.AccountPartition = value;
			}
		}

		[Parameter]
		public string SortBy
		{
			get
			{
				return (string)base.Fields["SortBy"];
			}
			set
			{
				base.Fields["SortBy"] = (string.IsNullOrEmpty(value) ? null : value);
				this.internalSortBy = QueryHelper.GetSortBy(this.SortBy, this.SortProperties);
			}
		}

		[Parameter]
		public OrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (OrganizationalUnitIdParameter)base.Fields["OrganizationalUnit"];
			}
			set
			{
				base.Fields["OrganizationalUnit"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		internal abstract ObjectSchema FilterableObjectSchema { get; }

		protected abstract PropertyDefinition[] SortProperties { get; }

		protected virtual RecipientType[] RecipientTypes
		{
			get
			{
				RecipientIdParameter recipientIdParameter = Activator.CreateInstance<TIdentity>();
				return recipientIdParameter.RecipientTypes;
			}
		}

		protected virtual RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return null;
			}
		}

		protected override SortBy InternalSortBy
		{
			get
			{
				return this.internalSortBy;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = this.ConstructQueryFilterWithCustomFilter(this.inputFilter);
				if (this.UsnForReconciliationSearch >= 0L)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.GreaterThan, ADRecipientSchema.UsnCreated, this.UsnForReconciliationSearch)
					});
				}
				return queryFilter;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected long UsnForReconciliationSearch
		{
			get
			{
				return this.usnForReconciliationSearch;
			}
			set
			{
				this.usnForReconciliationSearch = value;
			}
		}

		public SwitchParameter SoftDeletedObject
		{
			get
			{
				return (SwitchParameter)(base.Fields["SoftDeletedObject"] ?? false);
			}
			set
			{
				base.Fields["SoftDeletedObject"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.Identity != null && this.OrganizationalUnit != null && this.IgnoreDefaultScope)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorIdAndOUSetTogetherUnderIgnoreDefaultScope), ErrorCategory.InvalidArgument, null);
			}
			ExchangeOrganizationalUnit exchangeOrganizationalUnit = null;
			if (this.OrganizationalUnit != null)
			{
				bool useConfigNC = this.ConfigurationSession.UseConfigNC;
				bool useGlobalCatalog = this.ConfigurationSession.UseGlobalCatalog;
				this.ConfigurationSession.UseConfigNC = false;
				if (string.IsNullOrEmpty(this.ConfigurationSession.DomainController))
				{
					this.ConfigurationSession.UseGlobalCatalog = true;
				}
				try
				{
					exchangeOrganizationalUnit = (ExchangeOrganizationalUnit)base.GetDataObject<ExchangeOrganizationalUnit>(this.OrganizationalUnit, this.ConfigurationSession, (base.CurrentOrganizationId != null) ? base.CurrentOrganizationId.OrganizationalUnit : null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.OrganizationalUnit.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.OrganizationalUnit.ToString())));
					RecipientTaskHelper.IsOrgnizationalUnitInOrganization(this.ConfigurationSession, base.CurrentOrganizationId, exchangeOrganizationalUnit, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				}
				finally
				{
					this.ConfigurationSession.UseConfigNC = useConfigNC;
					this.ConfigurationSession.UseGlobalCatalog = useGlobalCatalog;
				}
			}
			if (exchangeOrganizationalUnit != null)
			{
				this.rootId = exchangeOrganizationalUnit.Id;
			}
			else
			{
				this.rootId = ((base.CurrentOrganizationId != null) ? base.CurrentOrganizationId.OrganizationalUnit : null);
			}
			if (this.UsnForReconciliationSearch >= 0L)
			{
				if (base.DomainController == null)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorDomainControllerNotSpecifiedWithUsnForReconciliationSearch), ErrorCategory.InvalidArgument, null);
				}
				base.InternalResultSize = Unlimited<uint>.UnlimitedValue;
				base.OptionalIdentityData.AdditionalFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, ADRecipientSchema.UsnCreated, this.UsnForReconciliationSearch);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			base.CheckExclusiveParameters(new object[]
			{
				"AccountPartition",
				"Organization"
			});
			base.InternalStateReset();
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 410, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\GetRecipientBase.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.IgnoreDefaultScope)
			{
				recipientSession.UseGlobalCatalog = (this.Identity == null && this.OrganizationalUnit == null);
			}
			return recipientSession;
		}

		protected override bool ShouldSkipObject(IConfigurable dataObject)
		{
			if (dataObject is PagedPositionInfo)
			{
				return false;
			}
			RecipientType recipientType;
			RecipientTypeDetails recipientTypeDetails;
			if (dataObject is ReducedRecipient)
			{
				ReducedRecipient reducedRecipient = dataObject as ReducedRecipient;
				recipientType = reducedRecipient.RecipientType;
				recipientTypeDetails = reducedRecipient.RecipientTypeDetails;
			}
			else
			{
				ADRecipient adrecipient = dataObject as ADRecipient;
				recipientType = adrecipient.RecipientType;
				recipientTypeDetails = adrecipient.RecipientTypeDetails;
			}
			return Array.IndexOf<RecipientType>(this.RecipientTypes, recipientType) == -1 || (this.InternalRecipientTypeDetails != null && this.InternalRecipientTypeDetails.Length > 0 && Array.IndexOf<RecipientTypeDetails>(this.InternalRecipientTypeDetails, recipientTypeDetails) == -1);
		}

		protected QueryFilter ConstructQueryFilterWithCustomFilter(QueryFilter customFilter)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			RecipientType[] recipientTypes = this.RecipientTypes;
			List<RecipientTypeDetails> list2 = new List<RecipientTypeDetails>();
			if (this.InternalRecipientTypeDetails != null && this.InternalRecipientTypeDetails.Length > 0)
			{
				foreach (RecipientTypeDetails recipientTypeDetails in this.InternalRecipientTypeDetails)
				{
					RecipientType recipientType = RecipientTaskHelper.RecipientTypeDetailsToRecipientType(recipientTypeDetails);
					if (recipientType != RecipientType.Invalid && Array.IndexOf<RecipientType>(this.RecipientTypes, recipientType) != -1)
					{
						list2.Add(recipientTypeDetails);
					}
					else if (base.IsVerboseOn)
					{
						base.WriteVerbose(Strings.VerboseRecipientTypeDetailsIgnored(recipientTypeDetails.ToString()));
					}
				}
				if (list2.Count == 0)
				{
					base.WriteError(new ArgumentException(Strings.ErrorRecipientTypeDetailsConflictWithRecipientType), ErrorCategory.InvalidArgument, null);
				}
			}
			QueryFilter internalFilter = base.InternalFilter;
			if (internalFilter != null)
			{
				list.Add(internalFilter);
			}
			QueryFilter recipientTypeDetailsFilter = RecipientIdParameter.GetRecipientTypeDetailsFilter(list2.ToArray());
			if (recipientTypeDetailsFilter != null)
			{
				list.Add(recipientTypeDetailsFilter);
			}
			else
			{
				list.Add(RecipientIdParameter.GetRecipientTypeFilter(recipientTypes));
			}
			if (this.Organization != null)
			{
				QueryFilter item = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, base.CurrentOrganizationId.OrganizationalUnit);
				list.Add(item);
			}
			if (customFilter != null)
			{
				list.Add(customFilter);
			}
			if (list.Count != 1)
			{
				return new AndFilter(list.ToArray());
			}
			return list[0];
		}

		protected ADObjectId GetDefaultPolicyId(ADObject user)
		{
			ADObjectId result;
			try
			{
				if (!this.defaultPolicyIds.ContainsKey(user.OrganizationId))
				{
					IConfigurationSession session = this.GenerateIConfigurationSessionForShareableObjects(user.OrganizationId);
					IList<MobileMailboxPolicy> defaultPolicies = DefaultMobileMailboxPolicyUtility<MobileMailboxPolicy>.GetDefaultPolicies(session);
					if (defaultPolicies.Count > 1)
					{
						this.WriteWarning(Strings.MultipleDefaultPoliciesExist(user.OrganizationId.ToString()));
					}
					this.defaultPolicyIds[user.OrganizationId] = ((defaultPolicies.Count > 0) ? defaultPolicies[0].Id : null);
				}
				result = this.defaultPolicyIds[user.OrganizationId];
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
				result = null;
			}
			return result;
		}

		internal ADObjectId GetDefaultRetentionPolicyId(ADObject user, SharedConfiguration sharedConfig)
		{
			if (OrganizationId.ForestWideOrgId.Equals(user.OrganizationId))
			{
				return null;
			}
			bool flag = false;
			if (user[OrgPersonPresentationObjectSchema.RecipientTypeDetails] != null && (RecipientTypeDetails)user[OrgPersonPresentationObjectSchema.RecipientTypeDetails] == RecipientTypeDetails.ArbitrationMailbox)
			{
				flag = true;
			}
			OrganizationId organizationId = user.OrganizationId;
			ADObjectId adobjectId = null;
			if (!flag)
			{
				if (this.defaultRetentionPolicyIds.TryGetValue(organizationId, out adobjectId))
				{
					return adobjectId;
				}
			}
			try
			{
				IConfigurationSession scopedSession = this.GenerateIConfigurationSessionForShareableObjects(organizationId, sharedConfig);
				IList<RetentionPolicy> defaultRetentionPolicy = SharedConfiguration.GetDefaultRetentionPolicy(scopedSession, flag, null, 1);
				if (defaultRetentionPolicy != null && defaultRetentionPolicy.Count > 0)
				{
					adobjectId = defaultRetentionPolicy[0].Id;
				}
				else
				{
					adobjectId = null;
				}
				if (!flag)
				{
					this.defaultRetentionPolicyIds.Add(organizationId, adobjectId);
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
				return null;
			}
			return adobjectId;
		}

		internal ADObjectId GetDefaultSharingPolicyId(ADObject user, SharedConfiguration sharedConfig)
		{
			ADObjectId result;
			try
			{
				if (!this.defaultSharingPolicyIds.ContainsKey(user.OrganizationId))
				{
					IConfigurationSession configurationSession = this.GenerateIConfigurationSessionForShareableObjects(user.OrganizationId, sharedConfig);
					FederatedOrganizationId federatedOrganizationId = configurationSession.GetFederatedOrganizationId(configurationSession.SessionSettings.CurrentOrganizationId);
					this.defaultSharingPolicyIds[user.OrganizationId] = ((federatedOrganizationId != null) ? federatedOrganizationId.DefaultSharingPolicyLink : null);
				}
				result = this.defaultSharingPolicyIds[user.OrganizationId];
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
				result = null;
			}
			return result;
		}

		private IConfigurationSession GenerateIConfigurationSessionForShareableObjects(OrganizationId organizationId)
		{
			return this.GenerateIConfigurationSessionForShareableObjects(organizationId, null);
		}

		private IConfigurationSession GenerateIConfigurationSessionForShareableObjects(OrganizationId organizationId, SharedConfiguration sharedConfig)
		{
			ADSessionSettings adsessionSettings = null;
			if (sharedConfig != null)
			{
				adsessionSettings = sharedConfig.GetSharedConfigurationSessionSettings();
			}
			if (adsessionSettings == null)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, organizationId, base.ExecutingUserOrganizationId, false);
				adsessionSettings.IsSharedConfigChecked = true;
			}
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, adsessionSettings, 720, "GenerateIConfigurationSessionForShareableObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\GetRecipientBase.cs");
		}

		private Dictionary<OrganizationId, ADObjectId> defaultPolicyIds = new Dictionary<OrganizationId, ADObjectId>(8);

		private Dictionary<OrganizationId, ADObjectId> defaultSharingPolicyIds = new Dictionary<OrganizationId, ADObjectId>(8);

		private readonly Dictionary<OrganizationId, ADObjectId> defaultRetentionPolicyIds = new Dictionary<OrganizationId, ADObjectId>(8);

		private QueryFilter inputFilter;

		private SortBy internalSortBy;

		private ADObjectId rootId;

		private long usnForReconciliationSearch = -1L;
	}
}
