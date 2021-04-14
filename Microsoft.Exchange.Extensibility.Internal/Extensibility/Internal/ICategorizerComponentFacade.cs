using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface ICategorizerComponentFacade
	{
		void EnqueueSideEffectMessage(ITransportMailItemFacade originalMailItem, ITransportMailItemFacade sideEffectMailItem, string agentName);
	}
}
