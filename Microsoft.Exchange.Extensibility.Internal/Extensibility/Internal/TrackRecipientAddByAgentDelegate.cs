using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal delegate void TrackRecipientAddByAgentDelegate(ITransportMailItemFacade mailItem, string recipEmail, RecipientP2Type recipientType, string agentName);
}
