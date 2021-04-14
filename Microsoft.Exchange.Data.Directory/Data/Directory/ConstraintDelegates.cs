using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal static class ConstraintDelegates
	{
		internal static PropertyConstraintViolationError ValidateUserSamAccountNameIncludeNoAt(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (propertyBag != null)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
				if (multiValuedProperty.Count > 0 && multiValuedProperty.Contains("user"))
				{
					string text = value as string;
					if (!string.IsNullOrEmpty(text) && -1 != text.IndexOf('@'))
					{
						return new PropertyConstraintViolationError(DirectoryStrings.ErrorUserAccountNameIncludeAt, propertyDefinition, value, owner);
					}
				}
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateUserSamAccountNameLength(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (propertyBag != null)
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
				if (multiValuedProperty.Count > 0 && multiValuedProperty.Contains("user"))
				{
					StringLengthConstraint stringLengthConstraint = new StringLengthConstraint(1, 20);
					return stringLengthConstraint.Validate(value, propertyDefinition, propertyBag);
				}
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateNonNeutralCulture(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			CultureInfo cultureInfo = value as CultureInfo;
			if (cultureInfo == null)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ExArgumentNullException(propertyDefinition.Name), propertyDefinition, value, owner);
			}
			if (cultureInfo.IsNeutralCulture)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorNeutralCulture(cultureInfo.Name), propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateDsnDefaultCulture(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			CultureInfo cultureInfo = value as CultureInfo;
			if (cultureInfo == null)
			{
				return null;
			}
			if (cultureInfo.IsNeutralCulture || CultureInfo.InvariantCulture.Equals(cultureInfo))
			{
				return new PropertyConstraintViolationError(DirectoryStrings.DsnDefaultLanguageMustBeSpecificCulture, propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateMailTip(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			string cultureAndMailTip = (string)value;
			string text;
			string mailTip;
			if (!ADRecipient.TryGetMailTipParts(cultureAndMailTip, out text, out mailTip))
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorMailTipTranslationFormatIncorrect, propertyDefinition, value, owner);
			}
			if (text == string.Empty)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorMailTipCultureNotSpecified, propertyDefinition, value, owner);
			}
			LocalizedString empty = LocalizedString.Empty;
			if (!ConstraintDelegates.TryValidateMailTipCulture(text, ref empty))
			{
				return new PropertyConstraintViolationError(empty, propertyDefinition, value, owner);
			}
			try
			{
				if (!ConstraintDelegates.TryValidateMailTipDisplayableLength(mailTip, ref empty))
				{
					return new PropertyConstraintViolationError(empty, propertyDefinition, value, owner);
				}
			}
			catch (ExchangeDataException ex)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorMailTipHtmlCorrupt(ex.Message), propertyDefinition, value, owner);
			}
			return null;
		}

		private static bool TryValidateMailTipCulture(string culture, ref LocalizedString errorString)
		{
			if (culture == "default")
			{
				return true;
			}
			try
			{
				CultureInfo.GetCultureInfo(culture);
			}
			catch (ArgumentException)
			{
				errorString = DirectoryStrings.ErrorMailTipTranslationCultureNotSupported(culture);
				return false;
			}
			return true;
		}

		private static bool TryValidateMailTipDisplayableLength(string mailTip, ref LocalizedString errorString)
		{
			HtmlToText htmlToText = new HtmlToText();
			bool result;
			using (TextReader textReader = new StringReader(mailTip))
			{
				using (TextWriter textWriter = new StringWriter())
				{
					htmlToText.Convert(textReader, textWriter);
					string text = textWriter.ToString().Trim();
					if (text.Length == 0)
					{
						errorString = DirectoryStrings.ErrorMailTipMustNotBeEmpty;
						result = false;
					}
					else if (text.Length > 175)
					{
						errorString = DirectoryStrings.ErrorMailTipDisplayableLengthExceeded(175);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		internal static PropertyConstraintViolationError ValidateDomainScope(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (propertyBag != null)
			{
				switch ((ScopeType)value)
				{
				case ScopeType.OU:
				case ScopeType.CustomRecipientScope:
				case ScopeType.OrganizationConfig:
				case ScopeType.CustomConfigScope:
				case ScopeType.PartnerDelegatedTenantScope:
				case ScopeType.ExclusiveRecipientScope:
				case ScopeType.ExclusiveConfigScope:
					return new PropertyConstraintViolationError(DirectoryStrings.InvalidRecipientScope(value), propertyDefinition, value, owner);
				}
				return null;
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateConfigReadScope(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (propertyBag != null)
			{
				ScopeType scopeType = (ScopeType)value;
				ScopeType scopeType2 = scopeType;
				switch (scopeType2)
				{
				case ScopeType.None:
				case ScopeType.NotApplicable:
					break;
				default:
					if (scopeType2 != ScopeType.OrganizationConfig)
					{
						return new PropertyConstraintViolationError(DirectoryStrings.InvalidConfigScope(value), propertyDefinition, value, owner);
					}
					break;
				}
				return null;
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateClientVersion(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			try
			{
				new MapiVersionRanges((string)value);
				return null;
			}
			catch (FormatException)
			{
			}
			catch (ArgumentOutOfRangeException)
			{
			}
			return new PropertyConstraintViolationError(DirectoryStrings.BlockedOutlookClientVersionPatternDescription, propertyDefinition, value, owner);
		}

		internal static PropertyConstraintViolationError ValidateMailboxMoveFlags(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (value == null)
			{
				return null;
			}
			RequestFlags requestFlags = (RequestFlags)value;
			if (requestFlags == RequestFlags.None)
			{
				return null;
			}
			bool flag = (requestFlags & RequestFlags.CrossOrg) != RequestFlags.None;
			bool flag2 = (requestFlags & RequestFlags.IntraOrg) != RequestFlags.None;
			bool flag3 = (requestFlags & RequestFlags.Pull) != RequestFlags.None;
			bool flag4 = (requestFlags & RequestFlags.Push) != RequestFlags.None;
			if (flag == flag2 || flag3 == flag4)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.InvalidMailboxMoveFlags(value), propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateHostname(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			string text = (string)value;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			Hostname hostname;
			if (!Hostname.TryParse(text, out hostname))
			{
				return new PropertyConstraintViolationError(DirectoryStrings.InvalidHostname(text), propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateCapabilities(IEnumerable collection, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (collection == null)
			{
				return null;
			}
			List<Capability> list = new List<Capability>();
			foreach (object obj in collection)
			{
				Capability capability = (Capability)obj;
				if (capability == Capability.None)
				{
					return new PropertyConstraintViolationError(DirectoryStrings.ErrorCapabilityNone, propertyDefinition, obj, owner);
				}
				if (CapabilityHelper.IsRootSKUCapability(capability))
				{
					list.Add(capability);
				}
			}
			if (list.Count > 1)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorMoreThanOneSKUCapability(MultiValuedPropertyBase.FormatMultiValuedProperty(list)), propertyDefinition, collection, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateOrganizationCapabilities(IEnumerable collection, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (collection == null)
			{
				return null;
			}
			foreach (object obj in collection)
			{
				if ((Capability)obj == Capability.None)
				{
					return new PropertyConstraintViolationError(DirectoryStrings.ErrorCapabilityNone, propertyDefinition, obj, owner);
				}
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateWhenMailboxCreated(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (propertyBag[propertyDefinition] != null && (DateTime)propertyBag[propertyDefinition] != (DateTime)value)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorNotResettableProperty(propertyDefinition.Name, propertyBag[propertyDefinition].ToString()), propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateThrottlingPolicyFlags(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			ThrottlingPolicyFlags throttlingPolicyFlags = (ThrottlingPolicyFlags)value;
			if ((throttlingPolicyFlags & ThrottlingPolicyFlags.GlobalScope) != ThrottlingPolicyFlags.None && (throttlingPolicyFlags & ThrottlingPolicyFlags.OrganizationScope) != ThrottlingPolicyFlags.None)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorThrottlingPolicyGlobalAndOrganizationScope, propertyDefinition, value, owner);
			}
			if ((throttlingPolicyFlags & ThrottlingPolicyFlags.GlobalScope) != ThrottlingPolicyFlags.None && (throttlingPolicyFlags & ThrottlingPolicyFlags.IsServiceAccount) != ThrottlingPolicyFlags.None)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorServiceAccountThrottlingPolicy(ThrottlingPolicyScopeType.Global.ToString()), propertyDefinition, value, owner);
			}
			if ((throttlingPolicyFlags & ThrottlingPolicyFlags.OrganizationScope) != ThrottlingPolicyFlags.None && (throttlingPolicyFlags & ThrottlingPolicyFlags.IsServiceAccount) != ThrottlingPolicyFlags.None)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ErrorServiceAccountThrottlingPolicy(ThrottlingPolicyScopeType.Organization.ToString()), propertyDefinition, value, owner);
			}
			return null;
		}

		internal static PropertyConstraintViolationError ValidateRemoteRecipientType(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			if (value != null && ((RemoteRecipientType)value & RemoteRecipientType.Migrated) == RemoteRecipientType.None)
			{
				RemoteRecipientType remoteRecipientType = (RemoteRecipientType)value & (RemoteRecipientType.ProvisionMailbox | RemoteRecipientType.ProvisionArchive | RemoteRecipientType.DeprovisionMailbox | RemoteRecipientType.DeprovisionArchive | RemoteRecipientType.RoomMailbox | RemoteRecipientType.EquipmentMailbox | RemoteRecipientType.TeamMailbox);
				if (Array.IndexOf<RemoteRecipientType>(RemoteRecipientTypeHelper.AllowedProvisionOrDeprovisionType, remoteRecipientType) == -1)
				{
					return new PropertyConstraintViolationError(DirectoryStrings.ErrorInvalidRemoteRecipientType(remoteRecipientType.ToString()), propertyDefinition, value, owner);
				}
			}
			return null;
		}
	}
}
