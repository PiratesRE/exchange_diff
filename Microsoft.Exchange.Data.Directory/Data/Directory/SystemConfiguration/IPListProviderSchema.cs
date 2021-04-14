using System;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class IPListProviderSchema : ADConfigurationObjectSchema
	{
		private const int EnabledMask = 1;

		private const int AnyMatchMask = 2;

		public static readonly ADPropertyDefinition ProviderFlags = new ADPropertyDefinition("ProviderFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageHygieneProviderFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LookupDomain = new ADPropertyDefinition("LookupDomain", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomain), "msExchMessageHygieneLookupDomain", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Priority = new ADPropertyDefinition("Priority", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMessageHygienePriority", ADPropertyDefinitionFlags.PersistDefaultValue, int.MaxValue, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Bitmask = new ADPropertyDefinition("Bitmask", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchMessageHygieneBitmask", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RejectionMessage = new ADPropertyDefinition("Rejectionmessage", ExchangeObjectVersion.Exchange2007, typeof(AsciiString), "msExchMessageHygieneRejectionMessage", ADPropertyDefinitionFlags.None, AsciiString.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 240)
		}, null, null);

		public static readonly ADPropertyDefinition IPAddress = new ADPropertyDefinition("IPAddress", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchMessageHygieneIPAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IPListProviderSchema.ProviderFlags
		}, null, ADObject.FlagGetterDelegate(1, IPListProviderSchema.ProviderFlags), ADObject.FlagSetterDelegate(1, IPListProviderSchema.ProviderFlags), null, null);

		public static readonly ADPropertyDefinition AnyMatch = new ADPropertyDefinition("AnyMatch", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			IPListProviderSchema.ProviderFlags
		}, null, ADObject.FlagGetterDelegate(2, IPListProviderSchema.ProviderFlags), ADObject.FlagSetterDelegate(2, IPListProviderSchema.ProviderFlags), null, null);
	}
}
