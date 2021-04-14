using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.ValidationRules;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "Recipient", DefaultParameterSetName = "Identity")]
	public sealed class GetRecipient : GetRecipientBase<RecipientIdParameter, ReducedRecipient>
	{
		static GetRecipient()
		{
			GetRecipient.OutputPropertiesToDefinitionDict = PartialPropertiesUtil.GetOutputPropertiesToDefinitionDict(ObjectSchema.GetInstance<ReducedRecipientSchema>(), typeof(ReducedRecipient), new Dictionary<string, string>
			{
				{
					"Identity",
					"Id"
				}
			});
		}

		protected override int PageSize
		{
			get
			{
				if (base.InternalResultSize.IsUnlimited)
				{
					return 1000;
				}
				return (int)Math.Min(base.InternalResultSize.Value - base.WriteObjectCount + 1U, 1000U);
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RecipientType[] RecipientType
		{
			get
			{
				RecipientType[] array = (RecipientType[])base.Fields["RecipientType"];
				if (array != null)
				{
					return array;
				}
				return base.RecipientTypes;
			}
			set
			{
				base.VerifyValues<RecipientType>(base.RecipientTypes, value);
				base.Fields["RecipientType"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetRecipient.AllowedRecipientTypeDetails.Union(GetRecipient.AllowedRecipientTypeDetailsForColloborationMailbox).ToArray<RecipientTypeDetails>(), value);
				base.Fields["RecipientTypeDetails"] = value;
			}
		}

		[Parameter]
		public PropertySet PropertySet
		{
			get
			{
				return (PropertySet)(base.Fields["PropertySet"] ?? PropertySet.All);
			}
			set
			{
				base.Fields["PropertySet"] = value;
			}
		}

		[Parameter]
		public string[] Properties
		{
			get
			{
				return (string[])base.Fields["Properties"];
			}
			set
			{
				base.Fields["Properties"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "RecipientPreviewFilterSet")]
		public string RecipientPreviewFilter
		{
			get
			{
				return (string)base.Fields["RecipientPreviewFilter"];
			}
			set
			{
				base.Fields["RecipientPreviewFilter"] = value;
			}
		}

		[Parameter(ParameterSetName = "Identity")]
		public string BookmarkDisplayName
		{
			get
			{
				return (string)base.Fields["BookmarkDisplayName"];
			}
			set
			{
				base.Fields["BookmarkDisplayName"] = value;
			}
		}

		[Parameter(ParameterSetName = "Identity")]
		public bool IncludeBookmarkObject
		{
			get
			{
				return (bool)(base.Fields["IncludeBookmarkObject"] ?? true);
			}
			set
			{
				base.Fields["IncludeBookmarkObject"] = value;
			}
		}

		[Parameter]
		public AuthenticationType AuthenticationType
		{
			get
			{
				return (AuthenticationType)(base.Fields["AuthenticationType"] ?? AuthenticationType.Managed);
			}
			set
			{
				base.Fields["AuthenticationType"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<Capability> Capabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)base.Fields["Capabilities"];
			}
			set
			{
				base.Fields["Capabilities"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "DatabaseSet", ValueFromPipeline = true)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ReducedRecipientSchema>();
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter;
				if (this.recipientPreviewFilter != null)
				{
					queryFilter = base.ConstructQueryFilterWithCustomFilter(this.recipientPreviewFilter);
				}
				else
				{
					queryFilter = base.InternalFilter;
				}
				QueryFilter queryFilter2 = null;
				if (this.database != null)
				{
					queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, this.database.Id);
				}
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					queryFilter2,
					this.GetCapabilitiesFilter(),
					this.GetAuthenticationTypeFilter()
				});
			}
		}

		private QueryFilter GetCapabilitiesFilter()
		{
			QueryFilter result = null;
			if (this.Capabilities != null && this.Capabilities.Count > 0)
			{
				List<QueryFilter> list = new List<QueryFilter>(this.Capabilities.Count);
				foreach (Capability capability in this.Capabilities)
				{
					if (capability == Capability.None)
					{
						base.WriteError(new NotImplementedException(DirectoryStrings.CannotBuildCapabilityFilterUnsupportedCapability(capability.ToString())), ErrorCategory.InvalidArgument, null);
					}
					else
					{
						CapabilityIdentifierEvaluator capabilityIdentifierEvaluator = CapabilityIdentifierEvaluatorFactory.Create(capability);
						QueryFilter item;
						LocalizedString value;
						if (capabilityIdentifierEvaluator.TryGetFilter(base.CurrentOrganizationId, out item, out value))
						{
							list.Add(item);
						}
						else
						{
							base.WriteError(new ArgumentException(value), ErrorCategory.InvalidArgument, null);
						}
					}
				}
				result = QueryFilter.AndTogether(list.ToArray());
			}
			return result;
		}

		private QueryFilter GetAuthenticationTypeFilter()
		{
			QueryFilter result = null;
			LocalizedString value;
			if (base.Fields.Contains("AuthenticationType") && !ADRecipient.TryGetAuthenticationTypeFilterInternal(this.AuthenticationType, base.CurrentOrganizationId, out result, out value))
			{
				base.WriteError(new ArgumentException(value), ErrorCategory.InvalidArgument, null);
			}
			return result;
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetRecipient.SortPropertiesArray;
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return this.RecipientType;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return this.RecipientTypeDetails ?? GetRecipient.AllowedRecipientTypeDetails;
			}
		}

		protected override IEnumerable<ReducedRecipient> GetPagedData()
		{
			if (this.usingALbasedVlv)
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsInALVerboseString(base.DataSession, typeof(ReducedRecipient), this.addressList));
				return ADVlvPagedReader<ReducedRecipient>.GetADVlvPagedReader(this.addressList, (IRecipientSession)base.DataSession, this.InternalSortBy, this.IncludeBookmarkObject, true, this.PageSize, 1, this.BookmarkDisplayName, this.propertiesToRead);
			}
			base.WriteVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(base.DataSession, typeof(ReducedRecipient), this.InternalFilter, this.RootId, this.DeepSearch));
			return ((IRecipientSession)base.DataSession).FindPaged<ReducedRecipient>((ADObjectId)this.RootId, this.DeepSearch ? QueryScope.SubTree : QueryScope.OneLevel, this.InternalFilter, this.InternalSortBy, this.PageSize, this.propertiesToRead);
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)base.CreateSession(), 594, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\GetRecipient.cs");
			this.usingALbasedVlv = this.IsUsingALbasedVlv(recipientSession);
			if (this.usingALbasedVlv)
			{
				recipientSession.UseGlobalCatalog = true;
				if (!string.IsNullOrEmpty(base.DomainController) && !recipientSession.IsReadConnectionAvailable())
				{
					IRecipientSession reducedRecipientSession = DirectorySessionFactory.Default.GetReducedRecipientSession(DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, recipientSession.NetworkCredential, recipientSession.SessionSettings, 603, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\GetRecipient.cs"), 603, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\GetRecipient.cs");
					reducedRecipientSession.UseGlobalCatalog = true;
					recipientSession = reducedRecipientSession;
				}
			}
			else
			{
				if (base.IgnoreDefaultScope)
				{
					recipientSession.EnforceDefaultScope = false;
				}
				if (this.recipientPreviewFilter != null && this.recipientPreviewFilter is CustomLdapFilter)
				{
					recipientSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds(60.0));
				}
			}
			if (base.ParameterSetName == "DatabaseSet")
			{
				IRecipientSession reducedRecipientSession2 = DirectorySessionFactory.Default.GetReducedRecipientSession(DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(recipientSession.DomainController, recipientSession.SearchRoot, recipientSession.Lcid, recipientSession.ReadOnly, recipientSession.ConsistencyMode, recipientSession.NetworkCredential, recipientSession.SessionSettings, ConfigScopes.TenantSubTree, 633, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\GetRecipient.cs"), 633, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\GetRecipient.cs");
				reducedRecipientSession2.EnforceDefaultScope = recipientSession.EnforceDefaultScope;
				reducedRecipientSession2.UseGlobalCatalog = recipientSession.UseGlobalCatalog;
				reducedRecipientSession2.LinkResolutionServer = recipientSession.LinkResolutionServer;
				reducedRecipientSession2.ServerTimeout = recipientSession.ServerTimeout;
				recipientSession = reducedRecipientSession2;
			}
			return recipientSession;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ReducedRecipient reducedRecipient = (ReducedRecipient)dataObject;
			if (reducedRecipient == null)
			{
				return null;
			}
			if (this.ShouldReadProperties(new ADPropertyDefinition[]
			{
				ReducedRecipientSchema.OwaMailboxPolicy,
				ReducedRecipientSchema.SharingPolicy,
				ReducedRecipientSchema.RetentionPolicy
			}) && reducedRecipient.RecipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox)
			{
				SharedConfiguration sharedConfig = null;
				if (SharedConfiguration.IsDehydratedConfiguration(reducedRecipient.OrganizationId) || (SharedConfiguration.GetSharedConfigurationState(reducedRecipient.OrganizationId) & SharedTenantConfigurationState.Static) != SharedTenantConfigurationState.UnSupported)
				{
					sharedConfig = SharedConfiguration.GetSharedConfiguration(reducedRecipient.OrganizationId);
				}
				if (this.ShouldReadProperties(new ADPropertyDefinition[]
				{
					ReducedRecipientSchema.OwaMailboxPolicy
				}) && reducedRecipient.OwaMailboxPolicy == null)
				{
					ADObjectId defaultOwaMailboxPolicyId = this.GetDefaultOwaMailboxPolicyId(reducedRecipient);
					if (defaultOwaMailboxPolicyId != null)
					{
						reducedRecipient.OwaMailboxPolicy = defaultOwaMailboxPolicyId;
					}
				}
				if (this.ShouldReadProperties(new ADPropertyDefinition[]
				{
					ReducedRecipientSchema.SharingPolicy
				}) && reducedRecipient.SharingPolicy == null)
				{
					reducedRecipient.SharingPolicy = base.GetDefaultSharingPolicyId(reducedRecipient, sharedConfig);
				}
				if (this.ShouldReadProperties(new ADPropertyDefinition[]
				{
					ReducedRecipientSchema.RetentionPolicy
				}) && reducedRecipient.RetentionPolicy == null && reducedRecipient.ShouldUseDefaultRetentionPolicy)
				{
					reducedRecipient.RetentionPolicy = base.GetDefaultRetentionPolicyId(reducedRecipient, sharedConfig);
				}
			}
			if (this.ShouldReadProperties(new ADPropertyDefinition[]
			{
				ReducedRecipientSchema.ActiveSyncMailboxPolicy,
				ReducedRecipientSchema.ActiveSyncMailboxPolicyIsDefaulted
			}) && reducedRecipient.ActiveSyncMailboxPolicy == null && !reducedRecipient.ExchangeVersion.IsOlderThan(ReducedRecipientSchema.ActiveSyncMailboxPolicy.VersionAdded))
			{
				ADObjectId defaultPolicyId = base.GetDefaultPolicyId(reducedRecipient);
				if (defaultPolicyId != null)
				{
					reducedRecipient.ActiveSyncMailboxPolicy = defaultPolicyId;
					reducedRecipient.ActiveSyncMailboxPolicyIsDefaulted = true;
				}
			}
			if (this.ShouldReadProperties(new ADPropertyDefinition[]
			{
				ReducedRecipientSchema.AuthenticationType
			}) && reducedRecipient.OrganizationId.ConfigurationUnit != null)
			{
				SmtpAddress windowsLiveID = reducedRecipient.WindowsLiveID;
				if (reducedRecipient.WindowsLiveID.Domain != null && !reducedRecipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
				{
					reducedRecipient.AuthenticationType = MailboxTaskHelper.GetNamespaceAuthenticationType(reducedRecipient.OrganizationId, reducedRecipient.WindowsLiveID.Domain);
				}
			}
			if (this.ShouldReadProperties(new ADPropertyDefinition[]
			{
				ReducedRecipientSchema.Capabilities
			}))
			{
				reducedRecipient.PopulateCapabilitiesProperty();
			}
			return reducedRecipient;
		}

		internal ADObjectId GetDefaultOwaMailboxPolicyId(ADObject user)
		{
			ADObjectId adobjectId = null;
			OrganizationId organizationId = user.OrganizationId;
			if (!this.owaMailboxPolicyCache.TryGetValue(organizationId, out adobjectId))
			{
				OwaMailboxPolicy defaultOwaMailboxPolicy = OwaSegmentationSettings.GetDefaultOwaMailboxPolicy(organizationId);
				if (defaultOwaMailboxPolicy != null)
				{
					adobjectId = defaultOwaMailboxPolicy.Id;
				}
				this.owaMailboxPolicyCache.Add(organizationId, adobjectId);
			}
			return adobjectId;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.CheckExclusiveParameters(new object[]
			{
				"Properties",
				"PropertySet"
			});
			if (this.Identity != null && (this.PropertySet != PropertySet.All || base.Fields.IsModified("Properties")))
			{
				this.WriteWarning(base.Fields.IsModified("Properties") ? Strings.WarningPropertiesIgnored : Strings.WarningPropertySetIgnored);
				this.PropertySet = PropertySet.All;
			}
			base.InternalBeginProcessing();
			if (!string.IsNullOrEmpty(this.RecipientPreviewFilter))
			{
				this.recipientPreviewFilter = this.GetOPathFilter(this.RecipientPreviewFilter);
				if (this.recipientPreviewFilter == null)
				{
					this.recipientPreviewFilter = new CustomLdapFilter(this.RecipientPreviewFilter);
				}
			}
			if (this.Identity == null)
			{
				if (base.Fields.IsModified("Properties"))
				{
					this.InitializePropertiesToRead();
				}
				else
				{
					this.propertiesToRead = ReducedRecipient.Properties[this.PropertySet];
				}
			}
			TaskLogger.LogExit();
		}

		private void InitializePropertiesToRead()
		{
			IList<string> userSpecifiedProperties = PartialPropertiesUtil.ParseUserSpecifiedProperties(this.Properties);
			LocalizedString value;
			if (!PartialPropertiesUtil.ValidateProperties(userSpecifiedProperties, GetRecipient.OutputPropertiesToDefinitionDict, out value))
			{
				base.ThrowTerminatingError(new ArgumentException(value), ErrorCategory.InvalidArgument, null);
			}
			this.propertiesToRead = PartialPropertiesUtil.CalculatePropertiesToRead(GetRecipient.OutputPropertiesToDefinitionDict, userSpecifiedProperties, GetRecipient.MandatoryOutputProperties, GetRecipient.PropertyRelationship, GetRecipient.SpecialPropertiesLeadToAllRead, base.IsVerboseOn ? new WriteVerboseDelegate(base.WriteVerbose) : null);
		}

		private bool ShouldReadProperties(params ADPropertyDefinition[] propertyDefinitions)
		{
			return this.propertiesToRead == null || propertyDefinitions.Any((ADPropertyDefinition property) => this.propertiesToRead.Contains(property));
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.database = null;
			if (this.Database != null)
			{
				this.Database.AllowLegacy = true;
				this.database = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalProcessRecord();
				if (this.usingALbasedVlv && base.WriteObjectCount == 0U && this.ConfigurationSession.Read<AddressBookBase>(this.addressList) == null)
				{
					this.WriteWarning(Strings.WarningSystemAddressListNotFound(this.addressList.Name));
					this.usingALbasedVlv = false;
					base.InternalProcessRecord();
				}
			}
			catch (ADOperationException ex)
			{
				if (!ADSession.IsLdapFilterError(ex) || string.IsNullOrEmpty(this.RecipientPreviewFilter))
				{
					throw;
				}
				base.WriteError(new ArgumentException(Strings.ErrorInvalidRecipientPreviewFilter(this.RecipientPreviewFilter)), ErrorCategory.InvalidArgument, null);
			}
			if (!this.usingALbasedVlv)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in GetRecipient.Parameters)
				{
					if (base.Fields.IsModified(text))
					{
						stringBuilder.Append(text);
						stringBuilder.Append(", ");
					}
				}
				if (stringBuilder.Length != 0)
				{
					this.WriteWarning(Strings.WarningParametersIgnored(stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString()));
				}
			}
		}

		private bool IsUsingALbasedVlv(IDirectorySession session)
		{
			ADSessionSettings sessionSettings = session.SessionSettings;
			if (this.Identity != null || !string.IsNullOrEmpty(base.Anr) || !string.IsNullOrEmpty(base.Filter) || !string.IsNullOrEmpty(this.RecipientPreviewFilter) || (this.InternalSortBy != null && this.InternalSortBy.ColumnDefinition != ADRecipientSchema.DisplayName) || (this.RecipientTypeDetails != null && this.RecipientTypeDetails.Length != 0) || base.Fields.Contains("AuthenticationType") || this.Database != null || this.Capabilities != null)
			{
				return false;
			}
			AddressListType addressListType;
			if (base.Fields["RecipientType"] == null)
			{
				addressListType = AddressListType.AllRecipients;
			}
			else
			{
				if (this.RecipientTypes.Length != 1 || this.RecipientTypes[0] != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox)
				{
					if (this.RecipientTypes.Length == 3)
					{
						if (Array.TrueForAll<RecipientType>(this.RecipientTypes, (RecipientType item) => item == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup || item == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || item == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup))
						{
							addressListType = AddressListType.AllGroups;
							goto IL_F3;
						}
					}
					return false;
				}
				addressListType = AddressListType.AllMailboxes;
			}
			IL_F3:
			if (this.RootId != null && !ADObjectId.Equals(base.CurrentOrganizationId.OrganizationalUnit, (ADObjectId)this.RootId))
			{
				return false;
			}
			if (base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && !session.UseGlobalCatalog)
			{
				return false;
			}
			Organization orgContainer = this.ConfigurationSession.GetOrgContainer();
			if (!orgContainer.IsAddressListPagingEnabled)
			{
				return false;
			}
			bool flag;
			if (!this.CheckDomainReadScopeRoot(sessionSettings) || !this.CheckDomainReadScopeFilter(sessionSettings, out flag))
			{
				return false;
			}
			switch (addressListType)
			{
			case AddressListType.AllRecipients:
				if (flag)
				{
					this.addressList = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId("All Recipients(VLV)");
				}
				else
				{
					this.addressList = this.defaultGlobalAddressList;
				}
				break;
			case AddressListType.AllMailboxes:
				if (flag)
				{
					this.addressList = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId("All Mailboxes(VLV)");
				}
				else
				{
					this.addressList = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId("Mailboxes(VLV)");
				}
				break;
			case AddressListType.AllGroups:
				if (flag)
				{
					this.addressList = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId("All Groups(VLV)");
				}
				else
				{
					this.addressList = base.CurrentOrgContainerId.GetDescendantId(SystemAddressList.RdnSystemAddressListContainerToOrganization).GetChildId("Groups(VLV)");
				}
				break;
			}
			return true;
		}

		private bool CheckDomainReadScopeRoot(ADSessionSettings settings)
		{
			return settings == null || settings.RecipientReadScope == null || (settings.RecipientReadScope.Root == null || (base.CurrentOrganizationId != OrganizationId.ForestWideOrgId && base.CurrentOrganizationId.OrganizationalUnit.IsDescendantOf(settings.RecipientReadScope.Root)));
		}

		private bool CheckDomainReadScopeFilter(ADSessionSettings settings, out bool isRecipientsHiddenFromALIncluded)
		{
			isRecipientsHiddenFromALIncluded = true;
			if (settings == null || settings.RecipientReadScope == null || settings.RecipientReadScope.Filter == null)
			{
				return true;
			}
			AddressBookBase[] array = this.ConfigurationSession.Find<AddressBookBase>(GlobalAddressListIdParameter.GetRootContainerId(this.ConfigurationSession), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, AddressBookBaseSchema.IsDefaultGlobalAddressList, true), null, 1);
			if (array == null || array.Length == 0)
			{
				return false;
			}
			this.defaultGlobalAddressList = array[0].Id;
			if (settings.RecipientReadScope.Filter is OrFilter)
			{
				foreach (QueryFilter queryFilter in ((OrFilter)settings.RecipientReadScope.Filter).Filters)
				{
					if (queryFilter is ComparisonFilter)
					{
						ComparisonFilter comparisonFilter = (ComparisonFilter)queryFilter;
						if (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && comparisonFilter.Property == ADRecipientSchema.AddressListMembership && ADObjectId.Equals((ADObjectId)comparisonFilter.PropertyValue, this.defaultGlobalAddressList))
						{
							isRecipientsHiddenFromALIncluded = false;
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private QueryFilter GetOPathFilter(string filterStr)
		{
			MonadFilter monadFilter = null;
			QueryFilter result = null;
			try
			{
				base.WriteVerbose(Strings.VerboseTryingToParseOPathFilter(filterStr));
				monadFilter = new MonadFilter(filterStr, this, ObjectSchema.GetInstance<ADRecipientProperties>());
				base.WriteVerbose(Strings.VerboseParsingOPathFilterSucceed(filterStr));
			}
			catch (InvalidCastException exception)
			{
				base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (ParsingException ex)
			{
				base.WriteVerbose(Strings.VerboseParsingOPathFilterFailed(filterStr, ex.Message));
			}
			if (monadFilter != null && monadFilter.InnerFilter != null)
			{
				result = monadFilter.InnerFilter;
			}
			return result;
		}

		private const int LdapFilterSearchTimeoutDefaultSeconds = 60;

		private const string ParamBookmarkDisplayName = "BookmarkDisplayName";

		private const string ParamIncludeBookmarkObject = "IncludeBookmarkObject";

		private const string ParamAuthenticationType = "AuthenticationType";

		private const string ParamCapabilities = "Capabilities";

		private const string ParamProperties = "Properties";

		private const string ParamPropertySet = "PropertySet";

		private const string ParamDatabase = "Database";

		private const string ParamSetDatabaseSet = "DatabaseSet";

		private static readonly IDictionary<string, PropertyDefinition> OutputPropertiesToDefinitionDict;

		private static object syncRoot = new object();

		private static readonly string[] Parameters = new string[]
		{
			"BookmarkDisplayName",
			"IncludeBookmarkObject"
		};

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = CannedSystemAddressLists.RecipientTypeDetailsForAllRecipientsAL;

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetailsForColloborationMailbox = CannedSystemAddressLists.RecipientTypeDetailsForAllPublicFolderMailboxesAL.Union(CannedSystemAddressLists.RecipientTypeDetailsForGroupMailboxesAL).ToArray<RecipientTypeDetails>();

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ReducedRecipientSchema.Alias,
			ReducedRecipientSchema.City,
			ReducedRecipientSchema.DisplayName,
			ReducedRecipientSchema.FirstName,
			ReducedRecipientSchema.LastName,
			ADObjectSchema.Name,
			ReducedRecipientSchema.Office,
			ReducedRecipientSchema.ServerLegacyDN
		};

		private static readonly PropertyDefinition[] MandatoryOutputProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADObjectSchema.OrganizationId,
			ReducedRecipientSchema.RecipientType,
			ReducedRecipientSchema.RecipientTypeDetails,
			ADObjectSchema.ExchangeVersion
		};

		private static readonly PropertyDefinition[] SpecialPropertiesLeadToAllRead = new PropertyDefinition[]
		{
			ReducedRecipientSchema.Capabilities
		};

		private static readonly IDictionary<PropertyDefinition, IList<PropertyDefinition>> PropertyRelationship = new Dictionary<PropertyDefinition, IList<PropertyDefinition>>
		{
			{
				ReducedRecipientSchema.AuthenticationType,
				new PropertyDefinition[]
				{
					ReducedRecipientSchema.WindowsLiveID
				}
			}
		};

		private IList<PropertyDefinition> propertiesToRead;

		private bool usingALbasedVlv;

		private ADObjectId addressList;

		private ADObjectId defaultGlobalAddressList;

		private QueryFilter recipientPreviewFilter;

		private MailboxDatabase database;

		private readonly Dictionary<OrganizationId, ADObjectId> owaMailboxPolicyCache = new Dictionary<OrganizationId, ADObjectId>(8);
	}
}
