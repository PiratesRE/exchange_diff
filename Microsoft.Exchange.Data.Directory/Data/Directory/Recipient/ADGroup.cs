using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADGroup : ADMailboxRecipient, IADGroup, IADMailboxRecipient, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag, IADMailStorage, IADSecurityPrincipal, IADDistributionList
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADGroup.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
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

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			RecipientType recipientType = base.RecipientType;
			if (RecipientType.MailUniversalDistributionGroup == recipientType || RecipientType.MailNonUniversalGroup == recipientType || RecipientType.MailUniversalSecurityGroup == recipientType)
			{
				if (!base.BypassModerationCheck && this.ReportToManagerEnabled && this.ManagedBy == null)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorReportToManagedEnabledWithoutManager(this.Identity.ToString(), ADGroupSchema.ReportToManagerEnabled.Name), this.Identity, string.Empty));
				}
				if (this.ReportToManagerEnabled && this.ReportToOriginatorEnabled)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorReportToBothManagerAndOriginator(this.Identity.ToString(), ADGroupSchema.ReportToManagerEnabled.Name, ADGroupSchema.ReportToOriginatorEnabled.Name), this.Identity, string.Empty));
				}
			}
			if (this.MemberDepartRestriction == MemberUpdateType.ApprovalRequired)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorGroupMemberDepartRestrictionApprovalRequired(this.Identity.ToString()), this.Identity, string.Empty));
			}
			if (this.ManagedBy.Count == 0 && this.MemberJoinRestriction == MemberUpdateType.ApprovalRequired)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorJoinApprovalRequiredWithoutManager(this.Identity.ToString()), this.Identity, string.Empty));
			}
		}

		internal override bool ShouldValidatePropertyLinkInSameOrganization(ADPropertyDefinition property)
		{
			return (!OrganizationId.ForestWideOrgId.Equals(base.OrganizationId) || !property.Equals(ADGroupSchema.Members) || base.RecipientTypeDetails == RecipientTypeDetails.MailNonUniversalGroup || base.RecipientTypeDetails == RecipientTypeDetails.MailUniversalDistributionGroup || base.RecipientTypeDetails == RecipientTypeDetails.MailUniversalSecurityGroup) && base.ShouldValidatePropertyLinkInSameOrganization(property);
		}

		internal ADGroup(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADGroup(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId(commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		internal ADGroup(IRecipientSession session, string commonName, ADObjectId containerId, GroupTypeFlags groupType)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId(commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
			this.GroupType = groupType;
		}

		public ADGroup()
		{
		}

		public string ExpansionServer
		{
			get
			{
				return (string)this[ADGroupSchema.ExpansionServer];
			}
			set
			{
				this[ADGroupSchema.ExpansionServer] = value;
			}
		}

		public GroupTypeFlags GroupType
		{
			get
			{
				return (GroupTypeFlags)this[ADGroupSchema.GroupType];
			}
			set
			{
				this[ADGroupSchema.GroupType] = value;
			}
		}

		public bool LocalizationDisabled
		{
			get
			{
				return ((int)this[ADGroupSchema.LocalizationFlags] & 1) == 1;
			}
			set
			{
				if (value)
				{
					this[ADGroupSchema.LocalizationFlags] = ((int)this[ADGroupSchema.LocalizationFlags] | 1);
					return;
				}
				this[ADGroupSchema.LocalizationFlags] = ((int)this[ADGroupSchema.LocalizationFlags] & -2);
			}
		}

		internal bool IsExecutingUserGroupOwner
		{
			get
			{
				return (bool)this[ADGroupSchema.IsExecutingUserGroupOwner];
			}
			set
			{
				this[ADGroupSchema.IsExecutingUserGroupOwner] = value;
			}
		}

		public ADObjectId HomeMTA
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.HomeMTA];
			}
			set
			{
				this[ADRecipientSchema.HomeMTA] = value;
			}
		}

		public bool HiddenGroupMembershipEnabled
		{
			get
			{
				return (bool)this[ADGroupSchema.HiddenGroupMembershipEnabled];
			}
		}

		public ADObjectId HomeMtaServerId
		{
			get
			{
				return (ADObjectId)this[ADGroupSchema.HomeMtaServerId];
			}
		}

		public SecurityIdentifier ForeignGroupSid
		{
			get
			{
				return (SecurityIdentifier)this[ADGroupSchema.ForeignGroupSid];
			}
			internal set
			{
				this[ADGroupSchema.ForeignGroupSid] = value;
			}
		}

		public string LinkedGroup
		{
			get
			{
				return (string)this[ADGroupSchema.LinkedGroup];
			}
			internal set
			{
				this.propertyBag.SetField(ADGroupSchema.LinkedGroup, value);
			}
		}

		public bool IsOrganizationalGroup
		{
			get
			{
				return (bool)this[ADGroupSchema.IsOrganizationalGroup];
			}
			internal set
			{
				this[ADGroupSchema.IsOrganizationalGroup] = value;
			}
		}

		internal MultiValuedProperty<Capability> RawCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[ADRecipientSchema.RawCapabilities];
			}
			set
			{
				this[ADRecipientSchema.RawCapabilities] = value;
			}
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

		internal string LinkedPartnerGroupId
		{
			get
			{
				return (string)this[ADGroupSchema.LinkedPartnerGroupId];
			}
			set
			{
				this[ADGroupSchema.LinkedPartnerGroupId] = value;
			}
		}

		internal string LinkedPartnerOrganizationId
		{
			get
			{
				return (string)this[ADGroupSchema.LinkedPartnerOrganizationId];
			}
			set
			{
				this[ADGroupSchema.LinkedPartnerOrganizationId] = value;
			}
		}

		public RoleGroupType RoleGroupType
		{
			get
			{
				return (RoleGroupType)this[ADGroupSchema.RoleGroupType];
			}
		}

		public string Description
		{
			get
			{
				return (string)this[ADGroupSchema.RoleGroupDescription];
			}
		}

		public ADObjectId RawManagedBy
		{
			get
			{
				return (ADObjectId)this[ADGroupSchema.RawManagedBy];
			}
			internal set
			{
				this[ADGroupSchema.RawManagedBy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> CoManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADGroupSchema.CoManagedBy];
			}
			internal set
			{
				this[ADGroupSchema.CoManagedBy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADGroupSchema.ManagedBy];
			}
			internal set
			{
				this[ADGroupSchema.ManagedBy] = value;
			}
		}

		ADObjectId IADDistributionList.ManagedBy
		{
			get
			{
				return this.RawManagedBy;
			}
			set
			{
				this.RawManagedBy = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Members
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADGroupSchema.Members];
			}
			set
			{
				this[ADGroupSchema.Members] = value;
			}
		}

		public MemberUpdateType MemberJoinRestriction
		{
			get
			{
				return (MemberUpdateType)this[ADGroupSchema.MemberJoinRestriction];
			}
			set
			{
				this[ADGroupSchema.MemberJoinRestriction] = value;
			}
		}

		public MemberUpdateType MemberDepartRestriction
		{
			get
			{
				return (MemberUpdateType)this[ADGroupSchema.MemberDepartRestriction];
			}
			set
			{
				this[ADGroupSchema.MemberDepartRestriction] = value;
			}
		}

		public bool ReportToManagerEnabled
		{
			get
			{
				return (bool)this[ADGroupSchema.ReportToManagerEnabled];
			}
			set
			{
				this[ADGroupSchema.ReportToManagerEnabled] = value;
			}
		}

		public bool ReportToOriginatorEnabled
		{
			get
			{
				return (bool)this[ADGroupSchema.ReportToOriginatorEnabled];
			}
			set
			{
				this[ADGroupSchema.ReportToOriginatorEnabled] = value;
			}
		}

		public DeliveryReportsReceiver SendDeliveryReportsTo
		{
			get
			{
				return (DeliveryReportsReceiver)this[ADGroupSchema.SendDeliveryReportsTo];
			}
			set
			{
				this[ADGroupSchema.SendDeliveryReportsTo] = value;
			}
		}

		public bool SendOofMessageToOriginatorEnabled
		{
			get
			{
				return (bool)this[ADGroupSchema.SendOofMessageToOriginatorEnabled];
			}
			set
			{
				this[ADGroupSchema.SendOofMessageToOriginatorEnabled] = value;
			}
		}

		internal static object SendDeliveryReportsToGetter(IPropertyBag propertyBag)
		{
			bool? flag = (bool?)propertyBag[IADDistributionListSchema.ReportToManagerEnabled];
			bool? flag2 = (bool?)propertyBag[IADDistributionListSchema.ReportToOriginatorEnabled];
			if (flag ?? false)
			{
				return DeliveryReportsReceiver.Manager;
			}
			if (flag2 ?? false)
			{
				return DeliveryReportsReceiver.Originator;
			}
			return DeliveryReportsReceiver.None;
		}

		internal static void SendDeliveryReportsToSetter(object value, IPropertyBag propertyBag)
		{
			switch ((DeliveryReportsReceiver)(value ?? DeliveryReportsReceiver.None))
			{
			case DeliveryReportsReceiver.Manager:
				propertyBag[IADDistributionListSchema.ReportToManagerEnabled] = true;
				propertyBag[IADDistributionListSchema.ReportToOriginatorEnabled] = null;
				return;
			case DeliveryReportsReceiver.Originator:
				propertyBag[IADDistributionListSchema.ReportToManagerEnabled] = null;
				propertyBag[IADDistributionListSchema.ReportToOriginatorEnabled] = true;
				return;
			}
			propertyBag[IADDistributionListSchema.ReportToManagerEnabled] = null;
			propertyBag[IADDistributionListSchema.ReportToOriginatorEnabled] = null;
		}

		internal static QueryFilter SendDeliveryReportsToFilterBuilder(SinglePropertyFilter filter)
		{
			DeliveryReportsReceiver deliveryReportsReceiver = (DeliveryReportsReceiver)ADObject.PropertyValueFromEqualityFilter(filter);
			switch (deliveryReportsReceiver)
			{
			case DeliveryReportsReceiver.None:
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.ReportToOriginatorEnabled, true),
					new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.ReportToManagerEnabled, true)
				});
			case DeliveryReportsReceiver.Manager:
				return new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ReportToManagerEnabled, true);
			case DeliveryReportsReceiver.Originator:
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ReportToOriginatorEnabled, true),
					new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.ReportToManagerEnabled, true)
				});
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValue(ADGroupSchema.SendDeliveryReportsTo.Name, deliveryReportsReceiver));
			}
		}

		internal static object IsSecurityPrincipalGetter(IPropertyBag propertyBag)
		{
			if (((MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass]).Contains("user"))
			{
				return true;
			}
			return ((GroupTypeFlags)propertyBag[ADGroupSchema.GroupType] & GroupTypeFlags.SecurityEnabled) != GroupTypeFlags.None;
		}

		internal static QueryFilter IsSecurityPrincipalFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			uint num = 2147483648U;
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new BitMaskAndFilter(ADGroupSchema.GroupType, (ulong)num),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADUser.MostDerivedClass)
			});
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static object HomeMtaServerIdGetter(IPropertyBag propertyBag)
		{
			return ADGroup.GetServerIdFromHomeMta((ADObjectId)propertyBag[ADRecipientSchema.HomeMTA]);
		}

		internal static ADObjectId GetServerIdFromHomeMta(ADObjectId homeMTA)
		{
			ADObjectId result;
			try
			{
				if (homeMTA == null || string.IsNullOrEmpty(homeMTA.DistinguishedName))
				{
					result = null;
				}
				else
				{
					if (homeMTA.Depth - homeMTA.DomainId.Depth > 8)
					{
						homeMTA = homeMTA.DescendantDN(8);
					}
					result = homeMTA;
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("HomeMtaServerId", ex.Message), ADGroupSchema.HomeMtaServerId, homeMTA), ex);
			}
			return result;
		}

		internal static QueryFilter ManagedByFilterBuilder(SinglePropertyFilter filter)
		{
			QueryFilter result;
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
				if (comparisonFilter.ComparisonOperator == ComparisonOperator.Equal)
				{
					result = new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.RawManagedBy, comparisonFilter.PropertyValue),
						new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.CoManagedBy, comparisonFilter.PropertyValue)
					});
				}
				else
				{
					if (comparisonFilter.ComparisonOperator != ComparisonOperator.NotEqual)
					{
						throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
					}
					result = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.RawManagedBy, comparisonFilter.PropertyValue),
						new ComparisonFilter(ComparisonOperator.NotEqual, ADGroupSchema.CoManagedBy, comparisonFilter.PropertyValue)
					});
				}
			}
			else
			{
				if (!(filter is ExistsFilter))
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForPropertyMultiple(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter), typeof(ExistsFilter)));
				}
				ExistsFilter existsFilter = (ExistsFilter)filter;
				result = new OrFilter(new QueryFilter[]
				{
					new ExistsFilter(ADGroupSchema.RawManagedBy),
					new ExistsFilter(ADGroupSchema.CoManagedBy)
				});
			}
			return result;
		}

		internal static object ManagedByGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = propertyBag[ADGroupSchema.RawManagedBy] as ADObjectId;
				MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
				if (adobjectId != null)
				{
					multiValuedProperty.Add(adobjectId);
				}
				MultiValuedProperty<ADObjectId> multiValuedProperty2 = propertyBag[ADGroupSchema.CoManagedBy] as MultiValuedProperty<ADObjectId>;
				foreach (ADObjectId item in multiValuedProperty2)
				{
					if (!multiValuedProperty.Contains(item))
					{
						multiValuedProperty.Add(item);
					}
				}
				multiValuedProperty.ResetChangeTracking();
				result = multiValuedProperty;
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("ManagedBy", ex.Message), ADGroupSchema.ManagedBy, propertyBag[ADGroupSchema.RawManagedBy]), ex);
			}
			return result;
		}

		internal static void ManagedBySetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)value;
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				propertyBag[ADGroupSchema.RawManagedBy] = null;
				propertyBag[ADGroupSchema.CoManagedBy] = null;
				return;
			}
			propertyBag[ADGroupSchema.RawManagedBy] = multiValuedProperty[0];
			MultiValuedProperty<ADObjectId> multiValuedProperty2 = new MultiValuedProperty<ADObjectId>();
			for (int i = 1; i < multiValuedProperty.Count; i++)
			{
				multiValuedProperty2.Add(multiValuedProperty[i]);
			}
			propertyBag[ADGroupSchema.CoManagedBy] = multiValuedProperty2;
		}

		internal static QueryFilter RoleGroupTypeFilterBuilder(SinglePropertyFilter filter)
		{
			RoleGroupType roleGroupType = (RoleGroupType)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new ExistsFilter(ADGroupSchema.ForeignGroupSid);
			QueryFilter result = new ExistsFilter(ADGroupSchema.LinkedPartnerGroupAndOrganizationId);
			if (roleGroupType == RoleGroupType.Linked)
			{
				return queryFilter;
			}
			if (roleGroupType == RoleGroupType.PartnerLinked)
			{
				return result;
			}
			return new NotFilter(queryFilter);
		}

		internal static object RoleGroupTypeGetter(IPropertyBag propertyBag)
		{
			SecurityIdentifier right = (SecurityIdentifier)propertyBag[ADGroupSchema.ForeignGroupSid];
			LinkedPartnerGroupInformation linkedPartnerGroupInformation = (LinkedPartnerGroupInformation)propertyBag[ADGroupSchema.LinkedPartnerGroupAndOrganizationId];
			if (null != right)
			{
				return RoleGroupType.Linked;
			}
			if (linkedPartnerGroupInformation != null)
			{
				return RoleGroupType.PartnerLinked;
			}
			return RoleGroupType.Standard;
		}

		internal ADPagedReader<ADRawEntry> Expand(int pageSize, params PropertyDefinition[] properties)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, base.Id);
			return base.Session.FindPagedADRawEntry(null, QueryScope.SubTree, filter, null, pageSize, properties);
		}

		ADPagedReader<ADRawEntry> IADDistributionList.Expand(int pageSize, params PropertyDefinition[] properties)
		{
			return this.Expand(pageSize, properties);
		}

		internal ADPagedReader<ADRecipient> Expand(int pageSize)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, base.Id);
			return base.Session.FindPaged(null, QueryScope.SubTree, filter, null, pageSize);
		}

		ADPagedReader<ADRecipient> IADDistributionList.Expand(int pageSize)
		{
			return this.Expand(pageSize);
		}

		internal ADPagedReader<TEntry> Expand<TEntry>(int pageSize, params PropertyDefinition[] properties) where TEntry : MiniRecipient, new()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, base.Id);
			return base.Session.FindPagedMiniRecipient<TEntry>(null, QueryScope.SubTree, filter, null, pageSize, properties);
		}

		ADPagedReader<TEntry> IADDistributionList.Expand<TEntry>(int pageSize, params PropertyDefinition[] properties)
		{
			return this.Expand<TEntry>(pageSize, properties);
		}

		internal ADPagedReader<ADRecipient> ExpandGroupOnly(int pageSize)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADGroup.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ADDynamicGroup.MostDerivedClass)
				}),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, base.Id)
			});
			return base.Session.FindPaged(null, QueryScope.SubTree, filter, null, pageSize);
		}

		internal bool ContainsMember(ADObjectId memberId, bool directOnly)
		{
			ADRecipient adrecipient = base.Session.Read(memberId);
			return adrecipient != null && adrecipient.IsMemberOf(base.Id, directOnly);
		}

		public const string SamAccountNameInvalidCharacters = "\"\\/[]:|<>+=;?,*\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\n\v\f\r\u000e\u000f\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001a\u001b\u001c\u001d\u001e\u001f";

		private static readonly ADGroupSchema schema = ObjectSchema.GetInstance<ADGroupSchema>();

		internal static string MostDerivedClass = "group";
	}
}
