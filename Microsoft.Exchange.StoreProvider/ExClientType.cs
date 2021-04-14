using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExClientType
	{
		internal const string ActiveSync = "Client=ActiveSync";

		internal const string AvailabilityService = "Client=AS";

		internal const string CI = "Client=CI";

		internal const string CIMoveDestination = "Client=CI;Client=CIMoveDestination";

		internal const string ELC = "Client=ELC";

		internal const string Management = "Client=Management";

		internal const string Monitoring = "Client=Monitoring";

		internal const string EventBasedAssistant = "Client=EBA";

		internal const string OWA = "Client=OWA";

		internal const string Pop = "Client=POP3/IMAP4;Protocol=POP3";

		internal const string Imap = "Client=POP3/IMAP4;Protocol=IMAP4";

		internal const string UnifiedMessaging = "Client=UM";

		internal const string WebServices = "Client=WebServices";

		internal const string ApprovalAPI = "Client=ApprovalAPI";

		internal const string TimeBasedAssistant = "Client=TBA";

		internal const string MSExchangeRPC = "Client=MSExchangeRPC";

		internal const string Migration = "Client=MSExchangeMigration";

		internal const string SimpleMigration = "Client=MSExchangeSimpleMigration";

		internal const string TransportSync = "Client=TransportSync";

		internal const string Transport = "Client=Hub Transport";

		internal const string HA = "Client=HA";

		internal const string Maintenance = "Client=Maintenance";

		internal const string Inference = "Client=Inference";

		internal const string StoreActiveMonitoring = "Client=StoreActiveMonitoring";

		internal const string PublicFolderSystem = "Client=PublicFolderSystem";

		internal const string EDiscoverySearch = "Client=EDiscoverySearch";

		internal const string AnchorService = "Client=AnchorService";

		internal const string MailboxLoadBalance = "Client=MSExchangeMailboxLoadBalance";

		internal const string OutlookService = "Client=OutlookService";

		internal const string UnifiedPolicy = "Client=UnifiedPolicy";

		internal const string NotificationBroker = "Client=NotificationBroker";

		internal const string SnackyService = "Client=SnackyService";
	}
}
