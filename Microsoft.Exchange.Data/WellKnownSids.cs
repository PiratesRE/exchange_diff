using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Microsoft.Exchange.Data
{
	internal static class WellKnownSids
	{
		[Description("MS Exchange\\Partner Servers")]
		public static SecurityIdentifier PartnerServers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-10");

		[Description("MS Exchange\\Hub Transport Servers")]
		public static SecurityIdentifier HubTransportServers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-21");

		[Description("MS Exchange\\Edge Transport Servers")]
		public static SecurityIdentifier EdgeTransportServers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-22");

		[Description("MS Exchange\\Externally Secured Servers")]
		public static SecurityIdentifier ExternallySecuredServers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-23");

		[Description("MS Exchange\\Legacy Exchange Servers")]
		public static SecurityIdentifier LegacyExchangeServers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-24");

		[Description("MS Exchange\\SiteMailbox Granted Access Members")]
		public static SecurityIdentifier SiteMailboxGrantedAccessMembers = new SecurityIdentifier("S-1-9-1419165041-1139599005-3936102811-1022490595-25");
	}
}
