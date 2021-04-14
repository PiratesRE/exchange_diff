using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MessageRecipientWell : ItemRecipientWell
	{
		internal MessageRecipientWell(MessageItem message)
		{
			this.message = message;
		}

		public MessageRecipientWell() : this(null)
		{
		}

		internal override IEnumerator<Participant> GetRecipientsCollection(RecipientWellType type)
		{
			if (this.message != null)
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
				if (recipientItemType == RecipientItemType.To && this.message is ReportMessage)
				{
					Participant participant = ((ReportMessage)this.message).ReceivedRepresenting;
					if (null != participant)
					{
						yield return participant;
					}
				}
				else if (type == RecipientWellType.From)
				{
					Participant participant2 = this.message.From;
					if (null != participant2)
					{
						yield return participant2;
					}
				}
				else
				{
					foreach (Recipient recipient in this.message.Recipients)
					{
						if (recipient.RecipientItemType == recipientItemType)
						{
							object isResentMessage = this.message.TryGetProperty(MessageItemSchema.IsResend);
							if (!(isResentMessage is bool) || !(bool)isResentMessage || !recipient.Submitted)
							{
								yield return recipient.Participant;
							}
						}
					}
				}
			}
			yield break;
		}

		private MessageItem message;
	}
}
