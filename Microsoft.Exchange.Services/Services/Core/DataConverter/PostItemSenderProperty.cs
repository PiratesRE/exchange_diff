using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PostItemSenderProperty : SenderProperty
	{
		public PostItemSenderProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static PostItemSenderProperty CreateCommand(CommandContext commandContext)
		{
			return new PostItemSenderProperty(commandContext);
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			return ((PostItem)storeItem).Sender;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
			((PostItem)storeItem).Sender = participant;
		}
	}
}
