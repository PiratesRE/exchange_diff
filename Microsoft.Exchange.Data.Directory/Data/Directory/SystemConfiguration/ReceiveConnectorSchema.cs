using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ReceiveConnectorSchema : ADConfigurationObjectSchema
	{
		internal static object SizeEnabledGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ReceiveConnectorSchema.ProtocolOptions];
			bool flag = 0 != (num & 1);
			bool flag2 = 0 != (num & 2);
			if (!flag)
			{
				return SizeMode.Disabled;
			}
			if (flag2)
			{
				return SizeMode.EnabledWithoutValue;
			}
			return SizeMode.Enabled;
		}

		internal static void SizeEnabledSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ReceiveConnectorSchema.ProtocolOptions];
			SizeMode sizeMode = (SizeMode)value;
			bool flag;
			bool flag2;
			if (sizeMode == SizeMode.Disabled)
			{
				flag = false;
				flag2 = false;
			}
			else if (sizeMode == SizeMode.EnabledWithoutValue)
			{
				flag = true;
				flag2 = true;
			}
			else
			{
				flag = true;
				flag2 = false;
			}
			num = (flag ? (num | 1) : (num & -2));
			propertyBag[ReceiveConnectorSchema.ProtocolOptions] = (flag2 ? (num | 2) : (num & -3));
		}

		public static readonly ADPropertyDefinition ProtocolOptions = new ADPropertyDefinition("ProtocolOptions", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveProtocolOptions", ADPropertyDefinitionFlags.PersistDefaultValue, 445, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtocolRestrictions = new ADPropertyDefinition("ProtocolRestrictions", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveProtocolRestrictions", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SecurityFlags = new ADPropertyDefinition("SecurityFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSMTPReceiveInboundSecurityFlag", ADPropertyDefinitionFlags.PersistDefaultValue, 29, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Banner = new ADPropertyDefinition("Banner", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSmtpReceiveBanner", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BinaryMimeEnabled = new ADPropertyDefinition("BinaryMimeEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 4), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 4), null, null);

		public static readonly ADPropertyDefinition Bindings = new ADPropertyDefinition("Bindings", ExchangeObjectVersion.Exchange2007, typeof(IPBinding), "msExchSmtpReceiveBindings", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ChunkingEnabled = new ADPropertyDefinition("ChunkingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 8), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 8), null, null);

		public static readonly ADPropertyDefinition DefaultDomain = new ADPropertyDefinition("DefaultDomain", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchSMTPReceiveDefaultAcceptedDomainLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeliveryStatusNotificationEnabled = new ADPropertyDefinition("DeliveryStatusNotificationEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 16), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 16), null, null);

		public static readonly ADPropertyDefinition EightBitMimeEnabled = new ADPropertyDefinition("EightBitMimeEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 256), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 256), null, null);

		public static readonly ADPropertyDefinition SmtpUtf8Enabled = new ADPropertyDefinition("SmtpUtf8Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 65536), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 65536), null, null);

		public static readonly ADPropertyDefinition BareLinefeedRejectionEnabled = new ADPropertyDefinition("BareLinefeedRejectionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 16384), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 16384), null, null);

		public static readonly ADPropertyDefinition DomainSecureEnabled = new ADPropertyDefinition("DomainSecureEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 512), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 512), null, null);

		public static readonly ADPropertyDefinition EnhancedStatusCodesEnabled = new ADPropertyDefinition("EnhancedStatusCodesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 32), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 32), null, null);

		public static readonly ADPropertyDefinition LongAddressesEnabled = new ADPropertyDefinition("LongAddressesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 1024), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 1024), null, null);

		public static readonly ADPropertyDefinition OrarEnabled = new ADPropertyDefinition("OrarEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 2048), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 2048), null, null);

		public static readonly ADPropertyDefinition SuppressXAnonymousTls = new ADPropertyDefinition("SuppressXAnonymousTls", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 4096), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 4096), null, null);

		public static readonly ADPropertyDefinition AdvertiseClientSettings = new ADPropertyDefinition("AdvertiseClientSettings", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(ReceiveConnectorSchema.ProtocolOptions, 8192UL)), ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 8192), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 8192), null, null);

		public static readonly ADPropertyDefinition ProxyEnabled = new ADPropertyDefinition("ProxyEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 32768), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 32768), null, null);

		public static readonly ADPropertyDefinition Fqdn = new ADPropertyDefinition("Fqdn", ExchangeObjectVersion.Exchange2007, typeof(Fqdn), "msExchSMTPReceiveConnectorFQDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServiceDiscoveryFqdn = new ADPropertyDefinition("ServiceDiscoveryFqdn", ExchangeObjectVersion.Exchange2007, typeof(Fqdn), "msExchSmtpReceiveAdvertisedDomain", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsCertificateName = new ADPropertyDefinition("TlsCertificateName", ExchangeObjectVersion.Exchange2007, typeof(SmtpX509Identifier), "msExchSmtpTLSCertificate", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Comment = new ADPropertyDefinition("Comment", ExchangeObjectVersion.Exchange2007, typeof(string), "AdminDescription", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 1024)
		}, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchSmtpReceiveEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectionTimeout = new ADPropertyDefinition("ConnectionTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSmtpReceiveConnectionTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(10.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectionInactivityTimeout = new ADPropertyDefinition("ConnectionInactivityTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSmtpReceiveConnectionInactivityTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageRateLimit = new ADPropertyDefinition("MessageRateLimit", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSMTPReceiveRelayControl", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageRateSource = new ADPropertyDefinition("MessageRateSource", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSMTPReceiveMessageRateSource", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 3)
		}, null, null);

		public static readonly ADPropertyDefinition MaxInboundConnection = new ADPropertyDefinition("MaxInboundConnection", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpReceiveMaxInboundConnections", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxInboundConnectionPerSource = new ADPropertyDefinition("MaxInboundConnectionPerSource", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpReceiveMaxInboundConnectionsPerSource", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, 10000)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxInboundConnectionPercentagePerSource = new ADPropertyDefinition("MaxInboundConnectionPercentagePerSource", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSMTPReceiveMaxInboundConnectionsPercPerSource", ADPropertyDefinitionFlags.PersistDefaultValue, 2, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 100)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxHeaderSize = new ADPropertyDefinition("MaxHeaderSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSmtpReceiveMaxHeaderSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromBytes(131072UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(32768UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxHopCount = new ADPropertyDefinition("MaxHopCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxHopCount", ADPropertyDefinitionFlags.PersistDefaultValue, 60, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 500)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxLocalHopCount = new ADPropertyDefinition("MaxLocalHopCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxLocalHopCount", ADPropertyDefinitionFlags.PersistDefaultValue, 12, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 50)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxLogonFailures = new ADPropertyDefinition("MaxLogonFailures", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxLogonFailures", ADPropertyDefinitionFlags.PersistDefaultValue, 3, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 10)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxMessageSize = new ADPropertyDefinition("MaxMessageSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSmtpReceiveMaxMessageSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromBytes(36700160UL), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(65536UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		}, null, null);

		public static readonly ADPropertyDefinition PipeliningEnabled = new ADPropertyDefinition("PipeliningEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 128), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolOptions, 128), null, null);

		public static readonly ADPropertyDefinition MaxProtocolErrors = new ADPropertyDefinition("MaxProtocolErrors", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpReceiveMaxProtocolErrors", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxRecipientsPerMessage = new ADPropertyDefinition("MaxRecipientsPerMessage", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxRecipientsPerMessage", ADPropertyDefinitionFlags.PersistDefaultValue, 200, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 512000)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PermissionGroups = new ADPropertyDefinition("PermissionGroups", ExchangeObjectVersion.Exchange2007, typeof(PermissionGroups), null, ADPropertyDefinitionFlags.TaskPopulated, Microsoft.Exchange.Data.PermissionGroups.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProtocolLoggingLevel = new ADPropertyDefinition("ProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchSmtpReceiveProtocolLoggingLevel", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteIPRanges = new ADPropertyDefinition("RemoteIPRanges", ExchangeObjectVersion.Exchange2007, typeof(IPRange), "msExchSmtpReceiveRemoteIPRanges", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RequireEHLODomain = new ADPropertyDefinition("RequireEHLODomain", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolRestrictions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 4), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 4), null, null);

		public static readonly ADPropertyDefinition RequireTLS = new ADPropertyDefinition("RequireTLS", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolRestrictions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 8), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 8), null, null);

		public static readonly ADPropertyDefinition EnableAuthGSSAPI = new ADPropertyDefinition("EnableAuthGSSAPI", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolRestrictions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 16), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 16), null, null);

		public static readonly ADPropertyDefinition ExtendedProtectionPolicy = new ADPropertyDefinition("ExtendedProtectionPolicy", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSMTPExtendedProtectionPolicy", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LiveCredentialEnabled = new ADPropertyDefinition("LiveCredentialEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolRestrictions
		}, null, ADObject.FlagGetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 32768), ADObject.FlagSetterDelegate(ReceiveConnectorSchema.ProtocolRestrictions, 32768), null, null);

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ReceiveConnector.ServerGetter), null, null, null);

		public static readonly ADPropertyDefinition SizeEnabled = new ADPropertyDefinition("SizeEnabled", ExchangeObjectVersion.Exchange2007, typeof(SizeMode), null, ADPropertyDefinitionFlags.Calculated, SizeMode.Enabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ReceiveConnectorSchema.ProtocolOptions
		}, null, new GetterDelegate(ReceiveConnectorSchema.SizeEnabledGetter), new SetterDelegate(ReceiveConnectorSchema.SizeEnabledSetter), null, null);

		public static readonly ADPropertyDefinition TarpitInterval = new ADPropertyDefinition("TarpitInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSmtpReceiveTarpitInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromMinutes(10.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxAcknowledgementDelay = new ADPropertyDefinition("MaxAcknowledgementDelay", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSmtpReceiveMaxAcknowledgementDelay", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromMinutes(10.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsDomainCapabilities = new ADPropertyDefinition("TlsDomainCapabilities", ExchangeObjectVersion.Exchange2007, typeof(SmtpReceiveDomainCapabilities), "msExchSmtpReceiveTlsDomainCapabilities", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportRole = new ADPropertyDefinition("TransportRole", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveRole", ADPropertyDefinitionFlags.PersistDefaultValue, 32, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		private enum ProtocolRestriction
		{
			RequireEHLODomain = 4,
			RequireTLS = 8,
			EnableAuthGSSAPI = 16,
			ExtendedProtectionTlsTerminatedAtProxy = 32,
			LiveCredentialEnabled = 32768
		}

		[Flags]
		internal enum ProtocolOption
		{
			SizeEnabled = 1,
			SizeLimitHidden = 2,
			BinaryMimeEnabled = 4,
			ChunkingEnabled = 8,
			DeliveryStatusNotificationEnabled = 16,
			EnhancedStatusCodesEnabled = 32,
			PipeliningEnabled = 128,
			EightBitMimeEnabled = 256,
			DomainSecureEnabled = 512,
			LongAddressesEnabled = 1024,
			OrarEnabled = 2048,
			SuppressXAnonymousTls = 4096,
			AdvertiseClientSettings = 8192,
			BareLinefeedRejectionEnabled = 16384,
			ProxyEnabled = 32768,
			SmtpUtf8Enabled = 65536
		}
	}
}
