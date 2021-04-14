using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADRecipient : ADObject, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADRecipientProperties.Instance;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.ExceptionMostDerivedOnBase("ADRecipient"));
			}
		}

		internal override bool SkipPiiRedaction
		{
			get
			{
				return ADRecipient.IsSystemMailbox(this.RecipientTypeDetails);
			}
		}

		public virtual string ObjectCategoryCN
		{
			get
			{
				return this.ObjectCategoryName;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return Filters.DefaultRecipientFilter;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ADRecipient.PublicFolderMailboxObjectVersion;
			}
		}

		internal ADRecipient(IRecipientSession session, PropertyBag propertyBag)
		{
			this.m_Session = session;
			this.propertyBag = (ADPropertyBag)propertyBag;
			this.SetIsReadOnly(session.ReadOnly);
		}

		public ADRecipient()
		{
		}

		internal static object MailboxRelationTypeGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADUserSchema.AuxMailboxParentObjectId];
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[ADUserSchema.AuxMailboxParentObjectIdBL];
			if (adobjectId != null && multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorInvalidMailboxRelationType, ADUserSchema.MailboxRelationType, string.Format("Parent ADObjectId: {0}, Aux Mailboxes: {1}", adobjectId, multiValuedProperty)));
			}
			MailboxRelationType mailboxRelationType = MailboxRelationType.None;
			if (adobjectId != null)
			{
				mailboxRelationType = MailboxRelationType.Primary;
			}
			else if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				mailboxRelationType = MailboxRelationType.Secondary;
			}
			return mailboxRelationType;
		}

		internal static object AntispamBypassEnabledGetter(IPropertyBag propertyBag)
		{
			MessageHygieneFlags messageHygieneFlags = (MessageHygieneFlags)propertyBag[ADRecipientSchema.MessageHygieneFlags];
			return BoxedConstants.GetBool((messageHygieneFlags & MessageHygieneFlags.AntispamBypass) == MessageHygieneFlags.AntispamBypass);
		}

		internal static void AntispamBypassEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			MessageHygieneFlags messageHygieneFlags = (MessageHygieneFlags)propertyBag[ADRecipientSchema.MessageHygieneFlags];
			if (flag)
			{
				propertyBag[ADRecipientSchema.MessageHygieneFlags] = (messageHygieneFlags | MessageHygieneFlags.AntispamBypass);
				return;
			}
			propertyBag[ADRecipientSchema.MessageHygieneFlags] = (messageHygieneFlags & ~MessageHygieneFlags.AntispamBypass);
		}

		internal static object UseMapiRichTextFormatGetter(IPropertyBag propertyBag)
		{
			bool? flag = (bool?)propertyBag[ADRecipientSchema.MapiRecipient];
			UseMapiRichTextFormat useMapiRichTextFormat;
			if (flag == null)
			{
				useMapiRichTextFormat = UseMapiRichTextFormat.UseDefaultSettings;
			}
			else if (flag == true)
			{
				useMapiRichTextFormat = UseMapiRichTextFormat.Always;
			}
			else
			{
				useMapiRichTextFormat = UseMapiRichTextFormat.Never;
			}
			return useMapiRichTextFormat;
		}

		internal static void UseMapiRichTextFormatSetter(object value, IPropertyBag propertyBag)
		{
			switch ((UseMapiRichTextFormat)value)
			{
			case UseMapiRichTextFormat.Never:
				propertyBag[ADRecipientSchema.MapiRecipient] = false;
				return;
			case UseMapiRichTextFormat.Always:
				propertyBag[ADRecipientSchema.MapiRecipient] = true;
				return;
			case UseMapiRichTextFormat.UseDefaultSettings:
				propertyBag[ADRecipientSchema.MapiRecipient] = null;
				return;
			default:
				return;
			}
		}

		internal static string OrganizationUnitFromADObjectId(ADObjectId id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			StringBuilder stringBuilder = new StringBuilder();
			ADObjectId parent = id.Parent;
			if (parent != null && parent.DistinguishedName.IndexOf("DC=", StringComparison.OrdinalIgnoreCase) != -1)
			{
				ADObjectId domainId = parent.DomainId;
				stringBuilder.Append(DNConvertor.FqdnFromDomainDistinguishedName(domainId.DistinguishedName));
				for (int i = 1; i <= parent.Depth - domainId.Depth; i++)
				{
					ADObjectId adobjectId = parent.DescendantDN(i);
					string prefix = adobjectId.Rdn.Prefix;
					if (prefix.Equals("OU", StringComparison.OrdinalIgnoreCase) || prefix.Equals("CN", StringComparison.OrdinalIgnoreCase))
					{
						stringBuilder.Append('/');
						stringBuilder.Append(adobjectId.Rdn.UnescapedName);
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal static object OUGetter(IPropertyBag propertyBag)
		{
			Exception ex = null;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId != null)
				{
					return ADRecipient.OrganizationUnitFromADObjectId(adobjectId);
				}
				ex = new ArgumentNullException(DirectoryStrings.ExArgumentNullException("Id"));
			}
			catch (InvalidOperationException ex2)
			{
				ex = ex2;
			}
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("OU", ex.Message), ADRecipientSchema.OrganizationalUnit, propertyBag[ADObjectSchema.Id]), ex);
		}

		internal static GetterDelegate OwaProtocolSettingsGetterDelegate()
		{
			return delegate(IPropertyBag propertyBag)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ProtocolSettings];
				string text = null;
				string text2 = null;
				foreach (string text3 in multiValuedProperty)
				{
					if (text3.StartsWith("OWA"))
					{
						text2 = text3;
					}
					else if (text3.StartsWith("HTTP"))
					{
						text = text3;
					}
				}
				string text4 = (text2 == null) ? text : text2;
				bool flag;
				if (text4 == null)
				{
					flag = true;
				}
				else
				{
					string[] array = text4.Split(new char[]
					{
						'§'
					});
					flag = (array.Length <= 1 || array[1].Equals("1"));
				}
				return flag;
			};
		}

		internal static SetterDelegate OwaProtocolSettingsSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ProtocolSettings];
				for (int i = 0; i < multiValuedProperty.Count; i++)
				{
					if (multiValuedProperty[i].StartsWith("OWA", StringComparison.OrdinalIgnoreCase) || multiValuedProperty[i].StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
					{
						multiValuedProperty.RemoveAt(i);
						i--;
					}
				}
				if ((bool)value)
				{
					multiValuedProperty.Add("OWA§1");
					multiValuedProperty.Add("HTTP§1§1§§§§§§");
					return;
				}
				multiValuedProperty.Add("OWA§0");
				multiValuedProperty.Add("HTTP§0§1§§§§§§");
			};
		}

		internal static object CommonNameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static object JournalArchiveAddressGetter(IReadOnlyPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddresses = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			return ADRecipient.JournalArchiveAddressInternalGetter(proxyAddresses);
		}

		private static SmtpAddress JournalArchiveAddressInternalGetter(ProxyAddressCollection proxyAddresses)
		{
			ProxyAddress proxyAddress;
			return ADRecipient.JournalArchiveAddressInternalGetterWithProxyAddress(proxyAddresses, out proxyAddress);
		}

		private static SmtpAddress JournalArchiveAddressInternalGetterWithProxyAddress(ProxyAddressCollection proxyAddresses, out ProxyAddress journalArchiveProxyAddress)
		{
			List<ProxyAddress> list = new List<ProxyAddress>();
			foreach (ProxyAddress proxyAddress in proxyAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.JRNL)
				{
					list.Add(proxyAddress);
				}
			}
			if (list.Count != 1 || list[0] is InvalidProxyAddress)
			{
				journalArchiveProxyAddress = null;
				return SmtpAddress.Empty;
			}
			journalArchiveProxyAddress = list[0];
			return new SmtpAddress(list[0].AddressString);
		}

		internal static void JournalArchiveAddressSetter(object value, IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			SmtpAddress smtpAddress = (SmtpAddress)value;
			if (!smtpAddress.IsValidAddress)
			{
				throw new FormatException(DataStrings.InvalidSmtpAddress(smtpAddress.ToString()));
			}
			ProxyAddress proxyAddress;
			SmtpAddress value2 = ADRecipient.JournalArchiveAddressInternalGetterWithProxyAddress(proxyAddressCollection, out proxyAddress);
			ProxyAddress item = ProxyAddress.Parse(ProxyAddressPrefix.JRNL.PrimaryPrefix, smtpAddress.ToString());
			if (smtpAddress == SmtpAddress.NullReversePath)
			{
				if (proxyAddress != null)
				{
					proxyAddressCollection.Remove(proxyAddress);
					return;
				}
			}
			else
			{
				if (value2 == SmtpAddress.Empty)
				{
					proxyAddressCollection.Add(item);
					return;
				}
				if (value2.CompareTo(smtpAddress) != 0)
				{
					proxyAddressCollection.Remove(proxyAddress);
					proxyAddressCollection.Add(item);
				}
			}
		}

		internal static QueryFilter JournalArchiveAddressFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			string str = ProxyAddressPrefix.JRNL + ":";
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, str + comparisonFilter.PropertyValue.ToString());
			case ComparisonOperator.NotEqual:
				return new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.EmailAddresses, str + comparisonFilter.PropertyValue.ToString());
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static object PrimarySmtpAddressGetter(IReadOnlyPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			List<ProxyAddress> list = new List<ProxyAddress>();
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
				{
					list.Add(proxyAddress);
				}
			}
			if (list.Count != 1 || list[0] is InvalidProxyAddress)
			{
				return SmtpAddress.Empty;
			}
			return new SmtpAddress(list[0].AddressString);
		}

		internal static void PrimarySmtpAddressSetter(object value, IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			SmtpAddress smtpAddress = (SmtpAddress)value;
			if (!smtpAddress.IsValidAddress)
			{
				throw new FormatException(DataStrings.InvalidSmtpAddress(smtpAddress.ToString()));
			}
			proxyAddressCollection.MakePrimary(ProxyAddress.Parse(smtpAddress.ToString()));
		}

		internal static QueryFilter PrimarySmtpAddressFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "SMTP:" + comparisonFilter.PropertyValue.ToString());
			case ComparisonOperator.NotEqual:
				return new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.EmailAddresses, "SMTP:" + comparisonFilter.PropertyValue.ToString());
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
		}

		internal static object EmailAddressPolicyEnabledGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesIncluded];
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesExcluded];
			string text = EmailAddressPolicy.PolicyGuid.ToString("B");
			if (!multiValuedProperty2.Contains(text))
			{
				foreach (string text2 in multiValuedProperty)
				{
					if (-1 != text2.IndexOf(text, StringComparison.OrdinalIgnoreCase))
					{
						return BoxedConstants.True;
					}
				}
			}
			return BoxedConstants.False;
		}

		internal static void EmailAddressPolicyEnabledSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesIncluded];
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesExcluded];
			string text = EmailAddressPolicy.PolicyGuid.ToString("B");
			if ((bool)value)
			{
				if (multiValuedProperty2.Contains(text))
				{
					multiValuedProperty2.Remove(text);
				}
				bool flag = false;
				foreach (string text2 in multiValuedProperty)
				{
					if (-1 != text2.IndexOf(text, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					multiValuedProperty.Add(text);
					return;
				}
			}
			else if (!multiValuedProperty2.Contains(text))
			{
				multiValuedProperty2.Add(text);
			}
		}

		internal static QueryFilter EmailAddressPolicyEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			string text = EmailAddressPolicy.PolicyGuid.ToString("B");
			return ADObject.BoolFilterBuilder(filter, new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.PoliciesExcluded, text),
				new TextFilter(ADRecipientSchema.PoliciesIncluded, text, MatchOptions.SubString, MatchFlags.IgnoreCase)
			}));
		}

		internal static object ReadOnlyAddressListMembershipGetter(IPropertyBag propertyBag)
		{
			return new MultiValuedProperty<ADObjectId>(true, ADRecipientSchema.ReadOnlyAddressListMembership, ((MultiValuedProperty<ADObjectId>)propertyBag[ADRecipientSchema.AddressListMembership]).ToArray());
		}

		internal static object ReadOnlyPoliciesIncludedGetter(IPropertyBag propertyBag)
		{
			return new MultiValuedProperty<string>(true, ADRecipientSchema.ReadOnlyPoliciesIncluded, ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesIncluded]).ToArray());
		}

		internal static object ReadOnlyPoliciesExcludedGetter(IPropertyBag propertyBag)
		{
			return new MultiValuedProperty<string>(true, ADRecipientSchema.ReadOnlyPoliciesExcluded, ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.PoliciesExcluded]).ToArray());
		}

		internal static object ReadOnlyProtocolSettingsGetter(IPropertyBag propertyBag)
		{
			return new MultiValuedProperty<string>(true, ADRecipientSchema.ReadOnlyProtocolSettings, ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ProtocolSettings]).ToArray());
		}

		internal static object RecipientTypeGetter(IPropertyBag propertyBag)
		{
			RecipientType recipientType = RecipientType.Invalid;
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			bool flag = !string.IsNullOrEmpty((string)propertyBag[ADRecipientSchema.Alias]);
			GroupTypeFlags groupTypeFlags = (GroupTypeFlags)propertyBag[ADGroupSchema.GroupType];
			if (multiValuedProperty.Contains("computer"))
			{
				recipientType = RecipientType.Computer;
			}
			else if (multiValuedProperty.Contains("user"))
			{
				if (flag && !string.IsNullOrEmpty((string)propertyBag[IADMailStorageSchema.ServerLegacyDN]))
				{
					ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
					if (adobjectId == null && (ObjectState)propertyBag[ADObjectSchema.ObjectState] != ObjectState.New)
					{
						throw new DataValidationException(new PropertyValidationError(DirectoryStrings.IdIsNotSet, ADRecipientSchema.RecipientType, null));
					}
					if (adobjectId != null && adobjectId.Parent.Rdn.UnescapedName.Equals("Microsoft Exchange System Objects", StringComparison.OrdinalIgnoreCase))
					{
						recipientType = RecipientType.SystemMailbox;
					}
					else
					{
						recipientType = RecipientType.UserMailbox;
					}
				}
				else if (flag)
				{
					recipientType = RecipientType.MailUser;
				}
				else
				{
					recipientType = RecipientType.User;
				}
			}
			else if (multiValuedProperty.Contains("contact"))
			{
				recipientType = (flag ? RecipientType.MailContact : RecipientType.Contact);
			}
			else if (multiValuedProperty.Contains("group"))
			{
				if (!flag)
				{
					recipientType = RecipientType.Group;
				}
				else if ((groupTypeFlags & GroupTypeFlags.Universal) == GroupTypeFlags.None)
				{
					recipientType = RecipientType.MailNonUniversalGroup;
				}
				else if ((groupTypeFlags & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled)
				{
					recipientType = RecipientType.MailUniversalSecurityGroup;
				}
				else
				{
					recipientType = RecipientType.MailUniversalDistributionGroup;
				}
			}
			else if (multiValuedProperty.Contains("msExchDynamicDistributionList"))
			{
				recipientType = (flag ? RecipientType.DynamicDistributionGroup : RecipientType.Invalid);
			}
			else if (multiValuedProperty.Contains(ADPublicFolder.MostDerivedClass))
			{
				recipientType = (flag ? RecipientType.PublicFolder : RecipientType.Invalid);
			}
			else if (multiValuedProperty.Contains(ADSystemAttendantMailbox.MostDerivedClass))
			{
				recipientType = (flag ? RecipientType.SystemAttendantMailbox : RecipientType.Invalid);
			}
			else if (multiValuedProperty.Contains(ADSystemMailbox.MostDerivedClass))
			{
				recipientType = ((flag && !string.IsNullOrEmpty((string)propertyBag[IADMailStorageSchema.ServerLegacyDN])) ? RecipientType.SystemMailbox : RecipientType.Invalid);
			}
			else if (multiValuedProperty.Contains(ADMicrosoftExchangeRecipient.MostDerivedClass))
			{
				recipientType = (flag ? RecipientType.MicrosoftExchange : RecipientType.Invalid);
			}
			else if (multiValuedProperty.Contains("msExchPublicMDB"))
			{
				recipientType = (flag ? RecipientType.PublicDatabase : RecipientType.Invalid);
			}
			return recipientType;
		}

		internal static object RecipientTypeDetailsGetter(IPropertyBag propertyBag)
		{
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)propertyBag[ADRecipientSchema.RecipientTypeDetailsValue];
			if (recipientTypeDetails == RecipientTypeDetails.None)
			{
				switch ((RecipientType)propertyBag[ADRecipientSchema.RecipientType])
				{
				case RecipientType.User:
				{
					UserAccountControlFlags userAccountControlFlags = (UserAccountControlFlags)propertyBag[ADUserSchema.UserAccountControl];
					recipientTypeDetails = (((userAccountControlFlags & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.AccountDisabled) ? RecipientTypeDetails.DisabledUser : RecipientTypeDetails.User);
					break;
				}
				case RecipientType.UserMailbox:
					recipientTypeDetails = RecipientTypeDetails.LegacyMailbox;
					break;
				case RecipientType.MailUser:
					recipientTypeDetails = RecipientTypeDetails.MailUser;
					break;
				case RecipientType.Contact:
					recipientTypeDetails = RecipientTypeDetails.Contact;
					break;
				case RecipientType.MailContact:
					recipientTypeDetails = RecipientTypeDetails.MailContact;
					break;
				case RecipientType.Group:
				{
					GroupTypeFlags groupTypeFlags = (GroupTypeFlags)propertyBag[ADGroupSchema.GroupType];
					if ((groupTypeFlags & GroupTypeFlags.Universal) == GroupTypeFlags.Universal)
					{
						recipientTypeDetails = (((groupTypeFlags & GroupTypeFlags.SecurityEnabled) == GroupTypeFlags.SecurityEnabled) ? RecipientTypeDetails.UniversalSecurityGroup : RecipientTypeDetails.UniversalDistributionGroup);
					}
					else
					{
						recipientTypeDetails = RecipientTypeDetails.NonUniversalGroup;
					}
					break;
				}
				case RecipientType.MailUniversalDistributionGroup:
					recipientTypeDetails = RecipientTypeDetails.MailUniversalDistributionGroup;
					break;
				case RecipientType.MailUniversalSecurityGroup:
					recipientTypeDetails = RecipientTypeDetails.MailUniversalSecurityGroup;
					break;
				case RecipientType.MailNonUniversalGroup:
					recipientTypeDetails = RecipientTypeDetails.MailNonUniversalGroup;
					break;
				case RecipientType.DynamicDistributionGroup:
					recipientTypeDetails = RecipientTypeDetails.DynamicDistributionGroup;
					break;
				case RecipientType.PublicFolder:
					recipientTypeDetails = RecipientTypeDetails.PublicFolder;
					break;
				case RecipientType.SystemAttendantMailbox:
					recipientTypeDetails = RecipientTypeDetails.SystemAttendantMailbox;
					break;
				case RecipientType.SystemMailbox:
					recipientTypeDetails = RecipientTypeDetails.SystemMailbox;
					break;
				case RecipientType.MicrosoftExchange:
					recipientTypeDetails = RecipientTypeDetails.MicrosoftExchange;
					break;
				case RecipientType.Computer:
					recipientTypeDetails = RecipientTypeDetails.Computer;
					break;
				}
			}
			return recipientTypeDetails;
		}

		internal static void RecipientTypeDetailsSetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				long num = (long)((RecipientTypeDetails)value & RecipientTypeDetails.AllUniqueRecipientTypes);
				if ((num - 1L & num) != 0L)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorTwoOrMoreUniqueRecipientTypes(value.ToString()), ADRecipientSchema.RecipientTypeDetails, value));
				}
			}
			propertyBag[ADRecipientSchema.RecipientTypeDetailsValue] = value;
		}

		internal static object RecipientTypeDetailsRawGetter(IPropertyBag propertyBag)
		{
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)propertyBag[ADRecipientSchema.RecipientTypeDetailsValue];
			return (long)recipientTypeDetails;
		}

		internal static QueryFilter HiddenFromAddressListsEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsValue, true));
		}

		internal static object HiddenFromAddressListsEnabledGetter(IPropertyBag propertyBag)
		{
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)propertyBag[ADRecipientSchema.RecipientTypeDetailsValue];
			int num = (int)(propertyBag[ADRecipientSchema.RecipientSoftDeletedStatus] ?? 0);
			if (recipientTypeDetails == RecipientTypeDetails.MailboxPlan || num != 0)
			{
				return true;
			}
			return propertyBag[ADRecipientSchema.HiddenFromAddressListsValue];
		}

		internal static void HiddenFromAddressListsEnabledSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADRecipientSchema.HiddenFromAddressListsValue] = value;
		}

		internal static object DefaultMailTipGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> translations = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.MailTipTranslations];
			return ADRecipient.DefaultMailTipGetter(translations);
		}

		internal static object DefaultMailTipGetter(MultiValuedProperty<string> translations)
		{
			foreach (string text in translations)
			{
				if (ADRecipient.IsDefaultTranslation(text))
				{
					return text.Substring("default".Length + 1);
				}
			}
			return null;
		}

		internal static bool IsAllowedDeliveryRestrictionGroup(RecipientType type)
		{
			return type == RecipientType.MailUniversalDistributionGroup || type == RecipientType.MailUniversalSecurityGroup || type == RecipientType.MailNonUniversalGroup || type == RecipientType.DynamicDistributionGroup;
		}

		internal static bool IsAllowedDeliveryRestrictionIndividual(RecipientType type)
		{
			return type == RecipientType.UserMailbox || type == RecipientType.MailUser || type == RecipientType.MailContact || type == RecipientType.MicrosoftExchange;
		}

		internal static void DefaultMailTipSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.MailTipTranslations];
			if (string.IsNullOrEmpty(value as string))
			{
				multiValuedProperty.Clear();
				return;
			}
			string text = "default:" + value;
			for (int i = 0; i < multiValuedProperty.Count; i++)
			{
				string translation = multiValuedProperty[i];
				if (ADRecipient.IsDefaultTranslation(translation))
				{
					multiValuedProperty[i] = text;
					return;
				}
			}
			multiValuedProperty.Add(text);
		}

		internal static object IsPersonToPersonTextMessagingEnabledGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<TextMessagingStateBase> multiValuedProperty = (MultiValuedProperty<TextMessagingStateBase>)propertyBag[ADRecipientSchema.TextMessagingState];
			foreach (TextMessagingStateBase textMessagingStateBase in multiValuedProperty)
			{
				TextMessagingDeliveryPointState textMessagingDeliveryPointState = textMessagingStateBase as TextMessagingDeliveryPointState;
				if (textMessagingDeliveryPointState != null && !textMessagingDeliveryPointState.Shared && textMessagingDeliveryPointState.PersonToPersonMessagingEnabled)
				{
					return BoxedConstants.True;
				}
			}
			return BoxedConstants.False;
		}

		internal static QueryFilter IsPersonToPersonTextMessagingEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter;
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				queryFilter = new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 536870912UL);
				break;
			case ComparisonOperator.NotEqual:
				queryFilter = new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 536870912UL));
				break;
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			return new AndFilter(new QueryFilter[]
			{
				new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, (ulong)int.MinValue)),
				new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 1073741824UL)),
				queryFilter
			});
		}

		internal static object IsMachineToPersonTextMessagingEnabledGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<TextMessagingStateBase> multiValuedProperty = (MultiValuedProperty<TextMessagingStateBase>)propertyBag[ADRecipientSchema.TextMessagingState];
			foreach (TextMessagingStateBase textMessagingStateBase in multiValuedProperty)
			{
				TextMessagingDeliveryPointState textMessagingDeliveryPointState = textMessagingStateBase as TextMessagingDeliveryPointState;
				if (textMessagingDeliveryPointState != null && !textMessagingDeliveryPointState.Shared && textMessagingDeliveryPointState.MachineToPersonMessagingEnabled)
				{
					return BoxedConstants.True;
				}
			}
			return BoxedConstants.False;
		}

		internal static QueryFilter IsMachineToPersonTextMessagingEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
			QueryFilter queryFilter;
			switch (comparisonFilter.ComparisonOperator)
			{
			case ComparisonOperator.Equal:
				queryFilter = new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 268435456UL);
				break;
			case ComparisonOperator.NotEqual:
				queryFilter = new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 268435456UL));
				break;
			default:
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
			}
			return new AndFilter(new QueryFilter[]
			{
				new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, (ulong)int.MinValue)),
				new NotFilter(new BitMaskAndFilter(ADRecipientSchema.TextMessagingState, 1073741824UL)),
				queryFilter
			});
		}

		internal static bool IsDefaultTranslation(string translation)
		{
			return translation.StartsWith("default:", StringComparison.OrdinalIgnoreCase);
		}

		internal static QueryFilter RecipientTypeDetailsFilterBuilder(SinglePropertyFilter filter)
		{
			RecipientTypeDetails recipientTypeDetails;
			if (filter is TextFilter)
			{
				recipientTypeDetails = RecipientFilterHelper.RecipientTypeDetailsValueFromTextFilter(filter as TextFilter);
			}
			else
			{
				recipientTypeDetails = (RecipientTypeDetails)ADObject.PropertyValueFromEqualityFilter(filter);
			}
			QueryFilter recipientTypeDetailsFilterOptimization = Filters.GetRecipientTypeDetailsFilterOptimization(recipientTypeDetails);
			if (recipientTypeDetailsFilterOptimization != null)
			{
				ExTraceGlobals.LdapFilterBuilderTracer.TraceDebug<QueryFilter, SinglePropertyFilter>(0L, "ADRecipient.RecipientTypeDetailsFilterBuilder:  RecipientTypeDetailsFilterBuilder found an optimized filter for RecipientTypeDetails. Will use {0} instead of {1}", recipientTypeDetailsFilterOptimization, filter);
				return recipientTypeDetailsFilterOptimization;
			}
			List<QueryFilter> list = new List<QueryFilter>(16);
			for (long num = 1L; num != 0L; num <<= 1)
			{
				RecipientTypeDetails recipientTypeDetails2 = (RecipientTypeDetails)num;
				if ((recipientTypeDetails2 & RecipientTypeDetails.AllUniqueRecipientTypes) != RecipientTypeDetails.None && (recipientTypeDetails2 & recipientTypeDetails) != RecipientTypeDetails.None)
				{
					if ((recipientTypeDetails & (RecipientTypeDetails)(-17592186044416L)) != RecipientTypeDetails.None)
					{
						throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValue(ADRecipientSchema.RecipientTypeDetails.Name, recipientTypeDetails));
					}
					recipientTypeDetailsFilterOptimization = Filters.GetRecipientTypeDetailsFilterOptimization(recipientTypeDetails2);
					if (recipientTypeDetailsFilterOptimization == null)
					{
						throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValue(ADRecipientSchema.RecipientTypeDetails.Name, recipientTypeDetails));
					}
					list.Add(recipientTypeDetailsFilterOptimization);
				}
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return new OrFilter(list.ToArray());
		}

		internal static QueryFilter LitigationHoldFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADUserSchema.ElcMailboxFlags, 8UL));
		}

		internal static QueryFilter SingleItemRecoveryFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADUserSchema.ElcMailboxFlags, 16UL));
		}

		internal static QueryFilter RetentionPolicySetFilterBuilder()
		{
			return new BitMaskAndFilter(IADMailStorageSchema.ElcMailboxFlags, 2UL);
		}

		internal static QueryFilter RetentionPolicyFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new AndFilter(new QueryFilter[]
				{
					ADRecipient.RetentionPolicySetFilterBuilder(),
					new ExistsFilter(IADMailStorageSchema.ElcPolicyTemplate)
				});
			}
			ObjectId propertyValue = (ObjectId)ADObject.PropertyValueFromComparisonFilter(filter, new List<ComparisonOperator>
			{
				ComparisonOperator.Equal,
				ComparisonOperator.NotEqual
			});
			return new AndFilter(new QueryFilter[]
			{
				ADRecipient.RetentionPolicySetFilterBuilder(),
				new ComparisonFilter(ComparisonOperator.Equal, IADMailStorageSchema.ElcPolicyTemplate, propertyValue)
			});
		}

		internal static object ShouldUseDefaultRetentionPolicyGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			return BoxedConstants.GetBool((elcMailboxFlags & ElcMailboxFlags.ShouldUseDefaultRetentionPolicy) == ElcMailboxFlags.ShouldUseDefaultRetentionPolicy);
		}

		internal static void ShouldUseDefaultRetentionPolicySetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)(value ?? false);
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[ADUserSchema.ElcMailboxFlags];
			if (flag)
			{
				propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags | ElcMailboxFlags.ShouldUseDefaultRetentionPolicy);
				return;
			}
			propertyBag[ADUserSchema.ElcMailboxFlags] = (elcMailboxFlags & ~ElcMailboxFlags.ShouldUseDefaultRetentionPolicy);
		}

		internal static void ArchiveDatabaseSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[IADMailStorageSchema.ArchiveDatabaseRaw] = value;
			propertyBag[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)propertyBag[IADMailStorageSchema.ElcMailboxFlags] | ElcMailboxFlags.ValidArchiveDatabase);
		}

		internal static object ArchiveDatabaseGetter(IPropertyBag propertyBag)
		{
			ElcMailboxFlags elcMailboxFlags = (ElcMailboxFlags)propertyBag[IADMailStorageSchema.ElcMailboxFlags];
			if (propertyBag[IADMailStorageSchema.ArchiveGuid] == null || (Guid)propertyBag[IADMailStorageSchema.ArchiveGuid] == Guid.Empty)
			{
				return null;
			}
			RecipientType recipientType = (RecipientType)propertyBag[ADRecipientSchema.RecipientType];
			if ((RecipientType.UserMailbox != recipientType && RecipientType.MailUser != recipientType) || (elcMailboxFlags & ElcMailboxFlags.ValidArchiveDatabase) != ElcMailboxFlags.None)
			{
				return (ADObjectId)propertyBag[IADMailStorageSchema.ArchiveDatabaseRaw];
			}
			if (propertyBag[IADMailStorageSchema.ArchiveDomain] != null)
			{
				return null;
			}
			return (ADObjectId)propertyBag[ADMailboxRecipientSchema.Database];
		}

		internal static object MultiMailboxLocationsGetter(IPropertyBag propertyBag)
		{
			return new MailboxLocationCollection(propertyBag);
		}

		internal static void MultiMailboxLocationsSetter(object value, IPropertyBag propertyBag)
		{
			IMailboxLocationCollection mailboxLocationCollection = (IMailboxLocationCollection)value;
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty = new MultiValuedProperty<ADObjectIdWithString>();
			MultiValuedProperty<Guid> multiValuedProperty2 = new MultiValuedProperty<Guid>();
			IMailboxLocationInfo mailboxLocationInfo2;
			IMailboxLocationInfo mailboxLocationInfo = mailboxLocationInfo2 = null;
			if (mailboxLocationCollection != null)
			{
				mailboxLocationInfo2 = mailboxLocationCollection.GetMailboxLocation(MailboxLocationType.Primary);
				mailboxLocationInfo = mailboxLocationCollection.GetMailboxLocation(MailboxLocationType.MainArchive);
				foreach (IMailboxLocationInfo mailboxLocationInfo3 in mailboxLocationCollection.GetMailboxLocations())
				{
					if ((mailboxLocationInfo2 == null || !mailboxLocationInfo3.MailboxGuid.Equals(mailboxLocationInfo2.MailboxGuid)) && (mailboxLocationInfo == null || !mailboxLocationInfo3.MailboxGuid.Equals(mailboxLocationInfo.MailboxGuid)))
					{
						multiValuedProperty.Add(new ADObjectIdWithString(mailboxLocationInfo3.ToString(), new ADObjectId()));
						multiValuedProperty2.Add(mailboxLocationInfo3.MailboxGuid);
					}
				}
			}
			if (mailboxLocationInfo2 != null)
			{
				propertyBag[IADMailStorageSchema.ExchangeGuid] = mailboxLocationInfo2.MailboxGuid;
				propertyBag[IADMailStorageSchema.Database] = mailboxLocationInfo2.DatabaseLocation;
			}
			else
			{
				propertyBag[IADMailStorageSchema.ExchangeGuid] = null;
				propertyBag[IADMailStorageSchema.Database] = null;
			}
			if (mailboxLocationInfo != null)
			{
				propertyBag[IADMailStorageSchema.ArchiveGuid] = mailboxLocationInfo.MailboxGuid;
				ADRecipient.ArchiveDatabaseSetter(mailboxLocationInfo.DatabaseLocation, propertyBag);
			}
			else
			{
				propertyBag[IADMailStorageSchema.ArchiveGuid] = null;
				ADRecipient.ArchiveDatabaseSetter(null, propertyBag);
			}
			propertyBag[IADMailStorageSchema.MailboxLocationsRaw] = multiValuedProperty;
			propertyBag[IADMailStorageSchema.MailboxGuidsRaw] = multiValuedProperty2;
		}

		internal static QueryFilter LocalArchiveFilter()
		{
			return new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(IADMailStorageSchema.ArchiveGuid),
				new AndFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADUserSchema.ArchiveDomain)),
					new OrFilter(new QueryFilter[]
					{
						new ExistsFilter(IADMailStorageSchema.Database),
						new ExistsFilter(IADMailStorageSchema.ArchiveDatabaseRaw)
					})
				})
			});
		}

		internal static QueryFilter ArchiveDatabaseFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return ADRecipient.LocalArchiveFilter();
			}
			ObjectId propertyValue = (ObjectId)ADObject.PropertyValueFromComparisonFilter(filter, new List<ComparisonOperator>
			{
				ComparisonOperator.Equal
			});
			QueryFilter queryFilter = new BitMaskAndFilter(IADMailStorageSchema.ElcMailboxFlags, 32UL);
			return new AndFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					new ExistsFilter(IADMailStorageSchema.ArchiveGuid),
					new NotFilter(new ExistsFilter(IADMailStorageSchema.ArchiveDomain))
				}),
				new OrFilter(new QueryFilter[]
				{
					new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new ComparisonFilter(ComparisonOperator.Equal, IADMailStorageSchema.ArchiveDatabaseRaw, propertyValue)
					}),
					new AndFilter(new QueryFilter[]
					{
						new NotFilter(queryFilter),
						new ComparisonFilter(ComparisonOperator.Equal, IADMailStorageSchema.Database, propertyValue)
					})
				})
			});
		}

		internal static QueryFilter ArchiveStateFilterBuilder(SinglePropertyFilter filter)
		{
			ArchiveState archiveState = (ArchiveState)ADObject.PropertyValueFromComparisonFilter(filter, new List<ComparisonOperator>
			{
				ComparisonOperator.Equal
			});
			QueryFilter result = null;
			ExistsFilter existsFilter = new ExistsFilter(ADUserSchema.ArchiveGuid);
			if (archiveState == ArchiveState.None)
			{
				result = new NotFilter(existsFilter);
			}
			else
			{
				ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveStatus, ArchiveStatusFlags.Active);
				ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveStatus, ArchiveStatusFlags.None);
				QueryFilter queryFilter = ADRecipient.LocalArchiveFilter();
				ComparisonFilter comparisonFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.RemoteRecipientType, RemoteRecipientType.None);
				switch (archiveState)
				{
				case ArchiveState.Local:
					result = QueryFilter.AndTogether(new QueryFilter[]
					{
						existsFilter,
						queryFilter
					});
					break;
				case ArchiveState.HostedProvisioned:
					result = QueryFilter.AndTogether(new QueryFilter[]
					{
						existsFilter,
						new NotFilter(queryFilter),
						new NotFilter(comparisonFilter3),
						comparisonFilter
					});
					break;
				case ArchiveState.HostedPending:
					result = QueryFilter.AndTogether(new QueryFilter[]
					{
						existsFilter,
						new NotFilter(queryFilter),
						new NotFilter(comparisonFilter3),
						comparisonFilter2
					});
					break;
				case ArchiveState.OnPremise:
					result = QueryFilter.AndTogether(new QueryFilter[]
					{
						existsFilter,
						new NotFilter(queryFilter),
						comparisonFilter3
					});
					break;
				}
			}
			return result;
		}

		internal static QueryFilter ManagedFolderFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new AndFilter(new QueryFilter[]
				{
					new NotFilter(ADRecipient.RetentionPolicySetFilterBuilder()),
					new ExistsFilter(IADMailStorageSchema.ElcPolicyTemplate)
				});
			}
			ObjectId propertyValue = (ObjectId)ADObject.PropertyValueFromComparisonFilter(filter, new List<ComparisonOperator>
			{
				ComparisonOperator.Equal,
				ComparisonOperator.NotEqual
			});
			return new AndFilter(new QueryFilter[]
			{
				new NotFilter(ADRecipient.RetentionPolicySetFilterBuilder()),
				new ComparisonFilter(ComparisonOperator.Equal, IADMailStorageSchema.ElcPolicyTemplate, propertyValue)
			});
		}

		internal static object PersistedMailboxProvisioningConstraintGetter(IPropertyBag propertyBag)
		{
			UserConfigXML userConfigXML = (UserConfigXML)propertyBag[ADRecipientSchema.ConfigurationXML];
			if (userConfigXML == null)
			{
				return null;
			}
			MailboxProvisioningConstraint mailboxProvisioningConstraint = (userConfigXML.MailboxProvisioningConstraints == null) ? null : userConfigXML.MailboxProvisioningConstraints.HardConstraint;
			if (mailboxProvisioningConstraint != null)
			{
				ADRecipient.ValidateMailboxProvisioningConstraint(mailboxProvisioningConstraint);
			}
			return mailboxProvisioningConstraint;
		}

		internal static object MailboxProvisioningConstraintGetter(IPropertyBag propertyBag)
		{
			MailboxProvisioningConstraint mailboxProvisioningConstraint = (MailboxProvisioningConstraint)ADRecipient.PersistedMailboxProvisioningConstraintGetter(propertyBag);
			string text = AppSettings.Current.DedicatedMailboxPlansCustomAttributeName ?? ((IAppSettings)AutoLoadAppSettings.Instance).DedicatedMailboxPlansCustomAttributeName;
			if ((mailboxProvisioningConstraint == null || mailboxProvisioningConstraint.IsEmpty) && text != null && ADRecipient.IsLegacyRegCodeSupportEnabled())
			{
				string customAttribute = ADRecipient.GetCustomAttribute(propertyBag, text);
				if (!string.IsNullOrEmpty(customAttribute))
				{
					string text2;
					ADRecipient.TryParseMailboxProvisioningData(customAttribute, out text2, out mailboxProvisioningConstraint);
				}
			}
			return mailboxProvisioningConstraint;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool IsLegacyRegCodeSupportEnabled()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.LegacyRegCodeSupport.Enabled;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsMailboxLocationsEnabled(ADUser user)
		{
			if (user == null)
			{
				return false;
			}
			MultiValuedProperty<ADObjectIdWithString> multiValuedProperty = user[IADMailStorageSchema.MailboxLocationsRaw] as MultiValuedProperty<ADObjectIdWithString>;
			return multiValuedProperty != null && multiValuedProperty.Count > 0;
		}

		internal static void MailboxProvisioningConstraintSetter(object value, IPropertyBag propertyBag)
		{
			UserConfigXML userConfigXML = (UserConfigXML)propertyBag[ADRecipientSchema.ConfigurationXML];
			if (userConfigXML == null)
			{
				userConfigXML = new UserConfigXML();
			}
			MailboxProvisioningConstraints mailboxProvisioningConstraints = userConfigXML.MailboxProvisioningConstraints;
			MailboxProvisioningConstraint hardConstraint = (value == null) ? null : ((MailboxProvisioningConstraint)value);
			if (mailboxProvisioningConstraints == null)
			{
				mailboxProvisioningConstraints = new MailboxProvisioningConstraints(hardConstraint, new MailboxProvisioningConstraint[0]);
				userConfigXML.MailboxProvisioningConstraints = mailboxProvisioningConstraints;
			}
			else
			{
				mailboxProvisioningConstraints.HardConstraint = hardConstraint;
			}
			propertyBag[ADRecipientSchema.ConfigurationXML] = userConfigXML;
		}

		internal static object MailboxProvisioningPreferencesGetter(IPropertyBag propertyBag)
		{
			UserConfigXML userConfigXML = (UserConfigXML)propertyBag[ADRecipientSchema.ConfigurationXML];
			if (userConfigXML == null)
			{
				return null;
			}
			MailboxProvisioningConstraints mailboxProvisioningConstraints = userConfigXML.MailboxProvisioningConstraints;
			if (userConfigXML.MailboxProvisioningConstraints != null)
			{
				foreach (OrderedMailboxProvisioningConstraint constraint in mailboxProvisioningConstraints.SoftConstraints)
				{
					ADRecipient.ValidateMailboxProvisioningConstraint(constraint);
				}
				return new MultiValuedProperty<MailboxProvisioningConstraint>(mailboxProvisioningConstraints.SoftConstraints);
			}
			return null;
		}

		internal static void ValidateMailboxProvisioningConstraint(MailboxProvisioningConstraint constraint)
		{
			InvalidMailboxProvisioningConstraintException ex;
			if (!constraint.TryValidate(out ex))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(ADRecipientSchema.MailboxProvisioningPreferences.Name, ex.Message), ADRecipientSchema.MailboxProvisioningPreferences, constraint.Value), ex);
			}
		}

		internal static void MailboxProvisioningPreferencesSetter(object value, IPropertyBag propertyBag)
		{
			UserConfigXML userConfigXML = (UserConfigXML)propertyBag[ADRecipientSchema.ConfigurationXML];
			if (userConfigXML == null)
			{
				userConfigXML = new UserConfigXML();
			}
			MailboxProvisioningConstraints mailboxProvisioningConstraints = userConfigXML.MailboxProvisioningConstraints;
			MailboxProvisioningConstraint[] softConstraints = (value == null) ? null : ((MultiValuedProperty<MailboxProvisioningConstraint>)value).ToArray();
			if (mailboxProvisioningConstraints == null)
			{
				mailboxProvisioningConstraints = new MailboxProvisioningConstraints(null, softConstraints);
			}
			else
			{
				mailboxProvisioningConstraints = new MailboxProvisioningConstraints(mailboxProvisioningConstraints.HardConstraint, softConstraints);
			}
			userConfigXML.MailboxProvisioningConstraints = mailboxProvisioningConstraints;
			propertyBag[ADRecipientSchema.ConfigurationXML] = userConfigXML;
		}

		internal static object UsageLocationGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADRecipientSchema.InternalUsageLocation];
			CountryInfo result = null;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					result = CountryInfo.Parse(text);
				}
				catch (InvalidCountryOrRegionException ex)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("UsageLocation", ex.Message), ADRecipientSchema.UsageLocation, propertyBag[ADRecipientSchema.InternalUsageLocation]), ex);
				}
			}
			return result;
		}

		internal static void UsageLocationSetter(object value, IPropertyBag propertyBag)
		{
			CountryInfo countryInfo = value as CountryInfo;
			if (countryInfo != null)
			{
				propertyBag[ADRecipientSchema.InternalUsageLocation] = countryInfo.Name;
				return;
			}
			propertyBag[ADRecipientSchema.InternalUsageLocation] = null;
		}

		internal static QueryFilter UsageLocationFilterBuilder(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			CountryInfo countryInfo = (CountryInfo)comparisonFilter.PropertyValue;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, ADRecipientSchema.InternalUsageLocation, countryInfo.Name);
		}

		internal static object MessageFormatGetter(IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			return (MessageFormat)(num & 262144);
		}

		internal static void MessageFormatSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			MessageFormat messageFormat = (MessageFormat)value;
			num &= -262145;
			propertyBag[ADRecipientSchema.InternetEncoding] = (num | (int)messageFormat);
		}

		internal static object MessageBodyFormatGetter(IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			num &= 1572864;
			if (num == 1572864)
			{
				return MessageBodyFormat.TextAndHtml;
			}
			return (MessageBodyFormat)num;
		}

		internal static void MessageBodyFormatSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			MessageBodyFormat messageBodyFormat = (MessageBodyFormat)value;
			num &= -1572865;
			propertyBag[ADRecipientSchema.InternetEncoding] = (num | (int)messageBodyFormat);
		}

		internal static object MacAttachmentFormatGetter(IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			return (MacAttachmentFormat)(num & 6291456);
		}

		internal static void MacAttachmentFormatSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)(propertyBag[ADRecipientSchema.InternetEncoding] ?? 0);
			MacAttachmentFormat macAttachmentFormat = (MacAttachmentFormat)value;
			num &= -6291457;
			propertyBag[ADRecipientSchema.InternetEncoding] = (num | (int)macAttachmentFormat);
		}

		private static object ResourceGetter(IPropertyBag propertyBag, string prefix)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceMetaData];
			string result = string.Empty;
			foreach (string text in multiValuedProperty)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(text, prefix + ':', CompareOptions.IgnoreCase))
				{
					result = text.Substring(prefix.Length + 1);
					break;
				}
			}
			return result;
		}

		internal static QueryFilter ResourceCustomFilterBuilder(SinglePropertyFilter filter)
		{
			string text = (string)ADObject.PropertyValueFromEqualityFilter(filter);
			if (!string.IsNullOrEmpty(text))
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ResourceSearchProperties, text);
			}
			return new NotFilter(new ExistsFilter(ADRecipientSchema.ResourceSearchProperties));
		}

		internal static object ResourceCustomGetter(IPropertyBag propertyBag)
		{
			string item = (string)ADRecipient.ResourceGetter(propertyBag, "ResourceType");
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceSearchProperties];
			MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>(false, null, multiValuedProperty);
			if (multiValuedProperty2.Contains(item))
			{
				multiValuedProperty2.Remove(item);
			}
			return new MultiValuedProperty<string>(multiValuedProperty.IsReadOnly, ADRecipientSchema.ResourceCustom, multiValuedProperty2);
		}

		internal static void ResourceCustomSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			string text = (string)ADRecipient.ResourceGetter(propertyBag, "ResourceType");
			if (text.Length != 0)
			{
				multiValuedProperty.Add(text);
			}
			if (value != null)
			{
				MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)value;
				foreach (string item in multiValuedProperty2)
				{
					if (!multiValuedProperty.Contains(item))
					{
						multiValuedProperty.Add(item);
					}
				}
			}
			propertyBag[ADRecipientSchema.ResourceSearchProperties] = multiValuedProperty;
			if (multiValuedProperty.Count == 0)
			{
				propertyBag[ADRecipientSchema.ResourcePropertiesDisplay] = string.Empty;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(multiValuedProperty[0]);
			for (int i = 1; i < multiValuedProperty.Count; i++)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(multiValuedProperty[i]);
			}
			propertyBag[ADRecipientSchema.ResourcePropertiesDisplay] = stringBuilder.ToString();
		}

		internal static object ResourceTypeGetter(IPropertyBag propertyBag)
		{
			string value = (string)ADRecipient.ResourceGetter(propertyBag, "ResourceType");
			ExchangeResourceType? exchangeResourceType = null;
			ExchangeResourceType value2;
			if (Enum.TryParse<ExchangeResourceType>(value, true, out value2))
			{
				exchangeResourceType = new ExchangeResourceType?(value2);
			}
			return exchangeResourceType;
		}

		internal static void ResourceTypeSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceSearchProperties];
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceMetaData];
			if ((ExchangeResourceType?)value != (ExchangeResourceType?)ADRecipient.ResourceTypeGetter(propertyBag))
			{
				multiValuedProperty2.Clear();
				multiValuedProperty.Clear();
				propertyBag[ADRecipientSchema.ResourcePropertiesDisplay] = string.Empty;
				if (value != null)
				{
					multiValuedProperty2.Add("ResourceType" + ':' + value.ToString());
					multiValuedProperty.Add(value.ToString());
					propertyBag[ADRecipientSchema.ResourcePropertiesDisplay] = value.ToString();
				}
			}
		}

		internal static QueryFilter ResourceTypeFilterBuilder(SinglePropertyFilter filter)
		{
			ExchangeResourceType? exchangeResourceType = (ExchangeResourceType?)ADObject.PropertyValueFromEqualityFilter(filter);
			if (exchangeResourceType == null)
			{
				return new AndFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.ResourceMetaData)),
					new NotFilter(new ExistsFilter(ADRecipientSchema.ResourceSearchProperties))
				});
			}
			return new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ResourceMetaData, "ResourceType" + ':' + exchangeResourceType.Value.ToString()),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ResourceSearchProperties, exchangeResourceType.Value.ToString())
			});
		}

		private static bool IsResourcePropertiesNotNull(IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceMetaData];
			MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ResourceSearchProperties];
			string value = (string)propertyBag[ADRecipientSchema.ResourcePropertiesDisplay];
			int? num = (int?)propertyBag[ADRecipientSchema.ResourceCapacity];
			return multiValuedProperty.Count > 0 || multiValuedProperty2.Count > 0 || num != null || !string.IsNullOrEmpty(value);
		}

		internal static object IsResourceGetter(IPropertyBag propertyBag)
		{
			SecurityIdentifier securityIdentifier = (SecurityIdentifier)propertyBag[ADRecipientSchema.MasterAccountSid];
			if (null != securityIdentifier && securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
			{
				return BoxedConstants.GetBool(ADRecipient.IsResourcePropertiesNotNull(propertyBag));
			}
			if (!(bool)ADRecipient.IsLinkedGetter(propertyBag))
			{
				return BoxedConstants.False;
			}
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)ADRecipient.RecipientTypeDetailsGetter(propertyBag);
			if (recipientTypeDetails == RecipientTypeDetails.LinkedRoomMailbox)
			{
				return BoxedConstants.GetBool(ADRecipient.IsResourcePropertiesNotNull(propertyBag));
			}
			return BoxedConstants.False;
		}

		internal static object IsSharedGetter(IPropertyBag propertyBag)
		{
			SecurityIdentifier securityIdentifier = (SecurityIdentifier)propertyBag[ADRecipientSchema.MasterAccountSid];
			if (null != securityIdentifier && securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
			{
				return BoxedConstants.GetBool(!ADRecipient.IsResourcePropertiesNotNull(propertyBag));
			}
			return BoxedConstants.False;
		}

		internal static object IsLinkedGetter(IPropertyBag propertyBag)
		{
			SecurityIdentifier securityIdentifier = (SecurityIdentifier)propertyBag[ADRecipientSchema.MasterAccountSid];
			bool value = null != securityIdentifier && !securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid) && !securityIdentifier.IsWellKnown(WellKnownSidType.NullSid);
			return BoxedConstants.GetBool(value);
		}

		internal static object RecipientPersonTypeGetter(IPropertyBag propertyBag)
		{
			switch ((RecipientType)ADRecipient.RecipientTypeGetter(propertyBag))
			{
			case RecipientType.Invalid:
			{
				object obj = propertyBag[NspiOnlyProperties.DisplayType];
				if (obj != null)
				{
					switch ((LegacyRecipientDisplayType)obj)
					{
					case LegacyRecipientDisplayType.MailUser:
					case LegacyRecipientDisplayType.RemoteMailUser:
					{
						object obj2 = propertyBag[NspiOnlyProperties.DisplayTypeEx];
						if (obj2 != null)
						{
							RecipientDisplayType recipientDisplayType = (RecipientDisplayType)obj2;
							RecipientDisplayType recipientDisplayType2 = recipientDisplayType;
							if (recipientDisplayType2 == Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.SyncedConferenceRoomMailbox || recipientDisplayType2 == Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.ConferenceRoomMailbox)
							{
								return PersonType.Room;
							}
							if (recipientDisplayType2 != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.GroupMailboxUser)
							{
								return PersonType.Person;
							}
							return PersonType.ModernGroup;
						}
						break;
					}
					case LegacyRecipientDisplayType.DistributionList:
					case LegacyRecipientDisplayType.DynamicDistributionList:
					case LegacyRecipientDisplayType.PersonalDistributionList:
						return PersonType.DistributionList;
					}
				}
				return PersonType.Unknown;
			}
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			case RecipientType.Contact:
			case RecipientType.MailContact:
			{
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)ADRecipient.RecipientTypeDetailsGetter(propertyBag);
				RecipientTypeDetails recipientTypeDetails2 = recipientTypeDetails;
				if (recipientTypeDetails2 == RecipientTypeDetails.RoomMailbox || recipientTypeDetails2 == RecipientTypeDetails.RemoteRoomMailbox)
				{
					return PersonType.Room;
				}
				if (recipientTypeDetails2 != RecipientTypeDetails.GroupMailbox)
				{
					return PersonType.Person;
				}
				return PersonType.ModernGroup;
			}
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				return PersonType.DistributionList;
			default:
				return PersonType.Unknown;
			}
		}

		private static QueryFilter GetResourceFilter()
		{
			return new OrFilter(new QueryFilter[]
			{
				new ExistsFilter(ADRecipientSchema.ResourceMetaData),
				new ExistsFilter(ADRecipientSchema.ResourceSearchProperties),
				new ExistsFilter(ADRecipientSchema.ResourcePropertiesDisplay),
				new ExistsFilter(ADRecipientSchema.ResourceCapacity)
			});
		}

		internal static QueryFilter IsLinkedFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(ADRecipientSchema.MasterAccountSid),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.MasterAccountSid, new SecurityIdentifier(WellKnownSidType.SelfSid, null)),
				new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.MasterAccountSid, new SecurityIdentifier(WellKnownSidType.NullSid, null))
			});
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static QueryFilter IsResourceFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				ADRecipient.GetResourceFilter(),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, new SecurityIdentifier(WellKnownSidType.SelfSid, null))
			});
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static QueryFilter IsSharedFilterBuilder(SinglePropertyFilter filter)
		{
			bool flag = (bool)ADObject.PropertyValueFromEqualityFilter(filter);
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new NotFilter(ADRecipient.GetResourceFilter()),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, new SecurityIdentifier(WellKnownSidType.SelfSid, null))
			});
			if (!flag)
			{
				return new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static QueryFilter IsExcludedFromBacksyncFilterBuilder(SinglePropertyFilter filter)
		{
			return new ComparisonFilter(((bool)ADObject.PropertyValueFromEqualityFilter(filter)) ? ComparisonOperator.Equal : ComparisonOperator.NotEqual, ADRecipientSchema.RawCapabilities, Capability.ExcludedFromBackSync);
		}

		internal static void ForwardingSmtpAddressSetter(object value, IPropertyBag propertyBag)
		{
			SmtpProxyAddress smtpProxyAddress = value as SmtpProxyAddress;
			if (value == null || smtpProxyAddress != null)
			{
				propertyBag[ADRecipientSchema.GenericForwardingAddress] = smtpProxyAddress;
				return;
			}
			throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ForwardingSmtpAddressNotValidSmtpAddress(value), ADRecipientSchema.ForwardingSmtpAddress, value));
		}

		internal static void OnPremisesObjectIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADRecipientSchema.RawOnPremisesObjectId] = ADRecipient.ConvertOnPremisesObjectIdToBytes((string)value);
		}

		internal static object OnPremisesObjectIdGetter(IPropertyBag propertyBag)
		{
			return ADRecipient.ConvertOnPremisesObjectIdToString(propertyBag[ADRecipientSchema.RawOnPremisesObjectId]);
		}

		internal static QueryFilter OnPremisesObjectIdFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ADRecipientSchema.RawOnPremisesObjectId);
			}
			string value = (string)ADObject.PropertyValueFromEqualityFilter(filter);
			byte[] propertyValue = ADRecipient.ConvertOnPremisesObjectIdToBytes(value);
			return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RawOnPremisesObjectId, propertyValue);
		}

		private static byte[] ConvertOnPremisesObjectIdToBytes(string value)
		{
			byte[] result = null;
			if (!string.IsNullOrEmpty(value))
			{
				result = Encoding.UTF8.GetBytes(value);
			}
			return result;
		}

		internal static string ConvertOnPremisesObjectIdToString(object value)
		{
			string result = string.Empty;
			byte[] array = value as byte[];
			if (array != null && array.Length > 0)
			{
				result = Encoding.UTF8.GetString(array);
			}
			return result;
		}

		internal static void ExternalEmailAddressSetter(object value, IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			ProxyAddress proxyAddress = (ProxyAddress)propertyBag[ADRecipientSchema.RawExternalEmailAddress];
			try
			{
				ProxyAddress proxyAddress2 = (ProxyAddress)value;
				if (null != proxyAddress2)
				{
					MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
					if (!multiValuedProperty.Contains(ADPublicFolder.MostDerivedClass))
					{
						proxyAddress2 = (ProxyAddress)proxyAddress2.ToPrimary();
						value = proxyAddress2;
					}
				}
				if (proxyAddress != proxyAddress2 && null != proxyAddress && proxyAddressCollection.Contains(proxyAddress) && (null == proxyAddress2 || !proxyAddressCollection.Contains(proxyAddress2)))
				{
					int index = proxyAddressCollection.IndexOf(proxyAddress);
					proxyAddress = proxyAddressCollection[index];
					if (proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == ProxyAddressPrefix.Smtp && null != proxyAddress2 && proxyAddress2.Prefix != ProxyAddressPrefix.Smtp)
					{
						throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorRemovePrimaryExternalSMTPAddress, ADRecipientSchema.ExternalEmailAddress, value));
					}
					if (null != proxyAddress2)
					{
						proxyAddressCollection[index] = proxyAddress2;
					}
					else
					{
						proxyAddressCollection.RemoveAt(index);
					}
				}
				propertyBag[ADRecipientSchema.RawExternalEmailAddress] = value;
				if (value != null && proxyAddressCollection.FindPrimary(ProxyAddressPrefix.Smtp) == (ProxyAddress)value)
				{
					propertyBag[ADObjectSchema.OriginalPrimarySmtpAddress] = new SmtpAddress((value as ProxyAddress).ValueString);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.FailedToUpdateEmailAddressesForExternal((value == null) ? "$null" : value.ToString(), ADRecipientSchema.EmailAddresses.Name, ex.Message), ADRecipientSchema.ExternalEmailAddress, value), ex);
			}
		}

		internal static QueryFilter UMEnabledFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADUserSchema.UMEnabledFlags, 1UL));
		}

		internal static QueryFilter UMProvisioningFlagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 2UL));
		}

		internal static QueryFilter UCSImListMigrationCompletedFlagFilterBuilder(SinglePropertyFilter filter)
		{
			return ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 256UL));
		}

		internal static CustomFilterBuilderDelegate ProtocolEnabledFilterBuilder(string protocol)
		{
			return delegate(SinglePropertyFilter filter)
			{
				if (!(filter is ComparisonFilter))
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
				}
				if (string.IsNullOrEmpty(protocol))
				{
					throw new ArgumentNullException("protocol");
				}
				ComparisonFilter comparisonFilter = (ComparisonFilter)filter;
				if (comparisonFilter.ComparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonFilter.ComparisonOperator)
				{
					throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedOperatorForProperty(comparisonFilter.Property.Name, comparisonFilter.ComparisonOperator.ToString()));
				}
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)comparisonFilter.Property;
				string str = ((bool)adpropertyDefinition.DefaultValue) ? "§0" : "§1";
				QueryFilter queryFilter = new TextFilter(ADRecipientSchema.ProtocolSettings, protocol + str, MatchOptions.Prefix, MatchFlags.IgnoreCase);
				if (protocol.Equals("OWA"))
				{
					QueryFilter queryFilter2 = new TextFilter(ADRecipientSchema.ProtocolSettings, "HTTP" + str, MatchOptions.Prefix, MatchFlags.IgnoreCase);
					queryFilter = new OrFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
				}
				if ((comparisonFilter.ComparisonOperator == ComparisonOperator.Equal && (bool)comparisonFilter.PropertyValue == (bool)adpropertyDefinition.DefaultValue) || (ComparisonOperator.NotEqual == comparisonFilter.ComparisonOperator && (bool)comparisonFilter.PropertyValue != (bool)adpropertyDefinition.DefaultValue))
				{
					return new NotFilter(queryFilter);
				}
				return queryFilter;
			};
		}

		internal static object SendModerationNotificationsGetter(IPropertyBag propertyBag)
		{
			int moderationFlags = (int)propertyBag[ADRecipientSchema.ModerationFlags];
			return ADRecipient.GetSendModerationNotificationsFromModerationFlags(moderationFlags);
		}

		internal static TransportModerationNotificationFlags GetSendModerationNotificationsFromModerationFlags(int moderationFlags)
		{
			if ((moderationFlags & 4) != 0 && (moderationFlags & 2) != 0)
			{
				return TransportModerationNotificationFlags.Always;
			}
			if ((moderationFlags & 2) != 0)
			{
				return TransportModerationNotificationFlags.Internal;
			}
			return TransportModerationNotificationFlags.Never;
		}

		internal static void SendModerationNotificationsSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ADRecipientSchema.ModerationFlags];
			TransportModerationNotificationFlags transportModerationNotificationFlags = (TransportModerationNotificationFlags)value;
			if (transportModerationNotificationFlags == TransportModerationNotificationFlags.Always)
			{
				num |= 2;
				num |= 4;
			}
			else if (transportModerationNotificationFlags == TransportModerationNotificationFlags.Internal)
			{
				num |= 2;
				num &= -5;
			}
			else
			{
				num &= -7;
			}
			propertyBag[ADRecipientSchema.ModerationFlags] = num;
		}

		internal static bool TryGetFromProxyAddress(ProxyAddress proxyAddress, IRecipientSession session, out ADRecipient recipient)
		{
			ADRecipient temporaryRecipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				temporaryRecipient = session.FindByProxyAddress(proxyAddress);
			});
			recipient = temporaryRecipient;
			return adoperationResult.Succeeded;
		}

		internal static ADOperationResult TryGetFromCrossTenantObjectId(CrossTenantObjectId externalDirectoryObjectId, out ADRecipient recipient)
		{
			ADRecipient temporaryRecipient = null;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryObjectId.ExternalDirectoryOrganizationId);
				IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 3118, "TryGetFromCrossTenantObjectId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipient.cs");
				temporaryRecipient = recipientSession.FindADUserByExternalDirectoryObjectId(externalDirectoryObjectId.ExternalDirectoryObjectId.ToString());
			});
			recipient = temporaryRecipient;
			return result;
		}

		public CrossTenantObjectId GetCrossTenantObjectId()
		{
			string externalDirectoryObjectId = this.ExternalDirectoryObjectId;
			if (string.IsNullOrEmpty(externalDirectoryObjectId))
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.CannotConstructCrossTenantObjectId("ExternalDirectoryObjectId"));
			}
			if (base.OrganizationId == null)
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.CannotConstructCrossTenantObjectId("OrganizationId"));
			}
			string text = base.OrganizationId.ToExternalDirectoryOrganizationId();
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidADObjectOperationException(DirectoryStrings.CannotConstructCrossTenantObjectId("OrganizationId.ToExternalDirectoryOrganizationId()"));
			}
			Guid externalDirectoryOrganizationId = string.IsNullOrEmpty(text) ? Guid.Empty : Guid.Parse(text);
			Guid externalDirectoryObjectId2 = Guid.Parse(externalDirectoryObjectId);
			return CrossTenantObjectId.FromExternalDirectoryIds(externalDirectoryOrganizationId, externalDirectoryObjectId2);
		}

		public override string ToString()
		{
			if (this.DisplayName != null)
			{
				return this.DisplayName;
			}
			if (base.Id != null)
			{
				return base.Id.ToString();
			}
			return base.ToString();
		}

		internal override void Initialize()
		{
			base.Initialize();
			if (!this.propertyBag.IsReadOnly)
			{
				this.OriginalPrimarySmtpAddress = this.PrimarySmtpAddress;
				this.OriginalWindowsEmailAddress = this.WindowsEmailAddress;
			}
		}

		public bool IsMemberOf(ADObjectId groupId, bool directOnly)
		{
			return ADRecipient.IsMemberOf(base.Id, groupId, directOnly, this.Session);
		}

		internal IThrottlingPolicy ReadThrottlingPolicy()
		{
			if (this.ThrottlingPolicy != null)
			{
				return ThrottlingPolicyCache.Singleton.Get(base.OrganizationId, this.ThrottlingPolicy);
			}
			return this.ReadDefaultThrottlingPolicy();
		}

		internal IThrottlingPolicy ReadDefaultThrottlingPolicy()
		{
			return ThrottlingPolicyCache.Singleton.Get(base.OrganizationId);
		}

		internal IRecipientSession Session
		{
			get
			{
				return (IRecipientSession)this.m_Session;
			}
		}

		internal static QueryFilter RecipientTypeFilterBuilder(SinglePropertyFilter filter)
		{
			RecipientType recipientType = (RecipientType)ADObject.PropertyValueFromEqualityFilter(filter);
			int num = (int)recipientType;
			if (num <= 0 || num >= Filters.RecipientTypeCount)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValue(ADRecipientSchema.RecipientType.Name, recipientType));
			}
			return Filters.RecipientTypeFilters[num];
		}

		public string UMExtension
		{
			get
			{
				return UMMailbox.GetPrimaryExtension(this.EmailAddresses, ProxyAddressPrefix.UM);
			}
		}

		internal void ClearEUMProxy(bool skipAirSyncEUMAddresses, UMDialPlan dialPlan)
		{
			Hashtable safeTable = null;
			if (skipAirSyncEUMAddresses)
			{
				safeTable = UMMailbox.GetAirSyncSafeTable(this.UMAddresses, ProxyAddressPrefix.ASUM, dialPlan);
			}
			UMMailbox.ClearProxy(this, this.EmailAddresses, ProxyAddressPrefix.UM, safeTable);
		}

		internal void ClearASUMProxy()
		{
			UMMailbox.ClearProxy(this, this.UMAddresses, ProxyAddressPrefix.ASUM, null);
		}

		internal void AddEUMProxyAddress(MultiValuedProperty<string> extensions, UMDialPlan dialPlan)
		{
			foreach (string extension in extensions)
			{
				UMMailbox.AddProxy(this, this.EmailAddresses, extension, dialPlan, ProxyAddressPrefix.UM);
			}
		}

		internal void AddEUMProxyAddress(string phoneNumber, UMDialPlan dialPlan)
		{
			UMMailbox.AddProxy(this, this.EmailAddresses, phoneNumber, dialPlan, ProxyAddressPrefix.UM);
		}

		internal void AddASUMProxyAddress(string phoneNumber, UMDialPlan dialPlan)
		{
			UMMailbox.AddProxy(this, this.UMAddresses, phoneNumber, dialPlan, ProxyAddressPrefix.ASUM);
		}

		internal bool PhoneNumberExistsInUMAddresses(string phoneNumber)
		{
			return UMMailbox.PhoneNumberExists(this.UMAddresses, ProxyAddressPrefix.ASUM, phoneNumber);
		}

		internal bool IsAirSyncNumberQuotaReached()
		{
			return UMMailbox.ProxyAddressCount(this.UMAddresses, ProxyAddressPrefix.ASUM) >= 3;
		}

		internal void SetUMDtmfMapIfNecessary()
		{
			if (base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				return;
			}
			string value = this[ADOrgPersonSchema.Phone] as string;
			if ((Array.IndexOf<RecipientType>(ADRecipient.DtmfMapAllowedRecipientTypes, this.RecipientType) != -1 || !string.IsNullOrEmpty(value)) && (base.IsModified(ADUserSchema.UMEnabled) || base.IsModified(ADOrgPersonSchema.LastName) || base.IsModified(ADOrgPersonSchema.FirstName) || base.IsModified(ADRecipientSchema.DisplayName) || base.IsModified(ADRecipientSchema.PrimarySmtpAddress) || base.IsModified(ADOrgPersonSchema.SanitizedPhoneNumbers)))
			{
				this.PopulateDtmfMap(true);
			}
		}

		internal void SetValidArchiveDatabase()
		{
			if (base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				return;
			}
			if (RecipientType.UserMailbox != this.RecipientType && RecipientType.MailUser != this.RecipientType)
			{
				return;
			}
			if (base.IsModified(IADMailStorageSchema.Database) || base.IsModified(IADMailStorageSchema.ArchiveDatabaseRaw))
			{
				bool flag = false;
				if (this[IADMailStorageSchema.ArchiveDatabaseRaw] != null && !this[IADMailStorageSchema.ArchiveDatabaseRaw].Equals(this[IADMailStorageSchema.Database]))
				{
					flag = true;
				}
				if (flag)
				{
					this[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)this[IADMailStorageSchema.ElcMailboxFlags] | ElcMailboxFlags.ValidArchiveDatabase);
					return;
				}
				this[IADMailStorageSchema.ElcMailboxFlags] = ((ElcMailboxFlags)this[IADMailStorageSchema.ElcMailboxFlags] & ~ElcMailboxFlags.ValidArchiveDatabase);
			}
		}

		internal void ValidateArchiveProperties(bool isDatacenter, List<ValidationError> errors)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this[IADMailStorageSchema.ArchiveGuid] != null && (Guid)this[IADMailStorageSchema.ArchiveGuid] != Guid.Empty;
			if (base.IsModified(IADMailStorageSchema.ArchiveGuid))
			{
				if (flag4)
				{
					flag = !this.ValidateArchiveLocationNoConflict(errors);
				}
				else
				{
					flag2 = !this.ValidateArchiveDatabaseNotSet(errors);
					flag3 = !this.ValidateArchiveDomainNotSet(errors);
				}
			}
			if (base.IsModified(IADMailStorageSchema.ArchiveDomain))
			{
				if (isDatacenter && this[IADMailStorageSchema.ArchiveDomain] != null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorArchiveDomainInvalidInDatacenter, IADMailStorageSchema.ArchiveDomain, this[IADMailStorageSchema.ArchiveDomain]));
				}
				if (flag4)
				{
					if (!flag)
					{
						flag = !this.ValidateArchiveLocationNoConflict(errors);
					}
				}
				else if (!flag3)
				{
					this.ValidateArchiveDomainNotSet(errors);
				}
			}
			if (base.IsModified(IADMailStorageSchema.ArchiveDatabaseRaw))
			{
				if (flag4)
				{
					if (!flag)
					{
						this.ValidateArchiveLocationNoConflict(errors);
						return;
					}
				}
				else if (!flag2)
				{
					this.ValidateArchiveDatabaseNotSet(errors);
				}
			}
		}

		private bool ValidateArchiveLocationNoConflict(List<ValidationError> errors)
		{
			if (this[IADMailStorageSchema.ArchiveDatabaseRaw] != null && this[IADMailStorageSchema.ArchiveDomain] != null)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorArchiveDatabaseArchiveDomainConflict, this.Identity, string.Empty));
				return false;
			}
			return true;
		}

		private bool ValidateArchiveDatabaseNotSet(List<ValidationError> errors)
		{
			if (this[IADMailStorageSchema.ArchiveDatabaseRaw] != null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorArchiveDatabaseSetForNonArchive, IADMailStorageSchema.ArchiveDatabaseRaw, this[IADMailStorageSchema.ArchiveDatabaseRaw]));
				return false;
			}
			return true;
		}

		private bool ValidateArchiveDomainNotSet(List<ValidationError> errors)
		{
			if (this[IADMailStorageSchema.ArchiveDomain] != null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorArchiveDomainSetForNonArchive, IADMailStorageSchema.ArchiveDomain, this[IADMailStorageSchema.ArchiveDomain]));
				return false;
			}
			return true;
		}

		private static bool IsValidRecipientTypeForModerator(RecipientType recipientType)
		{
			foreach (RecipientType recipientType2 in ADRecipient.AllowedModeratorsRecipientTypes)
			{
				if (recipientType == recipientType2)
				{
					return true;
				}
			}
			return false;
		}

		internal string GetAlternateMailboxLegDN(Guid guid)
		{
			return ADRecipient.CreateAlternateMailboxLegDN(this.LegacyExchangeDN, guid);
		}

		internal static string CreateAlternateMailboxLegDN(string parentLegacyDNString, Guid mailboxGuid)
		{
			LegacyDN parentLegacyDN = LegacyDN.Parse(parentLegacyDNString);
			LegacyDN legacyDN = new LegacyDN(parentLegacyDN, "guid", mailboxGuid.ToString("D"));
			return legacyDN.ToString();
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				this[ADRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		public ADObjectId ThrottlingPolicy
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.ThrottlingPolicy];
			}
			set
			{
				this[ADRecipientSchema.ThrottlingPolicy] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				this[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers];
			}
			internal set
			{
				this[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = value;
			}
		}

		public ADObjectId AddressBookPolicy
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.AddressBookPolicy];
			}
			set
			{
				this[ADRecipientSchema.AddressBookPolicy] = value;
			}
		}

		public ADObjectId GlobalAddressListFromAddressBookPolicy
		{
			get
			{
				if (this.AddressBookPolicy != null && this.globalAddressListFromAddressBookPolicy == null)
				{
					this.PopulateGlobalAddressListFromAddressBookPolicy();
				}
				return this.globalAddressListFromAddressBookPolicy;
			}
		}

		public MultiValuedProperty<ADObjectId> AddressListMembership
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.AddressListMembership];
			}
			set
			{
				this[ADRecipientSchema.AddressListMembership] = value;
			}
		}

		public MultiValuedProperty<string> AggregationSubscriptionCredential
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.AggregationSubscriptionCredential];
			}
			internal set
			{
				this[ADRecipientSchema.AggregationSubscriptionCredential] = value;
			}
		}

		public int RecipientSoftDeletedStatus
		{
			get
			{
				return (int)(this[ADRecipientSchema.RecipientSoftDeletedStatus] ?? 0);
			}
			internal set
			{
				this[ADRecipientSchema.RecipientSoftDeletedStatus] = value;
			}
		}

		public bool IsSoftDeletedByRemove
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsSoftDeletedByRemove];
			}
			set
			{
				this[ADRecipientSchema.IsSoftDeletedByRemove] = value;
			}
		}

		public bool IsSoftDeletedByDisable
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsSoftDeletedByDisable];
			}
			set
			{
				this[ADRecipientSchema.IsSoftDeletedByDisable] = value;
			}
		}

		public bool IsSoftDeleted
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsSoftDeletedByDisable] || (bool)this[ADRecipientSchema.IsSoftDeletedByRemove];
			}
		}

		public bool IsInactiveMailbox
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsInactiveMailbox];
			}
			set
			{
				this[ADRecipientSchema.IsInactiveMailbox] = value;
			}
		}

		public DateTime? WhenSoftDeleted
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.WhenSoftDeleted];
			}
			internal set
			{
				this[ADRecipientSchema.WhenSoftDeleted] = value;
			}
		}

		public bool IncludeInGarbageCollection
		{
			get
			{
				return (bool)this[ADRecipientSchema.IncludeInGarbageCollection];
			}
			internal set
			{
				this[ADRecipientSchema.IncludeInGarbageCollection] = value;
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[ADRecipientSchema.Alias];
			}
			set
			{
				this[ADRecipientSchema.Alias] = value;
			}
		}

		public bool AntispamBypassEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.AntispamBypassEnabled];
			}
			set
			{
				this[ADRecipientSchema.AntispamBypassEnabled] = value;
			}
		}

		public string AssistantName
		{
			get
			{
				return (string)this[ADRecipientSchema.AssistantName];
			}
			set
			{
				this[ADRecipientSchema.AssistantName] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> BypassModerationFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.BypassModerationFrom];
			}
			set
			{
				this[ADRecipientSchema.BypassModerationFrom] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> BypassModerationFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.BypassModerationFromDLMembers];
			}
			set
			{
				this[ADRecipientSchema.BypassModerationFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> BypassModerationFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.BypassModerationFromSendersOrMembers];
			}
			internal set
			{
				this[ADRecipientSchema.BypassModerationFromSendersOrMembers] = value;
			}
		}

		public MultiValuedProperty<byte[]> Certificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.Certificate];
			}
			set
			{
				this[ADRecipientSchema.Certificate] = value;
			}
		}

		public string WebPage
		{
			get
			{
				return (string)this[ADRecipientSchema.WebPage];
			}
			internal set
			{
				this[ADRecipientSchema.WebPage] = value;
			}
		}

		public string Notes
		{
			get
			{
				return (string)this[ADRecipientSchema.Notes];
			}
			set
			{
				this[ADRecipientSchema.Notes] = value;
			}
		}

		public string CustomAttribute1
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute1];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute1] = value;
			}
		}

		public string CustomAttribute10
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute10];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute10] = value;
			}
		}

		public string CustomAttribute11
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute11];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute11] = value;
			}
		}

		public string CustomAttribute12
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute12];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute12] = value;
			}
		}

		public string CustomAttribute13
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute13];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute13] = value;
			}
		}

		public string CustomAttribute14
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute14];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute14] = value;
			}
		}

		public string CustomAttribute15
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute15];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute15] = value;
			}
		}

		public string CustomAttribute2
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute2];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute2] = value;
			}
		}

		public string CustomAttribute3
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute3];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute3] = value;
			}
		}

		public string CustomAttribute4
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute4];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute4] = value;
			}
		}

		public string CustomAttribute5
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute5];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute5] = value;
			}
		}

		public string CustomAttribute6
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute6];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute6] = value;
			}
		}

		public string CustomAttribute7
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute7];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute7] = value;
			}
		}

		public string CustomAttribute8
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute8];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute8] = value;
			}
		}

		public string CustomAttribute9
		{
			get
			{
				return (string)this[ADRecipientSchema.CustomAttribute9];
			}
			set
			{
				this[ADRecipientSchema.CustomAttribute9] = value;
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ExtensionCustomAttribute1];
			}
			set
			{
				this[ADRecipientSchema.ExtensionCustomAttribute1] = value;
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ExtensionCustomAttribute2];
			}
			set
			{
				this[ADRecipientSchema.ExtensionCustomAttribute2] = value;
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ExtensionCustomAttribute3];
			}
			set
			{
				this[ADRecipientSchema.ExtensionCustomAttribute3] = value;
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ExtensionCustomAttribute4];
			}
			set
			{
				this[ADRecipientSchema.ExtensionCustomAttribute4] = value;
			}
		}

		public MultiValuedProperty<string> ExtensionCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ExtensionCustomAttribute5];
			}
			set
			{
				this[ADRecipientSchema.ExtensionCustomAttribute5] = value;
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ADRecipientSchema.EmailAddresses];
			}
			set
			{
				this[ADRecipientSchema.EmailAddresses] = value;
			}
		}

		public bool EmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.EmailAddressPolicyEnabled];
			}
			internal set
			{
				this[ADRecipientSchema.EmailAddressPolicyEnabled] = value;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this[ADRecipientSchema.ExternalDirectoryObjectId];
			}
			internal set
			{
				this[ADRecipientSchema.ExternalDirectoryObjectId] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ADRecipientSchema.DisplayName];
			}
			set
			{
				this[ADRecipientSchema.DisplayName] = value;
			}
		}

		public bool IsDirSynced
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsDirSynced];
			}
			set
			{
				this[ADRecipientSchema.IsDirSynced] = value;
			}
		}

		internal bool IsDirSyncEnabled
		{
			get
			{
				return ADObject.IsRecipientDirSynced(this.IsDirSynced);
			}
		}

		public MultiValuedProperty<string> DirSyncAuthorityMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.DirSyncAuthorityMetadata];
			}
			set
			{
				this[ADRecipientSchema.DirSyncAuthorityMetadata] = value;
			}
		}

		internal ProxyAddress RawExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[ADRecipientSchema.RawExternalEmailAddress];
			}
			set
			{
				this[ADRecipientSchema.RawExternalEmailAddress] = value;
			}
		}

		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)this[ADRecipientSchema.ExternalEmailAddress];
			}
			set
			{
				this[ADRecipientSchema.ExternalEmailAddress] = value;
			}
		}

		public bool IsCalculatedTargetAddress
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsCalculatedTargetAddress];
			}
			set
			{
				this[ADRecipientSchema.IsCalculatedTargetAddress] = value;
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.ForwardingAddress];
			}
			set
			{
				this[ADRecipientSchema.ForwardingAddress] = value;
			}
		}

		public ProxyAddress ForwardingSmtpAddress
		{
			get
			{
				return (ProxyAddress)this[ADRecipientSchema.ForwardingSmtpAddress];
			}
			set
			{
				this[ADRecipientSchema.ForwardingSmtpAddress] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> GrantSendOnBehalfTo
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				this[ADRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.HiddenFromAddressListsEnabled];
			}
			set
			{
				this[ADRecipientSchema.HiddenFromAddressListsEnabled] = value;
			}
		}

		public bool UsePreferMessageFormat
		{
			get
			{
				return (bool)this[ADRecipientSchema.UsePreferMessageFormat];
			}
			set
			{
				this[ADRecipientSchema.UsePreferMessageFormat] = value;
			}
		}

		public MessageFormat MessageFormat
		{
			get
			{
				return (MessageFormat)this[ADRecipientSchema.MessageFormat];
			}
			set
			{
				this[ADRecipientSchema.MessageFormat] = value;
			}
		}

		public MessageBodyFormat MessageBodyFormat
		{
			get
			{
				return (MessageBodyFormat)this[ADRecipientSchema.MessageBodyFormat];
			}
			set
			{
				this[ADRecipientSchema.MessageBodyFormat] = value;
			}
		}

		public MacAttachmentFormat MacAttachmentFormat
		{
			get
			{
				return (MacAttachmentFormat)this[ADRecipientSchema.MacAttachmentFormat];
			}
			set
			{
				this[ADRecipientSchema.MacAttachmentFormat] = value;
			}
		}

		public bool IsResource
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsResource];
			}
		}

		public bool IsLinked
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsLinked];
			}
		}

		public bool IsShared
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsShared];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[ADRecipientSchema.LegacyExchangeDN];
			}
			set
			{
				this[ADRecipientSchema.LegacyExchangeDN] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADRecipientSchema.MaxReceiveSize];
			}
			set
			{
				this[ADRecipientSchema.MaxReceiveSize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADRecipientSchema.MaxSendSize];
			}
			set
			{
				this[ADRecipientSchema.MaxSendSize] = value;
			}
		}

		internal MessageHygieneFlags MessageHygieneFlags
		{
			get
			{
				return (MessageHygieneFlags)this[ADRecipientSchema.MessageHygieneFlags];
			}
			set
			{
				this[ADRecipientSchema.MessageHygieneFlags] = value;
			}
		}

		public string OU
		{
			get
			{
				return (string)this[ADRecipientSchema.OrganizationalUnit];
			}
		}

		public bool MAPIEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.MAPIEnabled];
			}
			set
			{
				this[ADRecipientSchema.MAPIEnabled] = value;
			}
		}

		public bool? MapiHttpEnabled
		{
			get
			{
				return (bool?)this[ADRecipientSchema.MapiHttpEnabled];
			}
		}

		public bool MAPIBlockOutlookRpcHttp
		{
			get
			{
				return (bool)this[ADRecipientSchema.MAPIBlockOutlookRpcHttp];
			}
		}

		public string OnPremisesObjectId
		{
			get
			{
				return (string)this[ADRecipientSchema.OnPremisesObjectId];
			}
			set
			{
				this[ADRecipientSchema.OnPremisesObjectId] = value;
			}
		}

		public bool OWAEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.OWAEnabled];
			}
			set
			{
				this[ADRecipientSchema.OWAEnabled] = value;
			}
		}

		public bool MOWAEnabled
		{
			get
			{
				return (bool)this[ADUserSchema.OWAforDevicesEnabled];
			}
			set
			{
				this[ADUserSchema.OWAforDevicesEnabled] = value;
			}
		}

		public string PhoneticCompany
		{
			get
			{
				return (string)this[ADRecipientSchema.PhoneticCompany];
			}
			set
			{
				this[ADRecipientSchema.PhoneticCompany] = value;
			}
		}

		public bool? EwsEnabled
		{
			get
			{
				return CASMailboxHelper.ToBooleanNullable((int?)this[CASMailboxSchema.EwsEnabled]);
			}
		}

		public bool? EwsAllowOutlook
		{
			get
			{
				return (bool?)this[ADRecipientSchema.EwsAllowOutlook];
			}
		}

		public bool? EwsAllowMacOutlook
		{
			get
			{
				return (bool?)this[ADRecipientSchema.EwsAllowMacOutlook];
			}
		}

		public bool? EwsAllowEntourage
		{
			get
			{
				return (bool?)this[ADRecipientSchema.EwsAllowEntourage];
			}
		}

		public EwsApplicationAccessPolicy? EwsApplicationAccessPolicy
		{
			get
			{
				return (EwsApplicationAccessPolicy?)this[ADRecipientSchema.EwsApplicationAccessPolicy];
			}
		}

		public MultiValuedProperty<string> EwsExceptions
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.EwsExceptions];
			}
		}

		public string PhoneticDepartment
		{
			get
			{
				return (string)this[ADRecipientSchema.PhoneticDepartment];
			}
			set
			{
				this[ADRecipientSchema.PhoneticDepartment] = value;
			}
		}

		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[ADRecipientSchema.PhoneticDisplayName];
			}
			set
			{
				this[ADRecipientSchema.PhoneticDisplayName] = value;
			}
		}

		public string PhoneticFirstName
		{
			get
			{
				return (string)this[ADRecipientSchema.PhoneticFirstName];
			}
			set
			{
				this[ADRecipientSchema.PhoneticFirstName] = value;
			}
		}

		public string PhoneticLastName
		{
			get
			{
				return (string)this[ADRecipientSchema.PhoneticLastName];
			}
			set
			{
				this[ADRecipientSchema.PhoneticLastName] = value;
			}
		}

		public MultiValuedProperty<string> PoliciesIncluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.PoliciesIncluded];
			}
			set
			{
				this[ADRecipientSchema.PoliciesIncluded] = value;
			}
		}

		public MultiValuedProperty<string> PoliciesExcluded
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.PoliciesExcluded];
			}
			set
			{
				this[ADRecipientSchema.PoliciesExcluded] = value;
			}
		}

		internal bool InternalOnly
		{
			get
			{
				return (int)(this[ADRecipientSchema.RecipientSoftDeletedStatus] ?? 0) != 0 || (bool)this[ADRecipientSchema.InternalOnly];
			}
			set
			{
				this[ADRecipientSchema.InternalOnly] = value;
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.PrimarySmtpAddress];
			}
			set
			{
				this[ADRecipientSchema.PrimarySmtpAddress] = value;
			}
		}

		internal bool AllowArchiveAddressSync
		{
			get
			{
				return (bool)this[ADRecipientSchema.AllowArchiveAddressSync];
			}
			set
			{
				this[ADRecipientSchema.AllowArchiveAddressSync] = value;
			}
		}

		internal SmtpAddress OriginalPrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ADObjectSchema.OriginalPrimarySmtpAddress];
			}
			set
			{
				this[ADObjectSchema.OriginalPrimarySmtpAddress] = value;
			}
		}

		public MultiValuedProperty<string> ProtocolSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ProtocolSettings];
			}
			set
			{
				this[ADRecipientSchema.ProtocolSettings] = value;
			}
		}

		public string MAPIBlockOutlookVersions
		{
			get
			{
				return (string)this[ADRecipientSchema.MAPIBlockOutlookVersions];
			}
			internal set
			{
				this[ADRecipientSchema.MAPIBlockOutlookVersions] = value;
			}
		}

		public bool MAPIBlockOutlookExternalConnectivity
		{
			get
			{
				return (bool)this[ADRecipientSchema.MAPIBlockOutlookExternalConnectivity];
			}
		}

		public Unlimited<int> RecipientLimits
		{
			get
			{
				return (Unlimited<int>)this[ADRecipientSchema.RecipientLimits];
			}
			set
			{
				this[ADRecipientSchema.RecipientLimits] = value;
			}
		}

		public RecipientDisplayType? RecipientDisplayType
		{
			get
			{
				return (RecipientDisplayType?)this[ADRecipientSchema.RecipientDisplayType];
			}
			internal set
			{
				this[ADRecipientSchema.RecipientDisplayType] = value;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[ADRecipientSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[ADRecipientSchema.RecipientTypeDetails];
			}
			internal set
			{
				this[ADRecipientSchema.RecipientTypeDetails] = value;
			}
		}

		public RecipientTypeDetails PreviousRecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[ADRecipientSchema.PreviousRecipientTypeDetails];
			}
			internal set
			{
				this[ADRecipientSchema.PreviousRecipientTypeDetails] = value;
			}
		}

		public ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.DefaultPublicFolderMailbox];
			}
			internal set
			{
				this[ADRecipientSchema.DefaultPublicFolderMailbox] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFrom
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				this[ADRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				this[ADRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.RejectMessagesFromSendersOrMembers];
			}
			internal set
			{
				this[ADRecipientSchema.RejectMessagesFromSendersOrMembers] = value;
			}
		}

		public bool RequireAllSendersAreAuthenticated
		{
			get
			{
				return (bool)this[ADRecipientSchema.RequireAllSendersAreAuthenticated];
			}
			set
			{
				this[ADRecipientSchema.RequireAllSendersAreAuthenticated] = value;
			}
		}

		public SecurityIdentifier MasterAccountSid
		{
			get
			{
				return (SecurityIdentifier)this[ADRecipientSchema.MasterAccountSid];
			}
			internal set
			{
				this[ADRecipientSchema.MasterAccountSid] = value;
			}
		}

		public string LinkedMasterAccount
		{
			get
			{
				return (string)this[ADRecipientSchema.LinkedMasterAccount];
			}
			internal set
			{
				this.propertyBag.SetField(ADRecipientSchema.LinkedMasterAccount, value);
			}
		}

		public int? ResourceCapacity
		{
			get
			{
				return (int?)this[ADRecipientSchema.ResourceCapacity];
			}
			set
			{
				this[ADRecipientSchema.ResourceCapacity] = value;
			}
		}

		public MultiValuedProperty<string> ResourceCustom
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ResourceCustom];
			}
			set
			{
				this[ADRecipientSchema.ResourceCustom] = value;
			}
		}

		public MultiValuedProperty<string> ResourceMetaData
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ResourceMetaData];
			}
			set
			{
				this[ADRecipientSchema.ResourceMetaData] = value;
			}
		}

		public string ResourcePropertiesDisplay
		{
			get
			{
				return (string)this[ADRecipientSchema.ResourcePropertiesDisplay];
			}
			internal set
			{
				this[ADRecipientSchema.ResourcePropertiesDisplay] = value;
			}
		}

		public MultiValuedProperty<string> ResourceSearchProperties
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.ResourceSearchProperties];
			}
			internal set
			{
				this[ADRecipientSchema.ResourceSearchProperties] = value;
			}
		}

		public ExchangeResourceType? ResourceType
		{
			get
			{
				return (ExchangeResourceType?)this[ADRecipientSchema.ResourceType];
			}
			set
			{
				this[ADRecipientSchema.ResourceType] = value;
			}
		}

		public int? SCLDeleteThreshold
		{
			get
			{
				return (int?)this[ADRecipientSchema.SCLDeleteThreshold];
			}
			set
			{
				this[ADRecipientSchema.SCLDeleteThreshold] = value;
			}
		}

		public bool? SCLDeleteEnabled
		{
			get
			{
				return (bool?)this[ADRecipientSchema.SCLDeleteEnabled];
			}
			set
			{
				this[ADRecipientSchema.SCLDeleteEnabled] = value;
			}
		}

		public int? SCLRejectThreshold
		{
			get
			{
				return (int?)this[ADRecipientSchema.SCLRejectThreshold];
			}
			set
			{
				this[ADRecipientSchema.SCLRejectThreshold] = value;
			}
		}

		public bool? SCLRejectEnabled
		{
			get
			{
				return (bool?)this[ADRecipientSchema.SCLRejectEnabled];
			}
			set
			{
				this[ADRecipientSchema.SCLRejectEnabled] = value;
			}
		}

		public int? SCLQuarantineThreshold
		{
			get
			{
				return (int?)this[ADRecipientSchema.SCLQuarantineThreshold];
			}
			set
			{
				this[ADRecipientSchema.SCLQuarantineThreshold] = value;
			}
		}

		public bool? SCLQuarantineEnabled
		{
			get
			{
				return (bool?)this[ADRecipientSchema.SCLQuarantineEnabled];
			}
			set
			{
				this[ADRecipientSchema.SCLQuarantineEnabled] = value;
			}
		}

		public int? SCLJunkThreshold
		{
			get
			{
				return (int?)this[ADRecipientSchema.SCLJunkThreshold];
			}
			set
			{
				this[ADRecipientSchema.SCLJunkThreshold] = value;
			}
		}

		public bool? SCLJunkEnabled
		{
			get
			{
				return (bool?)this[ADRecipientSchema.SCLJunkEnabled];
			}
			set
			{
				this[ADRecipientSchema.SCLJunkEnabled] = value;
			}
		}

		public bool ShowGalAsDefaultView
		{
			get
			{
				return Convert.ToBoolean(this[ADRecipientSchema.AddressBookFlags]);
			}
			set
			{
				this[ADRecipientSchema.AddressBookFlags] = Convert.ToInt32(value);
			}
		}

		public string SimpleDisplayName
		{
			get
			{
				return (string)this[ADRecipientSchema.SimpleDisplayName];
			}
			set
			{
				this[ADRecipientSchema.SimpleDisplayName] = value;
			}
		}

		public MultiValuedProperty<byte[]> SMimeCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.SMimeCertificate];
			}
			set
			{
				this[ADRecipientSchema.SMimeCertificate] = value;
			}
		}

		public string TextEncodedORAddress
		{
			get
			{
				return (string)this[ADRecipientSchema.TextEncodedORAddress];
			}
			set
			{
				this[ADRecipientSchema.TextEncodedORAddress] = value;
			}
		}

		public ProxyAddressCollection UMAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ADRecipientSchema.UMAddresses];
			}
			internal set
			{
				this[ADRecipientSchema.UMAddresses] = value;
			}
		}

		public MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.UMDtmfMap];
			}
			set
			{
				this[ADRecipientSchema.UMDtmfMap] = value;
			}
		}

		public GeoCoordinates GeoCoordinates
		{
			get
			{
				return (GeoCoordinates)this[ADRecipientSchema.GeoCoordinates];
			}
			set
			{
				this[ADRecipientSchema.GeoCoordinates] = value;
			}
		}

		public AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
		{
			get
			{
				return (AllowUMCallsFromNonUsersFlags)this[ADRecipientSchema.AllowUMCallsFromNonUsers];
			}
			set
			{
				this[ADRecipientSchema.AllowUMCallsFromNonUsers] = value;
			}
		}

		public ADObjectId UMRecipientDialPlanId
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.UMRecipientDialPlanId];
			}
			set
			{
				this[ADRecipientSchema.UMRecipientDialPlanId] = value;
			}
		}

		public byte[] UMSpokenName
		{
			get
			{
				return (byte[])this[ADRecipientSchema.UMSpokenName];
			}
			set
			{
				this[ADRecipientSchema.UMSpokenName] = value;
			}
		}

		public UseMapiRichTextFormat UseMapiRichTextFormat
		{
			get
			{
				return (UseMapiRichTextFormat)this[ADRecipientSchema.UseMapiRichTextFormat];
			}
			set
			{
				this[ADRecipientSchema.UseMapiRichTextFormat] = value;
			}
		}

		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.WindowsEmailAddress];
			}
			set
			{
				this[ADRecipientSchema.WindowsEmailAddress] = value;
			}
		}

		internal SmtpAddress OriginalWindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[ADObjectSchema.OriginalWindowsEmailAddress];
			}
			set
			{
				this[ADObjectSchema.OriginalWindowsEmailAddress] = value;
			}
		}

		public byte[] SafeSendersHash
		{
			get
			{
				return (byte[])this[ADRecipientSchema.SafeSendersHash];
			}
			internal set
			{
				this[ADRecipientSchema.SafeSendersHash] = value;
			}
		}

		public byte[] SafeRecipientsHash
		{
			get
			{
				return (byte[])this[ADRecipientSchema.SafeRecipientsHash];
			}
			internal set
			{
				this[ADRecipientSchema.SafeRecipientsHash] = value;
			}
		}

		public byte[] BlockedSendersHash
		{
			get
			{
				return (byte[])this[ADRecipientSchema.BlockedSendersHash];
			}
			internal set
			{
				this[ADRecipientSchema.BlockedSendersHash] = value;
			}
		}

		public MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.MailTipTranslations];
			}
			set
			{
				this[ADRecipientSchema.MailTipTranslations] = value;
			}
		}

		public string DefaultMailTip
		{
			get
			{
				return (string)this[ADRecipientSchema.DefaultMailTip];
			}
			set
			{
				this[ADRecipientSchema.DefaultMailTip] = value;
			}
		}

		public bool ModerationEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.ModerationEnabled];
			}
			internal set
			{
				this[ADRecipientSchema.ModerationEnabled] = value;
			}
		}

		public int? HABSeniorityIndex
		{
			get
			{
				return (int?)this[ADRecipientSchema.HABSeniorityIndex];
			}
			internal set
			{
				this[ADRecipientSchema.HABSeniorityIndex] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ModeratedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADRecipientSchema.ModeratedBy];
			}
			internal set
			{
				this[ADRecipientSchema.ModeratedBy] = value;
			}
		}

		public ADObjectId ArbitrationMailbox
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.ArbitrationMailbox];
			}
			internal set
			{
				this[ADRecipientSchema.ArbitrationMailbox] = value;
			}
		}

		public ADObjectId MailboxPlan
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.MailboxPlan];
			}
			internal set
			{
				this[ADRecipientSchema.MailboxPlan] = value;
			}
		}

		public ADObjectId RoleAssignmentPolicy
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.RoleAssignmentPolicy];
			}
			internal set
			{
				this[ADRecipientSchema.RoleAssignmentPolicy] = value;
			}
		}

		public bool BypassNestedModerationEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.BypassNestedModerationEnabled];
			}
			internal set
			{
				this[ADRecipientSchema.BypassNestedModerationEnabled] = value;
			}
		}

		public TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				return (TransportModerationNotificationFlags)this[ADRecipientSchema.SendModerationNotifications];
			}
			internal set
			{
				this[ADRecipientSchema.SendModerationNotifications] = value;
			}
		}

		public byte[] ThumbnailPhoto
		{
			get
			{
				return (byte[])this[ADRecipientSchema.ThumbnailPhoto];
			}
			internal set
			{
				this[ADRecipientSchema.ThumbnailPhoto] = value;
			}
		}

		public string ImmutableId
		{
			get
			{
				return (string)this[ADRecipientSchema.ImmutableId];
			}
			set
			{
				this[ADRecipientSchema.ImmutableId] = value;
			}
		}

		public string ImmutableIdPartial
		{
			get
			{
				return (string)this[ADRecipientSchema.OnPremisesObjectId];
			}
		}

		internal bool UMProvisioningRequested
		{
			get
			{
				return (bool)this[ADRecipientSchema.UMProvisioningRequested];
			}
			set
			{
				this[ADRecipientSchema.UMProvisioningRequested] = value;
			}
		}

		internal bool UCSImListMigrationCompleted
		{
			get
			{
				return (bool)this[ADRecipientSchema.UCSImListMigrationCompleted];
			}
			set
			{
				this[ADRecipientSchema.UCSImListMigrationCompleted] = value;
			}
		}

		public string DirSyncId
		{
			get
			{
				return (string)this[ADRecipientSchema.DirSyncId];
			}
			internal set
			{
				this[ADRecipientSchema.DirSyncId] = value;
			}
		}

		internal MultiValuedProperty<TextMessagingStateBase> TextMessagingState
		{
			get
			{
				return (MultiValuedProperty<TextMessagingStateBase>)this[ADRecipientSchema.TextMessagingState];
			}
		}

		public bool IsPersonToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsPersonToPersonTextMessagingEnabled];
			}
		}

		public bool IsMachineToPersonTextMessagingEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsMachineToPersonTextMessagingEnabled];
			}
		}

		internal ADUser MailboxPlanObject
		{
			get
			{
				return (ADUser)this[ADRecipientSchema.MailboxPlanObject];
			}
			set
			{
				this[ADRecipientSchema.MailboxPlanObject] = value;
			}
		}

		public bool MailboxAuditEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.AuditEnabled];
			}
			set
			{
				this[ADRecipientSchema.AuditEnabled] = value;
			}
		}

		public EnhancedTimeSpan MailboxAuditLogAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[ADRecipientSchema.AuditLogAgeLimit];
			}
			set
			{
				this[ADRecipientSchema.AuditLogAgeLimit] = value;
			}
		}

		public MailboxAuditOperations AuditAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[ADRecipientSchema.AuditAdminFlags];
			}
			set
			{
				this[ADRecipientSchema.AuditAdminFlags] = value;
			}
		}

		public MailboxAuditOperations AuditDelegateOperations
		{
			get
			{
				return (MailboxAuditOperations)this[ADRecipientSchema.AuditDelegateFlags];
			}
			set
			{
				this[ADRecipientSchema.AuditDelegateFlags] = value;
			}
		}

		public MailboxAuditOperations AuditDelegateAdminOperations
		{
			get
			{
				return (MailboxAuditOperations)this[ADRecipientSchema.AuditDelegateAdminFlags];
			}
			set
			{
				this[ADRecipientSchema.AuditDelegateAdminFlags] = value;
			}
		}

		public MailboxAuditOperations AuditOwnerOperations
		{
			get
			{
				return (MailboxAuditOperations)this[ADRecipientSchema.AuditOwnerFlags];
			}
			set
			{
				this[ADRecipientSchema.AuditOwnerFlags] = value;
			}
		}

		public bool BypassAudit
		{
			get
			{
				return (bool)this[ADRecipientSchema.AuditBypassEnabled];
			}
			set
			{
				this[ADRecipientSchema.AuditBypassEnabled] = value;
			}
		}

		public DateTime? AuditLastAdminAccess
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.AuditLastAdminAccess];
			}
		}

		public DateTime? AuditLastDelegateAccess
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.AuditLastDelegateAccess];
			}
		}

		public DateTime? AuditLastExternalAccess
		{
			get
			{
				return (DateTime?)this[ADRecipientSchema.AuditLastExternalAccess];
			}
		}

		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[ADRecipientSchema.UsageLocation];
			}
			set
			{
				this[ADRecipientSchema.UsageLocation] = value;
			}
		}

		public MultiValuedProperty<string> InPlaceHolds
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.InPlaceHolds];
			}
			set
			{
				this[ADRecipientSchema.InPlaceHolds] = value;
			}
		}

		public SmtpAddress JournalArchiveAddress
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.JournalArchiveAddress];
			}
			set
			{
				this[ADRecipientSchema.JournalArchiveAddress] = value;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			switch (this.RecipientType)
			{
			case RecipientType.MailUser:
			case RecipientType.MailContact:
				if (this.ExternalEmailAddress == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired("ExternalEmailAddress", this.RecipientType.ToString()), ADRecipientSchema.ExternalEmailAddress, null));
				}
				break;
			}
			if (this.ExternalEmailAddress != null && this.ExternalEmailAddress.GetType() == typeof(InvalidProxyAddress))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExternalEmailAddressInvalid(((InvalidProxyAddress)this.ExternalEmailAddress).ParseException.Message), ADRecipientSchema.ExternalEmailAddress, this.ExternalEmailAddress));
			}
			if (this.EmailAddresses.Count == 0 && RecipientTypeDetails.ArbitrationMailbox == this.RecipientTypeDetails)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorArbitrationMailboxPropertyEmailAddressesEmpty, ADRecipientSchema.EmailAddresses, null));
			}
			if (base.OrganizationalUnitRoot != null && base.ConfigurationUnit != null)
			{
				string distinguishedName = base.Id.DistinguishedName;
				string distinguishedName2 = base.OrganizationalUnitRoot.DistinguishedName;
				string distinguishedName3 = base.ConfigurationUnit.DistinguishedName;
				bool flag = false;
				if (this.RecipientType == RecipientType.MicrosoftExchange)
				{
					if (!string.IsNullOrEmpty(distinguishedName3) && !distinguishedName.ToLower().EndsWith(distinguishedName3.ToLower()))
					{
						flag = true;
					}
				}
				else if (!string.IsNullOrEmpty(distinguishedName2) && !distinguishedName.ToLower().EndsWith(distinguishedName2.ToLower()))
				{
					flag = true;
				}
				if (flag)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidOrganizationId(distinguishedName, distinguishedName2 ?? "<null>", distinguishedName3 ?? "<null>"), this.Identity, string.Empty));
				}
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			RecipientType recipientType = this.RecipientType;
			switch (this.RecipientType)
			{
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			case RecipientType.MailContact:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				if (!ConsumerIdentityHelper.IsConsumerMailbox(base.Id))
				{
					if (string.IsNullOrEmpty(this.DisplayName))
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.PropertyRequired("DisplayName", recipientType.ToString()), ADRecipientSchema.DisplayName, null));
					}
					else if (VariantConfiguration.InvariantNoFlightingSnapshot.AD.DisplayNameMustContainReadableCharacter.Enabled && !ADRecipient.IsValidName(this.DisplayName))
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.ErrorDisplayNameInvalid, ADRecipientSchema.DisplayName, null));
					}
				}
				break;
			}
			if (string.IsNullOrEmpty(this.Alias))
			{
				return;
			}
			if (this.EmailAddresses.Count > 0 && this.PrimarySmtpAddress == SmtpAddress.Empty)
			{
				List<ProxyAddress> list = new List<ProxyAddress>();
				foreach (ProxyAddress proxyAddress in this.EmailAddresses)
				{
					if (proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
					{
						list.Add(proxyAddress);
					}
				}
				LocalizedString description;
				if (list.Count > 1)
				{
					description = DirectoryStrings.ErrorMultiplePrimaries(ProxyAddressPrefix.Smtp.PrimaryPrefix.ToString());
				}
				else if (list.Count == 1)
				{
					description = DirectoryStrings.ErrorPrimarySmtpInvalid(list[0].ToString());
				}
				else
				{
					description = DirectoryStrings.ErrorMissingPrimarySmtp;
				}
				errors.Add(new ObjectValidationError(description, this.Identity, string.Empty));
			}
			this.SetUMDtmfMapIfNecessary();
			if (ComplianceConfigImpl.ArchivePropertiesHardeningEnabled)
			{
				this.ValidateArchiveProperties(VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled, errors);
			}
			this.SetValidArchiveDatabase();
			if (this.ModerationEnabled)
			{
				if (this.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorArbitrationMailboxCannotBeModerated, this.Identity, string.Empty));
				}
				else if (!this.BypassModerationCheck && MultiValuedPropertyBase.IsNullOrEmpty(this.ModeratedBy) && this.RecipientType != RecipientType.MailUniversalDistributionGroup)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorModeratorRequiredForModeration, ADRecipientSchema.ModeratedBy, null));
				}
			}
			if (this.IsResource)
			{
				if (this.ResourceType == null)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorEmptyResourceTypeofResourceMailbox, this.Identity, string.Empty));
				}
				if (this.ResourceType != null && !((MultiValuedProperty<string>)this[ADRecipientSchema.ResourceSearchProperties]).Contains(this.ResourceType.Value.ToString()))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorMetadataNotSearchProperty, this.Identity, string.Empty));
				}
			}
			if (this.AcceptMessagesOnlyFromDLMembers.Count > 0 && this.RejectMessagesFromDLMembers.Count > 0)
			{
				foreach (ADObjectId adobjectId in this.RejectMessagesFromDLMembers)
				{
					if (this.AcceptMessagesOnlyFromDLMembers.Contains(adobjectId))
					{
						string text = ADRecipient.OrganizationUnitFromADObjectId(adobjectId);
						if (string.IsNullOrEmpty(text))
						{
							text = (adobjectId.DistinguishedName ?? string.Empty);
						}
						else
						{
							text = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
							{
								text,
								adobjectId.Name
							});
						}
						errors.Add(new ObjectValidationError(DirectoryStrings.ErrorDLAsBothAcceptedAndRejected(text ?? string.Empty), this.Identity, string.Empty));
					}
				}
			}
			if (this.AcceptMessagesOnlyFrom.Count > 0 && this.RejectMessagesFrom.Count > 0)
			{
				foreach (ADObjectId adobjectId2 in this.RejectMessagesFrom)
				{
					if (this.AcceptMessagesOnlyFrom.Contains(adobjectId2))
					{
						string text2 = ADRecipient.OrganizationUnitFromADObjectId(adobjectId2);
						if (string.IsNullOrEmpty(text2))
						{
							text2 = (adobjectId2.DistinguishedName ?? string.Empty);
						}
						else
						{
							text2 = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
							{
								text2,
								adobjectId2.Name
							});
						}
						errors.Add(new ObjectValidationError(DirectoryStrings.ErrorRecipientAsBothAcceptedAndRejected(text2), this.Identity, string.Empty));
					}
				}
			}
			if (this.SCLDeleteThreshold != null && this.SCLDeleteEnabled.GetValueOrDefault(false))
			{
				if (this.SCLDeleteThreshold.Value < 0 || this.SCLDeleteThreshold.Value > 9)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThresholdMustBeSet("Delete"), this.Identity, string.Empty));
				}
				if (this.SCLRejectEnabled.GetValueOrDefault(false) && this.SCLDeleteThreshold.Value <= this.SCLRejectThreshold.Value)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThisThresholdMustBeGreaterThanThatThreshold("Delete", "Reject"), this.Identity, string.Empty));
				}
				if (this.SCLQuarantineEnabled.GetValueOrDefault(false) && this.SCLDeleteThreshold.Value <= this.SCLQuarantineThreshold.Value)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThisThresholdMustBeGreaterThanThatThreshold("Delete", "Quarantine"), this.Identity, string.Empty));
				}
			}
			if (this.SCLRejectThreshold != null && this.SCLRejectEnabled.GetValueOrDefault(false))
			{
				if (this.SCLRejectThreshold.Value < 0 || this.SCLRejectThreshold.Value > 9)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThresholdMustBeSet("Reject"), this.Identity, string.Empty));
				}
				if (this.SCLQuarantineEnabled.GetValueOrDefault(false) && this.SCLRejectThreshold.Value <= this.SCLQuarantineThreshold.Value)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThisThresholdMustBeGreaterThanThatThreshold("Reject", "Quarantine"), this.Identity, string.Empty));
				}
			}
			if (this.SCLQuarantineEnabled.GetValueOrDefault(false) && this.SCLQuarantineThreshold != null && (this.SCLQuarantineThreshold.Value < 0 || this.SCLQuarantineThreshold.Value > 9))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThresholdMustBeSet("Quarantine"), this.Identity, string.Empty));
			}
			if (this.SCLJunkEnabled.GetValueOrDefault(false) && this.SCLJunkThreshold != null && (this.SCLJunkThreshold.Value < 0 || this.SCLJunkThreshold.Value > 9))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorThresholdMustBeSet("Junk"), this.Identity, string.Empty));
			}
			if (this.MessageFormat == MessageFormat.Text)
			{
				if (this.MessageBodyFormat == MessageBodyFormat.Html || this.MessageBodyFormat == MessageBodyFormat.TextAndHtml)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorTextMessageIncludingHtmlBody, this.Identity, string.Empty));
				}
				if (this.MacAttachmentFormat == MacAttachmentFormat.AppleSingle || this.MacAttachmentFormat == MacAttachmentFormat.AppleDouble)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorTextMessageIncludingAppleAttachment, this.Identity, string.Empty));
				}
			}
			else if (this.MacAttachmentFormat == MacAttachmentFormat.UuEncode)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorMimeMessageIncludingUuEncodedAttachment, this.Identity, string.Empty));
			}
			if (!this.propertyBag.IsReadOnly)
			{
				if (this.EmailAddressPolicyEnabled)
				{
					if (this.OriginalWindowsEmailAddress != SmtpAddress.Empty && this.WindowsEmailAddress != this.OriginalWindowsEmailAddress)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.ErrorCannotSetWindowsEmailAddress, this.Identity, string.Empty));
					}
					if (this.OriginalPrimarySmtpAddress != SmtpAddress.Empty && this.PrimarySmtpAddress != this.OriginalPrimarySmtpAddress)
					{
						errors.Add(new ObjectValidationError(DirectoryStrings.ErrorCannotSetPrimarySmtpAddress, this.Identity, string.Empty));
						return;
					}
				}
				else if (this.PrimarySmtpAddress != this.OriginalPrimarySmtpAddress && this.WindowsEmailAddress != this.OriginalWindowsEmailAddress && this.PrimarySmtpAddress != SmtpAddress.Empty && this.PrimarySmtpAddress != this.WindowsEmailAddress)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorPrimarySmtpAddressAndWindowsEmailAddressNotMatch, this.Identity, string.Empty));
				}
			}
		}

		internal static bool IsValidName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(name, 0);
			if (UnicodeCategory.Control == unicodeCategory || UnicodeCategory.Format == unicodeCategory)
			{
				return false;
			}
			for (int i = 0; i < name.Length; i++)
			{
				unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(name, i);
				if (UnicodeCategory.Control != unicodeCategory && UnicodeCategory.Format != unicodeCategory && UnicodeCategory.ConnectorPunctuation != unicodeCategory && UnicodeCategory.DashPunctuation != unicodeCategory && UnicodeCategory.OpenPunctuation != unicodeCategory && UnicodeCategory.ClosePunctuation != unicodeCategory && UnicodeCategory.InitialQuotePunctuation != unicodeCategory && UnicodeCategory.FinalQuotePunctuation != unicodeCategory && UnicodeCategory.OtherPunctuation != unicodeCategory && UnicodeCategory.SpaceSeparator != unicodeCategory && UnicodeCategory.LineSeparator != unicodeCategory && UnicodeCategory.ParagraphSeparator != unicodeCategory)
				{
					return true;
				}
			}
			return false;
		}

		internal bool BypassModerationCheck { get; set; }

		internal static bool IsMemberOf(ADObjectId recipientId, ADObjectId groupId, bool directOnly, IRecipientSession session)
		{
			int num = -1;
			bool result;
			ADRecipient.TryIsMemberOfWithLimit(recipientId, groupId, directOnly, session, ref num, out result);
			return result;
		}

		internal static bool TryIsMemberOfWithLimit(ADObjectId recipientId, ADObjectId groupId, bool directOnly, IRecipientSession session, ref int adQueryLimit, out bool isMember)
		{
			if (ADObjectId.Equals(recipientId, groupId))
			{
				isMember = true;
				return true;
			}
			if (adQueryLimit.Equals(0))
			{
				ExTraceGlobals.ADFindTracer.TraceDebug<int, ADObjectId, ADObjectId>(0L, "ADRecipient.TryIsMemberOfWithLimit: AD query limit {0} has been reached for {1} is a member of {2} lookup.", adQueryLimit, recipientId, groupId);
				isMember = false;
				return false;
			}
			int num = 1;
			bool result = ADRecipient.TryIsStrictMemberOfWithLimit(recipientId, session.Read(groupId), directOnly, session, new HashSet<ADObjectId>(), ref num, adQueryLimit, out isMember);
			adQueryLimit -= num;
			return result;
		}

		internal static bool TryParseMailboxProvisioningData(string mailboxProvisioningData, out string mailboxPlanName, out MailboxProvisioningConstraint regionConstraint)
		{
			string[] array = mailboxProvisioningData.Replace(" ", string.Empty).ToUpper().Split(new char[]
			{
				';'
			});
			mailboxPlanName = null;
			regionConstraint = null;
			string text = null;
			string text2 = null;
			string text3 = null;
			bool flag = true;
			bool flag2 = true;
			foreach (string text4 in array)
			{
				if (text4.Contains("MBX="))
				{
					if (text2 != null)
					{
						flag = false;
					}
					text2 = text4.Split(new char[]
					{
						'='
					})[1];
				}
				else if (text4.Contains("TYPE="))
				{
					if (text3 != null)
					{
						flag = false;
					}
					text3 = text4.Split(new char[]
					{
						'='
					})[1];
				}
				else if (text4.Contains("REG="))
				{
					if (text != null)
					{
						flag2 = false;
					}
					text = text4.Split(new char[]
					{
						'='
					})[1];
				}
			}
			if (string.IsNullOrWhiteSpace(text2) || string.IsNullOrWhiteSpace(text3))
			{
				flag = false;
			}
			if (flag)
			{
				mailboxPlanName = text3 + "_" + text2;
			}
			string text5 = null;
			if (text != null && flag2)
			{
				string a;
				if ((a = text) != null)
				{
					if (a == "NA")
					{
						text5 = "NAM";
						goto IL_16D;
					}
					if (a == "EU")
					{
						text5 = "EUR";
						goto IL_16D;
					}
					if (a == "AP")
					{
						text5 = "APAC";
						goto IL_16D;
					}
				}
				flag2 = false;
			}
			IL_16D:
			if (text5 != null)
			{
				regionConstraint = new MailboxProvisioningConstraint(string.Format("{{Region -eq '{0}'}}", text5.ToString()));
			}
			return flag2;
		}

		internal static string GetCustomAttribute(IPropertyBag propertyBag, string customAttributeName)
		{
			if (propertyBag != null)
			{
				switch (customAttributeName)
				{
				case "CustomAttribute1":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute1];
				case "CustomAttribute2":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute2];
				case "CustomAttribute3":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute3];
				case "CustomAttribute4":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute4];
				case "CustomAttribute5":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute5];
				case "CustomAttribute6":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute6];
				case "CustomAttribute7":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute7];
				case "CustomAttribute8":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute8];
				case "CustomAttribute9":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute9];
				case "CustomAttribute10":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute10];
				case "CustomAttribute11":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute11];
				case "CustomAttribute12":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute12];
				case "CustomAttribute13":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute13];
				case "CustomAttribute14":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute14];
				case "CustomAttribute15":
					return (string)propertyBag[ADRecipientSchema.CustomAttribute15];
				case "ExtensionCustomAttribute1":
					return ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ExtensionCustomAttribute1]).FirstOrDefault<string>();
				case "ExtensionCustomAttribute2":
					return ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ExtensionCustomAttribute2]).FirstOrDefault<string>();
				case "ExtensionCustomAttribute3":
					return ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ExtensionCustomAttribute3]).FirstOrDefault<string>();
				case "ExtensionCustomAttribute4":
					return ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ExtensionCustomAttribute4]).FirstOrDefault<string>();
				case "ExtensionCustomAttribute5":
					return ((MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ExtensionCustomAttribute5]).FirstOrDefault<string>();
				}
				return null;
			}
			return null;
		}

		private static bool TryIsStrictMemberOfWithLimit(ADObjectId recipientId, ADRecipient rootGroup, bool directOnly, IRecipientSession session, HashSet<ADObjectId> visitedGroups, ref int adQueryCount, int adQueryLimit, out bool isMember)
		{
			if (recipientId == null || rootGroup == null || !(rootGroup is IADDistributionList))
			{
				isMember = false;
				return true;
			}
			ADGroup adgroup = rootGroup as ADGroup;
			if (adgroup != null && ADRecipient.IsEmptyADGroup(adgroup))
			{
				isMember = false;
				return true;
			}
			if (ADRecipient.IsImmediateChild(adgroup, recipientId))
			{
				isMember = true;
				return true;
			}
			Stack<IADDistributionList> stack = new Stack<IADDistributionList>();
			stack.Push(rootGroup as IADDistributionList);
			while (stack.Count != 0)
			{
				IADDistributionList iaddistributionList = stack.Pop();
				ADObjectId id = (iaddistributionList as ADRecipient).Id;
				if (adQueryLimit.Equals(adQueryCount))
				{
					ExTraceGlobals.ADFindTracer.TraceDebug<int, ADObjectId, ADObjectId>(0L, "ADRecipient.TryIsStrictMemberOfWithLimit: AD query limit {0} has been reached for {1} is a member of {2} lookup.", adQueryLimit, recipientId, id);
					isMember = false;
					return false;
				}
				try
				{
					ADGroup adgroup2 = iaddistributionList as ADGroup;
					bool flag = false;
					ADPagedReader<ADRecipient> adpagedReader;
					if (adgroup2 != null && !adgroup2.HiddenGroupMembershipEnabled)
					{
						adpagedReader = adgroup2.ExpandGroupOnly(10000);
						flag = true;
					}
					else
					{
						adpagedReader = iaddistributionList.Expand(10000);
					}
					adQueryCount++;
					int num = 0;
					foreach (ADRecipient adrecipient in adpagedReader)
					{
						num++;
						if (!flag && ADObjectId.Equals(recipientId, adrecipient.Id))
						{
							isMember = true;
							return true;
						}
						if (10000 == num && (adpagedReader.MorePagesAvailable == null || adpagedReader.MorePagesAvailable.Value))
						{
							if (adQueryLimit.Equals(adQueryCount))
							{
								ExTraceGlobals.ADFindTracer.TraceDebug<int, ADObjectId, ADObjectId>(0L, "ADRecipient.TryIsStrictMemberOfWithLimit: AD query limit {0} has been reached for {1} is a member of {2} lookup.", adQueryLimit, recipientId, id);
								isMember = false;
								return false;
							}
							num = 0;
							adQueryCount++;
						}
						if (!directOnly)
						{
							ADGroup adgroup3 = adrecipient as ADGroup;
							if (ADRecipient.IsImmediateChild(adgroup3, recipientId))
							{
								isMember = true;
								return true;
							}
							if (ADRecipient.NeedIsMemberOfCheckForGroup(adrecipient, id, visitedGroups) && (adgroup3 == null || !ADRecipient.IsEmptyADGroup(adgroup3)))
							{
								visitedGroups.Add(adrecipient.Id);
								stack.Push(adrecipient as IADDistributionList);
							}
						}
					}
				}
				catch (DataValidationException arg)
				{
					ExTraceGlobals.ADFindTracer.TraceError<DataValidationException, ADObjectId, ADObjectId>(0L, "ADRecipient.TryIsStrictMemberOfWithLimit: AD query exception {0} was encountered for {1} is a member of {2} lookup.", arg, recipientId, id);
					isMember = false;
					return false;
				}
			}
			isMember = false;
			return true;
		}

		private static bool NeedIsMemberOfCheckForGroup(ADRecipient subDL, ADObjectId parentGroup, HashSet<ADObjectId> visitedGroups)
		{
			return subDL != null && !ADObjectId.Equals(subDL.Id, parentGroup) && !visitedGroups.Contains(subDL.Id) && subDL is IADDistributionList;
		}

		private static bool IsEmptyADGroup(ADGroup group)
		{
			return !group.HiddenGroupMembershipEnabled && (group.Members == null || group.Members.Count == 0);
		}

		private static bool IsImmediateChild(ADGroup group, ADObjectId recipientId)
		{
			return group != null && group.Members != null && group.Members.Contains(recipientId);
		}

		internal bool IsOWAEnabledStatusConsistent()
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[ADRecipientSchema.ProtocolSettings];
			string text = null;
			string text2 = null;
			foreach (string text3 in multiValuedProperty)
			{
				if (text3.StartsWith("OWA"))
				{
					text2 = text3;
				}
				else if (text3.StartsWith("HTTP"))
				{
					text = text3;
				}
			}
			bool result;
			if (text == null || text2 == null)
			{
				result = true;
			}
			else
			{
				string[] array = text2.Split(new char[]
				{
					'§'
				});
				string[] array2 = text.Split(new char[]
				{
					'§'
				});
				result = (array.Length <= 1 || array2.Length <= 1 || array[1].Equals(array2[1]));
			}
			return result;
		}

		internal void StampDefaultValues(RecipientType recipientType)
		{
			if (recipientType == RecipientType.UserMailbox)
			{
				this.StampMailboxDefaultValues(true);
				return;
			}
			if (recipientType == RecipientType.MailUser)
			{
				ADUser aduser = (ADUser)this;
				aduser.MessageFormat = MessageFormat.Mime;
				aduser.MessageBodyFormat = MessageBodyFormat.TextAndHtml;
				aduser.UseMapiRichTextFormat = UseMapiRichTextFormat.UseDefaultSettings;
				aduser.RecipientTypeDetails = RecipientTypeDetails.MailUser;
				aduser.RecipientDisplayType = new RecipientDisplayType?(Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.RemoteMailUser);
				aduser.ArchiveQuota = ProvisioningHelper.DefaultArchiveQuota;
				aduser.ArchiveWarningQuota = ProvisioningHelper.DefaultArchiveWarningQuota;
				aduser.RecoverableItemsQuota = ProvisioningHelper.DefaultRecoverableItemsQuota;
				aduser.RecoverableItemsWarningQuota = ProvisioningHelper.DefaultRecoverableItemsWarningQuota;
				aduser.CalendarLoggingQuota = ProvisioningHelper.DefaultCalendarLoggingQuota;
				return;
			}
			if (recipientType == RecipientType.MailContact)
			{
				ADContact adcontact = (ADContact)this;
				adcontact.MessageFormat = MessageFormat.Mime;
				adcontact.MessageBodyFormat = MessageBodyFormat.TextAndHtml;
				adcontact.DeliverToForwardingAddress = false;
				adcontact.RequireAllSendersAreAuthenticated = false;
				adcontact.UseMapiRichTextFormat = UseMapiRichTextFormat.UseDefaultSettings;
				adcontact.RecipientDisplayType = new RecipientDisplayType?(Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.RemoteMailUser);
				return;
			}
			if (recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup)
			{
				ADGroup adgroup = (ADGroup)this;
				adgroup.ReportToOriginatorEnabled = true;
				adgroup.RequireAllSendersAreAuthenticated = true;
				return;
			}
			if (recipientType == RecipientType.DynamicDistributionGroup)
			{
				ADDynamicGroup addynamicGroup = (ADDynamicGroup)this;
				addynamicGroup.ReportToOriginatorEnabled = true;
				addynamicGroup.RequireAllSendersAreAuthenticated = true;
			}
		}

		internal void StampMailboxDefaultValues(bool isNewState)
		{
			ADUser aduser = (ADUser)this;
			if (isNewState && aduser.UseDatabaseQuotaDefaults == null)
			{
				aduser.UseDatabaseQuotaDefaults = new bool?(true);
			}
			aduser.ArchiveQuota = ProvisioningHelper.DefaultArchiveQuota;
			aduser.ArchiveWarningQuota = ProvisioningHelper.DefaultArchiveWarningQuota;
			aduser.RecoverableItemsQuota = ProvisioningHelper.DefaultRecoverableItemsQuota;
			aduser.RecoverableItemsWarningQuota = ProvisioningHelper.DefaultRecoverableItemsWarningQuota;
			aduser.CalendarLoggingQuota = ProvisioningHelper.DefaultCalendarLoggingQuota;
		}

		internal void PopulateBypassModerationFromSendersOrMembers()
		{
			this.GetCombinedIdentities(ADRecipientSchema.BypassModerationFrom, ADRecipientSchema.BypassModerationFromDLMembers, ADRecipientSchema.BypassModerationFromSendersOrMembers);
		}

		internal void PopulateAcceptMessagesOnlyFromSendersOrMembers()
		{
			this.GetCombinedIdentities(ADRecipientSchema.AcceptMessagesOnlyFrom, ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers);
		}

		internal void PopulateRejectMessagesFromSendersOrMembers()
		{
			this.GetCombinedIdentities(ADRecipientSchema.RejectMessagesFrom, ADRecipientSchema.RejectMessagesFromDLMembers, ADRecipientSchema.RejectMessagesFromSendersOrMembers);
		}

		private void GetCombinedIdentities(ADPropertyDefinition propertyDefinition1, ADPropertyDefinition propertyDefinition2, ADPropertyDefinition propertyDefinitionResult)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)this.propertyBag[propertyDefinition1];
			MultiValuedProperty<ADObjectId> multiValuedProperty2 = (MultiValuedProperty<ADObjectId>)this.propertyBag[propertyDefinition2];
			MultiValuedProperty<ADObjectId> multiValuedProperty3 = new MultiValuedProperty<ADObjectId>();
			try
			{
				if (multiValuedProperty.Count != 0 || multiValuedProperty2.Count != 0)
				{
					HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
					foreach (ADObjectId item in multiValuedProperty)
					{
						if (hashSet.Add(item))
						{
							multiValuedProperty3.Add(item);
						}
					}
					foreach (ADObjectId item2 in multiValuedProperty2)
					{
						if (hashSet.Add(item2))
						{
							multiValuedProperty3.Add(item2);
						}
					}
				}
			}
			finally
			{
				multiValuedProperty3.ResetChangeTracking();
				this.propertyBag[propertyDefinitionResult] = multiValuedProperty3;
			}
		}

		internal static bool TryGetMailTipParts(string cultureAndMailTip, out string culture, out string mailTip)
		{
			culture = string.Empty;
			mailTip = string.Empty;
			if (cultureAndMailTip == null)
			{
				return false;
			}
			string[] array = cultureAndMailTip.Split(new char[]
			{
				':'
			}, 2);
			if (array.Length != 2)
			{
				return false;
			}
			culture = array[0];
			mailTip = array[1];
			return true;
		}

		internal static bool IsRecipientTypeDL(RecipientType recipientType)
		{
			bool result = false;
			switch (recipientType)
			{
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				result = true;
				break;
			}
			return result;
		}

		internal static bool IsSystemMailbox(RecipientTypeDetails rtd)
		{
			return rtd == RecipientTypeDetails.MailboxPlan || rtd == RecipientTypeDetails.ArbitrationMailbox || rtd == RecipientTypeDetails.DiscoveryMailbox || rtd == RecipientTypeDetails.MonitoringMailbox || rtd == RecipientTypeDetails.SystemAttendantMailbox || rtd == RecipientTypeDetails.SystemMailbox || rtd == RecipientTypeDetails.AuditLogMailbox;
		}

		internal bool? IsAclCapable
		{
			get
			{
				return (bool?)this[ADRecipientSchema.IsAclCapable];
			}
		}

		internal static object IsAclCapableGetter(IPropertyBag propertyBag)
		{
			RecipientDisplayType? recipientDisplayType = (RecipientDisplayType?)propertyBag[ADRecipientSchema.RecipientDisplayType];
			if (recipientDisplayType == null)
			{
				return null;
			}
			return (recipientDisplayType.Value & Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.ACLableMailboxUser) != Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.MailboxUser;
		}

		internal bool IsAnyAddressMatched(params string[] targetSmtpAddresses)
		{
			HashSet<string> hashSet = new HashSet<string>(targetSmtpAddresses, StringComparer.OrdinalIgnoreCase);
			foreach (ProxyAddress proxyAddress in this.EmailAddresses)
			{
				SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
				if (smtpProxyAddress != null && hashSet.Contains(smtpProxyAddress.SmtpAddress))
				{
					return true;
				}
			}
			return false;
		}

		internal bool IsValidSecurityPrincipal
		{
			get
			{
				return (bool)this[ADRecipientSchema.IsValidSecurityPrincipal];
			}
		}

		internal static object IsValidSecurityPrincipalGetter(IPropertyBag propertyBag)
		{
			return ADRecipient.IsValidRecipient(propertyBag, true);
		}

		internal static bool IsValidRecipient(IPropertyBag propertyBag, bool enforceAclCapableCheck)
		{
			switch ((RecipientType)propertyBag[ADRecipientSchema.RecipientType])
			{
			case RecipientType.Invalid:
			case RecipientType.User:
			case RecipientType.Contact:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.DynamicDistributionGroup:
			case RecipientType.PublicFolder:
			case RecipientType.PublicDatabase:
			case RecipientType.MicrosoftExchange:
				return false;
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			{
				UserAccountControlFlags userAccountControlFlags = (UserAccountControlFlags)propertyBag[ADUserSchema.UserAccountControl];
				if ((userAccountControlFlags & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.AccountDisabled)
				{
					SecurityIdentifier securityIdentifier = (SecurityIdentifier)propertyBag[ADRecipientSchema.MasterAccountSid];
					if (securityIdentifier == null || securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
					{
						return false;
					}
				}
				break;
			}
			case RecipientType.MailContact:
			case RecipientType.Group:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
			case RecipientType.Computer:
				break;
			default:
				return true;
			}
			string value = (string)propertyBag[ADRecipientSchema.LegacyExchangeDN];
			bool? flag = (bool?)propertyBag[ADRecipientSchema.IsAclCapable];
			if (string.IsNullOrEmpty(value) || (propertyBag[ADRecipientSchema.MasterAccountSid] == null && propertyBag[IADSecurityPrincipalSchema.Sid] == null) || (enforceAclCapableCheck && flag == false))
			{
				return false;
			}
			return true;
		}

		public virtual void PopulateDtmfMap(bool create)
		{
			this.PopulateDtmfMap(create, this.DisplayName, this.DisplayName, this.PrimarySmtpAddress, null);
		}

		internal void PopulateDtmfMap(bool create, string firstLast, string lastFirst, SmtpAddress smtpAddress, IList<string> phones)
		{
			if (!create && this.UMDtmfMap.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>(16);
			if (!string.IsNullOrEmpty(firstLast))
			{
				list.Add("firstNameLastName:" + DtmfString.DtmfEncode(firstLast));
			}
			if (!string.IsNullOrEmpty(lastFirst))
			{
				list.Add("lastNameFirstName:" + DtmfString.DtmfEncode(lastFirst));
			}
			if (SmtpAddress.Empty != smtpAddress)
			{
				list.Add("emailAddress:" + DtmfString.DtmfEncode(smtpAddress.Local));
			}
			if (phones != null)
			{
				foreach (string text in phones)
				{
					if (!string.IsNullOrEmpty(text))
					{
						list.Add("reversedPhone:" + DtmfString.Reverse(text));
					}
				}
			}
			bool flag = false;
			if (list.Count != this.UMDtmfMap.Count)
			{
				flag = true;
			}
			else
			{
				foreach (string item in list)
				{
					if (!this.UMDtmfMap.Contains(item))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				this.UMDtmfMap.Clear();
				foreach (string text2 in list)
				{
					this.UMDtmfMap.Add(text2.Substring(0, Math.Min(text2.Length, 256)));
				}
			}
		}

		internal static bool TryGetAuthenticationTypeFilterInternal(AuthenticationType authType, OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			queryFilter = null;
			if (organizationId == null || organizationId.OrganizationalUnit == null)
			{
				errorMessage = DirectoryStrings.CannotBuildAuthenticationTypeFilterBadArgument(authType.ToString());
				return false;
			}
			List<TextFilter> list = new List<TextFilter>();
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			IDictionary<string, AuthenticationType> namespaceAuthenticationTypeHash = organizationIdCacheValue.NamespaceAuthenticationTypeHash;
			foreach (string text in namespaceAuthenticationTypeHash.Keys)
			{
				if (namespaceAuthenticationTypeHash[text] == authType)
				{
					list.Add(new TextFilter(ADRecipientSchema.WindowsLiveID, "@" + text, MatchOptions.Suffix, MatchFlags.IgnoreCase));
				}
			}
			if (list.Count == 0)
			{
				errorMessage = DirectoryStrings.CannotBuildAuthenticationTypeFilterNoNamespacesOfType(authType.ToString());
				return false;
			}
			queryFilter = QueryFilter.OrTogether(list.ToArray());
			errorMessage = LocalizedString.Empty;
			return true;
		}

		private void PopulateGlobalAddressListFromAddressBookPolicy()
		{
			if (this.m_Session == null)
			{
				return;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(this.m_Session.SessionSettings.RootOrgId, base.OrganizationId, null, false), 6694, "PopulateGlobalAddressListFromAddressBookPolicy", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADRecipient.cs");
			if (tenantOrTopologyConfigurationSession != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = tenantOrTopologyConfigurationSession.Read<AddressBookMailboxPolicy>(this.AddressBookPolicy);
				this.globalAddressListFromAddressBookPolicy = ((addressBookMailboxPolicy != null) ? addressBookMailboxPolicy.GlobalAddressList : null);
			}
		}

		internal const string DefaultMailTipTranslationPrefix = "default";

		private const int MaximumAirSyncNumber = 3;

		internal const int DtmfMapEntryMaxLength = 256;

		internal const string DtmfMapFirstNameLastNamePrefix = "firstNameLastName:";

		internal const string DtmfMapLastNameFirstNamePrefix = "lastNameFirstName:";

		internal const string DtmfMapEmailAddressPrefix = "emailAddress:";

		internal const string DtmfMapReversedPhonePrefix = "reversedPhone:";

		internal const int MaxModeratorNum = 25;

		internal const int MaxModeratorNumOnTenant = 10;

		internal const char ResourceSeparator = ':';

		internal const string ResourceTypePrefix = "ResourceType";

		public const string UMExtensionDelimiter = ";";

		public const string UMDialPlanString = "phone-context=";

		public const string SoftDeletedObjectString = "SoftDeletedObject";

		public const string SoftDeletedContainerName = "Soft Deleted Objects";

		public const string StrictSoftDeletedContainerOUName = ",OU=Soft Deleted Objects,";

		public const string SoftDeletedContainerOUName = "OU=Soft Deleted Objects,";

		public const string AliasPattern = "^[!#%&'=`~\\$\\*\\+\\-\\/\\?\\^\\{\\|\\}a-zA-Z0-9_\\u00A1-\\u00FF]+(\\.[!#%&'=`~\\$\\*\\+\\-\\/\\?\\^\\{\\|\\}a-zA-Z0-9_\\u00A1-\\u00FF]+)*$";

		public const string AliasValidCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&'*+-/=?^_`{|}~.¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ";

		internal static readonly RecipientType[] AllowedModeratorsRecipientTypes = new RecipientType[]
		{
			RecipientType.MailContact,
			RecipientType.UserMailbox,
			RecipientType.MailUser
		};

		internal static readonly RecipientType[] DtmfMapAllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.MailContact,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.MailUniversalDistributionGroup
		};

		internal static readonly ExchangeObjectVersion ArbitrationMailboxObjectVersion = new ExchangeObjectVersion(1, 0, 14, 0, 0, 0);

		internal static readonly ExchangeObjectVersion PublicFolderMailboxObjectVersion = new ExchangeObjectVersion(1, 1, ExchangeObjectVersion.Exchange2012.ExchangeBuild);

		internal static readonly ExchangeObjectVersion TeamMailboxObjectVersion = new ExchangeObjectVersion(1, 1, ExchangeObjectVersion.Exchange2012.ExchangeBuild);

		private ADObjectId globalAddressListFromAddressBookPolicy;

		internal static readonly char[] ResourceSeperatorArray = new char[]
		{
			':'
		};
	}
}
