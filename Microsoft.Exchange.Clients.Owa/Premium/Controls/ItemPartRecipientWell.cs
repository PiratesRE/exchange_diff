using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class ItemPartRecipientWell : ItemRecipientWell
	{
		internal ItemPartRecipientWell(ItemPart itemPart)
		{
			this.itemPart = itemPart;
		}

		internal override IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type)
		{
			RecipientItemType recipientItemType;
			switch (type)
			{
			case RecipientWellType.To:
				recipientItemType = RecipientItemType.To;
				break;
			case RecipientWellType.Cc:
				recipientItemType = RecipientItemType.Cc;
				break;
			case RecipientWellType.Bcc:
				recipientItemType = RecipientItemType.Bcc;
				break;
			default:
				recipientItemType = RecipientItemType.Unknown;
				break;
			}
			foreach (IParticipant participant2 in this.itemPart.Recipients[recipientItemType])
			{
				Participant participant = (Participant)participant2;
				yield return participant;
			}
			yield break;
		}

		private ItemPart itemPart;
	}
}
