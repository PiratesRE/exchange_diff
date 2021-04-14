using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OpenEmbeddedMessageResultFactory : MessageHeaderResultFactory
	{
		internal OpenEmbeddedMessageResultFactory(int maxSize) : base(RopId.OpenEmbeddedMessage)
		{
			this.maxSize = maxSize;
		}

		public override RecipientCollector CreateRecipientCollector(MessageHeader messageHeader, PropertyTag[] extraPropertyTags, Encoding string8Encoding)
		{
			this.messageHeader = messageHeader;
			int num = 0;
			using (Writer writer = new CountWriter())
			{
				StoreId storeId = default(StoreId);
				writer.WriteBool(false);
				storeId.Serialize(writer);
				this.messageHeader.Serialize(writer, string8Encoding);
				writer.WriteCountAndPropertyTagArray(extraPropertyTags, FieldLength.WordSize);
				writer.WriteByte(0);
				num = (int)writer.Position;
			}
			if (num > this.maxSize)
			{
				throw new BufferTooSmallException();
			}
			return new RecipientCollector(this.maxSize - num - 6, extraPropertyTags, RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId messageId, RecipientCollector recipientCollector)
		{
			return new SuccessfulOpenEmbeddedMessageResult(serverObject, false, messageId, this.messageHeader, recipientCollector);
		}

		public RopResult CreateSuccessfulResult(IServerObject serverObject, StoreId messageId)
		{
			return new SuccessfulOpenEmbeddedMessageResult(serverObject, true, messageId, null, null);
		}

		private readonly int maxSize;

		private MessageHeader messageHeader;
	}
}
