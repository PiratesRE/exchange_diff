using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADDynamicGroup : ADRecipient, IADDistributionList
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADDynamicGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADDynamicGroup.MostDerivedClass;
			}
		}

		public override string ObjectCategoryCN
		{
			get
			{
				return ADDynamicGroup.ObjectCategoryCNInternal;
			}
		}

		internal override string ObjectCategoryName
		{
			get
			{
				return ADDynamicGroup.ObjectCategoryNameInternal;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.ObjectCategoryName);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		internal ADDynamicGroup(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADDynamicGroup(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADDynamicGroup()
		{
		}

		public ADObjectId RecipientContainer
		{
			get
			{
				return (ADObjectId)this[ADDynamicGroupSchema.RecipientContainer];
			}
			internal set
			{
				this[ADDynamicGroupSchema.RecipientContainer] = value;
			}
		}

		public string LdapRecipientFilter
		{
			get
			{
				return (string)this[ADDynamicGroupSchema.LdapRecipientFilter];
			}
			internal set
			{
				this[ADDynamicGroupSchema.LdapRecipientFilter] = value;
			}
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[ADDynamicGroupSchema.RecipientFilter];
			}
			internal set
			{
				this[ADDynamicGroupSchema.RecipientFilter] = value;
			}
		}

		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return (WellKnownRecipientType?)this[ADDynamicGroupSchema.IncludedRecipients];
			}
			internal set
			{
				this[ADDynamicGroupSchema.IncludedRecipients] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalDepartment];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalDepartment] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCompany];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCompany] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalStateOrProvince];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalStateOrProvince] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute1];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute1] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute2];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute2] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute3];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute3] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute4];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute4] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute5];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute5] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute6];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute6] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute7];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute7] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute8];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute8] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute9];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute9] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute10];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute10] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute11];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute11] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute12];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute12] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute13];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute13] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute14];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute14] = value;
			}
		}

		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADDynamicGroupSchema.ConditionalCustomAttribute15];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ConditionalCustomAttribute15] = value;
			}
		}

		public WellKnownRecipientFilterType RecipientFilterType
		{
			get
			{
				return (WellKnownRecipientFilterType)this[ADDynamicGroupSchema.RecipientFilterType];
			}
		}

		internal static QueryFilter IncludeRecipientFilterBuilder(SinglePropertyFilter filter)
		{
			return ADDynamicGroup.PrecannedRecipientFilterFilterBuilder(filter, ADDynamicGroupSchema.RecipientFilterMetadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:IncludedRecipients=");
		}

		internal void SetRecipientFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				this[ADDynamicGroupSchema.RecipientFilter] = string.Empty;
				this[ADDynamicGroupSchema.LdapRecipientFilter] = string.Empty;
			}
			else
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					filter,
					RecipientFilterHelper.ExcludingSystemMailboxFilter,
					RecipientFilterHelper.ExcludingCasMailboxFilter,
					RecipientFilterHelper.ExcludingMailboxPlanFilter,
					RecipientFilterHelper.ExcludingDiscoveryMailboxFilter,
					RecipientFilterHelper.ExcludingPublicFolderMailboxFilter,
					RecipientFilterHelper.ExcludingArbitrationMailboxFilter,
					RecipientFilterHelper.ExcludingAuditLogMailboxFilter
				});
				this[ADDynamicGroupSchema.RecipientFilter] = queryFilter.GenerateInfixString(FilterLanguage.Monad);
				this[ADDynamicGroupSchema.LdapRecipientFilter] = LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter);
			}
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Custom, this.propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata);
		}

		private static QueryFilter PrecannedRecipientFilterFilterBuilder(SinglePropertyFilter filter, ADPropertyDefinition filterMetadata, string attributePrefix)
		{
			object obj = ADObject.PropertyValueFromEqualityFilter(filter);
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			if (obj == null)
			{
				return new NotFilter(new TextFilter(filterMetadata, attributePrefix, MatchOptions.Prefix, MatchFlags.IgnoreCase));
			}
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, filterMetadata, attributePrefix + obj.ToString());
		}

		public int GroupMemberCount
		{
			get
			{
				return (int)this[ADGroupSchema.GroupMemberCount];
			}
			set
			{
				this[ADGroupSchema.GroupMemberCount] = value;
			}
		}

		public int GroupExternalMemberCount
		{
			get
			{
				return (int)this[ADGroupSchema.GroupExternalMemberCount];
			}
			set
			{
				this[ADGroupSchema.GroupExternalMemberCount] = value;
			}
		}

		public ADObjectId HomeMTA
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.HomeMTA];
			}
			internal set
			{
				this[ADRecipientSchema.HomeMTA] = value;
			}
		}

		public string ExpansionServer
		{
			get
			{
				return (string)this[ADDynamicGroupSchema.ExpansionServer];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ExpansionServer] = value;
			}
		}

		public ADObjectId ManagedBy
		{
			get
			{
				return (ADObjectId)this[ADDynamicGroupSchema.ManagedBy];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ManagedBy] = value;
			}
		}

		ADObjectId IADDistributionList.ManagedBy
		{
			get
			{
				return this.ManagedBy;
			}
			set
			{
				this.ManagedBy = value;
			}
		}

		public bool ReportToManagerEnabled
		{
			get
			{
				return (bool)this[ADDynamicGroupSchema.ReportToManagerEnabled];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ReportToManagerEnabled] = value;
			}
		}

		public bool ReportToOriginatorEnabled
		{
			get
			{
				return (bool)this[ADDynamicGroupSchema.ReportToOriginatorEnabled];
			}
			internal set
			{
				this[ADDynamicGroupSchema.ReportToOriginatorEnabled] = value;
			}
		}

		public DeliveryReportsReceiver SendDeliveryReportsTo
		{
			get
			{
				return (DeliveryReportsReceiver)this[ADDynamicGroupSchema.SendDeliveryReportsTo];
			}
			internal set
			{
				this[ADDynamicGroupSchema.SendDeliveryReportsTo] = value;
			}
		}

		DeliveryReportsReceiver IADDistributionList.SendDeliveryReportsTo
		{
			get
			{
				return this.SendDeliveryReportsTo;
			}
			set
			{
				this.SendDeliveryReportsTo = value;
			}
		}

		public bool SendOofMessageToOriginatorEnabled
		{
			get
			{
				return (bool)this[ADDynamicGroupSchema.SendOofMessageToOriginatorEnabled];
			}
			internal set
			{
				this[ADDynamicGroupSchema.SendOofMessageToOriginatorEnabled] = value;
			}
		}

		bool IADDistributionList.SendOofMessageToOriginatorEnabled
		{
			get
			{
				return this.SendOofMessageToOriginatorEnabled;
			}
			set
			{
				this.SendOofMessageToOriginatorEnabled = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.ReportToManagerEnabled && this.ManagedBy == null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorReportToManagedEnabledWithoutManager(this.Identity.ToString(), ADDynamicGroupSchema.ReportToManagerEnabled.Name), this.Identity, string.Empty));
			}
			if (this.ReportToManagerEnabled && this.ReportToOriginatorEnabled)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorReportToBothManagerAndOriginator(this.Identity.ToString(), ADDynamicGroupSchema.ReportToManagerEnabled.Name, ADDynamicGroupSchema.ReportToOriginatorEnabled.Name), this.Identity, string.Empty));
			}
			if (this.RecipientContainer == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorRecipientContainerCanNotNull, ADDynamicGroupSchema.RecipientContainer, string.Empty));
			}
			if (string.IsNullOrEmpty(this.RecipientFilter) && (base.IsModified(ADDynamicGroupSchema.RecipientFilter) || base.ObjectState == ObjectState.New))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorInvalidOpathFilter(this.RecipientFilter ?? string.Empty), ADDynamicGroupSchema.RecipientFilter, string.Empty));
			}
			ValidationError validationError = RecipientFilterHelper.ValidatePrecannedRecipientFilter(this.propertyBag, ADDynamicGroupSchema.RecipientFilterMetadata, ADDynamicGroupSchema.RecipientFilter, ADDynamicGroupSchema.IncludedRecipients, this.Identity);
			if (validationError != null)
			{
				errors.Add(validationError);
			}
		}

		MultiValuedProperty<ADObjectId> IADDistributionList.Members
		{
			get
			{
				return ADDynamicGroup.ddlMembersMvp;
			}
		}

		internal ADPagedReader<ADRawEntry> Expand(int pageSize, params PropertyDefinition[] properties)
		{
			if (this.CheckEmptyLdapRecipientFilter())
			{
				return new ADPagedReader<ADRawEntry>();
			}
			return new ADDynamicGroupPagedReader<ADRawEntry>(base.Session, this.RecipientContainer, QueryScope.SubTree, this.LdapRecipientFilter, pageSize, new CustomExceptionHandler(this.DDLExpansionHandler), properties);
		}

		internal ADPagedReader<TEntry> Expand<TEntry>(int pageSize, params PropertyDefinition[] properties) where TEntry : MiniRecipient, new()
		{
			if (this.CheckEmptyLdapRecipientFilter())
			{
				return new ADPagedReader<TEntry>();
			}
			return new ADDynamicGroupPagedReader<TEntry>(base.Session, this.RecipientContainer, QueryScope.SubTree, this.LdapRecipientFilter, pageSize, new CustomExceptionHandler(this.DDLExpansionHandler), properties);
		}

		ADPagedReader<ADRawEntry> IADDistributionList.Expand(int pageSize, params PropertyDefinition[] properties)
		{
			return this.Expand(pageSize, properties);
		}

		ADPagedReader<TEntry> IADDistributionList.Expand<TEntry>(int pageSize, params PropertyDefinition[] properties)
		{
			return this.Expand<TEntry>(pageSize, properties);
		}

		internal ADPagedReader<ADRecipient> Expand(int pageSize)
		{
			if (this.CheckEmptyLdapRecipientFilter())
			{
				return new ADPagedReader<ADRecipient>();
			}
			return new ADDynamicGroupPagedReader<ADRecipient>(base.Session, this.RecipientContainer, QueryScope.SubTree, this.LdapRecipientFilter, pageSize, new CustomExceptionHandler(this.DDLExpansionHandler), null);
		}

		ADPagedReader<ADRecipient> IADDistributionList.Expand(int pageSize)
		{
			return this.Expand(pageSize);
		}

		private bool CheckEmptyLdapRecipientFilter()
		{
			if (string.IsNullOrEmpty(this.LdapRecipientFilter))
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DynamicDistributionGroupFilterError, base.Id.ToString(), new object[]
				{
					base.Id.ToDNString(),
					base.OriginatingServer,
					DirectoryStrings.ErrorDDLFilterMissing
				});
				return true;
			}
			return false;
		}

		private void DDLExpansionHandler(DirectoryException de)
		{
			PropertyValidationError propertyValidationError = this.CheckForDDLMisconfiguration(de);
			if (propertyValidationError != null)
			{
				ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateCriticalValidationFailures, UpdateType.Update, 1U);
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DynamicDistributionGroupFilterError, base.Id.ToString(), new object[]
				{
					base.Id.ToDNString(),
					base.OriginatingServer,
					propertyValidationError.Description
				});
				throw new DataValidationException(propertyValidationError, de);
			}
		}

		private PropertyValidationError CheckForDDLMisconfiguration(DirectoryException e)
		{
			DirectoryOperationException ex = e as DirectoryOperationException;
			if (ex != null)
			{
				if (ex.Response == null)
				{
					return null;
				}
				ResultCode resultCode = ex.Response.ResultCode;
				if (resultCode == ResultCode.OperationsError)
				{
					return new PropertyValidationError(DirectoryStrings.ErrorDDLOperationsError, ADDynamicGroupSchema.RecipientContainer, this.RecipientContainer);
				}
				switch (resultCode)
				{
				case ResultCode.ReferralV2:
				case ResultCode.Referral:
					return new PropertyValidationError(DirectoryStrings.ErrorDDLReferral, ADDynamicGroupSchema.RecipientContainer, this.RecipientContainer);
				default:
					switch (resultCode)
					{
					case ResultCode.NoSuchObject:
						return new PropertyValidationError(DirectoryStrings.ErrorDDLNoSuchObject, ADDynamicGroupSchema.RecipientContainer, this.RecipientContainer);
					case ResultCode.InvalidDNSyntax:
						return new PropertyValidationError(DirectoryStrings.ErrorDDLInvalidDNSyntax, ADDynamicGroupSchema.RecipientContainer, this.RecipientContainer);
					}
					return null;
				}
			}
			else
			{
				LdapException ex2 = e as LdapException;
				if (ex2 != null && ex2.ErrorCode == 87)
				{
					return new PropertyValidationError(DirectoryStrings.ErrorDDLFilterError, ADDynamicGroupSchema.RecipientFilter, this.RecipientFilter);
				}
				return null;
			}
		}

		private static readonly ADDynamicGroupSchema schema = ObjectSchema.GetInstance<ADDynamicGroupSchema>();

		private static ADObjectId[] emptyIdArray = new ADObjectId[0];

		internal static string MostDerivedClass = "msExchDynamicDistributionList";

		internal static string ObjectCategoryNameInternal = "msExchDynamicDistributionList";

		internal static string ObjectCategoryCNInternal = "ms-Exch-Dynamic-Distribution-List";

		private static MultiValuedProperty<ADObjectId> ddlMembersMvp = new MultiValuedProperty<ADObjectId>();
	}
}
