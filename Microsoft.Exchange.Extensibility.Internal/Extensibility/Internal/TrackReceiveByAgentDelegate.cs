using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal delegate void TrackReceiveByAgentDelegate(ITransportMailItemFacade mailItem, string sourceContext, string connectorId, long? relatedMailItemId);
}
