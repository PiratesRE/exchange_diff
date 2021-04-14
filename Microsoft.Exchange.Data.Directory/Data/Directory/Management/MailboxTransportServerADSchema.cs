using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxTransportServerADSchema : ADLegacyVersionableObjectSchema
	{
		private static object AdminDisplayVersionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[MailboxTransportServerADSchema.SerialNumber];
			if (string.IsNullOrEmpty(text))
			{
				InvalidOperationException ex = new InvalidOperationException(DirectoryStrings.SerialNumberMissing);
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex.Message), MailboxTransportServerADSchema.AdminDisplayVersion, string.Empty), ex);
			}
			object result;
			try
			{
				result = ServerVersion.ParseFromSerialNumber(text);
			}
			catch (FormatException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex2.Message), MailboxTransportServerADSchema.AdminDisplayVersion, propertyBag[MailboxTransportServerADSchema.SerialNumber]), ex2);
			}
			return result;
		}

		private static void AdminDisplayVersionSetter(object value, IPropertyBag propertyBag)
		{
			ServerVersion serverVersion = (ServerVersion)value;
			propertyBag[MailboxTransportServerADSchema.SerialNumber] = serverVersion.ToString(true);
		}

		private static object EditionGetter(IPropertyBag propertyBag)
		{
			string serverTypeInAD = (string)propertyBag[MailboxTransportServerADSchema.ServerType];
			return ServerEdition.DecryptServerEdition(serverTypeInAD);
		}

		private static void EditionSetter(object value, IPropertyBag propertyBag)
		{
			ServerEditionType edition = (ServerEditionType)value;
			propertyBag[MailboxTransportServerADSchema.ServerType] = ServerEdition.EncryptServerEdition(edition);
		}

		internal static object IsMailboxServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole];
			return (serverRole & ServerRole.Mailbox) == ServerRole.Mailbox;
		}

		internal static void IsMailboxServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[MailboxTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[MailboxTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole] | ServerRole.Mailbox);
				return;
			}
			propertyBag[MailboxTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole] & ~ServerRole.Mailbox);
		}

		internal static object IsProvisionedServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole];
			return (serverRole & ServerRole.ProvisionedServer) == ServerRole.ProvisionedServer;
		}

		internal static void IsProvisionedServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[MailboxTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole] | ServerRole.ProvisionedServer);
				return;
			}
			propertyBag[MailboxTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[MailboxTransportServerADSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
		}

		public static readonly ADPropertyDefinition ConnectivityLogMaxAge = new ADPropertyDefinition("ConnectivityLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxConnectivityLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectivityLogMaxDirectorySize = new ADPropertyDefinition("ConnectivityLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportConnectivityLogDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectivityLogMaxFileSize = new ADPropertyDefinition("ConnectivityLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportConnectivityLogFileSize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectivityLogPath = new ADPropertyDefinition("ConnectivityLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportConnectivityLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition CurrentServerRole = new ADPropertyDefinition("CurrentServerRole", ExchangeObjectVersion.Exchange2007, typeof(ServerRole), "msExchCurrentServerRoles", ADPropertyDefinitionFlags.PersistDefaultValue, ServerRole.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyDN = new ADPropertyDefinition("ExchangeLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorProtocolLoggingLevel = new ADPropertyDefinition("TransportInMemoryReceiveConnectorProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchTransportInboundProtocolLoggingLevel", ADPropertyDefinitionFlags.None, ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxTransportServerFlags = new ADPropertyDefinition("TransportServerFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 7, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxConcurrentMailboxSubmissions = new ADPropertyDefinition("MaxConcurrentMailboxSubmissions", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxConcurrentMailboxSubmissions", ADPropertyDefinitionFlags.PersistDefaultValue, 20, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 256)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxConcurrentMailboxDeliveries = new ADPropertyDefinition("MaxConcurrentMailboxDeliveries", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxConcurrentMailboxDeliveries", ADPropertyDefinitionFlags.PersistDefaultValue, 20, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 256)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NetworkAddress = new ADPropertyDefinition("NetworkAddress", ExchangeObjectVersion.Exchange2003, typeof(NetworkAddress), "networkAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PipelineTracingPath = new ADPropertyDefinition("PipelineTracingPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportPipelineTracingPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition PipelineTracingSenderAddress = new ADPropertyDefinition("PipelineTracingSenderAddress", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress?), "msExchTransportPipelineTracingSenderAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320)
		}, null, null);

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxAge = new ADPropertyDefinition("ReceiveProtocolLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxReceiveProtocolLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxDirectorySize = new ADPropertyDefinition("ReceiveProtocolLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportMaxReceiveProtocolLogDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReceiveProtocolLogMaxFileSize = new ADPropertyDefinition("ReceiveProtocolLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportMaxReceiveProtocolLogFileSize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReceiveProtocolLogPath = new ADPropertyDefinition("ReceiveProtocolLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportReceiveProtocolLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition SendProtocolLogMaxAge = new ADPropertyDefinition("SendProtocolLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxSendProtocolLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendProtocolLogMaxDirectorySize = new ADPropertyDefinition("SendProtocolLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportMaxSendProtocolLogDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendProtocolLogMaxFileSize = new ADPropertyDefinition("SendProtocolLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportMaxSendProtocolLogFileSize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendProtocolLogPath = new ADPropertyDefinition("SendProtocolLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportSendProtocolLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition SerialNumber = new ADPropertyDefinition("SerialNumber", ExchangeObjectVersion.Exchange2003, typeof(string), "serialNumber", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerType = new ADPropertyDefinition("ServerType", ExchangeObjectVersion.Exchange2003, typeof(string), "type", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public new static readonly ADPropertyDefinition SystemFlags = new ADPropertyDefinition("SystemFlags", ExchangeObjectVersion.Exchange2003, typeof(SystemFlagsEnum), "systemFlags", ADPropertyDefinitionFlags.PersistDefaultValue, SystemFlagsEnum.DeleteImmediately | SystemFlagsEnum.Renamable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VersionNumber = new ADPropertyDefinition("VersionNumber", ExchangeObjectVersion.Exchange2003, typeof(int), "versionNumber", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.SerialNumber
		}, null, new GetterDelegate(MailboxTransportServerADSchema.AdminDisplayVersionGetter), new SetterDelegate(MailboxTransportServerADSchema.AdminDisplayVersionSetter), null, null);

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = new ADPropertyDefinition("ConnectivityLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.MailboxTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 1), ADObject.FlagSetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 1), null, null);

		public static readonly ADPropertyDefinition ContentConversionTracingEnabled = new ADPropertyDefinition("ContentConversionTracingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.MailboxTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 2), ADObject.FlagSetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 2), null, null);

		public static readonly ADPropertyDefinition Edition = new ADPropertyDefinition("Edition", ExchangeObjectVersion.Exchange2003, typeof(ServerEditionType), null, ADPropertyDefinitionFlags.Calculated, ServerEditionType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.ServerType
		}, null, new GetterDelegate(MailboxTransportServerADSchema.EditionGetter), new SetterDelegate(MailboxTransportServerADSchema.EditionSetter), null, null);

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorSmtpUtf8Enabled = new ADPropertyDefinition("TransportInMemoryReceiveConnectorSmtpUtf8Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.MailboxTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 8), ADObject.FlagSetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 8), null, null);

		public static readonly ADPropertyDefinition IsMailboxServer = new ADPropertyDefinition("IsMailboxServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.CurrentServerRole
		}, null, new GetterDelegate(MailboxTransportServerADSchema.IsMailboxServerGetter), new SetterDelegate(MailboxTransportServerADSchema.IsMailboxServerSetter), null, null);

		public static readonly ADPropertyDefinition IsProvisionedServer = new ADPropertyDefinition("IsProvisionedServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.CurrentServerRole
		}, null, new GetterDelegate(MailboxTransportServerADSchema.IsProvisionedServerGetter), new SetterDelegate(MailboxTransportServerADSchema.IsProvisionedServerSetter), null, null);

		public static readonly ADPropertyDefinition PipelineTracingEnabled = new ADPropertyDefinition("PipelineTracingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailboxTransportServerADSchema.MailboxTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 4), ADObject.FlagSetterDelegate(MailboxTransportServerADSchema.MailboxTransportServerFlags, 4), null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<MailboxTransportServerConfigXML>(MailboxTransportServerADSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogEnabled = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, bool>("MailboxSubmissionAgentLogEnabled", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, true, (MailboxTransportServerConfigXML configXml) => configXml.MailboxSubmissionAgentLog.Enabled, delegate(MailboxTransportServerConfigXML configXml, bool value)
		{
			configXml.MailboxSubmissionAgentLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxAge = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, EnhancedTimeSpan>("MailboxSubmissionAgentLogMaxAge", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (MailboxTransportServerConfigXML configXml) => configXml.MailboxSubmissionAgentLog.MaxAge, delegate(MailboxTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.MailboxSubmissionAgentLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxSubmissionAgentLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxSubmissionAgentLog.MaxDirectorySize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxSubmissionAgentLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxSubmissionAgentLogMaxFileSize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxSubmissionAgentLog.MaxFileSize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxSubmissionAgentLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxSubmissionAgentLogPath = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, LocalLongFullPath>("MailboxSubmissionAgentLogPath", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, null, (MailboxTransportServerConfigXML configXml) => configXml.MailboxSubmissionAgentLog.Path, delegate(MailboxTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.MailboxSubmissionAgentLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogEnabled = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, bool>("MailboxDeliveryAgentLogEnabled", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, true, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryAgentLog.Enabled, delegate(MailboxTransportServerConfigXML configXml, bool value)
		{
			configXml.MailboxDeliveryAgentLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxAge = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, EnhancedTimeSpan>("MailboxDeliveryAgentLogMaxAge", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryAgentLog.MaxAge, delegate(MailboxTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.MailboxDeliveryAgentLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxDeliveryAgentLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryAgentLog.MaxDirectorySize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxDeliveryAgentLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxDeliveryAgentLogMaxFileSize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryAgentLog.MaxFileSize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxDeliveryAgentLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryAgentLogPath = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, LocalLongFullPath>("MailboxDeliveryAgentLogPath", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, null, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryAgentLog.Path, delegate(MailboxTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.MailboxDeliveryAgentLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogEnabled = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, bool>("MailboxDeliveryThrottlingLogEnabled", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, false, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryThrottlingLog.Enabled, delegate(MailboxTransportServerConfigXML configXml, bool value)
		{
			configXml.MailboxDeliveryThrottlingLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxAge = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, EnhancedTimeSpan>("MailboxDeliveryThrottlingLogMaxAge", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryThrottlingLog.MaxAge, delegate(MailboxTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.MailboxDeliveryThrottlingLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxDeliveryThrottlingLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryThrottlingLog.MaxDirectorySize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxDeliveryThrottlingLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("MailboxDeliveryThrottlingLogMaxFileSize", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryThrottlingLog.MaxFileSize, delegate(MailboxTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.MailboxDeliveryThrottlingLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxDeliveryThrottlingLogPath = XMLSerializableBase.ConfigXmlProperty<MailboxTransportServerConfigXML, LocalLongFullPath>("MailboxDeliveryThrottlingLogPath", ExchangeObjectVersion.Exchange2007, MailboxTransportServerADSchema.ConfigurationXML, null, (MailboxTransportServerConfigXML configXml) => configXml.MailboxDeliveryThrottlingLog.Path, delegate(MailboxTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.MailboxDeliveryThrottlingLog.Path = value;
		}, null, null);
	}
}
