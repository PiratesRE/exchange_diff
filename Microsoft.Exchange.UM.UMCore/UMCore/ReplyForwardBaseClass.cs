using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ReplyForwardBaseClass : XsoRecordedMessage
	{
		internal ReplyForwardBaseClass(BaseUMCallSession vo, UMSubscriber user, RetrieveVoicemailManager context) : base(vo, user, context)
		{
		}

		protected override bool IsPureVoiceMessage
		{
			get
			{
				return true;
			}
		}

		protected override MessageItem GenerateProtectedMessage(MailboxSession session)
		{
			MessageItem messageItem = this.GenerateMessage(session);
			bool flag = false;
			foreach (AttachmentHandle handle in messageItem.AttachmentCollection)
			{
				using (Attachment attachment = messageItem.AttachmentCollection.Open(handle))
				{
					if (XsoUtil.IsValidAudioAttachment(attachment))
					{
						attachment.FileName = DRMUtils.GetProtectedUMFileNameToUse(attachment.FileName);
						attachment.Save();
						flag = true;
					}
				}
			}
			if (flag)
			{
				messageItem[MessageItemSchema.VoiceMessageAttachmentOrder] = DRMUtils.GetProtectedUMVoiceMessageAttachmentOrder(XsoUtil.GetAttachmentOrderString(messageItem));
			}
			return messageItem;
		}

		protected override void AddMessageHeader(Item originalMessage, MessageContentBuilder content)
		{
			content.AddEmailHeader((MessageItem)originalMessage);
		}
	}
}
