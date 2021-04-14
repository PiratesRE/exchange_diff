using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class MeetingRequestRecipientWell : ItemRecipientWell
	{
		internal MeetingRequestRecipientWell(UserContext userContext, MeetingRequest meetingRequest) : base(userContext)
		{
			this.meetingRequest = meetingRequest;
		}

		internal override IEnumerator<Participant> GetRecipientCollection(RecipientWellType type)
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
			foreach (BlobRecipient recipient in this.meetingRequest.GetMergedRecipientList())
			{
				if (MapiUtil.MapiRecipientTypeToRecipientItemType((RecipientType)recipient[InternalSchema.RecipientType]) == recipientItemType)
				{
					yield return recipient.Participant;
				}
			}
			yield break;
		}

		private MeetingRequest meetingRequest;
	}
}
