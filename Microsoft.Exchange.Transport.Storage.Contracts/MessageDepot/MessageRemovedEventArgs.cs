using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal sealed class MessageRemovedEventArgs : MessageEventArgs
	{
		public MessageRemovedEventArgs(IMessageDepotItemWrapper itemWrapper, MessageRemovalReason reason, bool generateNdr) : base(itemWrapper)
		{
			this.reason = reason;
			this.generateNdr = generateNdr;
		}

		public MessageRemovalReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		public bool GenerateNdr
		{
			get
			{
				return this.generateNdr;
			}
		}

		private readonly MessageRemovalReason reason;

		private readonly bool generateNdr;
	}
}
