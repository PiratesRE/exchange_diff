using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public enum DeliveryType : short
	{
		[LocDescription(DataStrings.IDs.DeliveryTypeUndefined)]
		Undefined,
		[LocDescription(DataStrings.IDs.DeliveryTypeDnsConnectorDelivery)]
		DnsConnectorDelivery,
		[LocDescription(DataStrings.IDs.DeliveryTypeMapiDelivery)]
		MapiDelivery,
		[LocDescription(DataStrings.IDs.DeliveryTypeNonSmtpGatewayDelivery)]
		NonSmtpGatewayDelivery,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmartHostConnectorDelivery)]
		SmartHostConnectorDelivery,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToRemoteAdSite)]
		SmtpRelayToRemoteAdSite,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToTiRg)]
		SmtpRelayToTiRg,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayWithinAdSite)]
		SmtpRelayWithinAdSite,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayWithinAdSiteToEdge)]
		SmtpRelayWithinAdSiteToEdge,
		[LocDescription(DataStrings.IDs.DeliveryTypeUnreachable)]
		Unreachable,
		[LocDescription(DataStrings.IDs.DeliveryTypeShadowRedundancy)]
		ShadowRedundancy,
		[LocDescription(DataStrings.IDs.DeliveryTypeHeartbeat)]
		Heartbeat,
		[LocDescription(DataStrings.IDs.DeliveryTypeDeliveryAgent)]
		DeliveryAgent,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpDeliveryToMailbox)]
		SmtpDeliveryToMailbox,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToDag)]
		SmtpRelayToDag,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToMailboxDeliveryGroup)]
		SmtpRelayToMailboxDeliveryGroup,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToConnectorSourceServers)]
		SmtpRelayToConnectorSourceServers,
		[LocDescription(DataStrings.IDs.DeliveryTypeSmtpRelayToServers)]
		SmtpRelayToServers
	}
}
