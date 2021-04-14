using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class RecipientFilterHelper
	{
		internal static QueryFilter DiscoveryMailboxFilterForAuditLog(string serverLegDn)
		{
			if (RecipientFilterHelper.discoveryMailboxFilterForAuditLog == null)
			{
				RecipientFilterHelper.discoveryMailboxFilterForAuditLog = QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}"),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.ArbitrationMailbox),
					new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ServerLegacyDN, serverLegDn)
				});
			}
			return RecipientFilterHelper.discoveryMailboxFilterForAuditLog;
		}

		internal static QueryFilter DiscoveryMailboxFilterUnifiedPolicy(ADObjectId databaseId)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}"),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.ArbitrationMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, databaseId)
			});
		}

		static RecipientFilterHelper()
		{
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap = new Dictionary<ADPropertyDefinition, string>();
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute1, ADRecipientSchema.CustomAttribute1.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute2, ADRecipientSchema.CustomAttribute2.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute3, ADRecipientSchema.CustomAttribute3.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute4, ADRecipientSchema.CustomAttribute4.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute5, ADRecipientSchema.CustomAttribute5.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute6, ADRecipientSchema.CustomAttribute6.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute7, ADRecipientSchema.CustomAttribute7.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute8, ADRecipientSchema.CustomAttribute8.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute9, ADRecipientSchema.CustomAttribute9.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute10, ADRecipientSchema.CustomAttribute10.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute11, ADRecipientSchema.CustomAttribute11.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute12, ADRecipientSchema.CustomAttribute12.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute13, ADRecipientSchema.CustomAttribute13.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute14, ADRecipientSchema.CustomAttribute14.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(ADDynamicGroupSchema.ConditionalCustomAttribute15, ADRecipientSchema.CustomAttribute15.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute1, ADRecipientSchema.CustomAttribute1.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute2, ADRecipientSchema.CustomAttribute2.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute3, ADRecipientSchema.CustomAttribute3.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute4, ADRecipientSchema.CustomAttribute4.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute5, ADRecipientSchema.CustomAttribute5.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute6, ADRecipientSchema.CustomAttribute6.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute7, ADRecipientSchema.CustomAttribute7.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute8, ADRecipientSchema.CustomAttribute8.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute9, ADRecipientSchema.CustomAttribute9.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute10, ADRecipientSchema.CustomAttribute10.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute11, ADRecipientSchema.CustomAttribute11.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute12, ADRecipientSchema.CustomAttribute12.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute13, ADRecipientSchema.CustomAttribute13.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute14, ADRecipientSchema.CustomAttribute14.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(AddressBookBaseSchema.ConditionalCustomAttribute15, ADRecipientSchema.CustomAttribute15.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute1, ADRecipientSchema.CustomAttribute1.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute2, ADRecipientSchema.CustomAttribute2.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute3, ADRecipientSchema.CustomAttribute3.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute4, ADRecipientSchema.CustomAttribute4.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute5, ADRecipientSchema.CustomAttribute5.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute6, ADRecipientSchema.CustomAttribute6.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute7, ADRecipientSchema.CustomAttribute7.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute8, ADRecipientSchema.CustomAttribute8.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute9, ADRecipientSchema.CustomAttribute9.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute10, ADRecipientSchema.CustomAttribute10.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute11, ADRecipientSchema.CustomAttribute11.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute12, ADRecipientSchema.CustomAttribute12.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute13, ADRecipientSchema.CustomAttribute13.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute14, ADRecipientSchema.CustomAttribute14.Name);
			RecipientFilterHelper.ConditionalToCustomAttributeNameMap.Add(EmailAddressPolicySchema.ConditionalCustomAttribute15, ADRecipientSchema.CustomAttribute15.Name);
		}

		private static QueryFilter ConstructSimpleComparisionFilters(ICollection<string> values, ADPropertyDefinition property)
		{
			if (values == null || values.Count == 0)
			{
				return null;
			}
			List<QueryFilter> list = new List<QueryFilter>(values.Count);
			foreach (string propertyValue in values)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, property, propertyValue));
			}
			if (1 != values.Count)
			{
				return new OrFilter(list.ToArray());
			}
			return list[0];
		}

		private static List<QueryFilter> GetRecipientTypeFilter(WellKnownRecipientType? includeRecipient)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if (WellKnownRecipientType.AllRecipients == includeRecipient.Value)
			{
				list.Add(new ExistsFilter(ADRecipientSchema.Alias));
			}
			else
			{
				if (WellKnownRecipientType.MailboxUsers == (WellKnownRecipientType.MailboxUsers & includeRecipient.Value))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox));
				}
				if (WellKnownRecipientType.MailContacts == (WellKnownRecipientType.MailContacts & includeRecipient.Value))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailContact));
				}
				if (WellKnownRecipientType.MailUsers == (WellKnownRecipientType.MailUsers & includeRecipient.Value))
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser));
				}
				if (WellKnownRecipientType.MailGroups == (WellKnownRecipientType.MailGroups & includeRecipient.Value))
				{
					list.Add(new OrFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUniversalDistributionGroup),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUniversalSecurityGroup),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailNonUniversalGroup),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.DynamicDistributionGroup)
					}));
				}
				if (WellKnownRecipientType.Resources == (WellKnownRecipientType.Resources & includeRecipient.Value))
				{
					list.Add(new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
						new TextFilter(ADRecipientSchema.ResourceMetaData, "ResourceType" + ':', MatchOptions.Prefix, MatchFlags.IgnoreCase),
						new ExistsFilter(ADRecipientSchema.ResourceSearchProperties)
					}));
				}
			}
			return list;
		}

		private static void PersistPrecannedRecipientFilter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			switch (RecipientFilterHelper.GetPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, false, list))
			{
			case -1:
				break;
			case 0:
				propertyBag[filter] = string.Empty;
				propertyBag[ldapFilter] = string.Empty;
				return;
			default:
				if (list.Count > 0)
				{
					QueryFilter queryFilter = (list.Count > 1) ? new AndFilter(list.ToArray()) : list[0];
					if (isDynamicGroup)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							RecipientFilterHelper.ExcludingSystemMailboxFilter,
							RecipientFilterHelper.ExcludingCasMailboxFilter,
							RecipientFilterHelper.ExcludingMailboxPlanFilter,
							RecipientFilterHelper.ExcludingDiscoveryMailboxFilter,
							RecipientFilterHelper.ExcludingPublicFolderMailboxFilter,
							RecipientFilterHelper.ExcludingArbitrationMailboxFilter,
							RecipientFilterHelper.ExcludingAuditLogMailboxFilter
						});
					}
					propertyBag[filter] = queryFilter.GenerateInfixString(FilterLanguage.Monad);
					propertyBag[ldapFilter] = LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter);
				}
				break;
			}
		}

		private static int GetPrecannedRecipientFilter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool validationOnly, List<QueryFilter> conditions)
		{
			WellKnownRecipientType? wellKnownRecipientType = (WellKnownRecipientType?)RecipientFilterHelper.IncludeRecipientGetter(propertyBag, filterMeatadata, filter);
			bool flag = wellKnownRecipientType == null || wellKnownRecipientType == WellKnownRecipientType.None;
			if (!flag && validationOnly)
			{
				return 1;
			}
			MultiValuedProperty<string> conditionVal = (MultiValuedProperty<string>)RecipientFilterHelper.DepartmentGetter(propertyBag, filterMeatadata, filter, null);
			if (!RecipientFilterHelper.MergeConditionFilter(conditionVal, ADOrgPersonSchema.Department, conditions, flag, validationOnly))
			{
				return -1;
			}
			MultiValuedProperty<string> conditionVal2 = (MultiValuedProperty<string>)RecipientFilterHelper.CompanyGetter(propertyBag, filterMeatadata, filter, null);
			if (!RecipientFilterHelper.MergeConditionFilter(conditionVal2, ADOrgPersonSchema.Company, conditions, flag, validationOnly))
			{
				return -1;
			}
			MultiValuedProperty<string> conditionVal3 = (MultiValuedProperty<string>)RecipientFilterHelper.StateOrProvinceGetter(propertyBag, filterMeatadata, filter, null);
			if (!RecipientFilterHelper.MergeConditionFilter(conditionVal3, ADOrgPersonSchema.StateOrProvince, conditions, flag, validationOnly))
			{
				return -1;
			}
			foreach (ADPropertyDefinition adpropertyDefinition in RecipientFilterHelper.allCustomAttributePropertyDefinition)
			{
				MultiValuedProperty<string> conditionVal4 = (MultiValuedProperty<string>)RecipientFilterHelper.InternalStringValuesGetter(propertyBag, filterMeatadata, filter, null, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:" + adpropertyDefinition.Name + "=");
				if (!RecipientFilterHelper.MergeConditionFilter(conditionVal4, adpropertyDefinition, conditions, flag, validationOnly))
				{
					return -1;
				}
			}
			if (wellKnownRecipientType == WellKnownRecipientType.None)
			{
				return 0;
			}
			if (validationOnly)
			{
				return 1;
			}
			if (wellKnownRecipientType != null)
			{
				List<QueryFilter> recipientTypeFilter = RecipientFilterHelper.GetRecipientTypeFilter(wellKnownRecipientType);
				if (recipientTypeFilter.Count > 1)
				{
					conditions.Add(new OrFilter(recipientTypeFilter.ToArray()));
				}
				else
				{
					if (recipientTypeFilter.Count != 1)
					{
						return -1;
					}
					conditions.Add(recipientTypeFilter[0]);
				}
			}
			return 1;
		}

		private static bool MergeConditionFilter(MultiValuedProperty<string> conditionVal, ADPropertyDefinition conditionDef, List<QueryFilter> conditions, bool noRecipients, bool validationOnly)
		{
			if (conditionVal != null && conditionVal.Count > 0)
			{
				if (noRecipients)
				{
					return false;
				}
				if (!validationOnly)
				{
					QueryFilter queryFilter = RecipientFilterHelper.ConstructSimpleComparisionFilters(conditionVal, conditionDef);
					if (queryFilter != null)
					{
						conditions.Add(queryFilter);
					}
				}
			}
			return true;
		}

		private static bool IsValidRecipientFilterMetadata(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition includedRecipients)
		{
			return 0 <= RecipientFilterHelper.GetPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, null, true, null);
		}

		internal static ValidationError ValidatePrecannedRecipientFilter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition includedRecipients, ObjectId id)
		{
			if (!RecipientFilterHelper.IsValidRecipientFilterMetadata(propertyBag, filterMeatadata, filter, includedRecipients))
			{
				return new ObjectValidationError(DirectoryStrings.ErrorNullRecipientTypeInPrecannedFilter(includedRecipients.Name), id, string.Empty);
			}
			return null;
		}

		internal static object IncludeRecipientGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter)
		{
			WellKnownRecipientType? wellKnownRecipientType = null;
			if (WellKnownRecipientFilterType.Precanned == (WellKnownRecipientFilterType)RecipientFilterHelper.RecipientFilterTypeGetter(propertyBag, filterMeatadata, filter))
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[filterMeatadata];
				wellKnownRecipientType = new WellKnownRecipientType?(WellKnownRecipientType.None);
				foreach (string text in multiValuedProperty)
				{
					if (text.StartsWith("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:IncludedRecipients=", StringComparison.OrdinalIgnoreCase))
					{
						int value;
						if (int.TryParse(text.Substring("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:IncludedRecipients=".Length), out value))
						{
							wellKnownRecipientType = new WellKnownRecipientType?((WellKnownRecipientType)value);
							break;
						}
						wellKnownRecipientType = null;
						break;
					}
				}
			}
			return wellKnownRecipientType;
		}

		internal static void IncludeRecipientSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			RecipientFilterHelper.InternalStringValuesSetter(new MultiValuedProperty<string>(((int)((WellKnownRecipientType?)(value ?? WellKnownRecipientType.None)).Value).ToString()), propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:IncludedRecipients=");
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Precanned, propertyBag, filterMeatadata);
			RecipientFilterHelper.PersistPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, isDynamicGroup);
		}

		private static object InternalStringValuesGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition, string filterPrefix)
		{
			MultiValuedProperty<string> result = null;
			if (WellKnownRecipientFilterType.Precanned == (WellKnownRecipientFilterType)RecipientFilterHelper.RecipientFilterTypeGetter(propertyBag, filterMeatadata, filter))
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[filterMeatadata];
				Collection<string> collection = new Collection<string>();
				foreach (string text in multiValuedProperty)
				{
					if (text.StartsWith(filterPrefix, StringComparison.OrdinalIgnoreCase) && !text.Equals(filterPrefix, StringComparison.OrdinalIgnoreCase))
					{
						collection.Add(text.Substring(filterPrefix.Length));
					}
				}
				result = new MultiValuedProperty<string>(multiValuedProperty.IsReadOnly, filterPropertyDefinition, collection);
			}
			return result;
		}

		private static void InternalStringValuesSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, string filterPrefix)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[filterMeatadata];
			RecipientFilterHelper.ClearNonExchange12RecipientFilterMetadata(multiValuedProperty);
			int num = multiValuedProperty.Count - 1;
			while (0 <= num)
			{
				if (multiValuedProperty[num].StartsWith(filterPrefix, StringComparison.OrdinalIgnoreCase))
				{
					multiValuedProperty.RemoveAt(num);
				}
				num--;
			}
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)value;
			if (multiValuedProperty2 != null && multiValuedProperty2.Count != 0)
			{
				foreach (string text in multiValuedProperty2)
				{
					if (!string.IsNullOrEmpty(text))
					{
						multiValuedProperty.Add(filterPrefix + text);
					}
				}
			}
		}

		internal static object DepartmentGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition)
		{
			return RecipientFilterHelper.InternalStringValuesGetter(propertyBag, filterMeatadata, filter, filterPropertyDefinition, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Department=");
		}

		internal static void DepartmentSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			RecipientFilterHelper.InternalStringValuesSetter(value, propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Department=");
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Precanned, propertyBag, filterMeatadata);
			RecipientFilterHelper.PersistPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, isDynamicGroup);
		}

		internal static object CustomAttributeGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition)
		{
			return RecipientFilterHelper.InternalStringValuesGetter(propertyBag, filterMeatadata, filter, filterPropertyDefinition, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:" + RecipientFilterHelper.ConditionalToCustomAttributeNameMap[filterPropertyDefinition] + "=");
		}

		internal static void CustomAttributeSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			RecipientFilterHelper.InternalStringValuesSetter(value, propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:" + RecipientFilterHelper.ConditionalToCustomAttributeNameMap[filterPropertyDefinition] + "=");
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Precanned, propertyBag, filterMeatadata);
			RecipientFilterHelper.PersistPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, isDynamicGroup);
		}

		internal static object CompanyGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition)
		{
			return RecipientFilterHelper.InternalStringValuesGetter(propertyBag, filterMeatadata, filter, filterPropertyDefinition, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Company=");
		}

		internal static void CompanySetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			RecipientFilterHelper.InternalStringValuesSetter(value, propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Company=");
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Precanned, propertyBag, filterMeatadata);
			RecipientFilterHelper.PersistPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, isDynamicGroup);
		}

		internal static object StateOrProvinceGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition filterPropertyDefinition)
		{
			return RecipientFilterHelper.InternalStringValuesGetter(propertyBag, filterMeatadata, filter, filterPropertyDefinition, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:StateOrProvincePrefix=");
		}

		internal static void StateOrProvinceSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter, ADPropertyDefinition ldapFilter, bool isDynamicGroup)
		{
			RecipientFilterHelper.InternalStringValuesSetter(value, propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:StateOrProvincePrefix=");
			RecipientFilterHelper.SetRecipientFilterType(WellKnownRecipientFilterType.Precanned, propertyBag, filterMeatadata);
			RecipientFilterHelper.PersistPrecannedRecipientFilter(propertyBag, filterMeatadata, filter, ldapFilter, isDynamicGroup);
		}

		internal static object RecipientFilterTypeGetter(IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata, ADPropertyDefinition filter)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[filterMeatadata];
			string text = (string)propertyBag[filter];
			WellKnownRecipientFilterType wellKnownRecipientFilterType;
			if (((ExchangeObjectVersion)propertyBag[ADObjectSchema.ExchangeVersion]).IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				wellKnownRecipientFilterType = WellKnownRecipientFilterType.Legacy;
			}
			else
			{
				wellKnownRecipientFilterType = WellKnownRecipientFilterType.Unknown;
				int num = multiValuedProperty.Count - 1;
				while (0 <= num)
				{
					if (multiValuedProperty[num].StartsWith("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:RecipientFilterType=", StringComparison.OrdinalIgnoreCase))
					{
						int num2;
						if (!int.TryParse(multiValuedProperty[num].Substring("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:RecipientFilterType=".Length), out num2))
						{
							wellKnownRecipientFilterType = WellKnownRecipientFilterType.Unknown;
							break;
						}
						wellKnownRecipientFilterType = (WellKnownRecipientFilterType)num2;
					}
					else if (0 < multiValuedProperty[num].Length && !multiValuedProperty[num].StartsWith("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:"))
					{
						wellKnownRecipientFilterType = WellKnownRecipientFilterType.Unknown;
						break;
					}
					num--;
				}
			}
			return wellKnownRecipientFilterType;
		}

		internal static void SetRecipientFilterType(WellKnownRecipientFilterType type, IPropertyBag propertyBag, ADPropertyDefinition filterMeatadata)
		{
			if (WellKnownRecipientFilterType.Custom == type)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[filterMeatadata];
				if (0 < multiValuedProperty.Count)
				{
					multiValuedProperty.Clear();
				}
			}
			int num = (int)type;
			RecipientFilterHelper.InternalStringValuesSetter(new MultiValuedProperty<string>(num.ToString()), propertyBag, filterMeatadata, "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:RecipientFilterType=");
		}

		private static void ClearNonExchange12RecipientFilterMetadata(MultiValuedProperty<string> purportedSearchUI)
		{
			int num = purportedSearchUI.Count - 1;
			while (0 <= num)
			{
				if (!purportedSearchUI[num].StartsWith("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:", StringComparison.OrdinalIgnoreCase))
				{
					purportedSearchUI.RemoveAt(num);
				}
				num--;
			}
		}

		internal static object RecipientFilterAppliedGetter(IPropertyBag propertyBag, ADPropertyDefinition filterFlags)
		{
			return RecipientFilterableObjectFlags.FilterApplied == (RecipientFilterableObjectFlags.FilterApplied & (RecipientFilterableObjectFlags)propertyBag[filterFlags]);
		}

		internal static void RecipientFilterAppliedSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition filterFlags)
		{
			if ((bool)value)
			{
				propertyBag[filterFlags] = (RecipientFilterableObjectFlags.FilterApplied | (RecipientFilterableObjectFlags)propertyBag[filterFlags]);
				return;
			}
			propertyBag[filterFlags] = (~RecipientFilterableObjectFlags.FilterApplied & (RecipientFilterableObjectFlags)propertyBag[filterFlags]);
		}

		internal static RecipientTypeDetails RecipientTypeDetailsValueFromTextFilter(TextFilter filter)
		{
			if (filter.MatchOptions != MatchOptions.SubString)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(filter.Property.Name, filter.MatchOptions.ToString()));
			}
			RecipientTypeDetails result;
			try
			{
				result = (RecipientTypeDetails)Enum.Parse(typeof(RecipientTypeDetails), filter.Text);
			}
			catch (ArgumentException)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValue(ADRecipientSchema.RecipientTypeDetails.Name, filter.Text));
			}
			return result;
		}

		internal static bool FixExchange12RecipientFilterMetadata(IPropertyBag propertyBag, ADPropertyDefinition objectVersionProperty, ADPropertyDefinition e2003MetadataProperty, ADPropertyDefinition e12MetadataProperty, string ldapRecipientFilter)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[e2003MetadataProperty];
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[e12MetadataProperty];
			bool flag = false;
			if (ExchangeObjectVersion.Exchange2007.IsSameVersion((ExchangeObjectVersion)propertyBag[objectVersionProperty]))
			{
				if (multiValuedProperty2.Count == 0)
				{
					foreach (string text in multiValuedProperty)
					{
						if (text.StartsWith("Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:", StringComparison.OrdinalIgnoreCase))
						{
							multiValuedProperty2.Add(text);
							flag = true;
						}
					}
				}
				flag |= RecipientFilterHelper.StampE2003FilterMetadata(propertyBag, ldapRecipientFilter, e2003MetadataProperty);
			}
			return flag;
		}

		internal static bool StampE2003FilterMetadata(IPropertyBag propertyBag, string ldapRecipientFilter, ADPropertyDefinition purportedSearchUIDefinition)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[purportedSearchUIDefinition];
			bool flag;
			if (string.IsNullOrEmpty(ldapRecipientFilter))
			{
				flag = (0 != multiValuedProperty.Count);
				if (flag)
				{
					multiValuedProperty.Clear();
				}
			}
			else
			{
				string[] array = new string[]
				{
					"CommonQuery_Handler=5EE6238AC231D011891C00A024AB2DBB",
					"CommonQuery_Form=E33FEE83D957D011B93200A024AB2DBB",
					"DsQuery_ViewMode=4868",
					"DsQuery_EnableFilter=0",
					"Microsoft.PropertyWell_Items=0",
					"Microsoft.PropertyWell_QueryString=" + ldapRecipientFilter
				};
				flag = (multiValuedProperty.Count != array.Length);
				if (!flag)
				{
					foreach (string item in array)
					{
						if (!multiValuedProperty.Contains(item))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					multiValuedProperty.Clear();
					foreach (string item2 in array)
					{
						multiValuedProperty.Add(item2);
					}
				}
			}
			return flag;
		}

		internal static bool IsRecipientFilterPropertiesModified(ADObject adObject, bool isChanged)
		{
			ISupportRecipientFilter supportRecipientFilter = (ISupportRecipientFilter)adObject;
			ADPropertyDefinition[] array = new ADPropertyDefinition[]
			{
				supportRecipientFilter.RecipientFilterSchema,
				supportRecipientFilter.LdapRecipientFilterSchema,
				supportRecipientFilter.IncludedRecipientsSchema,
				supportRecipientFilter.ConditionalDepartmentSchema,
				supportRecipientFilter.ConditionalCompanySchema,
				supportRecipientFilter.ConditionalStateOrProvinceSchema,
				supportRecipientFilter.ConditionalCustomAttribute1Schema,
				supportRecipientFilter.ConditionalCustomAttribute2Schema,
				supportRecipientFilter.ConditionalCustomAttribute3Schema,
				supportRecipientFilter.ConditionalCustomAttribute4Schema,
				supportRecipientFilter.ConditionalCustomAttribute5Schema,
				supportRecipientFilter.ConditionalCustomAttribute6Schema,
				supportRecipientFilter.ConditionalCustomAttribute7Schema,
				supportRecipientFilter.ConditionalCustomAttribute8Schema,
				supportRecipientFilter.ConditionalCustomAttribute9Schema,
				supportRecipientFilter.ConditionalCustomAttribute10Schema,
				supportRecipientFilter.ConditionalCustomAttribute11Schema,
				supportRecipientFilter.ConditionalCustomAttribute12Schema,
				supportRecipientFilter.ConditionalCustomAttribute13Schema,
				supportRecipientFilter.ConditionalCustomAttribute14Schema,
				supportRecipientFilter.ConditionalCustomAttribute15Schema
			};
			foreach (ADPropertyDefinition providerPropertyDefinition in array)
			{
				if (isChanged)
				{
					if (adObject.IsChanged(providerPropertyDefinition))
					{
						return true;
					}
				}
				else if (adObject.IsModified(providerPropertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		internal const string RecipientFilterMetadataPrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:";

		internal const string RecipientFilterTypePrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:RecipientFilterType=";

		internal const string IncludeRecipientPrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:IncludedRecipients=";

		internal const string DepartmentPrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Department=";

		internal const string CompanyPrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:Company=";

		internal const string StateOrProvincePrefix = "Microsoft.Exchange12.8f91d340bc0c47e4b4058a479602f94c:StateOrProvincePrefix=";

		internal static readonly QueryFilter ExcludingSystemMailboxFilter = new NotFilter(new TextFilter(ADObjectSchema.Name, "SystemMailbox{", MatchOptions.Prefix, MatchFlags.IgnoreCase));

		internal static readonly QueryFilter ExcludingCasMailboxFilter = new NotFilter(new TextFilter(ADObjectSchema.Name, "CAS_{", MatchOptions.Prefix, MatchFlags.IgnoreCase));

		internal static readonly QueryFilter ExcludingArbitrationMailboxFilter = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.ArbitrationMailbox));

		internal static readonly QueryFilter ExcludingPublicFolderMailboxFilter = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.PublicFolderMailbox));

		internal static readonly QueryFilter MailboxPlanFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan);

		internal static readonly QueryFilter ExcludingMailboxPlanFilter = new NotFilter(RecipientFilterHelper.MailboxPlanFilter);

		internal static readonly QueryFilter ExcludingLinkedUserFilter = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.LinkedUser));

		internal static readonly QueryFilter DiscoveryMailboxFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.DiscoveryMailbox);

		internal static readonly QueryFilter ExcludingAuditLogMailboxFilter = new NotFilter(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.AuditLogMailbox));

		private static QueryFilter discoveryMailboxFilterForAuditLog;

		internal static readonly QueryFilter ExcludingDiscoveryMailboxFilter = new NotFilter(RecipientFilterHelper.DiscoveryMailboxFilter);

		private static readonly Dictionary<ADPropertyDefinition, string> ConditionalToCustomAttributeNameMap;

		private static ADPropertyDefinition[] allCustomAttributePropertyDefinition = new ADPropertyDefinition[]
		{
			ADRecipientSchema.CustomAttribute1,
			ADRecipientSchema.CustomAttribute2,
			ADRecipientSchema.CustomAttribute3,
			ADRecipientSchema.CustomAttribute4,
			ADRecipientSchema.CustomAttribute5,
			ADRecipientSchema.CustomAttribute6,
			ADRecipientSchema.CustomAttribute7,
			ADRecipientSchema.CustomAttribute8,
			ADRecipientSchema.CustomAttribute9,
			ADRecipientSchema.CustomAttribute10,
			ADRecipientSchema.CustomAttribute11,
			ADRecipientSchema.CustomAttribute12,
			ADRecipientSchema.CustomAttribute13,
			ADRecipientSchema.CustomAttribute14,
			ADRecipientSchema.CustomAttribute15
		};
	}
}
