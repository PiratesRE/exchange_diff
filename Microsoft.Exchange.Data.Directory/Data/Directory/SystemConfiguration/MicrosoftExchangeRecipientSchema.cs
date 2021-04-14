using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MicrosoftExchangeRecipientSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition EmailAddresses = ADRecipientSchema.EmailAddresses;

		public static readonly ADPropertyDefinition PrimarySmtpAddress = ADRecipientSchema.PrimarySmtpAddress;

		public static readonly ADPropertyDefinition Alias = ADRecipientSchema.Alias;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition HiddenFromAddressListsEnabled = ADRecipientSchema.HiddenFromAddressListsEnabled;

		public static readonly ADPropertyDefinition EmailAddressPolicyEnabled = ADRecipientSchema.EmailAddressPolicyEnabled;

		public static readonly ADPropertyDefinition LegacyExchangeDN = ADRecipientSchema.LegacyExchangeDN;

		public static readonly ADPropertyDefinition ForwardingAddress = ADRecipientSchema.ForwardingAddress;
	}
}
