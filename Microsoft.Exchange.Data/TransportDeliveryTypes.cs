using System;

namespace Microsoft.Exchange.Data
{
	internal class TransportDeliveryTypes
	{
		internal static DeliveryType[] internalDeliveryTypes = new DeliveryType[]
		{
			DeliveryType.Undefined,
			DeliveryType.MapiDelivery,
			DeliveryType.SmtpRelayToRemoteAdSite,
			DeliveryType.SmtpRelayToTiRg,
			DeliveryType.SmtpRelayWithinAdSite,
			DeliveryType.SmtpRelayWithinAdSiteToEdge,
			DeliveryType.Unreachable,
			DeliveryType.ShadowRedundancy,
			DeliveryType.Heartbeat,
			DeliveryType.SmtpDeliveryToMailbox,
			DeliveryType.SmtpRelayToDag,
			DeliveryType.SmtpRelayToMailboxDeliveryGroup,
			DeliveryType.SmtpRelayToConnectorSourceServers,
			DeliveryType.SmtpRelayToServers
		};

		internal static DeliveryType[] externalDeliveryTypes = new DeliveryType[]
		{
			DeliveryType.DnsConnectorDelivery,
			DeliveryType.NonSmtpGatewayDelivery,
			DeliveryType.SmartHostConnectorDelivery,
			DeliveryType.DeliveryAgent
		};
	}
}
