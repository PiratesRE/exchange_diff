using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class SendReadReceipt : ServiceCommand<bool>
	{
		public SendReadReceipt(CallContext callContext, ItemId itemId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(itemId, "itemId", "SendReadReceipt::SendReadReceipt");
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.itemId = itemId;
		}

		protected override bool InternalExecute()
		{
			using (MessageItem messageItem = MessageItem.Bind(this.session, IdConverter.EwsIdToMessageStoreObjectId(this.itemId.Id)))
			{
				if (messageItem != null)
				{
					messageItem.SendReadReceipt();
					return true;
				}
			}
			return false;
		}

		private readonly MailboxSession session;

		private ItemId itemId;
	}
}
