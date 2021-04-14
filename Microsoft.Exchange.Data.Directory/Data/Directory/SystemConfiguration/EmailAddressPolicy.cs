using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class EmailAddressPolicy : ADLegacyVersionableObject, ISupportRecipientFilter
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchRecipientPolicy";
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return EmailAddressPolicy.schema;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			base.StampPersistableDefaultValues();
			if (!base.IsModified(EmailAddressPolicySchema.PolicyOptionListValue))
			{
				this[EmailAddressPolicySchema.PolicyOptionListValue] = new MultiValuedProperty<byte[]>(new object[]
				{
					EmailAddressPolicy.PolicyGuid.ToByteArray()
				});
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			ProxyAddressTemplateCollection proxyAddressTemplateCollection = (ProxyAddressTemplateCollection)this[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates];
			ProxyAddressTemplateCollection disabledEmailAddressTemplates = this.DisabledEmailAddressTemplates;
			List<ProxyAddressTemplate> list = new List<ProxyAddressTemplate>();
			list.AddRange(proxyAddressTemplateCollection);
			list.AddRange(disabledEmailAddressTemplates);
			if (list.Count > 0)
			{
				if (proxyAddressTemplateCollection != null && disabledEmailAddressTemplates != null)
				{
					foreach (ProxyAddressTemplate proxyAddressTemplate in proxyAddressTemplateCollection)
					{
						if (disabledEmailAddressTemplates.Contains(proxyAddressTemplate))
						{
							errors.Add(new ObjectValidationError(DirectoryStrings.EapDuplicatedEmailAddressTemplate(proxyAddressTemplate.ToString()), base.Id, string.Empty));
						}
					}
				}
				Dictionary<ProxyAddressPrefix, int> dictionary = new Dictionary<ProxyAddressPrefix, int>();
				foreach (ProxyAddressTemplate proxyAddressTemplate2 in list)
				{
					if (!dictionary.ContainsKey(proxyAddressTemplate2.Prefix))
					{
						dictionary[proxyAddressTemplate2.Prefix] = 0;
					}
					if (proxyAddressTemplate2.IsPrimaryAddress)
					{
						Dictionary<ProxyAddressPrefix, int> dictionary2;
						ProxyAddressPrefix prefix;
						(dictionary2 = dictionary)[prefix = proxyAddressTemplate2.Prefix] = dictionary2[prefix] + 1;
					}
				}
				foreach (ProxyAddressPrefix proxyAddressPrefix in dictionary.Keys)
				{
					if ((!(proxyAddressPrefix == ProxyAddressPrefix.Smtp) || dictionary[proxyAddressPrefix] != 0) && dictionary[proxyAddressPrefix] != 1)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.EapMustHaveOnePrimaryAddressTemplate(proxyAddressPrefix.ToString()), base.Id, string.Empty));
					}
				}
			}
			ValidationError validationError = RecipientFilterHelper.ValidatePrecannedRecipientFilter(this.propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata, EmailAddressPolicySchema.RecipientFilter, EmailAddressPolicySchema.IncludedRecipients, this.Identity);
			if (validationError != null)
			{
				errors.Add(validationError);
			}
			if (this.Priority == 0 && (base.IsChanged(EmailAddressPolicySchema.Priority) || base.ObjectState == ObjectState.New))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.CannotSetZeroAsEapPriority, EmailAddressPolicySchema.Priority, string.Empty));
			}
			if (string.IsNullOrEmpty(this.RecipientFilter) && (base.IsModified(EmailAddressPolicySchema.RecipientFilter) || base.ObjectState == ObjectState.New))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorInvalidOpathFilter(this.RecipientFilter ?? string.Empty), EmailAddressPolicySchema.RecipientFilter, string.Empty));
			}
		}

		internal IEnumerable<ADRecipient> FindMatchingRecipientsPaged(IRecipientSession recipientSession, OrganizationId organizationId, ADObjectId rootId, bool fixMissingAlias)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
			}
			bool isSearch = true;
			List<QueryFilter> filters = new List<QueryFilter>();
			filters.Add(new ExistsFilter(ADRecipientSchema.Alias));
			if (!string.IsNullOrEmpty(this.LdapRecipientFilter))
			{
				filters.Add(new CustomLdapFilter(this.LdapRecipientFilter));
			}
			QueryFilter filter = null;
			if (fixMissingAlias)
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.Alias)),
					new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchDynamicDistributionList"),
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "publicFolder"),
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchPublicMDB"),
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchSystemMailbox"),
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "msExchExchangeServerRecipient"),
						new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "exchangeAdminService")
					})
				});
				if (string.IsNullOrEmpty(this.LdapRecipientFilter))
				{
					filter = queryFilter;
				}
				else
				{
					filter = new OrFilter(new QueryFilter[]
					{
						new AndFilter(filters.ToArray()),
						queryFilter
					});
				}
			}
			else if (!string.IsNullOrEmpty(this.LdapRecipientFilter))
			{
				filter = new AndFilter(filters.ToArray());
			}
			else
			{
				isSearch = false;
			}
			if (isSearch)
			{
				ADPagedReader<ADRecipient> pagedReader = recipientSession.FindPaged(rootId ?? this.RecipientContainer, QueryScope.SubTree, filter, null, 0);
				foreach (ADRecipient item in pagedReader)
				{
					yield return item;
				}
				if (organizationId != OrganizationId.ForestWideOrgId && this.RecipientContainer == null && rootId == null)
				{
					bool originalUseConfigNC = recipientSession.UseConfigNC;
					recipientSession.UseConfigNC = !originalUseConfigNC;
					try
					{
						pagedReader = recipientSession.FindPaged(null, QueryScope.SubTree, filter, null, 0);
						foreach (ADRecipient item2 in pagedReader)
						{
							yield return item2;
						}
					}
					finally
					{
						recipientSession.UseConfigNC = originalUseConfigNC;
					}
				}
			}
			yield break;
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[EmailAddressPolicySchema.RecipientFilter];
			}
		}

		public string LdapRecipientFilter
		{
			get
			{
				return (string)this[EmailAddressPolicySchema.LdapRecipientFilter];
			}
		}

		public string LastUpdatedRecipientFilter
		{
			get
			{
				return (string)this[EmailAddressPolicySchema.LastUpdatedRecipientFilter];
			}
		}

		public bool RecipientFilterApplied
		{
			get
			{
				return (bool)this[EmailAddressPolicySchema.RecipientFilterApplied];
			}
			internal set
			{
				this[EmailAddressPolicySchema.RecipientFilterApplied] = value;
			}
		}

		[Parameter]
		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return (WellKnownRecipientType?)this[EmailAddressPolicySchema.IncludedRecipients];
			}
			set
			{
				this[EmailAddressPolicySchema.IncludedRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalDepartment];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalDepartment] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCompany];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCompany] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalStateOrProvince];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalStateOrProvince] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute1];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute1] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute2];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute2] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute3];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute3] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute4];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute4] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute5];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute5] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute6];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute6] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute7];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute7] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute8];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute8] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute9];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute9] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute10];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute10] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute11];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute11] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute12];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute12] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute13];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute13] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute14];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute14] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return (MultiValuedProperty<string>)this[EmailAddressPolicySchema.ConditionalCustomAttribute15];
			}
			set
			{
				this[EmailAddressPolicySchema.ConditionalCustomAttribute15] = value;
			}
		}

		public ADObjectId RecipientContainer
		{
			get
			{
				return (ADObjectId)this[EmailAddressPolicySchema.RecipientContainer];
			}
			set
			{
				this[EmailAddressPolicySchema.RecipientContainer] = value;
			}
		}

		public WellKnownRecipientFilterType RecipientFilterType
		{
			get
			{
				return (WellKnownRecipientFilterType)this[EmailAddressPolicySchema.RecipientFilterType];
			}
		}

		internal void SetRecipientFilter(QueryFilter filter)
		{
			if (filter == null)
			{
				this[EmailAddressPolicySchema.RecipientFilter] = string.Empty;
				this[EmailAddressPolicySchema.LdapRecipientFilter] = string.Empty;
			}
			else
			{
				this[EmailAddressPolicySchema.RecipientFilter] = filter.GenerateInfixString(FilterLanguage.Monad);
				this[EmailAddressPolicySchema.LdapRecipientFilter] = LdapFilterBuilder.LdapFilterFromQueryFilter(filter);
			}
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Custom, this.propertyBag, EmailAddressPolicySchema.RecipientFilterMetadata);
		}

		[Parameter]
		public EmailAddressPolicyPriority Priority
		{
			get
			{
				return (EmailAddressPolicyPriority)this[EmailAddressPolicySchema.Priority];
			}
			set
			{
				this[EmailAddressPolicySchema.Priority] = value;
			}
		}

		[Parameter]
		public string EnabledPrimarySMTPAddressTemplate
		{
			get
			{
				return (string)this[EmailAddressPolicySchema.EnabledPrimarySMTPAddressTemplate];
			}
			set
			{
				this[EmailAddressPolicySchema.EnabledPrimarySMTPAddressTemplate] = value;
			}
		}

		[Parameter]
		public ProxyAddressTemplateCollection EnabledEmailAddressTemplates
		{
			get
			{
				return (ProxyAddressTemplateCollection)this[EmailAddressPolicySchema.EnabledEmailAddressTemplates];
			}
			set
			{
				this[EmailAddressPolicySchema.EnabledEmailAddressTemplates] = value;
			}
		}

		[Parameter]
		public ProxyAddressTemplateCollection DisabledEmailAddressTemplates
		{
			get
			{
				return (ProxyAddressTemplateCollection)this[EmailAddressPolicySchema.DisabledEmailAddressTemplates];
			}
			set
			{
				this[EmailAddressPolicySchema.DisabledEmailAddressTemplates] = value;
			}
		}

		internal bool Enabled
		{
			get
			{
				return (bool)this[EmailAddressPolicySchema.Enabled];
			}
		}

		public bool HasEmailAddressSetting
		{
			get
			{
				return (bool)this[EmailAddressPolicySchema.HasEmailAddressSetting];
			}
		}

		public bool HasMailboxManagerSetting
		{
			get
			{
				return (bool)this[EmailAddressPolicySchema.HasMailboxManagerSetting];
			}
		}

		public ProxyAddressTemplateCollection NonAuthoritativeDomains
		{
			get
			{
				return (ProxyAddressTemplateCollection)this[EmailAddressPolicySchema.NonAuthoritativeDomains];
			}
			internal set
			{
				this[EmailAddressPolicySchema.NonAuthoritativeDomains] = value;
			}
		}

		public string AdminDescription
		{
			get
			{
				return (string)this[EmailAddressPolicySchema.AdminDescription];
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.RecipientFilterSchema
		{
			get
			{
				return EmailAddressPolicySchema.RecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.LdapRecipientFilterSchema
		{
			get
			{
				return EmailAddressPolicySchema.LdapRecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.IncludedRecipientsSchema
		{
			get
			{
				return EmailAddressPolicySchema.IncludedRecipients;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalDepartmentSchema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalDepartment;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCompanySchema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCompany;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalStateOrProvinceSchema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalStateOrProvince;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute1Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute1;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute2Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute2;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute3Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute3;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute4Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute4;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute5Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute5;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute6Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute6;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute7Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute7;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute8Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute8;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute9Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute9;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute10Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute10;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute11Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute11;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute12Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute12;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute13Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute13;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute14Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute14;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute15Schema
		{
			get
			{
				return EmailAddressPolicySchema.ConditionalCustomAttribute15;
			}
		}

		internal static object EnabledEmailAddressTemplatesGetter(IPropertyBag propertyBag)
		{
			MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)propertyBag[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates];
			return multiValuedPropertyBase.Clone();
		}

		internal static void EnabledEmailAddressTemplatesSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates] = value;
		}

		internal static object EnabledPrimarySMTPAddressTemplateGetter(IPropertyBag propertyBag)
		{
			ProxyAddressTemplateCollection proxyAddressTemplateCollection = (ProxyAddressTemplateCollection)propertyBag[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates];
			ProxyAddressTemplate proxyAddressTemplate = proxyAddressTemplateCollection.FindPrimary(ProxyAddressPrefix.Smtp);
			if (!(null == proxyAddressTemplate))
			{
				return proxyAddressTemplate.AddressTemplateString;
			}
			return string.Empty;
		}

		internal static void EnabledPrimarySMTPAddressTemplateSetter(object value, IPropertyBag propertyBag)
		{
			ProxyAddressTemplateCollection proxyAddressTemplateCollection = (ProxyAddressTemplateCollection)propertyBag[EmailAddressPolicySchema.RawEnabledEmailAddressTemplates];
			ProxyAddressTemplate proxyAddressTemplate = null;
			try
			{
				proxyAddressTemplate = ProxyAddressTemplate.Parse((string)value);
			}
			catch (ArgumentException ex)
			{
				throw new DataValidationException(new PropertyValidationError(new LocalizedString(ex.Message), EmailAddressPolicySchema.EnabledPrimarySMTPAddressTemplate, null), ex);
			}
			if (!(proxyAddressTemplate is SmtpProxyAddressTemplate))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorPrimarySmtpTemplateInvalid((string)value), EmailAddressPolicySchema.EnabledPrimarySMTPAddressTemplate, value), null);
			}
			ProxyAddressTemplate proxyAddressTemplate2 = proxyAddressTemplateCollection.FindPrimary(ProxyAddressPrefix.Smtp);
			if (proxyAddressTemplate2 != null && !StringComparer.CurrentCultureIgnoreCase.Equals(proxyAddressTemplate.AddressTemplateString, proxyAddressTemplate2.AddressTemplateString))
			{
				proxyAddressTemplateCollection.Remove(proxyAddressTemplate2);
			}
			proxyAddressTemplateCollection.MakePrimary(proxyAddressTemplate);
		}

		internal static bool IsOfPolicyType(MultiValuedProperty<byte[]> values, Guid policyGuid)
		{
			if (values != null)
			{
				byte[] array = policyGuid.ToByteArray();
				foreach (byte[] array2 in values)
				{
					if (array2.Length == array.Length)
					{
						bool flag = true;
						int num = 0;
						while (array.Length > num)
						{
							if (array2[num] != array[num])
							{
								flag = false;
								break;
							}
							num++;
						}
						if (flag)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private const string MostDerivedObjectClassInternal = "msExchRecipientPolicy";

		public static readonly string DefaultName = "Default Policy";

		internal static readonly Guid MailboxSettingPolicyGuid = new Guid("3b6813ec-ce89-42ba-9442-d87d4aa30dbc");

		internal static readonly Guid PolicyGuid = new Guid("26491CFC-9E50-4857-861B-0CB8DF22B5D7");

		public static readonly IComparer<EmailAddressPolicy> PriorityComparer = new EmailAddressPolicy.InternalPriorityComparer();

		private static EmailAddressPolicySchema schema = ObjectSchema.GetInstance<EmailAddressPolicySchema>();

		public static QueryFilter RecipientFilterForDefaultPolicy = new ExistsFilter(ADRecipientSchema.Alias);

		public static readonly ADObjectId RdnEapContainerToOrganization = new ADObjectId("CN=Recipient Policies");

		private class InternalPriorityComparer : IComparer, IComparer<EmailAddressPolicy>
		{
			int IComparer.Compare(object x, object y)
			{
				return ((IComparer<EmailAddressPolicy>)this).Compare((EmailAddressPolicy)x, (EmailAddressPolicy)y);
			}

			int IComparer<EmailAddressPolicy>.Compare(EmailAddressPolicy x, EmailAddressPolicy y)
			{
				if (x == null)
				{
					if (y != null)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (y != null)
					{
						return x.Priority - y.Priority;
					}
					return 1;
				}
			}
		}
	}
}
