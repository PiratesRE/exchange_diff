using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class FrontendTransportServerADSchema : ADLegacyVersionableObjectSchema
	{
		private static object AdminDisplayVersionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[FrontendTransportServerADSchema.SerialNumber];
			if (string.IsNullOrEmpty(text))
			{
				InvalidOperationException ex = new InvalidOperationException(DirectoryStrings.SerialNumberMissing);
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex.Message), FrontendTransportServerADSchema.AdminDisplayVersion, string.Empty), ex);
			}
			object result;
			try
			{
				result = ServerVersion.ParseFromSerialNumber(text);
			}
			catch (FormatException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdminDisplayVersion", ex2.Message), FrontendTransportServerADSchema.AdminDisplayVersion, propertyBag[FrontendTransportServerADSchema.SerialNumber]), ex2);
			}
			return result;
		}

		private static void AdminDisplayVersionSetter(object value, IPropertyBag propertyBag)
		{
			ServerVersion serverVersion = (ServerVersion)value;
			propertyBag[FrontendTransportServerADSchema.SerialNumber] = serverVersion.ToString(true);
		}

		private static object EditionGetter(IPropertyBag propertyBag)
		{
			string serverTypeInAD = (string)propertyBag[FrontendTransportServerADSchema.ServerType];
			return ServerEdition.DecryptServerEdition(serverTypeInAD);
		}

		private static void EditionSetter(object value, IPropertyBag propertyBag)
		{
			ServerEditionType edition = (ServerEditionType)value;
			propertyBag[FrontendTransportServerADSchema.ServerType] = ServerEdition.EncryptServerEdition(edition);
		}

		private static object ExternalDNSServersGetter(IPropertyBag propertyBag)
		{
			List<IPAddress> list = Server.ParseStringForAddresses((string)propertyBag[FrontendTransportServerADSchema.ExternalDNSServersStr]);
			if (list.Count > 0)
			{
				return new MultiValuedProperty<IPAddress>(false, null, list);
			}
			return new MultiValuedProperty<IPAddress>();
		}

		private static void ExternalDNSServersSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[FrontendTransportServerADSchema.ExternalDNSServersStr] = Server.FormatAddressesToString((MultiValuedProperty<IPAddress>)value);
		}

		private static object IsFrontendTransportServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole];
			return (serverRole & ServerRole.FrontendTransport) == ServerRole.FrontendTransport;
		}

		private static void IsFrontendTransportServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[FrontendTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
				propertyBag[FrontendTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole] | ServerRole.FrontendTransport);
				return;
			}
			propertyBag[FrontendTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole] & ~ServerRole.FrontendTransport);
		}

		private static object IsProvisionedServerGetter(IPropertyBag propertyBag)
		{
			ServerRole serverRole = (ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole];
			return (serverRole & ServerRole.ProvisionedServer) == ServerRole.ProvisionedServer;
		}

		private static void IsProvisionedServerSetter(object value, IPropertyBag propertyBag)
		{
			if ((bool)value)
			{
				propertyBag[FrontendTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole] | ServerRole.ProvisionedServer);
				return;
			}
			propertyBag[FrontendTransportServerADSchema.CurrentServerRole] = ((ServerRole)propertyBag[FrontendTransportServerADSchema.CurrentServerRole] & ~ServerRole.ProvisionedServer);
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

		public static readonly ADPropertyDefinition ExternalDNSAdapterGuid = new ADPropertyDefinition("ExternalDNSAdapterGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchTransportExternalDNSAdapterGuid", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDNSServersStr = new ADPropertyDefinition("ExternalDNSServersStr", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSmtpExternalDNSServers", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDNSProtocolOption = new ADPropertyDefinition("ExternalDNSProtocolOption", ExchangeObjectVersion.Exchange2007, typeof(ProtocolOption), "msExchTransportExternalDNSProtocolOption", ADPropertyDefinitionFlags.None, ProtocolOption.Any, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalIPAddress = new ADPropertyDefinition("ExternalIPAddress", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchTransportExternalIPAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDNSAdapterGuid = new ADPropertyDefinition("InternalDNSAdapter", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchTransportInternalDNSAdapterGuid", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDNSProtocolOption = new ADPropertyDefinition("InternalDNSProtocolOption", ExchangeObjectVersion.Exchange2007, typeof(ProtocolOption), "msExchTransportInternalDNSProtocolOption", ADPropertyDefinitionFlags.None, ProtocolOption.Any, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDNSServers = new ADPropertyDefinition("InternalDNSServers", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchTransportInternalDNSServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IntraOrgConnectorProtocolLoggingLevel = new ADPropertyDefinition("TransportIntraOrgConnectorProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchTransportOutboundProtocolLoggingLevel", ADPropertyDefinitionFlags.None, ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FrontendTransportServerFlags = new ADPropertyDefinition("TransportServerFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition IntraOrgConnectorSmtpMaxMessagesPerConnection = new ADPropertyDefinition("IntraOrgConnectorSmtpMaxMessagesPerConnection", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpMaxMessagesPerConnection", ADPropertyDefinitionFlags.PersistDefaultValue, 20, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxConnectionRatePerMinute = new ADPropertyDefinition("MaxConnectionRatePerMinute", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxConnectionRatePerMinute", ADPropertyDefinitionFlags.PersistDefaultValue, 1200, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxOutboundConnections = new ADPropertyDefinition("MaxOutboundConnections", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpMaxOutgoingConnections", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxPerDomainOutboundConnections = new ADPropertyDefinition("MaxPerDomainOutboundConnections", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpMaxOutgoingConnectionsPerDomain", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NetworkAddress = new ADPropertyDefinition("NetworkAddress", ExchangeObjectVersion.Exchange2003, typeof(NetworkAddress), "networkAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

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

		public static readonly ADPropertyDefinition TransientFailureRetryCount = new ADPropertyDefinition("TransientFailureRetryCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportTransientFailureRetryCount", ADPropertyDefinitionFlags.PersistDefaultValue, 6, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 15)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransientFailureRetryInterval = new ADPropertyDefinition("TransientFailureRetryInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportTransientFailureRetryInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.FromHours(12.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VersionNumber = new ADPropertyDefinition("VersionNumber", ExchangeObjectVersion.Exchange2003, typeof(int), "versionNumber", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.SerialNumber
		}, null, new GetterDelegate(FrontendTransportServerADSchema.AdminDisplayVersionGetter), new SetterDelegate(FrontendTransportServerADSchema.AdminDisplayVersionSetter), null, null);

		public static readonly ADPropertyDefinition AntispamAgentsEnabled = new ADPropertyDefinition("AntispamAgentsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.FrontendTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 1), ADObject.FlagSetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 1), null, null);

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = new ADPropertyDefinition("ConnectivityLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.FrontendTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 2), ADObject.FlagSetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 2), null, null);

		public static readonly ADPropertyDefinition ExternalDNSAdapterDisabled = new ADPropertyDefinition("ExternalDNSAdapterDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.FrontendTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 4), ADObject.FlagSetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 4), null, null);

		public static readonly ADPropertyDefinition Edition = new ADPropertyDefinition("Edition", ExchangeObjectVersion.Exchange2003, typeof(ServerEditionType), null, ADPropertyDefinitionFlags.Calculated, ServerEditionType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.ServerType
		}, null, new GetterDelegate(FrontendTransportServerADSchema.EditionGetter), new SetterDelegate(FrontendTransportServerADSchema.EditionSetter), null, null);

		public static readonly ADPropertyDefinition ExternalDNSServers = new ADPropertyDefinition("ExternalDNSServers", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.ExternalDNSServersStr
		}, null, new GetterDelegate(FrontendTransportServerADSchema.ExternalDNSServersGetter), new SetterDelegate(FrontendTransportServerADSchema.ExternalDNSServersSetter), null, null);

		public static readonly ADPropertyDefinition InternalDNSAdapterDisabled = new ADPropertyDefinition("InternalDNSAdapterDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.FrontendTransportServerFlags
		}, null, ADObject.FlagGetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 8), ADObject.FlagSetterDelegate(FrontendTransportServerADSchema.FrontendTransportServerFlags, 8), null, null);

		public static readonly ADPropertyDefinition IsFrontendTransportServer = new ADPropertyDefinition("IsFrontendTransportServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.CurrentServerRole
		}, null, new GetterDelegate(FrontendTransportServerADSchema.IsFrontendTransportServerGetter), new SetterDelegate(FrontendTransportServerADSchema.IsFrontendTransportServerSetter), null, null);

		public static readonly ADPropertyDefinition IsProvisionedServer = new ADPropertyDefinition("IsProvisionedServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			FrontendTransportServerADSchema.CurrentServerRole
		}, null, new GetterDelegate(FrontendTransportServerADSchema.IsProvisionedServerGetter), new SetterDelegate(FrontendTransportServerADSchema.IsProvisionedServerSetter), null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<FrontendTransportServerConfigXML>(FrontendTransportServerADSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition AgentLogEnabled = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, bool>("AgentLogEnabled", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, true, (FrontendTransportServerConfigXML configXml) => configXml.AgentLog.Enabled, delegate(FrontendTransportServerConfigXML configXml, bool value)
		{
			configXml.AgentLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxAge = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, EnhancedTimeSpan>("AgentLogMaxAge", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (FrontendTransportServerConfigXML configXml) => configXml.AgentLog.MaxAge, delegate(FrontendTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.AgentLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("AgentLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (FrontendTransportServerConfigXML configXml) => configXml.AgentLog.MaxDirectorySize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AgentLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("AgentLogMaxFileSize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (FrontendTransportServerConfigXML configXml) => configXml.AgentLog.MaxFileSize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AgentLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogPath = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, LocalLongFullPath>("AgentLogPath", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, null, (FrontendTransportServerConfigXML configXml) => configXml.AgentLog.Path, delegate(FrontendTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.AgentLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogEnabled = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, bool>("DnsLogEnabled", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, false, (FrontendTransportServerConfigXML configXml) => configXml.DnsLog.Enabled, delegate(FrontendTransportServerConfigXML configXml, bool value)
		{
			configXml.DnsLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxAge = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, EnhancedTimeSpan>("DnsLogMaxAge", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, EnhancedTimeSpan.FromDays(7.0), (FrontendTransportServerConfigXML configXml) => configXml.DnsLog.MaxAge, delegate(FrontendTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.DnsLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("DnsLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(100UL), (FrontendTransportServerConfigXML configXml) => configXml.DnsLog.MaxDirectorySize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.DnsLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("DnsLogMaxFileSize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(10UL), (FrontendTransportServerConfigXML configXml) => configXml.DnsLog.MaxFileSize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.DnsLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogPath = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, LocalLongFullPath>("DnsLogPath", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, null, (FrontendTransportServerConfigXML configXml) => configXml.DnsLog.Path, delegate(FrontendTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.DnsLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogEnabled = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, bool>("ResourceLogEnabled", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, true, (FrontendTransportServerConfigXML configXml) => configXml.ResourceLog.Enabled, delegate(FrontendTransportServerConfigXML configXml, bool value)
		{
			configXml.ResourceLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxAge = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, EnhancedTimeSpan>("ResourceLogMaxAge", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, EnhancedTimeSpan.FromDays(7.0), (FrontendTransportServerConfigXML configXml) => configXml.ResourceLog.MaxAge, delegate(FrontendTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.ResourceLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("ResourceLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(100UL), (FrontendTransportServerConfigXML configXml) => configXml.ResourceLog.MaxDirectorySize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ResourceLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("ResourceLogMaxFileSize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(10UL), (FrontendTransportServerConfigXML configXml) => configXml.ResourceLog.MaxFileSize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ResourceLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogPath = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, LocalLongFullPath>("ResourceLogPath", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, null, (FrontendTransportServerConfigXML configXml) => configXml.ResourceLog.Path, delegate(FrontendTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.ResourceLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition AttributionLogEnabled = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, bool>("AttributionLogEnabled", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, false, (FrontendTransportServerConfigXML configXml) => configXml.AttributionLog.Enabled, delegate(FrontendTransportServerConfigXML configXml, bool value)
		{
			configXml.AttributionLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition AttributionLogMaxAge = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, EnhancedTimeSpan>("AttributionLogMaxAge", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (FrontendTransportServerConfigXML configXml) => configXml.AttributionLog.MaxAge, delegate(FrontendTransportServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.AttributionLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition AttributionLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("AttributionLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (FrontendTransportServerConfigXML configXml) => configXml.AttributionLog.MaxDirectorySize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AttributionLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AttributionLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, Unlimited<ByteQuantifiedSize>>("AttributionLogMaxFileSize", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (FrontendTransportServerConfigXML configXml) => configXml.AttributionLog.MaxFileSize, delegate(FrontendTransportServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AttributionLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AttributionLogPath = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, LocalLongFullPath>("AttributionLogPath", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, null, (FrontendTransportServerConfigXML configXml) => configXml.AttributionLog.Path, delegate(FrontendTransportServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.AttributionLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition MaxReceiveTlsRatePerMinute = XMLSerializableBase.ConfigXmlProperty<FrontendTransportServerConfigXML, int>("MaxReceiveTlsRatePerMinute", ExchangeObjectVersion.Exchange2007, FrontendTransportServerADSchema.ConfigurationXML, 6000, (FrontendTransportServerConfigXML configXml) => configXml.MaxReceiveTlsRatePerMinute, delegate(FrontendTransportServerConfigXML configXml, int value)
		{
			configXml.MaxReceiveTlsRatePerMinute = value;
		}, null, null);
	}
}
