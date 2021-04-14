using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StoreToDirectorySchemaConverter : SchemaConverter
	{
		private StoreToDirectorySchemaConverter()
		{
			base.Add(ParticipantSchema.LegacyExchangeDN, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetLegacyExchangeDN), null);
			base.Add(ParticipantSchema.SmtpAddress, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetSmtpAddress), null);
			base.Add(InternalSchema.DisplayTypeExInternal, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetDisplayTypeEx), null);
			base.Add(ParticipantSchema.DisplayName, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetDisplayName), null);
			base.Add(ParticipantSchema.Alias, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetAlias), null);
			base.Add(ParticipantSchema.SimpleDisplayName, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetSimpleDisplayName), null);
			base.Add(ParticipantSchema.SipUri, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetSipUri), null);
			base.Add(ParticipantSchema.ParticipantSID, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetParticipantSid), null);
			base.Add(ParticipantSchema.ParticipantGuid, new SchemaConverter.Getter(StoreToDirectorySchemaConverter.GetGuid), null);
		}

		private static object GetLegacyExchangeDN(IReadOnlyPropertyBag propertyBag)
		{
			return StoreToDirectorySchemaConverter.DefaultToNotFound(propertyBag, ADRecipientSchema.LegacyExchangeDN);
		}

		private static object GetSmtpAddress(IReadOnlyPropertyBag propertyBag)
		{
			return ((SmtpAddress)propertyBag[ADRecipientSchema.PrimarySmtpAddress]).ToString();
		}

		private static object GetDisplayTypeEx(IReadOnlyPropertyBag propertyBag)
		{
			RecipientDisplayType? recipientDisplayType = (RecipientDisplayType?)StoreToDirectorySchemaConverter.TryGetValueOrDefault(propertyBag, ADRecipientSchema.RecipientDisplayType);
			RecipientDisplayType? recipientDisplayType2 = (recipientDisplayType != null) ? new RecipientDisplayType?(recipientDisplayType.GetValueOrDefault()) : StoreToDirectorySchemaConverter.ToRecipientDisplayType((RecipientType)StoreToDirectorySchemaConverter.TryGetValueOrDefault(propertyBag, ADRecipientSchema.RecipientType));
			if (recipientDisplayType2 == null)
			{
				return PropertyErrorCode.NotFound;
			}
			return (int)recipientDisplayType2.Value;
		}

		private static object GetDisplayName(IReadOnlyPropertyBag propertyBag)
		{
			return propertyBag[ADRecipientSchema.DisplayName] ?? PropertyErrorCode.NotFound;
		}

		private static object GetAlias(IReadOnlyPropertyBag propertyBag)
		{
			return StoreToDirectorySchemaConverter.DefaultToNotFound(propertyBag, ADRecipientSchema.Alias);
		}

		private static object GetSimpleDisplayName(IReadOnlyPropertyBag propertyBag)
		{
			return propertyBag[ADRecipientSchema.SimpleDisplayName] ?? PropertyErrorCode.NotFound;
		}

		private static object GetSipUri(IReadOnlyPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (proxyAddress.PrefixString.Equals("sip", StringComparison.OrdinalIgnoreCase))
				{
					return proxyAddress.ProxyAddressString.ToLower();
				}
			}
			return " ";
		}

		private static object GetParticipantSid(IReadOnlyPropertyBag propertyBag)
		{
			SecurityIdentifier userSid = (SecurityIdentifier)propertyBag[ADMailboxRecipientSchema.Sid];
			SecurityIdentifier masterAccountSid = (SecurityIdentifier)propertyBag[ADRecipientSchema.MasterAccountSid];
			SecurityIdentifier securityIdentifier = IdentityHelper.CalculateEffectiveSid(userSid, masterAccountSid);
			if (securityIdentifier == null)
			{
				return PropertyErrorCode.NotFound;
			}
			byte[] array = new byte[securityIdentifier.BinaryLength];
			securityIdentifier.GetBinaryForm(array, 0);
			return array;
		}

		private static object GetGuid(IReadOnlyPropertyBag propertyBag)
		{
			if ((Guid)propertyBag[ADObjectSchema.Guid] == Guid.Empty)
			{
				return PropertyErrorCode.NotFound;
			}
			return ((Guid)propertyBag[ADObjectSchema.Guid]).ToByteArray();
		}

		private static object DefaultToNotFound(IReadOnlyPropertyBag properyBag, ADPropertyDefinition propDef)
		{
			object obj = properyBag[propDef];
			if (object.Equals(obj, propDef.DefaultValue))
			{
				return PropertyErrorCode.NotFound;
			}
			return obj;
		}

		private static object TryGetValueOrDefault(IReadOnlyPropertyBag properyBag, ADPropertyDefinition propDef)
		{
			object result;
			try
			{
				result = properyBag[propDef];
			}
			catch (ValueNotPresentException)
			{
				result = propDef.DefaultValue;
			}
			return result;
		}

		private static RecipientDisplayType? ToRecipientDisplayType(RecipientType recipientType)
		{
			switch (recipientType)
			{
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
				return new RecipientDisplayType?(RecipientDisplayType.MailboxUser);
			case RecipientType.MailContact:
				return new RecipientDisplayType?(RecipientDisplayType.RemoteMailUser);
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailNonUniversalGroup:
				return new RecipientDisplayType?(RecipientDisplayType.DistributionGroup);
			case RecipientType.MailUniversalSecurityGroup:
				return new RecipientDisplayType?(RecipientDisplayType.SecurityDistributionGroup);
			case RecipientType.DynamicDistributionGroup:
				return new RecipientDisplayType?(RecipientDisplayType.DynamicDistributionGroup);
			case RecipientType.PublicFolder:
				return new RecipientDisplayType?(RecipientDisplayType.PublicFolder);
			}
			return null;
		}

		internal static readonly StoreToDirectorySchemaConverter Instance = new StoreToDirectorySchemaConverter();
	}
}
