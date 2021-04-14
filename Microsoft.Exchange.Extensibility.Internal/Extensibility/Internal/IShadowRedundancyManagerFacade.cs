using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IShadowRedundancyManagerFacade
	{
		void LinkSideEffectMailItemIfNeeded(ITransportMailItemFacade originalMailItem, ITransportMailItemFacade sideEffectMailItem);
	}
}
