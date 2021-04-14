using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.ClientAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PlayOnPhone : SingleStepServiceCommand<PlayOnPhoneRequest, PhoneCallId>
	{
		public PlayOnPhone(CallContext callContext, PlayOnPhoneRequest request) : base(callContext, request)
		{
			this.itemId = request.ItemId;
			this.dialString = request.DialString;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new PlayOnPhoneResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<PhoneCallId> Execute()
		{
			string text = null;
			MailboxSession mailboxSession = null;
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadOnly(this.itemId);
			mailboxSession = (idAndSession.Session as MailboxSession);
			if (mailboxSession == null)
			{
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorInvalidOperationForPublicFolderItems);
			}
			using (Item.Bind(mailboxSession, StoreId.GetStoreObjectId(idAndSession.Id)))
			{
			}
			using (UMClientCommon umclientCommon = new UMClientCommon(mailboxSession.MailboxOwner))
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(idAndSession.Id);
				text = umclientCommon.PlayOnPhone(Convert.ToBase64String(storeObjectId.ProviderLevelItemId), this.dialString);
			}
			PhoneCallId value = (text != null) ? new PhoneCallId(text) : null;
			return new ServiceResult<PhoneCallId>(value);
		}

		private ItemId itemId;

		private string dialString;
	}
}
