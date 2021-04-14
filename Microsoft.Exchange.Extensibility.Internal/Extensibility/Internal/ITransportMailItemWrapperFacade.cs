using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface ITransportMailItemWrapperFacade
	{
		ITransportMailItemFacade TransportMailItem { get; }
	}
}
