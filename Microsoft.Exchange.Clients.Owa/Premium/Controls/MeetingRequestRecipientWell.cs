using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class MeetingRequestRecipientWell : ItemRecipientWell
	{
		internal MeetingRequestRecipientWell(MeetingRequest meetingRequest)
		{
			this.meetingRequest = meetingRequest;
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
			foreach (BlobRecipient recipient in this.meetingRequest.GetMergedRecipientList())
			{
				if (MapiUtil.MapiRecipientTypeToRecipientItemType((RecipientType)recipient[InternalSchema.RecipientType]) == recipientItemType)
				{
					yield return recipient.Participant;
				}
			}
			yield break;
		}

		internal override void RenderContents(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWellNode.RenderFlags flags, RenderRecipientWellNode wellNode)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (this.meetingRequest.IsDelegated())
			{
				Utilities.SanitizeHtmlEncode(CalendarUtilities.GetDisplayAttendees(this.meetingRequest, type), writer);
				return;
			}
			base.RenderContents(writer, userContext, type, flags, wellNode);
		}

		public override bool HasRecipients(RecipientWellType type)
		{
			if (this.meetingRequest.IsDelegated())
			{
				return !string.IsNullOrEmpty(CalendarUtilities.GetDisplayAttendees(this.meetingRequest, type));
			}
			return base.HasRecipients(type);
		}

		private MeetingRequest meetingRequest;
	}
}
