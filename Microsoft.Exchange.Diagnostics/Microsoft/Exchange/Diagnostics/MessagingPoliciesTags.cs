using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MessagingPoliciesTags
	{
		public const int TransportRulesEngine = 0;

		public const int CodeGenerator = 1;

		public const int Journaling = 2;

		public const int AttachmentFiltering = 3;

		public const int AddressRewriting = 4;

		public const int RmSvcAgent = 5;

		public const int RulesEngine = 6;

		public const int Reconciliation = 7;

		public const int CrossPremise = 8;

		public const int OpenDomainRoutingAgent = 9;

		public const int JrdAgent = 10;

		public const int PreLicensingAgent = 11;

		public const int OutlookProtectionRules = 12;

		public const int EhfOutboundRoutingAgent = 13;

		public const int FaultInjection = 14;

		public const int RedirectionAgent = 15;

		public const int InboundTrustAgent = 16;

		public const int OutboundTrustAgent = 17;

		public const int CentralizedMailFlowAgent = 18;

		public const int InterceptorAgent = 19;

		public const int OutlookPolicyNudgeRules = 20;

		public const int ClassificationDefinitions = 21;

		public const int FfoFrontendProxyAgent = 22;

		public const int FrontendProxyAgent = 23;

		public const int AddressBookPolicyRoutingAgent = 24;

		public const int OwaRulesEngine = 25;

		public static Guid guid = new Guid("D368698B-B84F-402e-A300-FA98CC39020F");
	}
}
