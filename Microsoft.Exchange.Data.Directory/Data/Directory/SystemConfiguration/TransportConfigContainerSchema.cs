using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TransportConfigContainerSchema : ADAMTransportConfigContainerSchema
	{
		internal static object DSNConversionModeGetter(IPropertyBag propertyBag)
		{
			if ((bool)propertyBag[ADAMTransportConfigContainerSchema.ConvertReportToMessage])
			{
				return DSNConversionOption.DoNotConvert;
			}
			if ((bool)propertyBag[ADAMTransportConfigContainerSchema.PreserveReportBodypart])
			{
				return DSNConversionOption.PreserveDSNBody;
			}
			return DSNConversionOption.UseExchangeDSNs;
		}

		internal static void DSNConversionModeSetter(object value, IPropertyBag propertyBag)
		{
			switch ((DSNConversionOption)value)
			{
			case DSNConversionOption.UseExchangeDSNs:
				propertyBag[ADAMTransportConfigContainerSchema.PreserveReportBodypart] = false;
				propertyBag[ADAMTransportConfigContainerSchema.ConvertReportToMessage] = false;
				return;
			case DSNConversionOption.PreserveDSNBody:
				propertyBag[ADAMTransportConfigContainerSchema.PreserveReportBodypart] = true;
				propertyBag[ADAMTransportConfigContainerSchema.ConvertReportToMessage] = false;
				return;
			case DSNConversionOption.DoNotConvert:
				propertyBag[ADAMTransportConfigContainerSchema.PreserveReportBodypart] = false;
				propertyBag[ADAMTransportConfigContainerSchema.ConvertReportToMessage] = true;
				return;
			default:
				throw new ArgumentException("DSNConversionMode", "DSNConversionMode");
			}
		}

		internal static object TransportRuleCollectionAddedRecipientsLimitGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetIntValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (int)TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimit.DefaultValue);
		}

		internal static void TransportRuleCollectionAddedRecipientsLimitSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<int>((int)value, TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleLimitGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetIntValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (int)TransportConfigContainerSchema.TransportRuleLimit.DefaultValue);
		}

		internal static void TransportRuleLimitSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<int>((int)value, TransportConfigContainerSchema.TransportRuleLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleCollectionRegexCharsLimitGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetByteQuantifiedValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (ByteQuantifiedSize)TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimit.DefaultValue);
		}

		internal static void TransportRuleCollectionRegexCharsLimitSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<ByteQuantifiedSize>((ByteQuantifiedSize)value, TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleSizeLimitGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetByteQuantifiedValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleSizeLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (ByteQuantifiedSize)TransportConfigContainerSchema.TransportRuleSizeLimit.DefaultValue);
		}

		internal static void TransportRuleSizeLimitSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<ByteQuantifiedSize>((ByteQuantifiedSize)value, TransportConfigContainerSchema.TransportRuleSizeLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleAttachmentTextScanLimitGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetByteQuantifiedValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (ByteQuantifiedSize)TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimit.DefaultValue);
		}

		internal static void TransportRuleAttachmentTextScanLimitSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<ByteQuantifiedSize>((ByteQuantifiedSize)value, TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimit.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleRegexValidationTimeoutGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetTimespanValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleRegexValidationTimeout.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (EnhancedTimeSpan)TransportConfigContainerSchema.TransportRuleRegexValidationTimeout.DefaultValue);
		}

		internal static void TransportRuleRegexValidationTimeoutSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<EnhancedTimeSpan>((EnhancedTimeSpan)value, TransportConfigContainerSchema.TransportRuleRegexValidationTimeout.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static object TransportRuleMinProductVersionGetter(IPropertyBag propertyBag)
		{
			return MultivaluedPropertyAccessors.GetVersionValueFromMultivaluedProperty(TransportConfigContainerSchema.TransportRuleMinProductVersion.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig], (Version)TransportConfigContainerSchema.TransportRuleMinProductVersion.DefaultValue);
		}

		internal static void TransportRuleMinProductVersionSetter(object value, IPropertyBag propertyBag)
		{
			MultivaluedPropertyAccessors.UpdateMultivaluedProperty<Version>((Version)value, TransportConfigContainerSchema.TransportRuleMinProductVersion.Name, (MultiValuedProperty<string>)propertyBag[TransportConfigContainerSchema.TransportRuleConfig]);
		}

		internal static readonly EnhancedTimeSpan DefaultShadowHeartbeatFrequency = EnhancedTimeSpan.FromMinutes(2.0);

		internal static readonly EnhancedTimeSpan DefaultShadowResubmitTimeSpan = EnhancedTimeSpan.FromHours(3.0);

		public static readonly ADPropertyDefinition MaxDumpsterSizePerDatabase = new ADPropertyDefinition("MaxDumpsterSizePerDatabase", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), ByteQuantifiedSize.KilobyteQuantifierProvider, "msExchMaxDumpsterSizePerStorageGroup", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(18UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxDumpsterTime = new ADPropertyDefinition("MaxDumpsterTime", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchMaxDumpsterTime", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(7.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxReceiveSize = new ADPropertyDefinition("MaxReceiveSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "delivContLength", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2097151UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxRecipientEnvelopeLimit = new ADPropertyDefinition("MaxRecipientEnvelopeLimit", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchRecipLimit", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OrganizationFederatedMailbox = new ADPropertyDefinition("OrganizationFederatedMailbox", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress), "msExchOrgFederatedMailbox", ADPropertyDefinitionFlags.None, SmtpAddress.NullReversePath, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SupervisionTags = new ADPropertyDefinition("SupervisionTags", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchRelationTags", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new CharacterConstraint(SupervisionListEntryConstraint.SupervisionTagInvalidChars, false),
			new StringLengthConstraint(1, 20)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ShadowHeartbeatFrequency = new ADPropertyDefinition("ShadowHeartbeatFrequency", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchProvisioningFlags", ADPropertyDefinitionFlags.PersistDefaultValue, TransportConfigContainerSchema.DefaultShadowHeartbeatFrequency, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ShadowResubmitTimeSpan = new ADPropertyDefinition("ShadowResubmitTimeSpan", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchELCMailboxFlags", ADPropertyDefinitionFlags.PersistDefaultValue, TransportConfigContainerSchema.DefaultShadowResubmitTimeSpan, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportRuleConfig = new ADPropertyDefinition("TransportRuleConfig", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportRuleConfig", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AnonymousSenderToRecipientRatePerHour = new ADPropertyDefinition("AnonymousSenderToRecipientRatePerHour", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchOWASettings", ADPropertyDefinitionFlags.PersistDefaultValue, 1800, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<TransportSettingsConfigXml>(TransportConfigContainerSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition QueueDiagnosticsAggregationInterval = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, long>("QueueDiagnosticsAggregationInterval", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultQueueAggregationIntervalTicks, (TransportSettingsConfigXml configXml) => configXml.QueueAggregationIntervalTicks, delegate(TransportSettingsConfigXml configXml, long value)
		{
			configXml.QueueAggregationIntervalTicks = value;
		}, null, null);

		public static readonly ADPropertyDefinition DiagnosticsAggregationServicePort = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, int>("DiagnosticsAggregationServicePort", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultDiagnosticsAggregationServicePort, (TransportSettingsConfigXml configXml) => configXml.DiagnosticsAggregationServicePort, delegate(TransportSettingsConfigXml configXml, int value)
		{
			configXml.DiagnosticsAggregationServicePort = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentGeneratedMessageLoopDetectionInSubmissionEnabled = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, bool>("AgentGeneratedMessageLoopDetectionInSubmissionEnabled", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultAgentGeneratedMessageLoopDetectionInSubmissionEnabled, (TransportSettingsConfigXml configXml) => configXml.AgentGeneratedMessageLoopDetectionInSubmissionEnabled, delegate(TransportSettingsConfigXml configXml, bool value)
		{
			configXml.AgentGeneratedMessageLoopDetectionInSubmissionEnabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentGeneratedMessageLoopDetectionInSmtpEnabled = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, bool>("AgentGeneratedMessageLoopDetectionInSmtpEnabled", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultAgentGeneratedMessageLoopDetectionInSmtpEnabled, (TransportSettingsConfigXml configXml) => configXml.AgentGeneratedMessageLoopDetectionInSmtpEnabled, delegate(TransportSettingsConfigXml configXml, bool value)
		{
			configXml.AgentGeneratedMessageLoopDetectionInSmtpEnabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition MaxAllowedAgentGeneratedMessageDepth = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, uint>("MaxAllowedAgentGeneratedMessageDepth", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultMaxAllowedAgentGeneratedMessageDepth, (TransportSettingsConfigXml configXml) => configXml.MaxAllowedAgentGeneratedMessageDepth, delegate(TransportSettingsConfigXml configXml, uint value)
		{
			configXml.MaxAllowedAgentGeneratedMessageDepth = value;
		}, null, null);

		public static readonly ADPropertyDefinition MaxAllowedAgentGeneratedMessageDepthPerAgent = XMLSerializableBase.ConfigXmlProperty<TransportSettingsConfigXml, uint>("MaxAllowedAgentGeneratedMessageDepthPerAgent", ExchangeObjectVersion.Exchange2007, TransportConfigContainerSchema.ConfigurationXML, TransportSettingsConfigXml.DefaultMaxAllowedAgentGeneratedMessageDepthPerAgent, (TransportSettingsConfigXml configXml) => configXml.MaxAllowedAgentGeneratedMessageDepthPerAgent, delegate(TransportSettingsConfigXml configXml, uint value)
		{
			configXml.MaxAllowedAgentGeneratedMessageDepthPerAgent = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportRuleCollectionAddedRecipientsLimit = new ADPropertyDefinition("TransportRuleCollectionAddedRecipientsLimit", ExchangeObjectVersion.Exchange2007, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 100, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimitGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleCollectionAddedRecipientsLimitSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleLimit = new ADPropertyDefinition("TransportRuleLimit", ExchangeObjectVersion.Exchange2007, typeof(int), null, ADPropertyDefinitionFlags.Calculated, 300, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleLimitGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleLimitSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleCollectionRegexCharsLimit = new ADPropertyDefinition("TransportRuleCollectionRegexCharsLimit", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), null, ADPropertyDefinitionFlags.Calculated, ByteQuantifiedSize.FromKB(20UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimitGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleCollectionRegexCharsLimitSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleSizeLimit = new ADPropertyDefinition("TransportRuleSizeLimit", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), null, ADPropertyDefinitionFlags.Calculated, ByteQuantifiedSize.FromKB(4UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleSizeLimitGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleSizeLimitSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleAttachmentTextScanLimit = new ADPropertyDefinition("TransportRuleAttachmentTextScanLimit", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), null, ADPropertyDefinitionFlags.Calculated, ByteQuantifiedSize.FromKB(150UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimitGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleAttachmentTextScanLimitSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleRegexValidationTimeout = new ADPropertyDefinition("TransportRuleRegexValidationTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), null, ADPropertyDefinitionFlags.Calculated, EnhancedTimeSpan.FromMilliseconds(300.0), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleRegexValidationTimeoutGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleRegexValidationTimeoutSetter), null, null);

		public static readonly ADPropertyDefinition TransportRuleMinProductVersion = new ADPropertyDefinition("TransportRuleMinProductVersion", ExchangeObjectVersion.Exchange2007, typeof(Version), null, ADPropertyDefinitionFlags.Calculated, new Version("14.00.0000.000"), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADAMTransportConfigContainerSchema.Flags
		}, null, new GetterDelegate(TransportConfigContainerSchema.TransportRuleMinProductVersionGetter), new SetterDelegate(TransportConfigContainerSchema.TransportRuleMinProductVersionSetter), null, null);
	}
}
