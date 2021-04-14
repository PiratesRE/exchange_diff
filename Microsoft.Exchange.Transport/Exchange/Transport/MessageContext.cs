using System;

namespace Microsoft.Exchange.Transport
{
	internal class MessageContext : PoisonContext
	{
		public MessageContext(long messageId, string internetMessageId, MessageProcessingSource msgSource) : base(msgSource)
		{
			this.msgId = messageId;
			this.internetMessageId = internetMessageId;
		}

		internal long MessageId
		{
			get
			{
				return this.msgId;
			}
		}

		internal string InternetMessageId
		{
			get
			{
				return this.internetMessageId;
			}
		}

		private readonly long msgId;

		private readonly string internetMessageId;
	}
}
