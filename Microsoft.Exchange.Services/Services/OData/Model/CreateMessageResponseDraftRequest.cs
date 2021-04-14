using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class CreateMessageResponseDraftRequest : EntityActionRequest<Message>
	{
		public CreateMessageResponseDraftRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public MessageResponseType ResponseType { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			string actionName;
			if ((actionName = base.ActionName) != null)
			{
				if (actionName == "CreateReply")
				{
					this.ResponseType = MessageResponseType.Reply;
					return;
				}
				if (actionName == "CreateReplyAll")
				{
					this.ResponseType = MessageResponseType.ReplyAll;
					return;
				}
				if (!(actionName == "CreateForward"))
				{
					return;
				}
				this.ResponseType = MessageResponseType.Forward;
			}
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateMessageResponseDraftCommand(this);
		}
	}
}
