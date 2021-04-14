using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal interface IMessageDepotItemWrapper
	{
		MessageDepotItemState State { get; }

		IMessageDepotItem Item { get; }
	}
}
