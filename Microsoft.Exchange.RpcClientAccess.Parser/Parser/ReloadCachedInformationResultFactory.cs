using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReloadCachedInformationResultFactory : MessageHeaderResultFactory
	{
		internal ReloadCachedInformationResultFactory(int maxSize) : base(RopId.ReloadCachedInformation)
		{
			this.maxSize = maxSize;
		}

		public override RecipientCollector CreateRecipientCollector(MessageHeader messageHeader, PropertyTag[] extraPropertyTags, Encoding string8Encoding)
		{
			this.messageHeader = messageHeader;
			int num = 0;
			using (Writer writer = new CountWriter())
			{
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

		public RopResult CreateSuccessfulResult(RecipientCollector recipientCollector)
		{
			return new SuccessfulReloadCachedInformationResult(this.messageHeader, recipientCollector);
		}

		private readonly int maxSize;

		private MessageHeader messageHeader;
	}
}
