using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class DomainContentConfigSchema : EdgeDomainContentConfigSchema
	{
		private const int MaxIntAsPercent = 100;

		public static readonly ADPropertyDefinition PreferredInternetCodePageForShiftJis = new ADPropertyDefinition("PreferredInternetCodePageForShiftJis", ExchangeObjectVersion.Exchange2007, typeof(PreferredInternetCodePageForShiftJisEnum), "msExchContentPreferredInternetCodePageForShiftJis", ADPropertyDefinitionFlags.PersistDefaultValue, PreferredInternetCodePageForShiftJisEnum.Undefined, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(PreferredInternetCodePageForShiftJisEnum))
		}, null, null);

		public static readonly ADPropertyDefinition ByteEncoderTypeFor7BitCharsets = new ADPropertyDefinition("ByteEncoderTypeFor7BitCharsets", ExchangeObjectVersion.Exchange2007, typeof(ByteEncoderTypeFor7BitCharsetsEnum), "msExchContentByteEncoderTypeFor7BitCharsets", ADPropertyDefinitionFlags.PersistDefaultValue, ByteEncoderTypeFor7BitCharsetsEnum.Undefined, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ByteEncoderTypeFor7BitCharsetsEnum))
		}, null, null);

		public static readonly ADPropertyDefinition RequiredCharsetCoverage = new ADPropertyDefinition("RequiredCharsetCoverage", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchContentRequiredCharSetCoverage", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 100)
		}, null, null);

		internal static readonly ADPropertyDefinition ADCharacterSet = new ADPropertyDefinition("ADCharacterSet", ExchangeObjectVersion.Exchange2003, typeof(string), "characterSet", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AcceptMessageTypes = new ADPropertyDefinition("AcceptMessageTypes", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchRoutingAcceptMessageType", ADPropertyDefinitionFlags.PersistDefaultValue, 30, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ContentType = new ADPropertyDefinition("ContentType", ExchangeObjectVersion.Exchange2003, typeof(ContentType), "contentType", ADPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.SystemConfiguration.ContentType.MimeHtmlText, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LineWrapSize = new ADPropertyDefinition("LineWrapSize", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<int>), "lineWrap", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, 132)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisplaySenderName = new ADPropertyDefinition("MsExchRoutingDisplaySenderEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchRoutingDisplaySenderEnabled", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TNEFEnabled = new ADPropertyDefinition("TNEFEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool?), "sendTNEF", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowedOOFType = new ADPropertyDefinition("AllowedOOFType", ExchangeObjectVersion.Exchange2003, typeof(AllowedOOFType), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.SystemConfiguration.AllowedOOFType.External, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, new GetterDelegate(DomainContentConfig.AllowedOOFTypeGetter), new SetterDelegate(DomainContentConfig.AllowedOOFTypeSetter), null, null);

		public static readonly ADPropertyDefinition AutoReplyEnabled = new ADPropertyDefinition("AutoReplyEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 2), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 2), null, null);

		public static readonly ADPropertyDefinition AutoForwardEnabled = new ADPropertyDefinition("AutoForwardEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 4), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 4), null, null);

		public static readonly ADPropertyDefinition InternalDomain = new ADPropertyDefinition("InternalDomain", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 64), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 64), null, null);

		public static readonly ADPropertyDefinition TargetDeliveryDomain = new ADPropertyDefinition("TargetDeliveryDomain", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 256), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 256), null, null);

		public static readonly ADPropertyDefinition CharacterSet = new ADPropertyDefinition("CharacterSet", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.ADCharacterSet
		}, null, new GetterDelegate(DomainContentConfig.CharacterSetGetter), new SetterDelegate(DomainContentConfig.CharacterSetSetter), null, null);

		public static readonly ADPropertyDefinition DeliveryReportEnabled = new ADPropertyDefinition("DeliveryReportEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 8), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 8), null, null);

		public static readonly ADPropertyDefinition NDREnabled = new ADPropertyDefinition("NDREnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 16), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 16), null, null);

		public static readonly ADPropertyDefinition MFNEnabled = new ADPropertyDefinition("MFNEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 128), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 128), null, null);

		public static readonly ADPropertyDefinition UseSimpleDisplayName = new ADPropertyDefinition("UseSimpleDisplayName", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 512), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 512), null, null);

		public static readonly ADPropertyDefinition NDRDiagnosticInfoDisabled = new ADPropertyDefinition("NDRDiagnosticInfoDisabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DomainContentConfigSchema.AcceptMessageTypes
		}, null, ADObject.FlagGetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 1024), ADObject.FlagSetterDelegate(DomainContentConfigSchema.AcceptMessageTypes, 1024), null, null);

		public static readonly ADPropertyDefinition MessageCountThreshold = new ADPropertyDefinition("MessageCountThreshold", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchResolveP2", ADPropertyDefinitionFlags.PersistDefaultValue, int.MaxValue, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);
	}
}
