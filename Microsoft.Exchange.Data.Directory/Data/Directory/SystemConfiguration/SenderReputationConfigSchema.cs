using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SenderReputationConfigSchema : MessageHygieneAgentConfigSchema
	{
		public static readonly ADPropertyDefinition MinMessagesPerDatabaseTransaction = new ADPropertyDefinition("MinMessagesPerDatabaseTransaction", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMinMessagesPerDatabaseTransaction", ADPropertyDefinitionFlags.PersistDefaultValue, 20, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SrlBlockThreshold = new ADPropertyDefinition("SrlBlockThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationSrlBlockThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 7, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinMessagesPerTimeSlice = new ADPropertyDefinition("MinMessagesPerTimeSlice", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMinMessagePerTimeSlice", ADPropertyDefinitionFlags.PersistDefaultValue, 100, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TimeSliceInterval = new ADPropertyDefinition("TimeSliceInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationTimeSliceInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 48, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OpenProxyRescanInterval = new ADPropertyDefinition("OpenProxyRescanInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationOpenProxyRescanInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 10, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OpenProxyFlags = new ADPropertyDefinition("OpenProxyFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationOpenProxyFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinReverseDnsQueryPeriod = new ADPropertyDefinition("MinReverseDnsQueryPeriod", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMinReverseDnsQueryPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SenderBlockingPeriod = new ADPropertyDefinition("SenderBlockingPeriod", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationSenderBlockingPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, 24, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 48)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxWorkQueueSize = new ADPropertyDefinition("MaxWorkQueueSize", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMaxWorkQueueSize", ADPropertyDefinitionFlags.PersistDefaultValue, 1000, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxIdleTime = new ADPropertyDefinition("MaxIdleTime", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMaxIdleTime", ADPropertyDefinitionFlags.PersistDefaultValue, 10, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Socks4Ports = new ADPropertyDefinition("Socks4Ports", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationSocks4Ports", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Socks5Ports = new ADPropertyDefinition("Socks5Ports", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationSocks5Ports", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpConnectPorts = new ADPropertyDefinition("HttpConnectPorts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationHttpConnectPorts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpPostPorts = new ADPropertyDefinition("HttpPostPorts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationHttpPostPorts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CiscoPorts = new ADPropertyDefinition("CiscoPorts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationCiscoPorts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TelnetPorts = new ADPropertyDefinition("TelnetPorts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationTelnetPorts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WingatePorts = new ADPropertyDefinition("WingatePorts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationWingatePorts", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TablePurgeInterval = new ADPropertyDefinition("TablePurgeInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationTablePurgeInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 24, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxPendingOperations = new ADPropertyDefinition("MaxPendingOperations", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMaxPendingOperations", ADPropertyDefinitionFlags.PersistDefaultValue, 100, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyServerIP = new ADPropertyDefinition("ProxyServerIP", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSenderReputationProxyServerIP", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyServerType = new ADPropertyDefinition("ProxyServerType", ExchangeObjectVersion.Exchange2007, typeof(ProxyType), "msExchSenderReputationProxyServerType", ADPropertyDefinitionFlags.PersistDefaultValue, ProxyType.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ProxyType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProxyServerPort = new ADPropertyDefinition("ProxyServerPort", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationProxyServerPort", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9999)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MinDownloadInterval = new ADPropertyDefinition("MinDownloadInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMinDownloadInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 10, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxDownloadInterval = new ADPropertyDefinition("MaxDownloadInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSenderReputationMaxDownloadInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 100, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SrlSettingsDatabaseFileName = new ADPropertyDefinition("SrlSettingsDatabaseFileName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSenderReputationSrlSettingsDatabaseFileName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReputationServiceUrl = new ADPropertyDefinition("ReputationServiceUrl", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSenderReputationServiceUrl", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
