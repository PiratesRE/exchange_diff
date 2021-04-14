using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ExchangeServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ServerSchema>();
		}

		public static readonly ADPropertyDefinition DataPath = ServerSchema.DataPath;

		public static readonly ADPropertyDefinition Domain = ServerSchema.Domain;

		public static readonly ADPropertyDefinition Edition = ServerSchema.Edition;

		public static readonly ADPropertyDefinition ExchangeLegacyDN = ServerSchema.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition ExchangeLegacyServerRole = ServerSchema.ExchangeLegacyServerRole;

		public static readonly ADPropertyDefinition Fqdn = ServerSchema.Fqdn;

		public static readonly ADPropertyDefinition IsHubTransportServer = ServerSchema.IsHubTransportServer;

		public static readonly ADPropertyDefinition IsClientAccessServer = ServerSchema.IsClientAccessServer;

		public static readonly ADPropertyDefinition IsExchange2007OrLater = ServerSchema.IsExchange2007OrLater;

		public static readonly ADPropertyDefinition IsEdgeServer = ServerSchema.IsEdgeServer;

		public static readonly ADPropertyDefinition IsMailboxServer = ServerSchema.IsMailboxServer;

		public static readonly ADPropertyDefinition IsProvisionedServer = ServerSchema.IsProvisionedServer;

		public static readonly ADPropertyDefinition IsUnifiedMessagingServer = ServerSchema.IsUnifiedMessagingServer;

		public static readonly ADPropertyDefinition IsCafeServer = ServerSchema.IsCafeServer;

		public static readonly ADPropertyDefinition IsFrontendTransportServer = ServerSchema.IsFrontendTransportServer;

		public static readonly ADPropertyDefinition NetworkAddress = ServerSchema.NetworkAddress;

		public static readonly ADPropertyDefinition OrganizationalUnit = ServerSchema.OrganizationalUnit;

		public static readonly ADPropertyDefinition CurrentServerRole = ServerSchema.CurrentServerRole;

		public static readonly ADPropertyDefinition AdminDisplayVersion = ServerSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition ErrorReportingEnabled = ServerSchema.ErrorReportingEnabled;

		public static readonly ADPropertyDefinition StaticDomainControllers = ServerSchema.StaticDomainControllers;

		public static readonly ADPropertyDefinition StaticGlobalCatalogs = ServerSchema.StaticGlobalCatalogs;

		public static readonly ADPropertyDefinition StaticConfigDomainController = ServerSchema.StaticConfigDomainController;

		public static readonly ADPropertyDefinition StaticExcludedDomainControllers = ServerSchema.StaticExcludedDomainControllers;

		public static readonly ADPropertyDefinition Site = ServerSchema.ServerSite;

		public static readonly ADPropertyDefinition CurrentDomainControllers = ServerSchema.CurrentDomainControllers;

		public static readonly ADPropertyDefinition CurrentGlobalCatalogs = ServerSchema.CurrentGlobalCatalogs;

		public static readonly ADPropertyDefinition CurrentConfigDomainController = ServerSchema.CurrentConfigDomainController;

		public static readonly ADPropertyDefinition ProductID = ServerSchema.ProductID;

		public static readonly ADPropertyDefinition InternetWebProxy = ServerSchema.InternetWebProxy;

		public static readonly ADPropertyDefinition IsExchangeTrialEdition = ServerSchema.IsExchangeTrialEdition;

		public static readonly ADPropertyDefinition IsExpiredExchangeTrialEdition = ServerSchema.IsExpiredExchangeTrialEdition;

		public static readonly ADPropertyDefinition RemainingTrialPeriod = ServerSchema.RemainingTrialPeriod;

		public static readonly ADPropertyDefinition CustomerFeedbackEnabled = ServerSchema.CustomerFeedbackEnabled;

		public static readonly ADPropertyDefinition IsE14OrLater = ServerSchema.IsE14OrLater;

		public static readonly ADPropertyDefinition IsE15OrLater = ServerSchema.IsE15OrLater;

		public static readonly ADPropertyDefinition MonitoringGroup = ServerSchema.MonitoringGroup;
	}
}
