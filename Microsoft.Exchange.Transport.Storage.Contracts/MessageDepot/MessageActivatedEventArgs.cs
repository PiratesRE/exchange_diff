using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal sealed class MessageActivatedEventArgs : MessageEventArgs
	{
		public MessageActivatedEventArgs(IMessageDepotItemWrapper itemWrapper, MessageActivationReason reason) : base(itemWrapper)
		{
			this.reason = reason;
		}

		public MessageActivationReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly MessageActivationReason reason;
	}
}
