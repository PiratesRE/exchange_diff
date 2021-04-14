using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.EdgeSync
{
	internal static class Schema
	{
		public static class Query
		{
			public static string QueryAll
			{
				get
				{
					return "(objectClass=*)";
				}
			}

			public static string QueryRecipientsContainer
			{
				get
				{
					return "(CN=Recipients)";
				}
			}

			public static string QueryAllEnterpriseRecipients
			{
				get
				{
					if (Schema.Query.queryAllEnterpriseRecipients == null)
					{
						Schema.Query.queryAllEnterpriseRecipients = Schema.Query.BuildRecipientQuery(Schema.Query.RecipientQueryScenario.EnterpriseDirSyncQuery);
					}
					return Schema.Query.queryAllEnterpriseRecipients;
				}
			}

			public static string QueryAllSmtpRecipients
			{
				get
				{
					return Schema.Query.BuildRecipientQuery(Schema.Query.RecipientQueryScenario.EnterpriseTestEdgeSyncQuery);
				}
			}

			public static string QueryAllHostedSmtpRecipients
			{
				get
				{
					if (Schema.Query.queryAllHostedSmtpRecipients == null)
					{
						Schema.Query.queryAllHostedSmtpRecipients = Schema.Query.BuildRecipientQuery(Schema.Query.RecipientQueryScenario.DataCenterDirSyncQuery);
					}
					return Schema.Query.queryAllHostedSmtpRecipients;
				}
			}

			public static string QueryBridgeheads
			{
				get
				{
					return "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=32))";
				}
			}

			public static string QueryEdges
			{
				get
				{
					return "(&(objectClass=msExchExchangeServer)(msExchCurrentServerRoles:1.2.840.113556.1.4.803:=64))";
				}
			}

			public static string QuerySendConnectors
			{
				get
				{
					return "(objectClass=mailGateway)";
				}
			}

			public static string QueryPartnerDomains
			{
				get
				{
					return "(objectClass=msExchDomainContentConfig)";
				}
			}

			public static string QueryTransportSettings
			{
				get
				{
					return "(objectClass=msExchTransportSettings)";
				}
			}

			public static string QueryExchangeServerRecipients
			{
				get
				{
					return "(objectClass=msExchExchangeServerRecipient)";
				}
			}

			public static string QueryMessageClassifications
			{
				get
				{
					return "(objectClass=msExchMessageClassification)";
				}
			}

			public static string QueryAcceptedDomains
			{
				get
				{
					return "(objectClass=msExchAcceptedDomain)";
				}
			}

			public static string QueryHostedAcceptedDomains
			{
				get
				{
					return "(&(objectClass=msExchAcceptedDomain)(msExchCU=*)(msExchOURoot=*))";
				}
			}

			public static string QueryPerimeterSettings
			{
				get
				{
					return "(&(objectClass=msExchTenantPerimeterSettings)(msExchCU=*)(msExchOURoot=*))";
				}
			}

			public static string QueryUsgMailboxAndOrganization
			{
				get
				{
					return "(|(&(objectClass=group)(groupType:1.2.840.113556.1.4.803:=2147483656)(msExchCU=*)(msExchOURoot=*))(&(objectClass=user)(msExchCU=*)(msExchOURoot=*)(msExchWindowsLiveID=*))(&(objectClass=organizationalUnit)(msExchCU=*)(msExchOURoot=*)(msExchProvisioningFlags=*)))";
				}
			}

			public static string BuildHostedRecipientAddressQuery(string attributeName, List<string> addressList)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("(&");
				stringBuilder.Append("(msExchCU=*)(msExchOURoot=*)");
				stringBuilder.Append("(|");
				StringBuilder stringBuilder2 = new StringBuilder();
				string value;
				if (string.Equals(attributeName, "msExchUMAddresses", StringComparison.OrdinalIgnoreCase))
				{
					value = "=meum:";
				}
				else
				{
					value = "=smtp:";
				}
				foreach (string originalValue in addressList)
				{
					stringBuilder2.Append("(");
					stringBuilder2.Append(attributeName);
					stringBuilder2.Append(value);
					ADValueConvertor.EscapeAndAppendString(originalValue, stringBuilder2);
					stringBuilder2.Append(")");
				}
				stringBuilder.Append(stringBuilder2.ToString());
				stringBuilder.Append(")");
				stringBuilder.Append("(|");
				foreach (string value2 in Schema.Query.supportedHostedRecipientClasses)
				{
					stringBuilder.Append("(objectClass=");
					stringBuilder.Append(value2);
					stringBuilder.Append(")");
				}
				stringBuilder.Append("))");
				return stringBuilder.ToString();
			}

			private static string BuildRecipientQuery(Schema.Query.RecipientQueryScenario recipientQueryScenario)
			{
				string[] array = null;
				StringBuilder stringBuilder = new StringBuilder(300);
				stringBuilder.Append("(&");
				switch (recipientQueryScenario)
				{
				case Schema.Query.RecipientQueryScenario.DataCenterDirSyncQuery:
					stringBuilder.Append("(msExchCU=*)(msExchOURoot=*)");
					array = Schema.Query.supportedHostedRecipientClasses;
					break;
				case Schema.Query.RecipientQueryScenario.EnterpriseDirSyncQuery:
					array = Schema.Query.supportedEnterpriseRecipientClasses;
					break;
				case Schema.Query.RecipientQueryScenario.EnterpriseTestEdgeSyncQuery:
					stringBuilder.Append("(proxyAddresses=smtp*)");
					array = Schema.Query.supportedEnterpriseRecipientClasses;
					break;
				}
				stringBuilder.Append("(|");
				foreach (string value in array)
				{
					stringBuilder.Append("(objectClass=");
					stringBuilder.Append(value);
					stringBuilder.Append(")");
				}
				stringBuilder.Append(")");
				stringBuilder.Append("(!(objectclass=computer))");
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}

			public const string QueryAllProxyAddresses = "(proxyAddresses=*)";

			private const string QueryAllSmtpProxyAddresses = "(proxyAddresses=smtp*)";

			private const string HostedObjects = "(msExchCU=*)(msExchOURoot=*)";

			private static readonly string[] supportedEnterpriseRecipientClasses = new string[]
			{
				"msExchDynamicDistributionList",
				"publicFolder",
				"contact",
				"user",
				"group"
			};

			private static readonly string[] supportedHostedRecipientClasses = new string[]
			{
				"msExchDynamicDistributionList",
				"user",
				"group"
			};

			private static string queryAllEnterpriseRecipients;

			private static string queryAllHostedSmtpRecipients;

			internal enum RecipientQueryScenario
			{
				DataCenterDirSyncQuery,
				EnterpriseDirSyncQuery,
				EnterpriseTestEdgeSyncQuery
			}
		}

		public static class General
		{
			public const string ConfigContext = "configurationNamingContext";

			public const string DefaultContext = "defaultNamingContext";

			public const string NamingContexts = "namingContexts";

			public const string ServerName = "serverName";

			public const string HighestUSN = "highestCommittedUSN";

			public const string NtSecurityDescriptor = "nTSecurityDescriptor";

			public const string EdgeRecipientsPath = "CN=Recipients,OU=MSExchangeGateway";

			public const string MsExchVersion = "msExchVersion";

			public const string MinVersion = "msExchMinAdminVersion";

			public const string VersionNumber = "versionNumber";

			public const string LegacyExchangeDN = "legacyExchangeDN";

			public const string ObjectClass = "objectClass";

			public const string EdgeSyncSourceGuid = "msExchEdgeSyncSourceGuid";

			public const string EdgeSyncCookies = "msExchEdgeSyncCookies";

			public const string SystemFlags = "systemFlags";

			public const string ObjectGUID = "objectGUID";

			public const string WhenCreated = "whenCreated";

			public const string InstanceType = "instanceType";

			public const string ParentGUID = "parentGUID";

			public const string Name = "name";

			public const string SyncErrors = "msExchEdgeSyncCookies";

			public const string ConfigUnitDN = "msExchCU";

			public const string ExchOURoot = "msExchOURoot";

			public const string OtherWellKnownObjects = "otherWellKnownObjects";
		}

		public static class Server
		{
			public const string ClassName = "msExchExchangeServer";

			public const string EdgeSyncLease = "msExchEdgeSyncLease";

			public const string EdgeSyncStatus = "msExchEdgeSyncStatus";

			public const string NetworkAddress = "networkAddress";

			public const string ServerRoles = "msExchCurrentServerRoles";

			public const string ServerSite = "msExchServerSite";

			public const string TransportServerFlags = "msExchTransportFlags";

			public static readonly string[] FilterAttributes = new string[]
			{
				"msExchServerSite"
			};

			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchVersion",
				"msExchMinAdminVersion",
				"versionNumber",
				"legacyExchangeDN",
				"versionNumber",
				"networkAddress",
				"msExchCurrentServerRoles",
				"msExchEdgeSyncCredential",
				"msExchServerInternalTLSCert",
				"serialNumber",
				"type",
				"msExchProductID"
			};
		}

		public static class DomainConfig
		{
			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchVersion",
				"msExchMinAdminVersion",
				"domainName",
				"msExchDomainContentConfigFlags",
				"msExchPermittedAuthN",
				"msExchMLSDomainGatewaySMTPAddress",
				"msExchReceiveHashedPassword",
				"msExchReceiveUserName",
				"msExchSendEncryptedPassword",
				"msExchSendUserName",
				"msExchTlsAlternateSubject",
				"msExchNonMIMECharacterSet"
			};
		}

		public static class TransportConfig
		{
			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchInternalSMTPServers",
				"msExchTLSReceiveDomainSecureList",
				"msExchTLSSendDomainSecureList",
				"msExchTransportSettingsFlags",
				"msExchTransportShadowHeartbeatTimeoutInterval",
				"msExchTransportShadowHeartbeatRetryCount",
				"msExchTransportShadowMessageAutoDiscardInterval"
			};
		}

		public static class ExchangeRecipient
		{
			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchVersion",
				"proxyAddresses",
				"displayName"
			};
		}

		public static class AcceptedDomain
		{
			public const string ClassName = "msExchAcceptedDomain";

			public const string DomainName = "msExchAcceptedDomainName";

			public const string DomainFlags = "msExchAcceptedDomainFlags";

			public const string MailFlowPartner = "msExchTransportResellerSettingsLink";

			public const string PerimeterDuplicateDetectedFlags = "msExchTransportInboundSettings";

			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchVersion",
				"msExchAcceptedDomainName",
				"msExchAcceptedDomainFlags",
				"msExchEncryptedTLSP12"
			};
		}

		public static class SendConnector
		{
			public const string SourceBridgehead = "msExchSourceBridgeheadServersDN";

			public const string SendFlags = "msExchSmtpSendFlags";

			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchVersion",
				"msExchMinAdminVersion",
				"routingList",
				"msExchSmtpSmartHost",
				"msExchSmtpSendPort",
				"msExchSmtpSendConnectionTimeout",
				"delivContLength",
				"msExchSmtpSendProtocolLoggingLevel",
				"msExchSmtpSendFlags",
				"msExchSmtpSendBindingIPAddress",
				"deliveryMechanism",
				"msExchSmtpOutboundSecurityFlag",
				"msExchSMTPSendConnectorFQDN",
				"msExchSmtpSendEnabled",
				"msExchSMTPSendExternallySecuredAs",
				"msExchSmtpOutboundSecurityUserName",
				"msExchSmtpOutboundSecurityPassword",
				"msExchSmtpSendTlsDomain",
				"msExchSmtpSendNdrLevel",
				"msExchSmtpMaxMessagesPerConnection",
				"msExchSmtpTLSCertificate"
			};

			internal static readonly ServerVersion ServerVersion14_1_144 = new ServerVersion(14, 1, 144, 0);

			internal static readonly ServerVersion ServerVersion15_0_620 = new ServerVersion(15, 0, 620, 0);

			internal static readonly string[] NewAttributesInServerVersion14_1_144 = new string[]
			{
				"msExchSmtpSendTlsDomain",
				"msExchSmtpSendNdrLevel"
			};

			internal static readonly string[] NewAttributesInServerVersion15_0_620 = new string[]
			{
				"msExchSmtpTLSCertificate"
			};
		}

		public static class MessageClassification
		{
			public static readonly string[] PayloadAttributes = new string[]
			{
				"msExchMessageClassificationBanner",
				"msExchMessageClassificationConfidentialityAction",
				"msExchMessageClassificationDisplayPrecedence",
				"msExchMessageClassificationFlags",
				"msExchMessageClassificationIntegrityAction",
				"msExchMessageClassificationID",
				"msExchMessageClassificationLocale",
				"msExchMessageClassificationURL",
				"msExchMessageClassificationVersion"
			};
		}

		public static class PerimeterSettings
		{
			public const string ClassName = "msExchTenantPerimeterSettings";

			public const string EhfCompanyId = "msExchTenantPerimeterSettingsOrgID";

			public const string Flags = "msExchTenantPerimeterSettingsFlags";

			public const string GatewayIPAddresses = "msExchTenantPerimeterSettingsGatewayIPAddresses";

			public const string InternalServerIPAddresses = "msExchTenantPerimeterSettingsInternalServerIPAddresses";

			public const string ForceDomainSync = "msExchTransportInboundSettings";

			public const string TargetServerAdmins = "msExchTargetServerAdmins";

			public const string TargetServerViewOnlyAdmins = "msExchTargetServerViewOnlyAdmins";

			public const string TargetServerPartnerAdmins = "msExchTargetServerPartnerAdmins";

			public const string TargetServerPartnerViewOnlyAdmins = "msExchTargetServerPartnerViewOnlyAdmins";
		}

		public static class Recipient
		{
			public const string ProxyAddresses = "proxyAddresses";

			public const string SignupAddresses = "msExchSignupAddresses";

			public const string ExternalSyncState = "msExchExternalSyncState";

			public const string TransportSettings = "msExchTransportRecipientSettingsFlags";

			public const string TargetAddress = "targetAddress";

			public const string UMProxyAddresses = "msExchUMAddresses";

			public const string WindowsLiveId = "msExchWindowsLiveID";

			public const string ClassName = "user";

			public const string ArchiveGuid = "msExchArchiveGUID";

			public const string ArchiveAddress = "ArchiveAddress";

			public const string CapabilityIdentifiers = "msExchCapabilityIdentifiers";

			public const string PartnerGroupID = "msExchPartnerGroupID";

			public const string ExternalDirectoryObjectId = "msExchExternalDirectoryObjectId";

			public const string RecipientTypeDetailsValue = "msExchRecipientTypeDetails";

			public const string TenantCU = "msExchCU";

			public const string MailNickname = "mailNickname";

			public const string ServerLegacyDN = "msExchHomeServerName";
		}

		public static class UniversalSecurityGroup
		{
			public const string ClassName = "group";

			public const string Member = "member";
		}

		public static class MailFlowPartner
		{
			public const string PartnerInboundGatewayId = "msExchTransportResellerSettingsInboundGatewayID";

			public const string PartnerOutboundGatewayId = "msExchTransportResellerSettingsOutboundGatewayID";
		}

		public static class Organization
		{
			public const string ClassName = "organizationalUnit";

			public const string ProvisioningFlags = "msExchProvisioningFlags";
		}
	}
}
