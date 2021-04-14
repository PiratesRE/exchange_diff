using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal sealed class MessageDeactivatedEventArgs : MessageEventArgs
	{
		public MessageDeactivatedEventArgs(IMessageDepotItemWrapper itemWrapper, MessageDeactivationReason reason) : base(itemWrapper)
		{
			this.reason = reason;
		}

		public MessageDeactivationReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly MessageDeactivationReason reason;
	}
}
