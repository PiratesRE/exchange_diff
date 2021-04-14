using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal class MessageEventArgs
	{
		public MessageEventArgs(IMessageDepotItemWrapper itemWrapper)
		{
			if (itemWrapper == null)
			{
				throw new ArgumentNullException("itemWrapper");
			}
			this.itemWrapper = itemWrapper;
		}

		public IMessageDepotItemWrapper ItemWrapper
		{
			get
			{
				return this.itemWrapper;
			}
		}

		private readonly IMessageDepotItemWrapper itemWrapper;
	}
}
