using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReadRecipientsResultFactory : StandardResultFactory
	{
		internal ReadRecipientsResultFactory(int maxSize) : base(RopId.ReadRecipients)
		{
			this.maxSize = maxSize;
		}

		public RecipientCollector CreateRecipientCollector(PropertyTag[] extraPropertyTags)
		{
			return new RecipientCollector(this.maxSize - 6 - 1, extraPropertyTags, RecipientSerializationFlags.RecipientRowId | RecipientSerializationFlags.ExtraUnicodeProperties | RecipientSerializationFlags.CodePageId);
		}

		public RopResult CreateSuccessfulResult(RecipientCollector recipientCollector)
		{
			return new SuccessfulReadRecipientsResult(recipientCollector);
		}

		private readonly int maxSize;
	}
}
