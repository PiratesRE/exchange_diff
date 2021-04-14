using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct TransportTags
	{
		public const int General = 0;

		public const int SmtpReceive = 1;

		public const int SmtpSend = 2;

		public const int Pickup = 3;

		public const int Service = 4;

		public const int Queuing = 5;

		public const int DSN = 6;

		public const int Routing = 7;

		public const int Resolver = 8;

		public const int ContentConversion = 9;

		public const int Extensibility = 10;

		public const int Scheduler = 11;

		public const int SecureMail = 12;

		public const int MessageTracking = 13;

		public const int ResourceManager = 14;

		public const int Configuration = 15;

		public const int Dumpster = 16;

		public const int Expo = 17;

		public const int Certificate = 18;

		public const int Orar = 19;

		public const int ShadowRedundancy = 20;

		public const int Approval = 22;

		public const int TransportDumpster = 23;

		public const int TransportSettingsCache = 24;

		public const int TransportRulesCache = 25;

		public const int MicrosoftExchangeRecipientCache = 26;

		public const int RemoteDomainsCache = 27;

		public const int JournalingRulesCache = 28;

		public const int ResourcePool = 29;

		public const int DeliveryAgents = 30;

		public const int Supervision = 31;

		public const int RightsManagement = 32;

		public const int PerimeterSettingsCache = 33;

		public const int PreviousHopLatency = 34;

		public const int FaultInjection = 35;

		public const int OrganizationSettingsCache = 36;

		public const int AnonymousCertificateValidationResultCache = 37;

		public const int AcceptedDomainsCache = 38;

		public const int ProxyHubSelector = 39;

		public const int MessageResubmission = 40;

		public const int Storage = 41;

		public const int Poison = 42;

		public const int HostedEncryption = 43;

		public const int OutboundConnectorsCache = 44;

		public static Guid guid = new Guid("c3ea5adf-c135-45e7-9dff-e1dc3bd67999");
	}
}
