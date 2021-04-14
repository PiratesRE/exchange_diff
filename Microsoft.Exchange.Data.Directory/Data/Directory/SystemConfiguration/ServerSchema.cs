using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ServerSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition ExchangeLegacyDN = new ADPropertyDefinition("ExchangeLegacyDN", ExchangeObjectVersion.Exchange2003, typeof(string), "legacyExchangeDN", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalResponsibleMTA = new ADPropertyDefinition("InternalResponsibleMTA", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchResponsibleMTAServer", ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ResponsibleMTA = new ADPropertyDefinition("ResponsibleMTA", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.InternalResponsibleMTA
		}, null, (IPropertyBag propertyBag) => ((ADObjectId)propertyBag[ServerSchema.InternalResponsibleMTA]) ?? ((ADObjectId)propertyBag[ADObjectSchema.Id]).GetChildId("Microsoft MTA"), null, null, null);

		public static readonly ADPropertyDefinition DataPath = new ADPropertyDefinition("DataPath", ExchangeObjectVersion.Exchange2003, typeof(LocalLongFullPath), "msExchDataPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition InstallPath = new ADPropertyDefinition("InstallPath", ExchangeObjectVersion.Exchange2003, typeof(LocalLongFullPath), "msExchInstallPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition Heuristics = new ADPropertyDefinition("Heuristics", ExchangeObjectVersion.Exchange2003, typeof(int), "heuristics", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NetworkAddress = new ADPropertyDefinition("NetworkAddress", ExchangeObjectVersion.Exchange2003, typeof(NetworkAddress), "networkAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IsPhoneticSupportEnabled = new ADPropertyDefinition("IsPhoneticSupportEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchPhoneticSupport", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SerialNumber = new ADPropertyDefinition("SerialNumber", ExchangeObjectVersion.Exchange2003, typeof(string), "serialNumber", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerRole = new ADPropertyDefinition("ServerRole", ExchangeObjectVersion.Exchange2003, typeof(ServerRole), "serverRole", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.SystemConfiguration.ServerRole.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerType = new ADPropertyDefinition("ServerType", ExchangeObjectVersion.Exchange2003, typeof(string), "type", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VersionNumber = new ADPropertyDefinition("VersionNumber", ExchangeObjectVersion.Exchange2003, typeof(int), "versionNumber", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyServerRole = new ADPropertyDefinition("ExchangeLegacyServerRole", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchServerRole", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentServerRole = new ADPropertyDefinition("CurrentServerRole", ExchangeObjectVersion.Exchange2007, typeof(ServerRole), "msExchCurrentServerRoles", ADPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.SystemConfiguration.ServerRole.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HomeRoutingGroup = new ADPropertyDefinition("HomeRoutingGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchHomeRoutingGroup", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CustomerFeedbackEnabled = new ADPropertyDefinition("CustomerFeedbackEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool?), "msExchCustomerFeedbackEnabled", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EdgeSyncCredentials = new ADPropertyDefinition("EdgeSyncCredential", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchEdgeSyncCredential", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EdgeSyncStatus = new ADPropertyDefinition("EdgeSyncStatus", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncStatus", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalTransportCertificate = new ADPropertyDefinition("InternalTransportCertificate", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchServerInternalTLSCert", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalTransportCertificateThumbprint = new ADPropertyDefinition("InternalTransportCertificateThumbprint", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.InternalTransportCertificate
		}, null, new GetterDelegate(Server.InternalTransportCertificateThumbprintGetter), null, null, null);

		public static readonly ADPropertyDefinition EdgeSyncLease = new ADPropertyDefinition("EdgeSyncLease", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchEdgeSyncLease", ADPropertyDefinitionFlags.Binary, null, new PropertyDefinitionConstraint[]
		{
			new ByteArrayLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EdgeSyncCookies = SharedPropertyDefinitions.EdgeSyncCookies;

		public static readonly ADPropertyDefinition EdgeSyncSourceGuid = new ADPropertyDefinition("EdgeSyncSourceGuid", ExchangeObjectVersion.Exchange2007, typeof(byte[]), "msExchEdgeSyncSourceGuid", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ProductID = new ADPropertyDefinition("ProductID", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchProductID", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 100)
		}, null, null);

		public static readonly ADPropertyDefinition ComponentStates = new ADPropertyDefinition("ComponentStates", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchComponentStates", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MonitoringGroup = new ADPropertyDefinition("MonitoringGroup", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchShadowDisplayName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Edition = new ADPropertyDefinition("Edition", ExchangeObjectVersion.Exchange2003, typeof(ServerEditionType), null, ADPropertyDefinitionFlags.Calculated, ServerEditionType.Unknown, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.ServerType
		}, null, new GetterDelegate(Server.EditionGetter), new SetterDelegate(Server.EditionSetter), null, null);

		public static readonly ADPropertyDefinition Fqdn = new ADPropertyDefinition("Fqdn", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.NetworkAddress
		}, new CustomFilterBuilderDelegate(Server.FqdnFilterBuilder), new GetterDelegate(Server.FqdnGetter), null, null, null);

		public static readonly ADPropertyDefinition Domain = new ADPropertyDefinition("Domain", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.NetworkAddress
		}, null, new GetterDelegate(Server.DomainGetter), null, null, null);

		public static readonly ADPropertyDefinition OrganizationalUnit = new ADPropertyDefinition("OU", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.NetworkAddress,
			ADObjectSchema.RawName
		}, null, new GetterDelegate(Server.OuGetter), null, null, null);

		public static readonly ADPropertyDefinition InternetWebProxy = new ADPropertyDefinition("InternetWebProxy", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchInternetWebProxy", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition IsPreE12FrontEnd = new ADPropertyDefinition("IsPreE12FrontEnd", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.ServerRole,
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsPreE12FrontEndGetter), null, null, null);

		public static readonly ADPropertyDefinition IsPreE12RPCHTTPEnabled = new ADPropertyDefinition("IsPreE12RPCHTTPEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.Heuristics,
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsPreE12RPCHTTPEnabledGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchange2003OrLater = new ADPropertyDefinition("IsExchange2003OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsExchange2003OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchange2003Sp1OrLater = new ADPropertyDefinition("IsExchange2003Sp1OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsExchange2003Sp1OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchange2003Sp2OrLater = new ADPropertyDefinition("IsExchange2003Sp2OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsExchange2003Sp2OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchange2003Sp3OrLater = new ADPropertyDefinition("IsExchange2003Sp3OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsExchange2003Sp3OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchange2007OrLater = new ADPropertyDefinition("IsExchange2007OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsE12OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsE14OrLater = new ADPropertyDefinition("IsE14OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsE14OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsE14Sp1OrLater = new ADPropertyDefinition("IsE14Sp1OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsE14Sp1OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition IsE15OrLater = new ADPropertyDefinition("IsE15OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.IsE15OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.SerialNumber
		}, null, new GetterDelegate(Server.AdminDisplayVersionGetter), new SetterDelegate(Server.AdminDisplayVersionSetter), null, null);

		public static readonly ADPropertyDefinition MajorVersion = new ADPropertyDefinition("MajorVersion", ExchangeObjectVersion.Exchange2003, typeof(int), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber
		}, null, new GetterDelegate(Server.MajorVersionGetter), null, null, null);

		public static readonly ADPropertyDefinition IsMailboxServer = new ADPropertyDefinition("IsMailboxServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, new CustomFilterBuilderDelegate(Server.MailboxServerRoleFlagFilterBuilder), new GetterDelegate(Server.IsMailboxServerGetter), new SetterDelegate(Server.IsMailboxServerSetter), null, null);

		public static readonly ADPropertyDefinition IsClientAccessServer = new ADPropertyDefinition("IsClientAccessServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsClientAccessServerGetter), new SetterDelegate(Server.IsClientAccessServerSetter), null, null);

		public static readonly ADPropertyDefinition IsUnifiedMessagingServer = new ADPropertyDefinition("IsUnifiedMessagingServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsUnifiedMessagingServerGetter), new SetterDelegate(Server.IsUnifiedMessagingServerSetter), null, null);

		public static readonly ADPropertyDefinition IsHubTransportServer = new ADPropertyDefinition("IsHubTransportServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsHubTransportServerGetter), new SetterDelegate(Server.IsHubTransportServerSetter), null, null);

		public static readonly ADPropertyDefinition IsEdgeServer = new ADPropertyDefinition("IsEdgeServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsEdgeServerGetter), new SetterDelegate(Server.IsEdgeServerSetter), null, null);

		public static readonly ADPropertyDefinition IsProvisionedServer = new ADPropertyDefinition("IsProvisionedServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsProvisionedServerGetter), new SetterDelegate(Server.IsProvisionedServerSetter), null, null);

		public static readonly ADPropertyDefinition IsCafeServer = new ADPropertyDefinition("IsCafeServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, new CustomFilterBuilderDelegate(Server.CafeServerRoleFlagFilterBuilder), new GetterDelegate(Server.IsCafeServerGetter), new SetterDelegate(Server.IsCafeServerSetter), null, null);

		public static readonly ADPropertyDefinition IsFrontendTransportServer = new ADPropertyDefinition("IsFrontendTransportServer", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.IsFrontendTransportServerGetter), new SetterDelegate(Server.IsFrontendTransportServerSetter), null, null);

		public static readonly ADPropertyDefinition EmptyDomainAllowed = new ADPropertyDefinition("EmptyDomainAllowed", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.CurrentServerRole
		}, null, new GetterDelegate(Server.EmptyDomainAllowedGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExchangeTrialEdition = new ADPropertyDefinition("IsExchangeTrialEdition", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber,
			ServerSchema.ProductID,
			ServerSchema.ServerType
		}, null, new GetterDelegate(Server.IsExchangeTrialEditionGetter), null, null, null);

		public static readonly ADPropertyDefinition IsExpiredExchangeTrialEdition = new ADPropertyDefinition("IsExpiredExchangeTrialEdition", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber,
			ServerSchema.ProductID,
			ServerSchema.ServerType,
			ADObjectSchema.WhenCreatedRaw
		}, null, new GetterDelegate(Server.IsExpiredExchangeTrialEditionGetter), null, null, null);

		public static readonly ADPropertyDefinition RemainingTrialPeriod = new ADPropertyDefinition("RemainingTrialPeriod", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, EnhancedTimeSpan.Zero, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.VersionNumber,
			ServerSchema.ProductID,
			ServerSchema.ServerType,
			ADObjectSchema.WhenCreatedRaw
		}, null, new GetterDelegate(Server.RemainingTrialPeriodGetter), null, null, null);

		public static readonly ADPropertyDefinition ElcSchedule = new ADPropertyDefinition("ElcSchedule", ExchangeObjectVersion.Exchange2007, typeof(ScheduleInterval[]), "msExchELCSchedule", ADPropertyDefinitionFlags.Binary, null, new PropertyDefinitionConstraint[]
		{
			new ElcScheduleIntervalsConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DelayNotificationTimeout = new ADPropertyDefinition("DelayNotificationTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportDelayNotificationTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromHours(4.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.FromDays(30.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageExpirationTimeout = new ADPropertyDefinition("MessageExpirationTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMessageExpirationTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(2.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(5.0), EnhancedTimeSpan.FromDays(90.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QueueMaxIdleTime = new ADPropertyDefinition("QueueMaxIdleTime", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxQueueIdleTime", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(3.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(5.0), EnhancedTimeSpan.OneHour),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageRetryInterval = new ADPropertyDefinition("MessageRetryInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMessageRetryInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(15.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransientFailureRetryInterval = new ADPropertyDefinition("TransientFailureRetryInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportTransientFailureRetryInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(5.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.FromHours(12.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransientFailureRetryCount = new ADPropertyDefinition("TransientFailureRetryCount", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportTransientFailureRetryCount", ADPropertyDefinitionFlags.PersistDefaultValue, 6, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 15)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxOutboundConnections = new ADPropertyDefinition("MaxOutboundConnections", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpMaxOutgoingConnections", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxPerDomainOutboundConnections = new ADPropertyDefinition("MaxPerDomainOutboundConnections", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<int>), "msExchSmtpMaxOutgoingConnectionsPerDomain", ADPropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxConnectionRatePerMinute = new ADPropertyDefinition("MaxConnectionRatePerMinute", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpReceiveMaxConnectionRatePerMinute", ADPropertyDefinitionFlags.PersistDefaultValue, 1200, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReceiveProtocolLogPath = new ADPropertyDefinition("ReceiveProtocolLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportReceiveProtocolLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition SendProtocolLogPath = new ADPropertyDefinition("SendProtocolLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportSendProtocolLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition OutboundConnectionFailureRetryInterval = new ADPropertyDefinition("OutboundConnectionFailureRetryInterval", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportOutboundConnectionFailureRetryInterval", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(10.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.FromDays(20.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

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

		public static readonly ADPropertyDefinition InternalDNSAdapterGuid = new ADPropertyDefinition("InternalDNSAdapter", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchTransportInternalDNSAdapterGuid", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDNSServers = new ADPropertyDefinition("InternalDNSServers", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchTransportInternalDNSServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalDNSProtocolOption = new ADPropertyDefinition("InternalDNSProtocolOption", ExchangeObjectVersion.Exchange2007, typeof(ProtocolOption), "msExchTransportInternalDNSProtocolOption", ADPropertyDefinitionFlags.None, ProtocolOption.Any, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDNSAdapterGuid = new ADPropertyDefinition("ExternalDNSAdapterGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), "msExchTransportExternalDNSAdapterGuid", ADPropertyDefinitionFlags.Binary, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDNSServersStr = new ADPropertyDefinition("ExternalDNSServersStr", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchSmtpExternalDNSServers", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalDNSServers = new ADPropertyDefinition("ExternalDNSServers", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.ExternalDNSServersStr
		}, null, new GetterDelegate(Server.ExternalDNSServersGetter), new SetterDelegate(Server.ExternalDNSServersSetter), null, null);

		public static readonly ADPropertyDefinition ExternalDNSProtocolOption = new ADPropertyDefinition("ExternalDNSProtocolOption", ExchangeObjectVersion.Exchange2007, typeof(ProtocolOption), "msExchTransportExternalDNSProtocolOption", ADPropertyDefinitionFlags.None, ProtocolOption.Any, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalIPAddress = new ADPropertyDefinition("ExternalIPAddress", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchTransportExternalIPAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxConcurrentMailboxDeliveries = new ADPropertyDefinition("MaxConcurrentMailboxDeliveries", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxConcurrentMailboxDeliveries", ADPropertyDefinitionFlags.PersistDefaultValue, 20, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 256)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PoisonThreshold = new ADPropertyDefinition("PoisonThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportPoisonMessageThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 2, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 10)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogPath = new ADPropertyDefinition("MessageTrackingLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportMessageTrackingPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogMaxAge = new ADPropertyDefinition("MessageTrackingLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxMessageTrackingLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogMaxDirectorySize = new ADPropertyDefinition("MessageTrackingLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportMaxMessageTrackingDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogMaxFileSize = new ADPropertyDefinition("MessageTrackingLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportMaxMessageTrackingFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromGB(4UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition IrmLogPath = new ADPropertyDefinition("IrmLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchIRMLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition IrmLogMaxAge = new ADPropertyDefinition("IrmLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchIRMLogMaxAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IrmLogMaxDirectorySize = new ADPropertyDefinition("IrmLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchIRMLogMaxDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IrmLogMaxFileSize = new ADPropertyDefinition("IrmLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchIRMLogMaxFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(10UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogPath = new ADPropertyDefinition("ActiveUserStatisticsLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportRecipientStatisticsPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxAge = new ADPropertyDefinition("ActiveUserStatisticsLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxRecipientStatisticsLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxDirectorySize = new ADPropertyDefinition("ActiveUserStatisticsLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportRecipientStatisticsDirectorySize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromGB(4UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition ActiveUserStatisticsLogMaxFileSize = new ADPropertyDefinition("ActiveUserStatisticsLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportRecipientStatisticsFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(10UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition ServerStatisticsLogPath = new ADPropertyDefinition("ServerStatisticsLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportServerStatisticsPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxAge = new ADPropertyDefinition("ServerStatisticsLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportMaxServerStatisticsLogAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxDirectorySize = new ADPropertyDefinition("ServerStatisticsLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportServerStatisticsDirectorySize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromGB(4UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition ServerStatisticsLogMaxFileSize = new ADPropertyDefinition("ServerStatisticsLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportServerStatisticsFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(10UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromGB(4UL))
		}, null, null);

		public static readonly ADPropertyDefinition PipelineTracingPath = new ADPropertyDefinition("PipelineTracingPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportPipelineTracingPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition PipelineTracingSenderAddress = new ADPropertyDefinition("PipelineTracingSenderAddress", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress?), "msExchTransportPipelineTracingSenderAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new UIImpactStringLengthConstraint(0, 320)
		}, null, null);

		public static readonly ADPropertyDefinition ConnectivityLogPath = new ADPropertyDefinition("ConnectivityLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportConnectivityLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

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

		public static readonly ADPropertyDefinition PickupDirectoryPath = new ADPropertyDefinition("PickupDirectoryPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportPickupDirectoryPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition ReplayDirectoryPath = new ADPropertyDefinition("ReplayDirectoryPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportReplayDirectoryPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition PickupDirectoryMaxMessagesPerMinute = new ADPropertyDefinition("PickupDirectoryMaxMessagesPerMinute", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxPickupDirectoryMessagesPerMinute", ADPropertyDefinitionFlags.PersistDefaultValue, 100, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 20000)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PickupDirectoryMaxHeaderSize = new ADPropertyDefinition("PickupDirectoryMaxHeaderSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchTransportMaxPickupDirectoryHeaderSize", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(64UL), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(32768UL), ByteQuantifiedSize.FromBytes(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PickupDirectoryMaxRecipientsPerMessage = new ADPropertyDefinition("PickupDirectoryMaxRecipientsPerMessage", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportMaxPickupDirectoryRecipients", ADPropertyDefinitionFlags.PersistDefaultValue, 100, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 10000)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoutingTableLogPath = new ADPropertyDefinition("RoutingTableLogPath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchTransportRoutingLogPath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition RoutingTableLogMaxAge = new ADPropertyDefinition("RoutingTableLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchTransportRoutingLogMaxAge", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(7.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RoutingTableLogMaxDirectorySize = new ADPropertyDefinition("RoutingTableLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(Unlimited<ByteQuantifiedSize>), "msExchTransportRoutingLogMaxDirectorySize", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromBytes(1UL), ByteQuantifiedSize.FromBytes(9223372036854775807UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalPostmasterAddress = new ADPropertyDefinition("E12TransportExternalPostmasterAddress", ExchangeObjectVersion.Exchange2007, typeof(SmtpAddress?), "msExchTransportExternalPostmasterAddress", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IntraOrgConnectorProtocolLoggingLevel = new ADPropertyDefinition("TransportIntraOrgConnectorProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchTransportOutboundProtocolLoggingLevel", ADPropertyDefinitionFlags.None, ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorProtocolLoggingLevel = new ADPropertyDefinition("TransportInMemoryReceiveConnectorProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchTransportInboundProtocolLoggingLevel", ADPropertyDefinitionFlags.None, ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportServerFlags = new ADPropertyDefinition("TransportServerFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 17417, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition ConnectivityLogEnabled = new ADPropertyDefinition("ConnectivityLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 8192), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 8192), null, null);

		public static readonly ADPropertyDefinition InternalDNSAdapterDisabled = new ADPropertyDefinition("InternalDNSAdapterDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 4), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 4), null, null);

		public static readonly ADPropertyDefinition InMemoryReceiveConnectorSmtpUtf8Enabled = new ADPropertyDefinition("TransportInMemoryReceiveConnectorSmtpUtf8Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 4194304), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 4194304), null, null);

		public static readonly ADPropertyDefinition ExternalDNSAdapterDisabled = new ADPropertyDefinition("ExternalDNSAdapterDisabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 2), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 2), null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogEnabled = new ADPropertyDefinition("MessageTrackingLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 1), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 1), null, null);

		public static readonly ADPropertyDefinition MessageTrackingLogSubjectLoggingEnabled = new ADPropertyDefinition("MessageTrackingLogSubjectLoggingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 16384), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 16384), null, null);

		public static readonly ADPropertyDefinition PipelineTracingEnabled = new ADPropertyDefinition("PipelineTracingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 32768), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 32768), null, null);

		public static readonly ADPropertyDefinition ContentConversionTracingEnabled = new ADPropertyDefinition("ContentConversionTracingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 1048576), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 1048576), null, null);

		public static readonly ADPropertyDefinition IrmLogEnabled = new ADPropertyDefinition("IrmLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 1024), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 1024), null, null);

		public static readonly ADPropertyDefinition GatewayEdgeSyncSubscribed = new ADPropertyDefinition("GatewayEdgeSyncSubscribed", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 65536), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 65536), null, null);

		public static readonly ADPropertyDefinition AntispamUpdatesEnabled = new ADPropertyDefinition("AntispamUpdatesEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 524288), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 524288), null, null);

		public static readonly ADPropertyDefinition PoisonMessageDetectionEnabled = new ADPropertyDefinition("PoisonMessageDetectionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 8), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 8), null, null);

		public static readonly ADPropertyDefinition RecipientValidationCacheEnabled = new ADPropertyDefinition("RecipientValidationCacheEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 2048), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 2048), null, null);

		public static readonly ADPropertyDefinition AntispamAgentsEnabled = new ADPropertyDefinition("AntispamAgentsEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportServerFlags, 4096), ADObject.FlagSetterDelegate(ServerSchema.TransportServerFlags, 4096), null, null);

		public static readonly ADPropertyDefinition SubmissionServerOverrideList = new ADPropertyDefinition("SubmissionServerOverrideList", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchTransportSubmissionServerOverrideList", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Status = new ADPropertyDefinition("Status", ExchangeObjectVersion.Exchange2007, typeof(ServerStatus), "msExchUMServerStatus", ADPropertyDefinitionFlags.None, ServerStatus.Enabled, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ServerStatus))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMStartupMode = new ADPropertyDefinition("UMStartupMode", ExchangeObjectVersion.Exchange2007, typeof(UMStartupMode), "msExchUMStartupMode", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.SystemConfiguration.UMStartupMode.TCP, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMStartupMode))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ClientAccessArray = new ADPropertyDefinition("ClientAccessArray", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchServerAssociationLink", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseAvailabilityGroup = new ADPropertyDefinition("DatabaseAvailabilityGroup", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchMDBAvailabilityGroupLink", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LanguagesTemp = new ADPropertyDefinition("LanguagesTemp", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMAvailableLanguages", ADPropertyDefinitionFlags.MultiValued, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1048576)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Languages = new ADPropertyDefinition("Languages", ExchangeObjectVersion.Exchange2007, typeof(UMLanguage), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.LanguagesTemp
		}, null, delegate(IPropertyBag propertyBag)
		{
			MultiValuedProperty<int> multiValuedProperty = (MultiValuedProperty<int>)propertyBag[ServerSchema.LanguagesTemp];
			MultiValuedProperty<UMLanguage> multiValuedProperty2 = new MultiValuedProperty<UMLanguage>();
			if (multiValuedProperty != null)
			{
				foreach (int lcid in multiValuedProperty)
				{
					try
					{
						UMLanguage item = new UMLanguage(lcid);
						multiValuedProperty2.Add(item);
					}
					catch (ArgumentException)
					{
					}
				}
			}
			return multiValuedProperty2;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<UMLanguage> multiValuedProperty = value as MultiValuedProperty<UMLanguage>;
			if (multiValuedProperty != null)
			{
				MultiValuedProperty<int> multiValuedProperty2 = new MultiValuedProperty<int>();
				foreach (UMLanguage umlanguage in multiValuedProperty)
				{
					multiValuedProperty2.Add(umlanguage.LCID);
				}
				propertyBag[ServerSchema.LanguagesTemp] = multiValuedProperty2;
				return;
			}
			propertyBag[ServerSchema.LanguagesTemp] = null;
		}, null, null);

		public static readonly ADPropertyDefinition MaxCallsAllowed = new ADPropertyDefinition("MaxCallsAllowed", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchUMMaximumCallsAllowed", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(0, 200)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DialPlans = new ADPropertyDefinition("DialPlans", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchUMServerDialPlanLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GrammarGenerationSchedule = new ADPropertyDefinition("GrammarGenerationSchedule", ExchangeObjectVersion.Exchange2007, typeof(ScheduleInterval[]), "msExchUMGrammarGenerationSchedule", ADPropertyDefinitionFlags.Binary, new ScheduleInterval[]
		{
			new ScheduleInterval(DayOfWeek.Sunday, 2, 0, DayOfWeek.Sunday, 2, 30),
			new ScheduleInterval(DayOfWeek.Monday, 2, 0, DayOfWeek.Monday, 2, 30),
			new ScheduleInterval(DayOfWeek.Tuesday, 2, 0, DayOfWeek.Tuesday, 2, 30),
			new ScheduleInterval(DayOfWeek.Wednesday, 2, 0, DayOfWeek.Wednesday, 2, 30),
			new ScheduleInterval(DayOfWeek.Thursday, 2, 0, DayOfWeek.Thursday, 2, 30),
			new ScheduleInterval(DayOfWeek.Friday, 2, 0, DayOfWeek.Friday, 2, 30),
			new ScheduleInterval(DayOfWeek.Saturday, 2, 0, DayOfWeek.Saturday, 2, 30)
		}, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalHostFqdn = new ADPropertyDefinition("ExternalHostFqdn", ExchangeObjectVersion.Exchange2007, typeof(UMSmartHost), "msExchUMRedirectTarget", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMPodRedirectTemplate = new ADPropertyDefinition("UMPodRedirectTemplate", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMSiteRedirectTarget", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMForwardingAddressTemplate = new ADPropertyDefinition("UMForwardingAddressTemplate", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMForwardingAddressTemplate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalFolderAffinityCustom = new ADPropertyDefinition("InternalFolderAffinityCustom", ExchangeObjectVersion.Exchange2003, typeof(PublicFolderReferralOption), "msExchFolderAffinityCustom", ADPropertyDefinitionFlags.DoNotProvisionalClone, PublicFolderReferralOption.ADSite, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FolderAffinityCustom = new ADPropertyDefinition("FolderAffinityCustom", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.InternalFolderAffinityCustom
		}, null, (IPropertyBag propertyBag) => (PublicFolderReferralOption)propertyBag[ServerSchema.InternalFolderAffinityCustom] == PublicFolderReferralOption.CustomList, delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[ServerSchema.InternalFolderAffinityCustom] = (((bool)value) ? PublicFolderReferralOption.CustomList : PublicFolderReferralOption.ADSite);
		}, null, null);

		public static readonly ADPropertyDefinition FolderAffinityList = new ADPropertyDefinition("FolderAffinityList", ExchangeObjectVersion.Exchange2003, typeof(ServerCostPair), "msExchFolderAffinityList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Locale = new ADPropertyDefinition("Locale", ExchangeObjectVersion.Exchange2003, typeof(CultureInfo), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ErrorReportingEnabled = new ADPropertyDefinition("ErrorReportingEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StaticDomainControllers = new ADPropertyDefinition("StaticDomainControllers", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StaticGlobalCatalogs = new ADPropertyDefinition("StaticGlobalCatalogs", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StaticConfigDomainController = new ADPropertyDefinition("StaticConfigDomainController", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StaticExcludedDomainControllers = new ADPropertyDefinition("StaticExcludedDomainControllers", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentDomainControllers = new ADPropertyDefinition("CurrentDomainControllers", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentGlobalCatalogs = new ADPropertyDefinition("CurrentGlobalCatalogs", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CurrentConfigDomainController = new ADPropertyDefinition("CurrentConfigDomainController", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public new static readonly ADPropertyDefinition SystemFlags = new ADPropertyDefinition("SystemFlags", ExchangeObjectVersion.Exchange2003, typeof(SystemFlagsEnum), "systemFlags", ADPropertyDefinitionFlags.PersistDefaultValue, SystemFlagsEnum.DeleteImmediately | SystemFlagsEnum.Renamable, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RootDropDirectoryPath = new ADPropertyDefinition("RootDropDirectoryPath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchTransportRootDropDirectoryPath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ServerSite = new ADPropertyDefinition("ServerSite", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchServerSite", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncServerFlags = new ADPropertyDefinition("TransportSyncServerFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncEnabled = new ADPropertyDefinition("TransportSyncEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 1), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 1), null, null);

		public static readonly ADPropertyDefinition TransportSyncPopEnabled = new ADPropertyDefinition("TransportSyncPopEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 4), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 4), null, null);

		public static readonly ADPropertyDefinition WindowsLiveHotmailTransportSyncEnabled = new ADPropertyDefinition("WindowsLiveHotmailTransportSyncEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 8), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 8), null, null);

		public static readonly ADPropertyDefinition TransportSyncExchangeEnabled = new ADPropertyDefinition("TransportSyncExchangeEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 32), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 32), null, null);

		public static readonly ADPropertyDefinition TransportSyncImapEnabled = new ADPropertyDefinition("TransportSyncImapEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 64), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 64), null, null);

		public static readonly ADPropertyDefinition TransportSyncFacebookEnabled = new ADPropertyDefinition("TransportSyncFacebookEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 8192), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 8192), null, null);

		public static readonly ADPropertyDefinition TransportSyncDispatchEnabled = new ADPropertyDefinition("TransportSyncDispatchEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 512), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 512), null, null);

		public static readonly ADPropertyDefinition TransportSyncLinkedInEnabled = new ADPropertyDefinition("TransportSyncLinkedInEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 16384), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 16384), null, null);

		public static readonly ADPropertyDefinition MaxNumberOfTransportSyncAttempts = new ADPropertyDefinition("MaxNumberOfTransportSyncAttempts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationMaxNumberOfAttempts", ADPropertyDefinitionFlags.PersistDefaultValue, 3, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxAcceptedTransportSyncJobsPerProcessor = new ADPropertyDefinition("MaxAcceptedTransportSyncJobsPerProcessor", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationMaxAcceptedJobsPerProcessor", ADPropertyDefinitionFlags.PersistDefaultValue, 64, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxActiveTransportSyncJobsPerProcessor = new ADPropertyDefinition("MaxActiveTransportSyncJobsPerProcessor", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationMaxActiveJobsPerProcessor", ADPropertyDefinitionFlags.PersistDefaultValue, 16, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpTransportSyncProxyServer = new ADPropertyDefinition("HttpTransportSyncProxyServer", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchContentAggregationProxyServerURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogEnabled = new ADPropertyDefinition("HttpProtocolLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 128), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 128), null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogFilePath = new ADPropertyDefinition("HttpProtocolLogFilePath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchHTTPProtocolLogFilePath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogMaxAge = new ADPropertyDefinition("HttpProtocolLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchHttpProtocolLogAgeQuotaInHours", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromHours(168.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogMaxDirectorySize = new ADPropertyDefinition("HttpProtocolLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchHTTPProtocolLogDirectorySizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(256000UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogMaxFileSize = new ADPropertyDefinition("HttpProtocolLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchHTTPProtocolLogPerFileSizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(10240UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HttpProtocolLogLoggingLevel = new ADPropertyDefinition("HttpProtocolLogLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchHTTPProtocolLogLoggingLevel", ADPropertyDefinitionFlags.PersistDefaultValue, ProtocolLoggingLevel.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<ProtocolLoggingLevel>(ProtocolLoggingLevel.None, (ProtocolLoggingLevel)2147483647)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncLogEnabled = new ADPropertyDefinition("TransportSyncLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 256), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 256), null, null);

		public static readonly ADPropertyDefinition TransportSyncLogFilePath = new ADPropertyDefinition("TransportSyncLogFilePath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchSyncLogFilePath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncLogLoggingLevel = new ADPropertyDefinition("TransportSyncLogLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(SyncLoggingLevel), "msExchSyncLogLoggingLevel", ADPropertyDefinitionFlags.PersistDefaultValue, SyncLoggingLevel.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<SyncLoggingLevel>(SyncLoggingLevel.None, (SyncLoggingLevel)2147483647)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncLogMaxAge = new ADPropertyDefinition("TransportSyncLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSyncLogAgeQuotaInHours", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromHours(720.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncLogMaxDirectorySize = new ADPropertyDefinition("TransportSyncLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSyncLogDirectorySizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromGB(10UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncLogMaxFileSize = new ADPropertyDefinition("TransportSyncLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSyncLogPerFileSizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(10240UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonDetectionEnabled = new ADPropertyDefinition("TransportSyncAccountsPoisonDetectionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 1024), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 1024), null, null);

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonAccountThreshold = new ADPropertyDefinition("TransportSyncAccountsPoisonAccountThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncAccountsPoisonAccountThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 2, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncAccountsPoisonItemThreshold = new ADPropertyDefinition("TransportSyncAccountsPoisonItemThreshold", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSyncAccountsPoisonItemThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 2, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncRemoteConnectionTimeout = new ADPropertyDefinition("TransportSyncRemoteConnectionTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchContentAggregationRemoteConnectionTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMilliseconds(100000.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadSizePerItem = new ADPropertyDefinition("TransportSyncMaxDownloadSizePerItem", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchContentAggregationMaxDownloadSizePerItem", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(36UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadSizePerConnection = new ADPropertyDefinition("TransportSyncMaxDownloadSizePerConnection", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchContentAggregationMaxDownloadSizePerConnection", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromMB(50UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMaxDownloadItemsPerConnection = new ADPropertyDefinition("TransportSyncMaxDownloadItemsPerConnection", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationMaxDownloadItemsPerConnection", ADPropertyDefinitionFlags.PersistDefaultValue, 1000, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeltaSyncClientCertificateThumbprint = new ADPropertyDefinition("DeltaSyncClientCertificateThumbprint", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchDeltaSyncClientCertificateThumbprint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMCertificateThumbprint = new ADPropertyDefinition("UMCertificateThumbprint", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchUMCertificateThumbprint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SIPAccessService = new ADPropertyDefinition("SIPAccessService", ExchangeObjectVersion.Exchange2007, typeof(ProtocolConnectionSettings), "msExchSIPAccessService", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxTransportSyncDispatchers = new ADPropertyDefinition("MaxTransportSyncDispatchers", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchContentAggregationMaxDispatchers", ADPropertyDefinitionFlags.PersistDefaultValue, 5, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogEnabled = new ADPropertyDefinition("TransportSyncMailboxLogEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ServerSchema.TransportSyncServerFlags
		}, null, ADObject.FlagGetterDelegate(ServerSchema.TransportSyncServerFlags, 2048), ADObject.FlagSetterDelegate(ServerSchema.TransportSyncServerFlags, 2048), null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogLoggingLevel = new ADPropertyDefinition("TransportSyncMailboxLogLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(SyncLoggingLevel), "msExchSyncMailboxLogLoggingLevel", ADPropertyDefinitionFlags.PersistDefaultValue, SyncLoggingLevel.None, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<SyncLoggingLevel>(SyncLoggingLevel.None, (SyncLoggingLevel)2147483647)
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogMaxFileSize = new ADPropertyDefinition("TransportSyncMailboxLogMaxFileSize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSyncMailboxLogPerFileSizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromKB(10240UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogMaxAge = new ADPropertyDefinition("TransportSyncMailboxLogMaxAge", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSyncMailboxLogAgeQuotaInHours", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromHours(720.0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.MaxValue),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogMaxDirectorySize = new ADPropertyDefinition("TransportSyncMailboxLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, typeof(ByteQuantifiedSize), "msExchSyncMailboxLogDirectorySizeQuota", ADPropertyDefinitionFlags.PersistDefaultValue, ByteQuantifiedSize.FromGB(2UL), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TransportSyncMailboxLogFilePath = new ADPropertyDefinition("TransportSyncMailboxLogFilePath", ExchangeObjectVersion.Exchange2007, typeof(LocalLongFullPath), "msExchSyncMailboxLogFilePath", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint
		}, null, null);

		public static readonly ADPropertyDefinition IntraOrgConnectorSmtpMaxMessagesPerConnection = new ADPropertyDefinition("IntraOrgConnectorSmtpMaxMessagesPerConnection", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpMaxMessagesPerConnection", ADPropertyDefinitionFlags.PersistDefaultValue, 20, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MaxActiveMailboxDatabases = new ADPropertyDefinition("MaxActiveMailboxDatabases", ExchangeObjectVersion.Exchange2007, typeof(int?), "msExchMaxActiveMailboxDatabases", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringFlags = new ADPropertyDefinition("MalwareFilteringFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 16, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringForceRescan = ADObject.BitfieldProperty("MalwareFilteringForceRescan", 1, ServerSchema.MalwareFilteringFlags);

		public static readonly ADPropertyDefinition MalwareFilteringBypass = ADObject.BitfieldProperty("MalwareFilteringBypass", 2, ServerSchema.MalwareFilteringFlags);

		public static readonly ADPropertyDefinition MalwareFilteringScanErrorAction = ADObject.BitfieldProperty("MalwareFilteringScanErrorAction", 3, 1, ServerSchema.MalwareFilteringFlags);

		public static readonly ADPropertyDefinition MinimumSuccessfulEngineScans = ADObject.BitfieldProperty("MinimumSuccessfulEngineScans", 4, 3, ServerSchema.MalwareFilteringFlags);

		public static readonly ADPropertyDefinition MalwareFilteringDeferWaitTime = new ADPropertyDefinition("MalwareFilteringDeferWaitTime", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringDeferWaitTime", ADPropertyDefinitionFlags.PersistDefaultValue, 5, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 15)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringDeferAttempts = new ADPropertyDefinition("MalwareFilteringDeferAttempts", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringDeferAttempts", ADPropertyDefinitionFlags.PersistDefaultValue, 3, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 5)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringUpdateFrequency = new ADPropertyDefinition("MalwareFilteringUpdateFrequency", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringUpdateFrequency", ADPropertyDefinitionFlags.PersistDefaultValue, 30, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 38880)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringUpdateTimeout = new ADPropertyDefinition("MalwareFilteringUpdateTimeout", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringUpdateTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, 150, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(60, 300)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringScanTimeout = new ADPropertyDefinition("MalwareFilteringScanTimeout", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMalwareFilteringScanTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, 300, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(10, 900)
		}, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringPrimaryUpdatePath = new ADPropertyDefinition("MalwareFilteringPrimaryUpdatePath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMalwareFilteringPrimaryUpdatePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MalwareFilteringSecondaryUpdatePath = new ADPropertyDefinition("MalwareFilteringSecondaryUpdatePath", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMalwareFilteringSecondaryUpdatePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<ServerConfigXML>(ServerSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition MaxPreferredActiveDatabases = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, int?>("MaximumPreferredActiveDatabases", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.MaximumPreferredActiveDatabases, delegate(ServerConfigXML configXml, int? value)
		{
			configXml.MaximumPreferredActiveDatabases = value;
		}, null, null);

		public static readonly ADPropertyDefinition QueueLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("QueueLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.QueueLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.QueueLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition QueueLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("QueueLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.QueueLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.QueueLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition QueueLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("QueueLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.QueueLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.QueueLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition QueueLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("QueueLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.QueueLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.QueueLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition WlmLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("WlmLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.WlmLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.WlmLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition WlmLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("WlmLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.WlmLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.WlmLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition WlmLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("WlmLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.WlmLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.WlmLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition WlmLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("WlmLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.WlmLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.WlmLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("AgentLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.AgentLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.AgentLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("AgentLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.AgentLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AgentLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("AgentLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.AgentLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.AgentLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("AgentLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.AgentLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.AgentLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition AgentLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("AgentLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.AgentLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.AgentLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition FlowControlLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("FlowControlLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, EnhancedTimeSpan.FromDays(30.0), (ServerConfigXML configXml) => configXml.FlowControlLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.FlowControlLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition FlowControlLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("FlowControlLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.FlowControlLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.FlowControlLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition FlowControlLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("FlowControlLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.FlowControlLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.FlowControlLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition FlowControlLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("FlowControlLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.FlowControlLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.FlowControlLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition FlowControlLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("FlowControlLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.FlowControlLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.FlowControlLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition ProcessingSchedulerLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("ProcessingSchedulerLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.ProcessingSchedulerLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.ProcessingSchedulerLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition ProcessingSchedulerLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("ProcessingSchedulerLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.ProcessingSchedulerLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ProcessingSchedulerLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ProcessingSchedulerLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("ProcessingSchedulerLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.ProcessingSchedulerLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ProcessingSchedulerLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ProcessingSchedulerLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("ProcessingSchedulerLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.ProcessingSchedulerLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.ProcessingSchedulerLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition ProcessingSchedulerLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("ProcessingSchedulerLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.ProcessingSchedulerLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.ProcessingSchedulerLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("ResourceLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.ResourceLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.ResourceLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("ResourceLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.ResourceLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ResourceLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("ResourceLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.ResourceLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.ResourceLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("ResourceLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.ResourceLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.ResourceLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition ResourceLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("ResourceLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.ResourceLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.ResourceLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("DnsLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, EnhancedTimeSpan.FromDays(7.0), (ServerConfigXML configXml) => configXml.DnsLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.DnsLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("DnsLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(100UL), (ServerConfigXML configXml) => configXml.DnsLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.DnsLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("DnsLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(10UL), (ServerConfigXML configXml) => configXml.DnsLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.DnsLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("DnsLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.DnsLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.DnsLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition DnsLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("DnsLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, false, (ServerConfigXML configXml) => configXml.DnsLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.DnsLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxProvisioningAttributes = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, MailboxProvisioningAttributes>("MailboxProvisioningAttributes", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.MailboxProvisioningAttributes, delegate(ServerConfigXML configXml, MailboxProvisioningAttributes value)
		{
			configXml.MailboxProvisioningAttributes = value;
		}, null, null);

		public static readonly ADPropertyDefinition JournalLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("JournalLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxAge, (ServerConfigXML configXml) => configXml.JournalLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.JournalLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition JournalLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("JournalLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxDirectorySize, (ServerConfigXML configXml) => configXml.JournalLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.JournalLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition JournalLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("JournalLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, LogConfigXML.DefaultMaxFileSize, (ServerConfigXML configXml) => configXml.JournalLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.JournalLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition JournalLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("JournalLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.JournalLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.JournalLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition JournalLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("JournalLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.JournalLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.JournalLog.Enabled = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportMaintenanceLogMaxAge = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, EnhancedTimeSpan>("MaintenanceLogMaxAge", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, EnhancedTimeSpan.FromDays(30.0), (ServerConfigXML configXml) => configXml.TransportMaintenanceLog.MaxAge, delegate(ServerConfigXML configXml, EnhancedTimeSpan value)
		{
			configXml.TransportMaintenanceLog.MaxAge = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportMaintenanceLogMaxDirectorySize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("MaintenanceLogMaxDirectorySize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(50UL), (ServerConfigXML configXml) => configXml.TransportMaintenanceLog.MaxDirectorySize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.TransportMaintenanceLog.MaxDirectorySize = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportMaintenanceLogMaxFileSize = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, Unlimited<ByteQuantifiedSize>>("MaintenanceLogMaxFileSize", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, ByteQuantifiedSize.FromMB(1UL), (ServerConfigXML configXml) => configXml.TransportMaintenanceLog.MaxFileSize, delegate(ServerConfigXML configXml, Unlimited<ByteQuantifiedSize> value)
		{
			configXml.TransportMaintenanceLog.MaxFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportMaintenanceLogPath = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, LocalLongFullPath>("MaintenanceLogPath", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, null, (ServerConfigXML configXml) => configXml.TransportMaintenanceLog.Path, delegate(ServerConfigXML configXml, LocalLongFullPath value)
		{
			configXml.TransportMaintenanceLog.Path = value;
		}, null, null);

		public static readonly ADPropertyDefinition TransportMaintenanceLogEnabled = XMLSerializableBase.ConfigXmlProperty<ServerConfigXML, bool>("MaintenanceLogEnabled", ExchangeObjectVersion.Exchange2007, ServerSchema.ConfigurationXML, true, (ServerConfigXML configXml) => configXml.TransportMaintenanceLog.Enabled, delegate(ServerConfigXML configXml, bool value)
		{
			configXml.TransportMaintenanceLog.Enabled = value;
		}, null, null);
	}
}
