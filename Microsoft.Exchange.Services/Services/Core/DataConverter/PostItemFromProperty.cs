using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PostItemFromProperty : FromProperty
	{
		public PostItemFromProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static PostItemFromProperty CreateCommand(CommandContext commandContext)
		{
			return new PostItemFromProperty(commandContext);
		}

		protected override Participant GetParticipant(Item storeItem)
		{
			return ((PostItem)storeItem).From;
		}

		protected override void SetParticipant(Item storeItem, Participant participant)
		{
			((PostItem)storeItem).From = participant;
		}
	}
}
