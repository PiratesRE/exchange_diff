using System;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	public enum ShadowBreadcrumbs
	{
		Empty,
		Open,
		PrepareForCommand,
		WriteInternal,
		WriteQueuingBuffer,
		WriteQueuingCommand,
		WriteDequeuingBuffer,
		WriteDequeuingCommand,
		WriteToProxy,
		WriteProxyDataComplete,
		WriteAfterCloseSkipped,
		WriteNewBdatCommand,
		NextHopConnectionFailedOver,
		NextHopConnectionAckMailItem,
		NextHopAckConnection,
		SessionAckConnectionFailure,
		SessionAckMessage,
		Complete,
		Close,
		MessageShadowingComplete,
		ProxyFailover,
		LocalMessageDiscarded,
		LocalMessageRejected
	}
}
