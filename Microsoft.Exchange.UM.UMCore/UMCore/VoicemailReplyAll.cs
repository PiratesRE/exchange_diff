using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoicemailReplyAll : VoicemailReply
	{
		internal VoicemailReplyAll(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, ContactInfo sender, RetrieveVoicemailManager context, bool isOriginalMessageIsProtected) : base(vo, originalId, user, sender, context, isOriginalMessageIsProtected)
		{
		}

		protected override MessageItem InternalCreateReply(MessageItem original, MailboxSession session, ReplyForwardConfiguration replyConfiguration)
		{
			return original.CreateReplyAll(XsoUtil.GetDraftsFolderId(session), replyConfiguration);
		}
	}
}
